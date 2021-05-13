﻿using JapaneseLookup.Anki;
using JapaneseLookup.Deconjugation;
using JapaneseLookup.EDICT;
using JapaneseLookup.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JapaneseLookup
{
    internal static class MainWindowUtilities
    {
        public static bool ready = false;
        public static string Backlog = "";
        public const string FakeFrequency = "1000000";
        public enum DictionaryName { JMdict, JMnedict };

        public static readonly Regex JapaneseRegex =
            new(@"[\u3000-\u303f\u3040-\u309f\u30a0-\u30ff\uff00-\uff9f\u4e00-\u9faf\u3400-\u4dbf]");

        // Consider checking for \t, \r, "　", " ", ., !, ?, –, —, ―, ‒, ~, ‥, ♪, ～, ♡, ♥, ☆, ★
        private static readonly List<string> JapanesePunctuation = new(new[]
            {"。", "！", "？", "…", "―", "\n"});

        public static void MainWindowInitializer()
        {
            Task<Dictionary<string, List<List<JsonElement>>>> taskFreqLoaderVN = Task.Run(() =>
                FrequencyLoader.LoadJSON(Path.Join(ConfigManager.ApplicationPath, "Resources/freqlist_vns.json")));
            Task<Dictionary<string, List<List<JsonElement>>>> taskFreqLoaderNovel = Task.Run(() =>
                FrequencyLoader.LoadJSON(Path.Join(ConfigManager.ApplicationPath, "Resources/freqlist_novels.json")));
            Task<Dictionary<string, List<List<JsonElement>>>> taskFreqLoaderNarou = Task.Run(() =>
                FrequencyLoader.LoadJSON(Path.Join(ConfigManager.ApplicationPath, "Resources/freqlist_narou.json")));

            Task.Run(() => EdictLoader.Load()).ContinueWith(_ =>
            {
                //Task.WaitAll(taskFreqLoaderVN, taskFreqLoaderNovel, taskFreqLoaderNarou);
                FrequencyLoader.AddToJMdict("VN", taskFreqLoaderVN.Result);
                FrequencyLoader.AddToJMdict("Novel", taskFreqLoaderNovel.Result);
                FrequencyLoader.AddToJMdict("Narou", taskFreqLoaderNarou.Result);
                ready = true;
            });

            Task.Run(JMnedictLoader.Load);

            // init AnkiConnect so that it doesn't block later
            Task.Run(AnkiConnect.GetDeckNames);
        }

        public static (string sentence, int endPosition) FindSentence(string text, int position)
        {
            int startPosition = -1;
            int endPosition = -1;

            foreach (string punctuation in JapanesePunctuation)
            {
                int tempIndex = text.LastIndexOf(punctuation, position, StringComparison.Ordinal);

                if (tempIndex > startPosition)
                    startPosition = tempIndex;

                tempIndex = text.IndexOf(punctuation, position, StringComparison.Ordinal);
                if (tempIndex != -1 && (endPosition == -1 || tempIndex < endPosition))
                    endPosition = tempIndex;
            }

            ++startPosition;

            if (endPosition == -1)
                endPosition = text.Length - 1;

            // Consider trimming \t, \r, (, ), "　", " "
            return (
                text.Substring(startPosition, endPosition - startPosition + 1)
                    .Trim('「', '」', '『', '』', '（', '）', '\n'),
                endPosition
            );
            //text = text.Substring(startPosition, endPosition - startPosition + 1).TrimStart('「', '『', '（', '\n').TrimEnd('」', '』', '）', '\n');
        }

        public static List<Dictionary<string, List<string>>> LookUp(string text)
        {
            Dictionary<string, (List<EdictResult> jMdictResults, List<string> processList, string foundForm)> wordResults =
                new();
            Dictionary<string, (List<JMnedictResult> jMdictResults, List<string> processList, string foundForm)> nameResults =
                new();

            int succAttempt = 0;
            for (int i = 0; i < text.Length; i++)
            {
                string textInHiragana = Kana.KatakanaToHiraganaConverter(text[..^i]);

                bool tryLongVowelConversion = true;

                if (EdictLoader.jMdictDictionary.TryGetValue(textInHiragana, out var tempResult))
                {
                    wordResults.TryAdd(textInHiragana, (tempResult, new List<string>(), text[..^i]));
                    tryLongVowelConversion = false;
                }

                if (JMnedictLoader.jMnedictDictionary.TryGetValue(textInHiragana, out var tempNameResult))
                {
                    nameResults.TryAdd(textInHiragana, (tempNameResult, new List<string>(), text[..^i]));
                }

                if (succAttempt < 3)
                {
                    var deconjugationResults = Deconjugator.Deconjugate(textInHiragana);
                    foreach (var result in deconjugationResults)
                    {
                        if (wordResults.ContainsKey(result.Text))
                            continue;

                        if (EdictLoader.jMdictDictionary.TryGetValue(result.Text, out var temp))
                        {
                            List<EdictResult> resultsList = new();

                            foreach (var rslt in temp)
                            {
                                if (rslt.WordClasses.SelectMany(pos => pos).Intersect(result.Tags).Any())
                                {
                                    resultsList.Add(rslt);
                                }
                            }

                            if (resultsList.Any())
                            {
                                wordResults.Add(result.Text,
                                    (resultsList, result.Process, text[..result.OriginalText.Length]));
                                ++succAttempt;
                                tryLongVowelConversion = false;
                            }
                        }
                    }
                }

                if (tryLongVowelConversion && textInHiragana.Contains("ー") && textInHiragana[0] != 'ー')
                {
                    string textWithoutLongVowelMark = Kana.LongVowelMarkConverter(textInHiragana);
                    if (EdictLoader.jMdictDictionary.TryGetValue(textWithoutLongVowelMark, out var tmpResult))
                    {
                        wordResults.Add(textInHiragana, (tmpResult, new List<string>(), text[..^i]));
                    }
                }
            }

            if (!wordResults.Any() && !nameResults.Any())
                return null;

            List<Dictionary<string, List<string>>> results = new();

            if (wordResults.Any())
                results.AddRange(WordResultBuilder(wordResults));

            if (nameResults.Any())
                results.AddRange(NameResultBuilder(nameResults));

            results = results
                .OrderByDescending(dict => dict["foundForm"][0].Length)
                .ThenBy(dict => Convert.ToInt32(dict["frequency"][0])).ToList();
            return results;
        }

        private static List<Dictionary<string, List<string>>> NameResultBuilder
            (Dictionary<string, (List<JMnedictResult> jMdictResults, List<string> processList, string foundForm)> nameResult)
        {
            var results = new List<Dictionary<string, List<string>>>();

            foreach (var wordResult in nameResult)
            {
                foreach (var jMDictResult in wordResult.Value.jMdictResults)
                {
                    var result = new Dictionary<string, List<string>>();

                    var foundSpelling = new List<string> { jMDictResult.PrimarySpelling };
                    List<string> readings;

                    if (jMDictResult.Readings != null)
                        readings = jMDictResult.Readings.ToList();
                    else
                        readings = new List<string>();

                    var foundForm = new List<string> { wordResult.Value.foundForm };
                    var jmdictID = new List<string> { jMDictResult.Id };


                    List<string> alternativeSpellings;
                    if (jMDictResult.AlternativeSpellings != null)
                        alternativeSpellings = jMDictResult.AlternativeSpellings;
                    else
                        alternativeSpellings = new List<string>();

                    var process = wordResult.Value.processList;

                    var definitions = new List<string> { BuildNameDefinition(jMDictResult) };

                    result.Add("foundSpelling", foundSpelling);
                    result.Add("readings", readings);
                    result.Add("definitions", definitions);
                    result.Add("foundForm", foundForm);
                    result.Add("jmdictID", jmdictID);
                    result.Add("alternativeSpellings", alternativeSpellings);
                    result.Add("process", process);
                    result.Add("frequency", new() { FakeFrequency });
                    result.Add("kanaSpellings", new List<string>());

                    results.Add(result);
                }
            }
            return results;
        }

        private static List<Dictionary<string, List<string>>> WordResultBuilder
            (Dictionary<string, (List<EdictResult> jMdictResults, List<string> processList, string foundForm)> wordResults)
        {
            var results = new List<Dictionary<string, List<string>>>();

            foreach (var wordResult in wordResults)
            {
                foreach (var jMDictResult in wordResult.Value.jMdictResults)
                {
                    var result = new Dictionary<string, List<string>>();

                    var foundSpelling = new List<string> { jMDictResult.PrimarySpelling };

                    List<string> kanaSpellings;

                    if (jMDictResult.KanaSpellings != null)
                        kanaSpellings = jMDictResult.KanaSpellings;
                    else
                        kanaSpellings = new List<string>();

                    var readings = jMDictResult.Readings.ToList();
                    var foundForm = new List<string> { wordResult.Value.foundForm };
                    var jmdictID = new List<string> { jMDictResult.Id };

                    List<string> alternativeSpellings;
                    if (jMDictResult.AlternativeSpellings != null)
                        alternativeSpellings = jMDictResult.AlternativeSpellings.ToList();
                    else
                        alternativeSpellings = new List<string>();
                    var process = wordResult.Value.processList;

                    List<string> frequency;
                    if (jMDictResult.FrequencyDict != null)
                    {
                        jMDictResult.FrequencyDict.TryGetValue(ConfigManager.FrequencyList, out var freqList);
                        var maybeFreq = freqList?.FrequencyRank;
                        frequency = new List<string> { maybeFreq == null ? FakeFrequency : maybeFreq.ToString() };
                    }

                    else frequency = new List<string> { FakeFrequency };

                    var definitions = new List<string> { BuildWordDefinition(jMDictResult) };

                    //var POrthographyInfoList = jMDictResult.POrthographyInfoList;
                    //var AOrthographyInfoList = jMDictResult.AOrthographyInfoList;
                    //var ROrthographyInfoList = jMDictResult.ROrthographyInfoList;

                    result.Add("foundSpelling", foundSpelling);
                    result.Add("kanaSpellings", kanaSpellings);
                    result.Add("readings", readings);
                    result.Add("definitions", definitions);
                    result.Add("foundForm", foundForm);
                    result.Add("jmdictID", jmdictID);
                    result.Add("alternativeSpellings", alternativeSpellings);
                    result.Add("process", process);
                    result.Add("frequency", frequency);

                    //result.Add("pOrthographyInfoList", POrthographyInfoList);
                    //result.Add("aOrthographyInfoList", AOrthographyInfoList);
                    //result.Add("rOrthographyInfoList", ROrthographyInfoList);

                    results.Add(result);
                }
            }
            return results;
        }
        private static string BuildNameDefinition(JMnedictResult jMDictResult)
        {
            int count = 1;
            string defResult = "";
            for (int i = 0; i < jMDictResult.Definitions.Count; i++)
            {
                if (jMDictResult.Definitions.Any())
                {
                    if (jMDictResult.NameTypes != null && jMDictResult.NameTypes[i] != null)
                    {
                        defResult += "(";
                        defResult += jMDictResult.NameTypes[i];
                        defResult += ") ";
                    }

                    if (jMDictResult.Definitions.Count>0)
                    defResult += "(" + count + ") ";

                    defResult += string.Join("; ", jMDictResult.Definitions[i]) + " ";

                    //defResult += "\n";
                    ++count;
                }
            }
            return defResult;
        }
        private static string BuildWordDefinition(EdictResult jMDictResult)
        {
            int count = 1;
            string defResult = "";
            for (int i = 0; i < jMDictResult.Definitions.Count; i++)
            {
                if (jMDictResult.WordClasses.Any() && jMDictResult.WordClasses[i].Any())
                {
                    defResult += "(";
                    defResult += string.Join(", ", jMDictResult.WordClasses[i]);
                    defResult += ") ";
                }

                if (jMDictResult.Definitions.Any())
                {
                    defResult += "(" + count + ") ";

                    if (jMDictResult.SpellingInfo.Any() && jMDictResult.SpellingInfo[i] != null)
                    {
                        defResult += "(";
                        defResult += jMDictResult.SpellingInfo[i];
                        defResult += ") ";
                    }

                    if (jMDictResult.MiscList.Any() && jMDictResult.MiscList[i].Any())
                    {
                        defResult += "(";
                        defResult += string.Join(", ", jMDictResult.MiscList[i]);
                        defResult += ") ";
                    }

                    defResult += string.Join("; ", jMDictResult.Definitions[i]) + " ";

                    if (jMDictResult.RRestrictions != null && jMDictResult.RRestrictions[i].Any()
                        || jMDictResult.KRestrictions != null && jMDictResult.KRestrictions[i].Any())
                    {
                        defResult += "(only applies to ";

                        if (jMDictResult.KRestrictions != null && jMDictResult.KRestrictions[i].Any())
                            defResult += string.Join("; ", jMDictResult.KRestrictions[i]);

                        if (jMDictResult.RRestrictions != null && jMDictResult.RRestrictions[i].Any())
                            defResult += string.Join("; ", jMDictResult.RRestrictions[i]);

                        defResult += ") ";
                    }
                    ++count;
                }
            }
            return defResult;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using JapaneseLookup.GUI;

namespace JapaneseLookup
{
    public static class PopupWindowUtilities
    {
        public static List<Dictionary<LookupResult, List<string>>> LastLookupResults { get; set; } = new();

        // super bad hack that improves performance by a lot when many dictionaries are loaded...
        private const int MaxNumberOfResultsWhenNotInMiningMode = 10;

        public static void DisplayResults(bool generateAllResults)
        {
            var results = LastLookupResults;
            // apparently you can't get the desired size of a control before the layout pass
            // probably won't be worth (performance-wise) forcing that to happen instead of just using a magic number
            int resultsCount = generateAllResults
                ? results.Count
                : Math.Min(results.Count, MaxNumberOfResultsWhenNotInMiningMode);

            for (int index = 0; index < resultsCount; index++)
            {
                if (index > ConfigManager.MaxResults)
                    return;

                var result = results[index];
                StackPanel resultStackPanel = MakeResultStackPanel(result, index, results.Count);

                PopupWindow.Instance.ResultStackPanels.Add(resultStackPanel);
            }
        }

        internal static StackPanel MakeResultStackPanel(Dictionary<LookupResult, List<string>> result,
            int index, int resultsCount)
        {
            var innerStackPanel = new StackPanel
            {
                Margin = new Thickness(4, 2, 4, 2),
            };
            var top = new WrapPanel();
            var bottom = new StackPanel();


            // top
            TextBlock textBlockFoundSpelling = null;
            TextBlock textBlockPOrthographyInfo = null;
            TextBlock textBlockReadings = null;
            TextBlock textBlockAlternativeSpellings = null;
            TextBlock textBlockProcess = null;
            TextBlock textBlockFrequency = null;
            TextBlock textBlockFoundForm = null;
            TextBlock textBlockDictType = null;
            TextBlock textBlockEdictID = null;

            // bottom
            TextBlock textBlockDefinitions = null;
            TextBlock textBlockNanori = null;
            TextBlock textBlockOnReadings = null;
            TextBlock textBlockKunReadings = null;
            TextBlock textBlockStrokeCount = null;
            TextBlock textBlockGrade = null;
            TextBlock textBlockComposition = null;


            foreach ((LookupResult key, var value) in result)
            {
                switch (key)
                {
                    // common
                    case LookupResult.FoundForm:
                        textBlockFoundForm = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = string.Join("", value),
                            Visibility = Visibility.Collapsed
                        };
                        break;

                    case LookupResult.Frequency:

                        textBlockFrequency = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = "#" + string.Join(", ", value),
                            Foreground = ConfigManager.FrequencyColor,
                            FontSize = ConfigManager.FrequencyFontSize,
                            Margin = new Thickness(5, 0, 0, 0),
                        };
                        break;

                    case LookupResult.DictType:
                        textBlockDictType = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = value[0],
                            Foreground = ConfigManager.DictTypeColor,
                            FontSize = ConfigManager.DictTypeFontSize,
                            Margin = new Thickness(5, 0, 0, 0),
                        };
                        break;


                    // EDICT
                    case LookupResult.FoundSpelling:
                        textBlockFoundSpelling = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = value[0],
                            Tag = index, // for audio
                            Foreground = ConfigManager.FoundSpellingColor,
                            FontSize = ConfigManager.FoundSpellingFontSize,
                        };
                        textBlockFoundSpelling.MouseEnter += PopupWindow.FoundSpelling_MouseEnter; // for audio
                        textBlockFoundSpelling.MouseLeave += PopupWindow.FoundSpelling_MouseLeave; // for audio
                        textBlockFoundSpelling.PreviewMouseUp += PopupWindow.FoundSpelling_PreviewMouseUp; // for mining
                        break;

                    case LookupResult.KanaSpellings:
                        // var textBlockKanaSpellings = new TextBlock
                        // {
                        //     Name = "kanaSpellings",
                        //     Text = string.Join(" ", result["kanaSpellings"]),
                        //     TextWrapping = TextWrapping.Wrap,
                        //     Foreground = Brushes.White
                        // };
                        break;

                    case LookupResult.Readings:
                        result.TryGetValue(LookupResult.ROrthographyInfoList, out var rOrthographyInfoList);
                        rOrthographyInfoList ??= new List<string>();

                        textBlockReadings = MakeTextBlockReadings(result[LookupResult.Readings], rOrthographyInfoList);
                        break;

                    case LookupResult.Definitions:
                        textBlockDefinitions = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = string.Join(", ", value),
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = ConfigManager.DefinitionsColor,
                            FontSize = ConfigManager.DefinitionsFontSize,
                            Margin = new Thickness(2, 2, 2, 2),
                        };
                        break;

                    case LookupResult.EdictID:
                        textBlockEdictID = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = string.Join(", ", value),
                            Visibility = Visibility.Collapsed
                        };
                        break;

                    case LookupResult.AlternativeSpellings:
                        result.TryGetValue(LookupResult.AOrthographyInfoList, out var aOrthographyInfoList);
                        aOrthographyInfoList ??= new List<string>();

                        textBlockAlternativeSpellings =
                            MakeTextBlockAlternativeSpellings(result[LookupResult.AlternativeSpellings],
                                aOrthographyInfoList);
                        break;

                    case LookupResult.Process:
                        textBlockProcess = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = string.Join(", ", value),
                            Foreground = ConfigManager.ProcessColor,
                            FontSize = ConfigManager.ProcessFontSize,
                            Margin = new Thickness(5, 0, 0, 0),
                        };
                        break;

                    case LookupResult.POrthographyInfoList:
                        textBlockPOrthographyInfo = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = "(" + string.Join(",", value) + ")",
                            //Foreground = ConfigManager.pOrthographyInfoColor,
                            //FontSize = ConfigManager.pOrthographyInfoFontSize,
                            Margin = new Thickness(5, 0, 0, 0),
                        };
                        break;

                    case LookupResult.ROrthographyInfoList:
                        // processed in MakeTextBlockReadings()
                        break;

                    case LookupResult.AOrthographyInfoList:
                        // processed in MakeTextBlockAlternativeSpellings()
                        break;


                    // KANJIDIC
                    case LookupResult.OnReadings:
                        if (!value.Any())
                            break;

                        textBlockOnReadings = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = "On" + ": " + string.Join(", ", value),
                            Foreground = ConfigManager.ReadingsColor,
                            FontSize = ConfigManager.ReadingsFontSize,
                            Margin = new Thickness(2, 0, 0, 0),
                        };
                        break;

                    case LookupResult.KunReadings:
                        if (!value.Any())
                            break;

                        textBlockKunReadings = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = "Kun" + ": " + string.Join(", ", value),
                            Foreground = ConfigManager.ReadingsColor,
                            FontSize = ConfigManager.ReadingsFontSize,
                            Margin = new Thickness(2, 0, 0, 0),
                        };
                        break;

                    case LookupResult.Nanori:
                        if (!value.Any())
                            break;

                        textBlockNanori = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = key + ": " + string.Join(", ", value),
                            Foreground = ConfigManager.ReadingsColor,
                            FontSize = ConfigManager.ReadingsFontSize,
                            Margin = new Thickness(2, 0, 0, 0),
                        };
                        break;

                    case LookupResult.StrokeCount:
                        textBlockStrokeCount = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = "Strokes" + ": " + string.Join(", ", value),
                            // Foreground = ConfigManager. Color,
                            FontSize = ConfigManager.DefinitionsFontSize,
                            Margin = new Thickness(2, 2, 2, 2),
                        };
                        break;

                    case LookupResult.Grade:
                        var gradeString = "";
                        var gradeInt = Convert.ToInt32(value[0]);
                        gradeString = gradeInt switch
                        {
                            0 => "Hyougai",
                            <= 6 => $"Kyouiku ({gradeInt})",
                            8 => $"Jouyou ({gradeInt})",
                            <= 10 => $"Jinmeiyou ({gradeInt})",
                            _ => gradeString
                        };

                        textBlockGrade = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = key + ": " + gradeString,
                            // Foreground = ConfigManager. Color,
                            FontSize = ConfigManager.DefinitionsFontSize,
                            Margin = new Thickness(2, 2, 2, 2),
                        };
                        break;

                    case LookupResult.Composition:
                        textBlockComposition = new TextBlock
                        {
                            Name = key.ToString(),
                            Text = key + ": " + string.Join(", ", value),
                            // Foreground = ConfigManager. Color,
                            FontSize = ConfigManager.ReadingsFontSize,
                            Margin = new Thickness(2, 2, 2, 2),
                        };
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            TextBlock[] babies =
            {
                textBlockFoundSpelling, textBlockPOrthographyInfo,
                textBlockReadings,
                textBlockAlternativeSpellings,
                textBlockProcess,
                textBlockFoundForm, textBlockEdictID, // undisplayed, for mining
                textBlockFrequency, textBlockDictType
            };
            foreach (TextBlock baby in babies)
            {
                if (baby == null) continue;

                Enum.TryParse(baby.Name, out LookupResult enumName);

                // common emptiness check; these two have their text as inline Runs
                if (baby.Text == "" &&
                    !(enumName == LookupResult.AlternativeSpellings || enumName == LookupResult.Readings))
                    continue;

                // POrthographyInfo check
                if (baby.Text == "()")
                    continue;

                // Frequency check
                if ((baby.Text == ("#" + MainWindowUtilities.FakeFrequency)) || baby.Text == "#0")
                    continue;

                top.Children.Add(baby);
            }

            bottom.Children.Add(textBlockDefinitions);

            TextBlock[] babiesKanji =
            {
                textBlockOnReadings,
                textBlockKunReadings,
                textBlockNanori,
                textBlockGrade,
                textBlockStrokeCount,
                textBlockComposition,
            };
            foreach (TextBlock baby in babiesKanji)
            {
                if (baby == null) continue;

                // common emptiness check
                if (baby.Text == "")
                    continue;

                bottom.Children.Add(baby);
            }

            if (index != resultsCount - 1 && index != ConfigManager.MaxResults)
            {
                bottom.Children.Add(new Separator
                {
                    // TODO: Fix thickness' differing from one separator to another
                    Width = PopupWindow.Instance.Width,
                    Background = ConfigManager.SeparatorColor
                });
            }

            innerStackPanel.Children.Add(top);
            innerStackPanel.Children.Add(bottom);
            return innerStackPanel;
        }

        private static TextBlock MakeTextBlockReadings(List<string> readings, List<string> rOrthographyInfoList)
        {
            var textBlockReadings = new TextBlock
            {
                Name = LookupResult.Readings.ToString(),
                Text = "",
                Tag = string.Join(", ", readings), // for mining
                Foreground = ConfigManager.ReadingsColor,
                FontSize = ConfigManager.ReadingsFontSize,
                Margin = new Thickness(5, 0, 0, 0),
            };

            if (readings.Count == 0) return textBlockReadings;

            for (var index = 0; index < readings.Count; index++)
            {
                var runReading = new Run(readings[index])
                {
                    Foreground = ConfigManager.ReadingsColor,
                    FontSize = ConfigManager.ReadingsFontSize,
                };
                textBlockReadings.Inlines.Add(runReading);

                if (rOrthographyInfoList != null)
                {
                    if (index < rOrthographyInfoList.Count)
                    {
                        var runReadingOrtho = new Run("(" + rOrthographyInfoList[index] + ")")
                        {
                            Foreground = ConfigManager.ROrthographyInfoColor,
                            FontSize = ConfigManager.ROrthographyInfoFontSize,
                        };
                        if (runReadingOrtho.Text != "()")
                        {
                            textBlockReadings.Inlines.Add(" ");
                            textBlockReadings.Inlines.Add(runReadingOrtho);
                        }
                    }
                }

                if (index != readings.Count - 1)
                {
                    textBlockReadings.Inlines.Add(", ");
                }
            }

            return textBlockReadings;
        }

        private static TextBlock MakeTextBlockAlternativeSpellings(List<string> alternativeSpellings,
            List<string> aOrthographyInfoList)
        {
            var textBlockAlternativeSpellings = new TextBlock
            {
                Name = LookupResult.AlternativeSpellings.ToString(),
                Text = "",
                Tag = string.Join(", ", alternativeSpellings), // for mining
                Foreground = ConfigManager.AlternativeSpellingsColor,
                FontSize = ConfigManager.AlternativeSpellingsFontSize,
                Margin = new Thickness(5, 0, 0, 0),
            };

            if (alternativeSpellings.Count == 0) return textBlockAlternativeSpellings;

            textBlockAlternativeSpellings.Inlines.Add("(");

            for (var index = 0; index < alternativeSpellings.Count; index++)
            {
                var runAlt = new Run(alternativeSpellings[index])
                {
                    Foreground = ConfigManager.AlternativeSpellingsColor,
                    FontSize = ConfigManager.AlternativeSpellingsFontSize,
                };
                textBlockAlternativeSpellings.Inlines.Add(runAlt);

                if (index < aOrthographyInfoList.Count)
                {
                    var runAltOrtho = new Run("(" + aOrthographyInfoList[index] + ")")
                    {
                        Foreground = ConfigManager.AOrthographyInfoColor,
                        FontSize = ConfigManager.AOrthographyInfoFontSize,
                    };
                    if (runAltOrtho.Text != "()")
                    {
                        textBlockAlternativeSpellings.Inlines.Add(" ");
                        textBlockAlternativeSpellings.Inlines.Add(runAltOrtho);
                    }
                }

                if (index != alternativeSpellings.Count - 1)
                {
                    textBlockAlternativeSpellings.Inlines.Add(", ");
                }
            }

            textBlockAlternativeSpellings.Inlines.Add(")");

            return textBlockAlternativeSpellings;
        }

        public static string FindSentence(string text, int position)
        {
            List<string> japanesePunctuation = new() { "。", "！", "？", "…", ".", "\n", };

            Dictionary<string, string> japaneseParentheses = new()
            {
                { "「", "」" },
                { "『", "』" },
                { "（", "）" },
            };

            int startPosition = -1;
            int endPosition = -1;

            foreach (string punctuation in japanesePunctuation)
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

            string sentence;

            if (startPosition < endPosition)
            {
                sentence = text[startPosition..(endPosition + 1)].Trim('\n', '\t', '\r', ' ', '　');
            }

            else
            {
                sentence = "";
            }

            if (sentence.Length > 1)
            {
                if (japaneseParentheses.ContainsValue(sentence.First().ToString()))
                {
                    sentence = sentence[1..];
                }

                if (japaneseParentheses.Keys.Contains(sentence.LastOrDefault().ToString()))
                {
                    sentence = sentence[..^1];
                }

                if (japaneseParentheses.TryGetValue(sentence.FirstOrDefault().ToString(), out string rightParenthesis))
                {
                    if (sentence.Last().ToString() == rightParenthesis)
                        sentence = sentence[1..^1];

                    else if (!sentence.Contains(rightParenthesis))
                        sentence = sentence[1..];

                    else if (sentence.Contains(rightParenthesis))
                    {
                        int numberOfLeftParentheses = sentence.Count(p => p == sentence[0]);
                        int numberOfRightParentheses = sentence.Count(p => p == rightParenthesis[0]);

                        if (numberOfLeftParentheses == numberOfRightParentheses + 1)
                            sentence = sentence[1..];
                    }
                }

                else if (japaneseParentheses.ContainsValue(sentence.LastOrDefault().ToString()))
                {
                    string leftParenthesis = japaneseParentheses.First(p => p.Value == sentence.Last().ToString()).Key;

                    if (!sentence.Contains(leftParenthesis))
                        sentence = sentence[..^1];

                    else if (sentence.Contains(leftParenthesis))
                    {
                        int numberOfLeftParentheses = sentence.Count(p => p == leftParenthesis[0]);
                        int numberOfRightParentheses = sentence.Count(p => p == sentence.Last());

                        if (numberOfRightParentheses == numberOfLeftParentheses + 1)
                            sentence = sentence[..^1];
                    }
                }
            }

            return sentence;
        }
    }
}
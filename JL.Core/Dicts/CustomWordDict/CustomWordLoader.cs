using JL.Core.Utilities;

namespace JL.Core.Dicts.CustomWordDict;

public static class CustomWordLoader
{
    private static readonly string[] s_verbs = {
        "v1",
        "v1-s",
        "v4r",
        "v5aru",
        "v5b",
        "v5g",
        "v5k",
        "v5k-s",
        "v5m",
        "v5n",
        "v5r",
        "v5r-i",
        "v5s",
        "v5t",
        "v5u",
        "v5u-s",
        "vk",
        "vs-c",
        "vs-i",
        "vs-s",
        "vz"
    };

    private static readonly string[] s_adjectives = {
        "adj-i",
        "adj-na"
    };

    private static readonly string[] s_noun = {
        "noun"
    };

    private static readonly string[] s_other = {
        "other"
    };

    internal static void Load(Dict dict, CancellationToken cancellationToken)
    {
        string fullPath = Path.GetFullPath(dict.Path, Utils.ApplicationPath);
        if (File.Exists(fullPath))
        {
            Dictionary<string, IList<IDictRecord>> customWordDictionary = dict.Contents;

            foreach (string line in File.ReadLines(fullPath))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    customWordDictionary.Clear();
                    break;
                }

                string[] lParts = line.Split("\t", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (lParts.Length > 3)
                {
                    string[] spellings = lParts[0].Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    string[]? readings = lParts[1].Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    if (readings.Length is 0)
                    {
                        readings = null;
                    }

                    string[] definitions = lParts[2].Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    string partOfSpeech = lParts[3];

                    string[]? wordClasses = null;
                    if (lParts.Length is 5)
                    {
                        wordClasses = lParts[4].Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    }

                    AddToDictionary(spellings, readings, definitions, partOfSpeech, wordClasses, customWordDictionary);
                }
            }
        }
    }

    public static void AddToDictionary(string[] spellings, string[]? readings, string[] definitions,
        string rawPartOfSpeech, string[]? wordClasses, Dictionary<string, IList<IDictRecord>> customWordDictionary)
    {
        for (int i = 0; i < spellings.Length; i++)
        {
            string[]? alternativeSpellings = spellings.RemoveAt(i);
            if (alternativeSpellings.Length is 0)
            {
                alternativeSpellings = null;
            }

            string spelling = spellings[i];

            bool hasUserDefinedWordClasses = wordClasses?.Length > 0;

            string[] wordClassArray = rawPartOfSpeech switch
            {
                "Verb" => hasUserDefinedWordClasses ? wordClasses! : s_verbs,
                "Adjective" => s_adjectives,
                "Noun" => s_noun,
                _ => s_other
            };

            CustomWordRecord newWordRecord = new(spelling, alternativeSpellings, readings, definitions, wordClassArray, hasUserDefinedWordClasses);

            if (customWordDictionary.TryGetValue(JapaneseUtils.KatakanaToHiragana(spelling), out IList<IDictRecord>? result))
            {
                if (result.Contains(newWordRecord))
                {
                    break;
                }

                result.Add(newWordRecord);
            }
            else
            {
                customWordDictionary.Add(JapaneseUtils.KatakanaToHiragana(spelling),
                    new List<IDictRecord> { newWordRecord });
            }
        }
    }
}

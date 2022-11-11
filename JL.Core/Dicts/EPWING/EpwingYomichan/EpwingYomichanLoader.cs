﻿using System.Text.Json;

namespace JL.Core.Dicts.EPWING.EpwingYomichan;

public static class EpwingYomichanLoader
{
    public static async Task Load(Dict dict)
    {
        if (!Directory.Exists(dict.Path) && !File.Exists(dict.Path))
            return;

        string[] jsonFiles = Directory.EnumerateFiles(dict.Path, "*_bank_*.json", SearchOption.TopDirectoryOnly)
            .Where(s => s.Contains("term") || s.Contains("kanji"))
            .ToArray();

        foreach (string jsonFile in jsonFiles)
        {
            List<List<JsonElement>>? jsonObjects;

            FileStream fileStream = File.OpenRead(jsonFile);
            await using (fileStream.ConfigureAwait(false))
            {
                jsonObjects = await JsonSerializer
                    .DeserializeAsync<List<List<JsonElement>>>(fileStream)
                    .ConfigureAwait(false);
            }

            if (jsonObjects == null)
                continue;

            foreach (List<JsonElement> jsonObj in jsonObjects)
            {
                DictionaryBuilder(new EpwingYomichanRecord(jsonObj), dict);
            }
        }

        dict.Contents.TrimExcess();
    }

    private static void DictionaryBuilder(EpwingYomichanRecord yomichanRecord, Dict dict)
    {
        if (!EpwingUtils.IsValidEpwingResultForDictType(yomichanRecord, dict))
            return;

        string hiraganaExpression = Kana.KatakanaToHiraganaConverter(yomichanRecord.PrimarySpelling);

        if (!string.IsNullOrEmpty(yomichanRecord.Reading))
        {
            string hiraganaReading = Kana.KatakanaToHiraganaConverter(yomichanRecord.Reading);

            if (dict.Contents.TryGetValue(hiraganaReading, out List<IDictRecord>? tempList2))
                tempList2.Add(yomichanRecord);
            else
                dict.Contents.Add(hiraganaReading, new List<IDictRecord> { yomichanRecord });
        }

        if (dict.Contents.TryGetValue(hiraganaExpression, out List<IDictRecord>? tempList))
            tempList.Add(yomichanRecord);
        else
            dict.Contents.Add(hiraganaExpression, new List<IDictRecord> { yomichanRecord });
    }
}

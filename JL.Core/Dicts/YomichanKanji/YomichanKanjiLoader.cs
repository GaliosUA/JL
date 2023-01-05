using System.Text.Json;

namespace JL.Core.Dicts.YomichanKanji;

internal static class YomichanKanjiLoader
{
    public static async Task Load(Dict dict)
    {
        if (!Directory.Exists(dict.Path) && !File.Exists(dict.Path))
        {
            return;
        }

        string[] jsonFiles = Directory.EnumerateFiles(dict.Path, "kanji_bank_*.json", SearchOption.TopDirectoryOnly).ToArray();

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

            if (jsonObjects is null)
            {
                continue;
            }

            foreach (List<JsonElement> jsonObj in jsonObjects)
            {
                YomichanKanjiRecord yomichanKanjiRecord = new(jsonObj);
                string kanji = jsonObj[0].ToString();

                if (dict.Contents.TryGetValue(kanji, out List<IDictRecord>? kanjiResult))
                {
                    kanjiResult.Add(yomichanKanjiRecord);
                }
                else
                {
                    dict.Contents.Add(kanji, new List<IDictRecord> { yomichanKanjiRecord });
                }
            }
        }

        dict.Contents.TrimExcess();
    }
}

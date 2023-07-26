using System.Text.Json;
using JL.Core.Utilities;

namespace JL.Core.Freqs.FrequencyNazeka;

internal static class FrequencyNazekaLoader
{
    public static async Task Load(Freq freq)
    {
        if (!File.Exists(freq.Path))
        {
            return;
        }

        Dictionary<string, List<FrequencyRecord>> freqDict = freq.Contents;
        Dictionary<string, List<List<JsonElement>>>? frequencyJson;

        FileStream fileStream = File.OpenRead(freq.Path);
        await using (fileStream.ConfigureAwait(false))
        {
            frequencyJson = await JsonSerializer.DeserializeAsync<Dictionary<string, List<List<JsonElement>>>>(fileStream)
                .ConfigureAwait(false);
        }

        foreach ((string reading, List<List<JsonElement>> value) in frequencyJson!)
        {
            int valueCount = value.Count;
            for (int i = 0; i < valueCount; i++)
            {
                List<JsonElement> elementList = value[i];

                string exactSpelling = elementList[0].ToString();
                _ = elementList[1].TryGetInt32(out int frequencyRank);

                if (freqDict.TryGetValue(reading, out List<FrequencyRecord>? readingFreqResult))
                {
                    readingFreqResult.Add(new FrequencyRecord(exactSpelling, frequencyRank));
                }

                else
                {
                    freqDict.Add(reading,
                        new List<FrequencyRecord> { new(exactSpelling, frequencyRank) });
                }

                string exactSpellingInHiragana = JapaneseUtils.KatakanaToHiragana(exactSpelling);

                if (exactSpellingInHiragana != reading)
                {
                    if (freqDict.TryGetValue(exactSpellingInHiragana, out List<FrequencyRecord>? exactSpellingFreqResult))
                    {
                        exactSpellingFreqResult.Add(new FrequencyRecord(reading, frequencyRank));
                    }

                    else
                    {
                        freqDict.Add(exactSpellingInHiragana,
                            new List<FrequencyRecord> { new(reading, frequencyRank) });
                    }
                }
            }
        }

        freqDict.TrimExcess();
    }
}

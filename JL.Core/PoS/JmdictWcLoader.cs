﻿using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using JL.Core.Dicts;
using JL.Core.Dicts.EDICT.JMdict;

namespace JL.Core.PoS;

public static class JmdictWcLoader
{
    public static async Task Load()
    {
        FileStream openStream = File.OpenRead($"{Storage.ResourcesPath}/PoS.json");
        await using (openStream.ConfigureAwait(false))
        {
            Storage.WcDict = (await JsonSerializer.DeserializeAsync<Dictionary<string, List<JmdictWc>>>(openStream))!;
        }

        foreach (List<JmdictWc> jmDictWcEntryList in Storage.WcDict.Values.ToList())
        {
            int jmDictWcEntryListCount = jmDictWcEntryList.Count;
            for (int i = 0; i < jmDictWcEntryListCount; i++)
            {
                JmdictWc jMDictWcEntry = jmDictWcEntryList[i];

                if (jMDictWcEntry.Readings != null)
                {
                    int readingCount = jMDictWcEntry.Readings.Count;
                    for (int j = 0; j < readingCount; j++)
                    {
                        string reading = jMDictWcEntry.Readings[j];

                        if (Storage.WcDict.TryGetValue(reading, out List<JmdictWc>? result))
                        {
                            result.Add(jMDictWcEntry);
                        }

                        else
                        {
                            Storage.WcDict.Add(reading, new List<JmdictWc> { jMDictWcEntry });
                        }
                    }
                }
            }
        }

        Storage.WcDict.TrimExcess();
    }

    public static async Task JmdictWordClassSerializer()
    {
        Dictionary<string, List<JmdictWc>> jmdictWcDict = new();

        string[] usedWcs =
        {
            "adj-i", "adj-na", "v1", "v1-s", "v4r", "v5aru", "v5b", "v5g", "v5k", "v5k-s", "v5m",
            "v5n", "v5r", "v5r-i", "v5s", "v5t", "v5u", "v5u-s", "vk", "vs-c", "vs-i", "vs-s", "vz"
        };

        foreach (List<IResult> jMdictResultList in Storage.Dicts.Values.First(dict => dict.Type == DictType.JMdict).Contents.Values.ToList())
        {
            int jMdictResultListCount = jMdictResultList.Count;
            for (int i = 0; i < jMdictResultListCount; i++)
            {
                var value = (JMdictResult)jMdictResultList[i];

                if ((!value.WordClasses?.Any()) ?? true)
                    continue;

                List<string> wordClasses = value.WordClasses?.Where(wc => wc != null).SelectMany(wc => wc!).ToHashSet().Intersect(usedWcs).ToList() ?? new();

                if (!wordClasses.Any())
                    continue;

                if (jmdictWcDict.TryGetValue(value.PrimarySpelling, out List<JmdictWc>? psr))
                {
                    if (!psr.Any(r =>
                            r.Readings?.SequenceEqual(value.Readings ?? new List<string>()) ??
                            value.Readings == null && r.Spelling == value.PrimarySpelling))
                        psr.Add(new JmdictWc(value.PrimarySpelling, value.Readings, wordClasses));
                }

                else
                {
                    jmdictWcDict.Add(value.PrimarySpelling,
                        new List<JmdictWc> { new(value.PrimarySpelling, value.Readings, wordClasses) });
                }

                if (value.AlternativeSpellings != null)
                {
                    int alternativeSpellingCount = value.AlternativeSpellings.Count;
                    for (int j = 0; j < alternativeSpellingCount; j++)
                    {
                        string spelling = value.AlternativeSpellings[j];

                        if (jmdictWcDict.TryGetValue(spelling, out List<JmdictWc>? asr))
                        {
                            if (!asr.Any(r =>
                                    r.Readings?.SequenceEqual(value.Readings ?? new List<string>()) ??
                                    value.Readings == null && r.Spelling == spelling))
                                asr.Add(new JmdictWc(spelling, value.Readings, wordClasses));
                        }

                        else
                        {
                            jmdictWcDict.Add(spelling,
                                new List<JmdictWc> { new(spelling, value.Readings, wordClasses) });
                        }
                    }
                }
            }
        }

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        await File.WriteAllBytesAsync($"{Storage.ResourcesPath}/PoS.json",
            JsonSerializer.SerializeToUtf8Bytes(jmdictWcDict, options)).ConfigureAwait(false);
    }
}

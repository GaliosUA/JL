using System.Globalization;
using System.Text;
using JL.Core.Dicts.Options;
using JL.Core.Freqs;
using JL.Core.Utilities;

namespace JL.Core.Dicts.EPWING.EpwingNazeka;

internal sealed class EpwingNazekaRecord : IEpwingRecord, IGetFrequency
{
    public string PrimarySpelling { get; }
    public string? Reading { get; }
    public string[]? AlternativeSpellings { get; }
    public string[] Definitions { get; set; }

    public EpwingNazekaRecord(string primarySpelling, string? reading, string[]? alternativeSpellings, string[] definitions)
    {
        PrimarySpelling = primarySpelling;
        Reading = reading;
        AlternativeSpellings = alternativeSpellings;
        Definitions = definitions;
    }

    public string BuildFormattedDefinition(DictOptions? options)
    {
        StringBuilder defResult = new();

        string separator = options is { NewlineBetweenDefinitions.Value: false }
            ? ""
            : "\n";

        for (int i = 0; i < Definitions.Length; i++)
        {
            _ = defResult.Append(CultureInfo.InvariantCulture, $"{Definitions[i]}{separator}");
        }

        return defResult.Remove(defResult.Length - separator.Length, separator.Length).ToString();
    }

    public int GetFrequency(Freq freq)
    {
        int frequency = int.MaxValue;

        if (freq.Contents.TryGetValue(JapaneseUtils.KatakanaToHiragana(PrimarySpelling),
                out IList<FrequencyRecord>? freqResults))
        {
            int freqResultsCount = freqResults.Count;
            for (int i = 0; i < freqResultsCount; i++)
            {
                FrequencyRecord freqResult = freqResults[i];

                if (Reading == freqResult.Spelling || PrimarySpelling == freqResult.Spelling)
                {
                    if (frequency > freqResult.Frequency)
                    {
                        frequency = freqResult.Frequency;
                    }
                }
            }

            if (frequency is int.MaxValue && AlternativeSpellings is not null)
            {
                for (int i = 0; i < AlternativeSpellings.Length; i++)
                {
                    if (freq.Contents.TryGetValue(
                            JapaneseUtils.KatakanaToHiragana(AlternativeSpellings[i]),
                            out IList<FrequencyRecord>? alternativeSpellingFreqResults))
                    {
                        int alternativeSpellingFreqResultsCount = alternativeSpellingFreqResults.Count;
                        for (int j = 0; j < alternativeSpellingFreqResultsCount; j++)
                        {
                            FrequencyRecord alternativeSpellingFreqResult = alternativeSpellingFreqResults[j];

                            if (Reading == alternativeSpellingFreqResult.Spelling)
                            {
                                if (frequency > alternativeSpellingFreqResult.Frequency)
                                {
                                    frequency = alternativeSpellingFreqResult.Frequency;
                                }
                            }
                        }
                    }
                }
            }
        }

        else if (Reading is not null)
        {
            if (freq.Contents.TryGetValue(JapaneseUtils.KatakanaToHiragana(Reading),
                    out IList<FrequencyRecord>? readingFreqResults))
            {
                int readingFreqResultsCount = readingFreqResults.Count;
                for (int j = 0; j < readingFreqResultsCount; j++)
                {
                    FrequencyRecord readingFreqResult = readingFreqResults[j];

                    if ((Reading == readingFreqResult.Spelling && JapaneseUtils.IsKatakana(Reading))
                        || (AlternativeSpellings?.Contains(readingFreqResult.Spelling) ?? false))
                    {
                        if (frequency > readingFreqResult.Frequency)
                        {
                            frequency = readingFreqResult.Frequency;
                        }
                    }
                }
            }
        }

        return frequency;
    }
}

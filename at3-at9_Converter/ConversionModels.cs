namespace at3_at9_Converter
{
    public enum InputFileKind
    {
        Invalid,
        Wav,
        Encoded,
        Mp3
    }

    public enum At9ConversionMode
    {
        WavToAt9,
        At9ToWav,
        Mp3ToAt9,
        At9ToMp3
    }

    public enum At3ConversionMode
    {
        WavToAt3,
        At3ToWav,
        Mp3ToAt3,
        At3ToMp3
    }

    public enum PlaybackFileKind
    {
        Unsupported,
        At9,
        At3
    }

    public enum ConversionMode
    {
        None,
        WavToAt9,
        WavToAt3,
        Mp3ToAt9,
        Mp3ToAt3,
        At9ToWav,
        At9ToMp3,
        At3ToWav,
        At3ToMp3
    }

    public enum ToolOptionLevel
    {
        Basic,
        Advanced,
        Expert
    }

    public enum ToolLoopMode
    {
        WholeLoop,
        NoLoop,
        CustomLoop,
        DefaultWholeLoop
    }

    public enum ToolPcmOutputFormat
    {
        Int16,
        Int24,
        Float
    }

    public sealed class ConversionToolSettings
    {
        public ToolOptionLevel Level { get; set; } = ToolOptionLevel.Basic;
        public ToolLoopMode LoopMode { get; set; } = ToolLoopMode.WholeLoop;
        public int LoopStart { get; set; }
        public int LoopEnd { get; set; }
        public int DecodeRepeat { get; set; } = 1;
        public bool UseSamplingRate { get; set; }
        public int SamplingRate { get; set; } = 48000;
        public bool UseLoopList { get; set; }
        public string LoopListPath { get; set; } = "";
        public int SuperframeMode { get; set; }
        public bool DualMode { get; set; }
        public bool UseQuantizedBands { get; set; }
        public int QuantizedBands { get; set; } = 8;
        public bool UseIntensityBand { get; set; }
        public int IntensityBand { get; set; } = -1;
        public bool UseGradientMode { get; set; }
        public int GradientMode { get; set; } = 4;
        public bool WideBand { get; set; }
        public bool BandExtension { get; set; }
        public bool LfeSuperLowCut { get; set; }
        public bool WaveExtensibleHeader { get; set; }
        public ToolPcmOutputFormat PcmOutputFormat { get; set; } = ToolPcmOutputFormat.Int16;
        public string CustomEncodeArgs { get; set; } = "";
        public string CustomDecodeArgs { get; set; } = "";

        public ConversionToolSettings Clone()
        {
            return (ConversionToolSettings)MemberwiseClone();
        }

        public void CopyFrom(ConversionToolSettings source)
        {
            if (source == null)
                return;

            Level = source.Level;
            LoopMode = source.LoopMode;
            LoopStart = source.LoopStart;
            LoopEnd = source.LoopEnd;
            DecodeRepeat = source.DecodeRepeat;
            UseSamplingRate = source.UseSamplingRate;
            SamplingRate = source.SamplingRate;
            UseLoopList = source.UseLoopList;
            LoopListPath = source.LoopListPath;
            SuperframeMode = source.SuperframeMode;
            DualMode = source.DualMode;
            UseQuantizedBands = source.UseQuantizedBands;
            QuantizedBands = source.QuantizedBands;
            UseIntensityBand = source.UseIntensityBand;
            IntensityBand = source.IntensityBand;
            UseGradientMode = source.UseGradientMode;
            GradientMode = source.GradientMode;
            WideBand = source.WideBand;
            BandExtension = source.BandExtension;
            LfeSuperLowCut = source.LfeSuperLowCut;
            WaveExtensibleHeader = source.WaveExtensibleHeader;
            PcmOutputFormat = source.PcmOutputFormat;
            CustomEncodeArgs = source.CustomEncodeArgs;
            CustomDecodeArgs = source.CustomDecodeArgs;
        }
    }

    public static class ConversionModeInfo
    {
        public static bool IsAt9Mode(ConversionMode mode)
        {
            return mode == ConversionMode.WavToAt9
                || mode == ConversionMode.Mp3ToAt9
                || mode == ConversionMode.At9ToWav
                || mode == ConversionMode.At9ToMp3;
        }

        public static bool IsAt3Mode(ConversionMode mode)
        {
            return mode == ConversionMode.WavToAt3
                || mode == ConversionMode.Mp3ToAt3
                || mode == ConversionMode.At3ToWav
                || mode == ConversionMode.At3ToMp3;
        }

        public static bool NeedsBitRate(ConversionMode mode)
        {
            return mode == ConversionMode.WavToAt9
                || mode == ConversionMode.WavToAt3
                || mode == ConversionMode.Mp3ToAt9
                || mode == ConversionMode.Mp3ToAt3;
        }

        public static bool NeedsConsole(ConversionMode mode)
        {
            return mode != ConversionMode.None;
        }

        public static string GetDisplayText(ConversionMode mode)
        {
            switch (mode)
            {
                case ConversionMode.WavToAt9:
                    return "Wav -> AT9";
                case ConversionMode.WavToAt3:
                    return "Wav -> AT3";
                case ConversionMode.Mp3ToAt9:
                    return "MP3 -> AT9";
                case ConversionMode.Mp3ToAt3:
                    return "MP3 -> AT3";
                case ConversionMode.At9ToWav:
                    return "AT9 -> Wav";
                case ConversionMode.At9ToMp3:
                    return "AT9 -> MP3";
                case ConversionMode.At3ToWav:
                    return "AT3 -> Wav";
                case ConversionMode.At3ToMp3:
                    return "AT3 -> MP3";
                default:
                    return "";
            }
        }
    }

    public sealed class ConversionState
    {
        public string SelectedFile { get; set; } = "";
        public string ConvertFile { get; set; } = "";
        public string FinalFile { get; set; } = "";
        public string IntermediateWavFile { get; set; } = "";
        public string FileExtension { get; set; } = "";
        public string CurrentFilePath { get; set; } = "";
        public string DroppedFile { get; set; } = "";
        public string DirectoryPath { get; set; } = "";
        public string OriginalFileName { get; set; } = "";
        public string ToolName { get; set; } = "";
        public string BitRate { get; set; } = "";
        public ConversionToolSettings ToolSettings { get; private set; } = new ConversionToolSettings();

        public void Reset()
        {
            SelectedFile = "";
            ConvertFile = "";
            FinalFile = "";
            IntermediateWavFile = "";
            FileExtension = "";
            CurrentFilePath = "";
            DroppedFile = "";
            DirectoryPath = "";
            OriginalFileName = "";
            ToolName = "";
            BitRate = "";
            ToolSettings = new ConversionToolSettings();
        }

        public void ResetToolSettings()
        {
            ToolSettings = new ConversionToolSettings();
        }

        public void SetDroppedFile(string filePath, string originalFileName, string directoryPath, string extension)
        {
            OriginalFileName = originalFileName;
            DirectoryPath = directoryPath;
            FileExtension = extension;
            CurrentFilePath = filePath;
        }
    }

    public sealed class ConversionWorkflowResult
    {
        public bool Succeeded { get; private set; }
        public bool AskDeleteWav { get; private set; }
        public bool AskPlay { get; private set; }
        public string ErrorKey { get; private set; }
        public string ErrorMessage { get; private set; }

        public static ConversionWorkflowResult Success(bool askDeleteWav, bool askPlay)
        {
            return new ConversionWorkflowResult
            {
                Succeeded = true,
                AskDeleteWav = askDeleteWav,
                AskPlay = askPlay
            };
        }

        public static ConversionWorkflowResult Failed()
        {
            return new ConversionWorkflowResult();
        }

        public static ConversionWorkflowResult Failed(string errorKey, string errorMessage)
        {
            return new ConversionWorkflowResult
            {
                ErrorKey = errorKey,
                ErrorMessage = errorMessage
            };
        }
    }

    public sealed class ConversionCommandLogContext
    {
        public ToolOptionLevel Level { get; set; } = ToolOptionLevel.Basic;
        public string ConsoleName { get; set; } = "";
        public string ConversionMode { get; set; } = "";
        public string InputFile { get; set; } = "";
        public string OutputFile { get; set; } = "";
        public string BitRate { get; set; } = "";
        public string Options { get; set; } = "";
        public string CustomEncodeArgs { get; set; } = "";
        public string CustomDecodeArgs { get; set; } = "";
    }

    public sealed class At9ConversionRequest
    {
        public At9ConversionMode Mode { get; set; }
        public string SelectedFile { get; set; }
        public string FinalFile { get; set; }
        public string IntermediateWavFile { get; set; }
        public string ToolName { get; set; }
        public string BitRate { get; set; }
        public ConversionToolSettings ToolSettings { get; set; }
    }

    public sealed class At3ConversionRequest
    {
        public At3ConversionMode Mode { get; set; }
        public string SelectedFile { get; set; }
        public string FinalFile { get; set; }
        public string IntermediateWavFile { get; set; }
        public string ToolName { get; set; }
        public string BitRate { get; set; }
        public string ConsoleName { get; set; }
        public ConversionToolSettings ToolSettings { get; set; }
    }

    public static class ConversionOptions
    {
        public static readonly string[] At3Consoles = new string[]
        {
            "PSP",
            "PS3"
        };

        public static readonly string[] At9Consoles = new string[]
        {
            "PS4",
            "PSVita"
        };

        public static readonly string[] PspBitRates = new string[]
        {
            "32",
            "48",
            "52",
            "64",
            "66",
            "96",
            "105",
            "128",
            "132",
            "160",
            "192",
            "256",
            "320",
            "352"
        };

        public static readonly string[] Ps3BitRates = new string[]
        {
            "32",
            "48",
            "57",
            "64",
            "72",
            "96",
            "114",
            "128",
            "144",
            "160",
            "192",
            "256",
            "320",
            "384",
            "512",
            "768"
        };

        public static readonly string[] PsvitaBitRates = new string[]
        {
            "36",
            "48",
            "60",
            "72",
            "84",
            "96",
            "120",
            "144",
            "168",
            "192"
        };

        public static readonly string[] Ps4BitRates = new string[]
        {
            "36",
            "48",
            "60",
            "72",
            "84",
            "96",
            "120",
            "144",
            "168",
            "192",
            "240",
            "288",
            "300",
            "384",
            "336",
            "360",
            "384",
            "420",
            "480",
            "504",
            "672"
        };
    }
}

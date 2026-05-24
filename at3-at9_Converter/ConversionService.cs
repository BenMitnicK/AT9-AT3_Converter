using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace at3_at9_Converter
{
    public sealed class ConversionService
    {
        private readonly string baseDirectory;
        private readonly string logPath;
        private readonly string commandLogPath;

        public ConversionService(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
            logPath = Path.Combine(baseDirectory, "conversion_errors.log");
            commandLogPath = Path.Combine(baseDirectory, "conversion_commands.log");
        }

        public async Task<bool> RunExternalProcessAsync(string fileName, string arguments, ConversionCommandLogContext logContext = null)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = Path.Combine(baseDirectory, fileName),
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = baseDirectory
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();

                    await Task.Run(() => process.WaitForExit());

                    string output = await outputTask;
                    string error = await errorTask;
                    LogCommandResult(fileName, arguments, output, error, process.ExitCode, logContext);

                    if (process.ExitCode != 0)
                    {
                        LogProcessError(fileName, arguments, output, error, process.ExitCode);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logPath, "\r\nCRITICAL EXCEPTION: " + DateTime.Now + " - " + ex.Message + "\r\n");
                LogCommandException(fileName, arguments, ex, logContext);
                return false;
            }
        }

        public string CreateTempWavPath(string sourceFile)
        {
            string directory = Path.GetDirectoryName(sourceFile);
            string name = Path.GetFileNameWithoutExtension(sourceFile);
            return Path.Combine(directory, name + "." + Guid.NewGuid().ToString("N") + ".tmp.wav");
        }

        private void LogProcessError(string fileName, string arguments, string output, string error, int exitCode)
        {
            StringBuilder logEntry = new StringBuilder();
            logEntry.AppendLine();
            logEntry.AppendLine("--- " + DateTime.Now + " ---");
            logEntry.AppendLine("File: " + fileName);
            logEntry.AppendLine("ExitCode: " + exitCode);
            logEntry.AppendLine("Command: " + fileName + " " + arguments);
            logEntry.AppendLine("Output:");
            logEntry.AppendLine(output);
            logEntry.AppendLine("Errors:");
            logEntry.AppendLine(error);
            logEntry.AppendLine("--------------------------");

            File.AppendAllText(logPath, logEntry.ToString());
        }

        private void LogCommandResult(string fileName, string arguments, string output, string error, int exitCode, ConversionCommandLogContext context)
        {
            StringBuilder logEntry = new StringBuilder();
            ToolOptionLevel level = context == null ? ToolOptionLevel.Basic : context.Level;

            logEntry.AppendLine();
            logEntry.AppendLine("--- " + DateTime.Now + " ---");
            logEntry.AppendLine("Level: " + level);
            AppendContext(logEntry, context);
            logEntry.AppendLine("Tool: " + fileName);
            logEntry.AppendLine("Arguments: " + arguments);
            logEntry.AppendLine("Command: " + fileName + " " + arguments);
            logEntry.AppendLine("Result: " + (exitCode == 0 ? "Success" : "Failed"));
            logEntry.AppendLine("ExitCode: " + exitCode);

            if (level == ToolOptionLevel.Advanced || level == ToolOptionLevel.Expert || exitCode != 0)
            {
                logEntry.AppendLine("Output:");
                logEntry.AppendLine(output);
                logEntry.AppendLine("Errors:");
                logEntry.AppendLine(error);
            }

            if (level == ToolOptionLevel.Expert)
            {
                logEntry.AppendLine("Expert details:");
                logEntry.AppendLine("CustomEncodeArgs: " + Safe(context == null ? "" : context.CustomEncodeArgs));
                logEntry.AppendLine("CustomDecodeArgs: " + Safe(context == null ? "" : context.CustomDecodeArgs));
            }

            logEntry.AppendLine("--------------------------");
            File.AppendAllText(commandLogPath, logEntry.ToString());
        }

        private void LogCommandException(string fileName, string arguments, Exception ex, ConversionCommandLogContext context)
        {
            StringBuilder logEntry = new StringBuilder();
            logEntry.AppendLine();
            logEntry.AppendLine("--- " + DateTime.Now + " ---");
            logEntry.AppendLine("Level: " + (context == null ? ToolOptionLevel.Basic : context.Level));
            AppendContext(logEntry, context);
            logEntry.AppendLine("Tool: " + fileName);
            logEntry.AppendLine("Arguments: " + arguments);
            logEntry.AppendLine("Command: " + fileName + " " + arguments);
            logEntry.AppendLine("Result: Exception");
            logEntry.AppendLine("Exception: " + ex.Message);
            logEntry.AppendLine("--------------------------");
            File.AppendAllText(commandLogPath, logEntry.ToString());
        }

        private static void AppendContext(StringBuilder logEntry, ConversionCommandLogContext context)
        {
            if (context == null)
                return;

            logEntry.AppendLine("Console: " + Safe(context.ConsoleName));
            logEntry.AppendLine("Conversion: " + Safe(context.ConversionMode));
            logEntry.AppendLine("BitRate: " + Safe(context.BitRate));
            logEntry.AppendLine("Input: " + Safe(context.InputFile));
            logEntry.AppendLine("Output: " + Safe(context.OutputFile));
            logEntry.AppendLine("Options: " + Safe(context.Options));
        }

        private static string Safe(string value)
        {
            return string.IsNullOrEmpty(value) ? "(none)" : value;
        }
    }

    public sealed class ConversionFileService
    {
        private readonly string baseDirectory;

        public ConversionFileService(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public void DeleteIntermediateWavAndMoveFinal(ConversionState state)
        {
            if (File.Exists(state.IntermediateWavFile))
                File.Delete(state.IntermediateWavFile);

            MoveFinalFile(state);
        }

        public void MoveFinalAndIntermediateFiles(ConversionState state)
        {
            try
            {
                MoveFinalFile(state);
                MoveFileFromBaseDirectory(state.IntermediateWavFile);
            }
            catch
            {
            }
        }

        public void MoveFinalFile(ConversionState state)
        {
            try
            {
                MoveFileFromBaseDirectory(state.FinalFile);
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(baseDirectory, "conversion_errors.log"),
                    "\r\nError in MoveFinalFile: " + ex.Message);
            }
        }

        private void MoveFileFromBaseDirectory(string destinationPath)
        {
            if (string.IsNullOrEmpty(destinationPath))
                return;

            string fileName = Path.GetFileName(destinationPath);
            string sourcePath = Path.Combine(baseDirectory, fileName);

            if (File.Exists(sourcePath) && !string.Equals(sourcePath, destinationPath, StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(destinationPath))
                    File.Delete(destinationPath);

                File.Move(sourcePath, destinationPath);
            }
        }
    }

    internal static class ConversionToolArgumentBuilder
    {
        public static string BuildEncodeOptions(ConversionToolSettings settings, bool isAt9, bool isPs4)
        {
            if (settings == null || settings.Level == ToolOptionLevel.Basic)
                return " -wholeloop";

            string arguments = "";

            switch (settings.LoopMode)
            {
                case ToolLoopMode.NoLoop:
                    break;
                case ToolLoopMode.CustomLoop:
                    arguments += " -loop " + settings.LoopStart + " " + settings.LoopEnd;
                    break;
                case ToolLoopMode.DefaultWholeLoop:
                    arguments += isPs4 ? " -defaultWL" : " -wholeloop";
                    break;
                default:
                    arguments += " -wholeloop";
                    break;
            }

            if (settings.Level != ToolOptionLevel.Expert)
                return arguments;

            if (isAt9 && settings.UseSamplingRate)
                arguments += " -fs " + settings.SamplingRate;

            if (isAt9 && settings.UseLoopList && !string.IsNullOrWhiteSpace(settings.LoopListPath))
                arguments += " -looplist \"" + settings.LoopListPath + "\"";

            if (isAt9 && settings.SuperframeMode == 1)
                arguments += " -supframeon";
            else if (isAt9 && settings.SuperframeMode == 2)
                arguments += " -supframeoff";

            if (isAt9 && settings.DualMode)
                arguments += " -dual";

            if (isAt9 && settings.UseQuantizedBands)
                arguments += " -nbands " + settings.QuantizedBands;

            if (isAt9 && settings.UseIntensityBand)
                arguments += " -isband " + settings.IntensityBand;

            if (isAt9 && settings.UseGradientMode)
                arguments += " -gradmode " + settings.GradientMode;

            if (isPs4 && settings.WideBand)
                arguments += " -wband";

            if (isPs4 && settings.BandExtension)
                arguments += " -bex";

            if (isPs4 && settings.LfeSuperLowCut)
                arguments += " -slc";

            if (!string.IsNullOrWhiteSpace(settings.CustomEncodeArgs))
                arguments += " " + settings.CustomEncodeArgs.Trim();

            return arguments;
        }

        public static string BuildDecodeOptions(ConversionToolSettings settings, bool isPs4)
        {
            if (settings == null || settings.Level == ToolOptionLevel.Basic)
                return " -repeat 1";

            int repeat = settings.DecodeRepeat < 1 ? 1 : settings.DecodeRepeat;
            string arguments = " -repeat " + repeat;

            if (settings.Level == ToolOptionLevel.Expert && isPs4 && settings.WaveExtensibleHeader)
                arguments += " -wext";

            if ((settings.Level == ToolOptionLevel.Advanced || settings.Level == ToolOptionLevel.Expert) && isPs4)
            {
                if (settings.PcmOutputFormat == ToolPcmOutputFormat.Int24)
                    arguments += " -int24";
                else if (settings.PcmOutputFormat == ToolPcmOutputFormat.Float)
                    arguments += " -float";
                else
                    arguments += " -int16";
            }

            if (settings.Level == ToolOptionLevel.Expert && !string.IsNullOrWhiteSpace(settings.CustomDecodeArgs))
                arguments += " " + settings.CustomDecodeArgs.Trim();

            return arguments;
        }
    }

    public sealed class At9ConversionWorkflow
    {
        private readonly ConversionService conversionService;

        public At9ConversionWorkflow(ConversionService conversionService)
        {
            this.conversionService = conversionService;
        }

        public async Task<ConversionWorkflowResult> ExecuteAsync(At9ConversionRequest request, Action<string> updateStatus)
        {
            switch (request.Mode)
            {
                case At9ConversionMode.WavToAt9:
                    return await WavToAt9(request, updateStatus);
                case At9ConversionMode.At9ToWav:
                    return await At9ToWav(request, updateStatus);
                case At9ConversionMode.Mp3ToAt9:
                    return await Mp3ToAt9(request, updateStatus);
                case At9ConversionMode.At9ToMp3:
                    return await At9ToMp3(request, updateStatus);
                default:
                    return ConversionWorkflowResult.Failed();
            }
        }

        private async Task<ConversionWorkflowResult> WavToAt9(At9ConversionRequest request, Action<string> updateStatus)
        {
            string wavToProcess = request.SelectedFile;
            bool isTempWav = false;

            try
            {
                using (var reader = new WaveFileReader(request.SelectedFile))
                {
                    if (reader.WaveFormat.SampleRate != 48000)
                    {
                        updateStatus("resampling_wav");
                        wavToProcess = conversionService.CreateTempWavPath(request.SelectedFile);
                        using (var resampler = new WaveFormatConversionStream(new WaveFormat(48000, 16, reader.WaveFormat.Channels), reader))
                        {
                            WaveFileWriter.CreateWaveFile(wavToProcess, resampler);
                        }
                        isTempWav = true;
                    }
                }

                updateStatus("at9_progress");
                string options = ConversionToolArgumentBuilder.BuildEncodeOptions(request.ToolSettings, true, IsPs4Tool(request.ToolName));
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + options + " \"" + wavToProcess + "\" \"" + request.FinalFile + "\"", updateStatus,
                    CreateLogContext(request, "WAV -> AT9", wavToProcess, request.FinalFile, options));
                return success
                    ? ConversionWorkflowResult.Success(false, true)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed("wav_preprocess_error", ex.Message);
            }
            finally
            {
                if (isTempWav && File.Exists(wavToProcess))
                    File.Delete(wavToProcess);
            }
        }

        private async Task<ConversionWorkflowResult> At9ToWav(At9ConversionRequest request, Action<string> updateStatus)
        {
            try
            {
                updateStatus("wav_progress");
                string options = ConversionToolArgumentBuilder.BuildDecodeOptions(request.ToolSettings, IsPs4Tool(request.ToolName));
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d" + options + " \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus,
                    CreateLogContext(request, "AT9 -> WAV", request.SelectedFile, request.IntermediateWavFile, options));
                return success
                    ? ConversionWorkflowResult.Success(false, false)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<ConversionWorkflowResult> Mp3ToAt9(At9ConversionRequest request, Action<string> updateStatus)
        {
            try
            {
                updateStatus("wav_progress");
                using (Mp3FileReader mp3 = new Mp3FileReader(request.SelectedFile))
                {
                    using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(48000, 16, mp3.WaveFormat.Channels), mp3))
                    {
                        WaveFileWriter.CreateWaveFile(request.IntermediateWavFile, pcm);
                    }
                }

                updateStatus("at9_progress");
                string options = ConversionToolArgumentBuilder.BuildEncodeOptions(request.ToolSettings, true, IsPs4Tool(request.ToolName));
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + options + " \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus,
                    CreateLogContext(request, "MP3 -> AT9", request.IntermediateWavFile, request.FinalFile, options));
                return success
                    ? ConversionWorkflowResult.Success(true, true)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<ConversionWorkflowResult> At9ToMp3(At9ConversionRequest request, Action<string> updateStatus)
        {
            try
            {
                string options = ConversionToolArgumentBuilder.BuildDecodeOptions(request.ToolSettings, IsPs4Tool(request.ToolName));
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d" + options + " \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus,
                    CreateLogContext(request, "AT9 -> MP3 decode", request.SelectedFile, request.IntermediateWavFile, options));
                if (!success)
                    return ConversionWorkflowResult.Failed();

                success = await RunTool(@"LAME\lame.exe", "-V2 \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus,
                    CreateLogContext(request, "AT9 -> MP3 encode", request.IntermediateWavFile, request.FinalFile, "-V2"));
                return success
                    ? ConversionWorkflowResult.Success(true, false)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<bool> RunTool(string fileName, string arguments, Action<string> updateStatus, ConversionCommandLogContext logContext)
        {
            bool success = await conversionService.RunExternalProcessAsync(fileName, arguments, logContext);
            if (!success)
                updateStatus("conversion_log_error");

            return success;
        }

        private static ConversionCommandLogContext CreateLogContext(At9ConversionRequest request, string mode, string inputFile, string outputFile, string options)
        {
            ConversionToolSettings settings = request.ToolSettings ?? new ConversionToolSettings();
            return new ConversionCommandLogContext
            {
                Level = settings.Level,
                ConsoleName = GetConsoleName(request.ToolName),
                ConversionMode = mode,
                InputFile = inputFile,
                OutputFile = outputFile,
                BitRate = request.BitRate,
                Options = options,
                CustomEncodeArgs = settings.CustomEncodeArgs,
                CustomDecodeArgs = settings.CustomDecodeArgs
            };
        }

        private static bool IsPs4Tool(string toolName)
        {
            return string.Equals(toolName, "PS4_at9tool.exe", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetConsoleName(string toolName)
        {
            if (string.Equals(toolName, "PS4_at9tool.exe", StringComparison.OrdinalIgnoreCase))
                return "PS4";
            if (string.Equals(toolName, "PSVita_at9tool.exe", StringComparison.OrdinalIgnoreCase))
                return "PSVita";

            return "";
        }
    }

    public sealed class At3ConversionWorkflow
    {
        private readonly ConversionService conversionService;

        public At3ConversionWorkflow(ConversionService conversionService)
        {
            this.conversionService = conversionService;
        }

        public async Task<ConversionWorkflowResult> ExecuteAsync(At3ConversionRequest request, Action<string> updateStatus)
        {
            switch (request.Mode)
            {
                case At3ConversionMode.WavToAt3:
                    return await WavToAt3(request, updateStatus);
                case At3ConversionMode.At3ToWav:
                    return await At3ToWav(request, updateStatus);
                case At3ConversionMode.Mp3ToAt3:
                    return await Mp3ToAt3(request, updateStatus);
                case At3ConversionMode.At3ToMp3:
                    return await At3ToMp3(request, updateStatus);
                default:
                    return ConversionWorkflowResult.Failed();
            }
        }

        private async Task<ConversionWorkflowResult> WavToAt3(At3ConversionRequest request, Action<string> updateStatus)
        {
            string wavToProcess = request.SelectedFile;
            bool isTempWav = false;

            try
            {
                int targetRate = GetTargetRate(request.ConsoleName);
                int targetChannels = request.ConsoleName == "PSP" ? 2 : 0;

                using (var reader = new WaveFileReader(request.SelectedFile))
                {
                    if (reader.WaveFormat.SampleRate != targetRate || (targetChannels == 2 && reader.WaveFormat.Channels != 2))
                    {
                        updateStatus("normalizing_psp");
                        wavToProcess = conversionService.CreateTempWavPath(request.SelectedFile);
                        var outFormat = new WaveFormat(targetRate, 16, targetChannels == 2 ? 2 : reader.WaveFormat.Channels);
                        using (var resampler = new WaveFormatConversionStream(outFormat, reader))
                        {
                            WaveFileWriter.CreateWaveFile(wavToProcess, resampler);
                        }
                        isTempWav = true;
                    }
                }

                updateStatus("at3_progress");
                string options = ConversionToolArgumentBuilder.BuildEncodeOptions(request.ToolSettings, false, false);
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + options + " \"" + wavToProcess + "\" \"" + request.FinalFile + "\"", updateStatus,
                    CreateLogContext(request, "WAV -> AT3", wavToProcess, request.FinalFile, options));
                return success
                    ? ConversionWorkflowResult.Success(false, true)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed("psp_conversion_error", ex.Message);
            }
            finally
            {
                if (isTempWav && File.Exists(wavToProcess))
                    File.Delete(wavToProcess);
            }
        }

        private async Task<ConversionWorkflowResult> At3ToWav(At3ConversionRequest request, Action<string> updateStatus)
        {
            try
            {
                string options = ConversionToolArgumentBuilder.BuildDecodeOptions(request.ToolSettings, false);
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d" + options + " \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus,
                    CreateLogContext(request, "AT3 -> WAV", request.SelectedFile, request.IntermediateWavFile, options));
                return success
                    ? ConversionWorkflowResult.Success(false, false)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<ConversionWorkflowResult> Mp3ToAt3(At3ConversionRequest request, Action<string> updateStatus)
        {
            try
            {
                int targetRate = GetTargetRate(request.ConsoleName);
                using (Mp3FileReader mp3 = new Mp3FileReader(request.SelectedFile))
                {
                    using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(targetRate, 16, mp3.WaveFormat.Channels), mp3))
                    {
                        WaveFileWriter.CreateWaveFile(request.IntermediateWavFile, pcm);
                    }
                }

                string options = ConversionToolArgumentBuilder.BuildEncodeOptions(request.ToolSettings, false, false);
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + options + " \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus,
                    CreateLogContext(request, "MP3 -> AT3", request.IntermediateWavFile, request.FinalFile, options));
                return success
                    ? ConversionWorkflowResult.Success(true, true)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<ConversionWorkflowResult> At3ToMp3(At3ConversionRequest request, Action<string> updateStatus)
        {
            try
            {
                string options = ConversionToolArgumentBuilder.BuildDecodeOptions(request.ToolSettings, false);
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d" + options + " \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus,
                    CreateLogContext(request, "AT3 -> MP3 decode", request.SelectedFile, request.IntermediateWavFile, options));
                if (!success)
                    return ConversionWorkflowResult.Failed();

                success = await RunTool(@"LAME\lame.exe", "-V2 \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus,
                    CreateLogContext(request, "AT3 -> MP3 encode", request.IntermediateWavFile, request.FinalFile, "-V2"));
                return success
                    ? ConversionWorkflowResult.Success(true, false)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<bool> RunTool(string fileName, string arguments, Action<string> updateStatus, ConversionCommandLogContext logContext)
        {
            bool success = await conversionService.RunExternalProcessAsync(fileName, arguments, logContext);
            if (!success)
                updateStatus("conversion_log_error");

            return success;
        }

        private static ConversionCommandLogContext CreateLogContext(At3ConversionRequest request, string mode, string inputFile, string outputFile, string options)
        {
            ConversionToolSettings settings = request.ToolSettings ?? new ConversionToolSettings();
            return new ConversionCommandLogContext
            {
                Level = settings.Level,
                ConsoleName = request.ConsoleName,
                ConversionMode = mode,
                InputFile = inputFile,
                OutputFile = outputFile,
                BitRate = request.BitRate,
                Options = options,
                CustomEncodeArgs = settings.CustomEncodeArgs,
                CustomDecodeArgs = settings.CustomDecodeArgs
            };
        }

        private static int GetTargetRate(string consoleName)
        {
            return consoleName == "PSP" ? 44100 : 48000;
        }
    }
}

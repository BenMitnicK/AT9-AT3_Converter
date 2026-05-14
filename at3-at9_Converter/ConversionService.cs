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

        public ConversionService(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
            logPath = Path.Combine(baseDirectory, "conversion_errors.log");
        }

        public async Task<bool> RunExternalProcessAsync(string fileName, string arguments)
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
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + " -wholeloop \"" + wavToProcess + "\" \"" + request.FinalFile + "\"", updateStatus);
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
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus);
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
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + " -wholeloop \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus);
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
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus);
                if (!success)
                    return ConversionWorkflowResult.Failed();

                success = await RunTool(@"LAME\lame.exe", "-V2 \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus);
                return success
                    ? ConversionWorkflowResult.Success(true, false)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<bool> RunTool(string fileName, string arguments, Action<string> updateStatus)
        {
            bool success = await conversionService.RunExternalProcessAsync(fileName, arguments);
            if (!success)
                updateStatus("conversion_log_error");

            return success;
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
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + " -wholeloop \"" + wavToProcess + "\" \"" + request.FinalFile + "\"", updateStatus);
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
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus);
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

                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -e -br " + request.BitRate + " -wholeloop \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus);
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
                bool success = await RunTool(@"ATRAC\" + request.ToolName, " -d \"" + request.SelectedFile + "\" \"" + request.IntermediateWavFile + "\"", updateStatus);
                if (!success)
                    return ConversionWorkflowResult.Failed();

                success = await RunTool(@"LAME\lame.exe", "-V2 \"" + request.IntermediateWavFile + "\" \"" + request.FinalFile + "\"", updateStatus);
                return success
                    ? ConversionWorkflowResult.Success(true, false)
                    : ConversionWorkflowResult.Failed();
            }
            catch (Exception ex)
            {
                return ConversionWorkflowResult.Failed(null, ex.Message);
            }
        }

        private async Task<bool> RunTool(string fileName, string arguments, Action<string> updateStatus)
        {
            bool success = await conversionService.RunExternalProcessAsync(fileName, arguments);
            if (!success)
                updateStatus("conversion_log_error");

            return success;
        }

        private static int GetTargetRate(string consoleName)
        {
            return consoleName == "PSP" ? 44100 : 48000;
        }
    }
}

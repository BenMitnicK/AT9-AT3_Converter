using System;
using System.IO;
using System.Threading.Tasks;

namespace at3_at9_Converter
{
    public sealed class At3Controller
    {
        private readonly At3ConversionWorkflow workflow;
        private readonly ConversionState state;

        public At3Controller(At3ConversionWorkflow workflow, ConversionState state)
        {
            this.workflow = workflow;
            this.state = state;
        }

        public InputFileKind LoadFile(string filePath)
        {
            state.Reset();
            state.SetDroppedFile(
                filePath,
                Path.GetFileName(filePath),
                Path.GetDirectoryName(filePath),
                Path.GetExtension(filePath));

            state.SelectedFile = filePath;

            if (IsExtension(".wav"))
            {
                state.FinalFile = Path.ChangeExtension(state.SelectedFile, ".at3");
                return InputFileKind.Wav;
            }

            if (IsExtension(".at3"))
            {
                state.IntermediateWavFile = Path.ChangeExtension(state.SelectedFile, ".wav");
                state.FinalFile = Path.ChangeExtension(state.SelectedFile, ".mp3");
                return InputFileKind.Encoded;
            }

            if (IsExtension(".mp3"))
            {
                state.IntermediateWavFile = Path.ChangeExtension(state.SelectedFile, ".wav");
                state.FinalFile = Path.ChangeExtension(state.SelectedFile, ".at3");
                return InputFileKind.Mp3;
            }

            state.Reset();
            return InputFileKind.Invalid;
        }

        public void SelectConsole(string consoleName)
        {
            if (consoleName == "PSP")
                state.ToolName = "PSP_at3tool.exe";
            else if (consoleName == "PS3")
                state.ToolName = "PS3_at3tool.exe";
        }

        public void SelectBitRate(string bitRate)
        {
            state.BitRate = bitRate;
            UpdateBitRateOutputName(".at3");
        }

        public At3ConversionMode GetMode(bool wavToAt3, bool at3ToWav, bool mp3ToAt3)
        {
            if (wavToAt3)
                return At3ConversionMode.WavToAt3;
            if (at3ToWav)
                return At3ConversionMode.At3ToWav;
            if (mp3ToAt3)
                return At3ConversionMode.Mp3ToAt3;

            return At3ConversionMode.At3ToMp3;
        }

        public bool OutputFilesExist(At3ConversionMode mode)
        {
            if (mode == At3ConversionMode.Mp3ToAt3)
                return File.Exists(state.FinalFile) || File.Exists(state.IntermediateWavFile);

            if (mode == At3ConversionMode.At3ToWav)
                return File.Exists(state.IntermediateWavFile);

            return File.Exists(state.FinalFile);
        }

        public Task<ConversionWorkflowResult> ConvertAsync(At3ConversionMode mode, string consoleName, Action<string> updateStatus)
        {
            return workflow.ExecuteAsync(new At3ConversionRequest
            {
                Mode = mode,
                SelectedFile = state.SelectedFile,
                FinalFile = state.FinalFile,
                IntermediateWavFile = state.IntermediateWavFile,
                ToolName = state.ToolName,
                BitRate = state.BitRate,
                ConsoleName = consoleName
            }, updateStatus);
        }

        private bool IsExtension(string extension)
        {
            return string.Equals(state.FileExtension, extension, StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateBitRateOutputName(string extension)
        {
            if (string.IsNullOrEmpty(state.SelectedFile))
                return;

            string dirPath = Path.GetDirectoryName(state.SelectedFile);
            string fileNameOnly = Path.GetFileNameWithoutExtension(state.SelectedFile);
            state.FinalFile = Path.Combine(dirPath, fileNameOnly + "_" + state.BitRate + "bit" + extension);
            state.IntermediateWavFile = Path.Combine(dirPath, fileNameOnly + ".wav");
        }
    }
}

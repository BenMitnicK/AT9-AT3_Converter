using System;
using System.IO;
using System.Threading.Tasks;

namespace at3_at9_Converter
{
    public sealed class At9Controller
    {
        private readonly At9ConversionWorkflow workflow;
        private readonly ConversionState state;

        public At9Controller(At9ConversionWorkflow workflow, ConversionState state)
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
                state.FinalFile = Path.ChangeExtension(state.SelectedFile, ".at9");
                return InputFileKind.Wav;
            }

            if (IsExtension(".at9"))
            {
                state.IntermediateWavFile = Path.ChangeExtension(state.SelectedFile, ".wav");
                state.FinalFile = Path.ChangeExtension(state.SelectedFile, ".mp3");
                return InputFileKind.Encoded;
            }

            if (IsExtension(".mp3"))
            {
                state.IntermediateWavFile = Path.ChangeExtension(state.SelectedFile, ".wav");
                state.FinalFile = Path.ChangeExtension(state.SelectedFile, ".at9");
                return InputFileKind.Mp3;
            }

            state.Reset();
            return InputFileKind.Invalid;
        }

        public void SelectConsole(string consoleName)
        {
            if (consoleName == "PS4")
                state.ToolName = "PS4_at9tool.exe";
            else if (consoleName == "PSVita")
                state.ToolName = "PSVita_at9tool.exe";
        }

        public void SelectBitRate(string bitRate)
        {
            state.BitRate = bitRate;
            UpdateBitRateOutputName(".at9");
        }

        public At9ConversionMode GetMode(bool wavToAt9, bool at9ToWav, bool mp3ToAt9)
        {
            if (wavToAt9)
                return At9ConversionMode.WavToAt9;
            if (at9ToWav)
                return At9ConversionMode.At9ToWav;
            if (mp3ToAt9)
                return At9ConversionMode.Mp3ToAt9;

            return At9ConversionMode.At9ToMp3;
        }

        public bool OutputFilesExist(At9ConversionMode mode)
        {
            if (mode == At9ConversionMode.Mp3ToAt9)
                return File.Exists(state.FinalFile) || File.Exists(state.IntermediateWavFile);

            if (mode == At9ConversionMode.At9ToWav)
                return File.Exists(state.IntermediateWavFile);

            return File.Exists(state.FinalFile);
        }

        public Task<ConversionWorkflowResult> ConvertAsync(At9ConversionMode mode, Action<string> updateStatus)
        {
            return workflow.ExecuteAsync(new At9ConversionRequest
            {
                Mode = mode,
                SelectedFile = state.SelectedFile,
                FinalFile = state.FinalFile,
                IntermediateWavFile = state.IntermediateWavFile,
                ToolName = state.ToolName,
                BitRate = state.BitRate
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

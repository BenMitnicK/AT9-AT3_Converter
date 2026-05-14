using System;
using System.Diagnostics;
using System.IO;

namespace at3_at9_Converter
{
    public sealed class AudioPlayerService : IDisposable
    {
        private readonly string baseDirectory;
        private readonly At9Player at9Player = new At9Player();
        private Process playerProcess;

        public AudioPlayerService(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public bool IsAt3Playing
        {
            get { return playerProcess != null && !playerProcess.HasExited; }
        }

        public event EventHandler At9PlaybackStopped
        {
            add { at9Player.PlaybackStopped += value; }
            remove { at9Player.PlaybackStopped -= value; }
        }

        public PlaybackFileKind PlayFile(string filePath, EventHandler at3Exited)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".at9")
            {
                PlayAt9(filePath);
                return PlaybackFileKind.At9;
            }

            if (extension == ".at3")
            {
                PlayAt3(filePath, at3Exited);
                return PlaybackFileKind.At3;
            }

            return PlaybackFileKind.Unsupported;
        }

        public void PlayAt9(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            at9Player.Play(filePath);
        }

        public void PlayAt3(string filePath, EventHandler exited)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            StopAt3();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Path.Combine(baseDirectory, @"PLAYER\MiniPlayer.exe"),
                Arguments = "\"" + filePath + "\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = baseDirectory
            };

            playerProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            if (exited != null)
                playerProcess.Exited += exited;

            playerProcess.Start();

            if (playerProcess.HasExited && exited != null)
                exited(playerProcess, EventArgs.Empty);
        }

        public void StopAt3()
        {
            if (playerProcess == null)
                return;

            try
            {
                if (!playerProcess.HasExited)
                    playerProcess.Kill();
            }
            finally
            {
                playerProcess.Dispose();
                playerProcess = null;
            }
        }

        public void StopAt9()
        {
            at9Player.Stop();
        }

        public void Dispose()
        {
            StopAt3();
            at9Player.Dispose();
        }
    }
}

using at3_at9_Converter.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace at3_at9_Converter
{

    public partial class MainForm : Form
    {

        private Dictionary<string, string> lang = new Dictionary<string, string>();
        private readonly ConversionService conversionService;
        private readonly ConversionFileService conversionFileService;
        private readonly LanguageService languageService;
        private readonly AudioPlayerService audioPlayerService;
        private readonly At9ConversionWorkflow at9Workflow;
        private readonly At3ConversionWorkflow at3Workflow;
        private readonly At9Controller at9Controller;
        private readonly At3Controller at3Controller;
        private readonly ConversionState at9State = new ConversionState();
        private readonly ConversionState at3State = new ConversionState();
        private readonly HashSet<string> warnedOutdatedLanguages = new HashSet<string>();
        private readonly Color activeButtonBackColor = Color.FromArgb(37, 99, 235);
        private readonly Color disabledButtonBackColor = Color.FromArgb(229, 231, 235);
        private readonly Color disabledButtonForeColor = Color.FromArgb(55, 65, 81);
        private ConversionMode currentConversionMode = ConversionMode.None;
        private string currentStatusKey = "ready";
        private bool conversionInProgress;
        private bool previousConvertButtonEnabled;
        private bool previousConsoleComboBoxEnabled;
        private bool previousBitrateComboBoxEnabled;
        private bool previousLanguageComboBoxEnabled;
        private bool previousToolOptionsButtonEnabled;
        private bool previousFilePathTextBoxEnabled;
        private bool previousBitrateInfoPictureBoxEnabled;
        private bool previousDropLabelAllowDrop;
        private const string StudioGitHubUrl = "https://github.com/BenMitnicK";

        // ... (remaining existing variables) ...
        public static string dir = "", appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), version = "";
        // ... (keep all class variables here) ...

        private ConversionState CurrentState
        {
            get
            {
                return ConversionModeInfo.IsAt3Mode(currentConversionMode) ? at3State : at9State;
            }
        }

        public MainForm()
        {
            conversionService = new ConversionService(AppDomain.CurrentDomain.BaseDirectory);
            conversionFileService = new ConversionFileService(AppDomain.CurrentDomain.BaseDirectory);
            languageService = new LanguageService(AppDomain.CurrentDomain.BaseDirectory);
            audioPlayerService = new AudioPlayerService(AppDomain.CurrentDomain.BaseDirectory);
            at9Workflow = new At9ConversionWorkflow(conversionService);
            at3Workflow = new At3ConversionWorkflow(conversionService);
            at9Controller = new At9Controller(at9Workflow, at9State);
            at3Controller = new At3Controller(at3Workflow, at3State);

            InitializeComponent();
            audioPlayerService.At9PlaybackStopped += At9Player_PlaybackStopped;
            this.dropLabel.AllowDrop = true;
            this.dropLabel.DragEnter += new DragEventHandler(conversionDropLabel_DragEnter);
            this.dropLabel.DragDrop += new DragEventHandler(conversionDropLabel_DragDrop);
            this.FormClosed += MainForm_FormClosed;
   
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            audioPlayerService.At9PlaybackStopped -= At9Player_PlaybackStopped;
            audioPlayerService.Dispose();
        }

        void conversionDropLabel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void conversionDropLabel_DragDrop(object sender, DragEventArgs e)
        {
            if (conversionInProgress)
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0)
                return;

            LoadDroppedFile(files[0]);
        }

        private void LoadDroppedFile(string file)
        {
            ResetConversionUi();
            filePathTextBox.Text = file;

            string extension = Path.GetExtension(file).ToLower();
            if (extension == ".at9")
            {
                at9Controller.LoadFile(file);
                SelectEncodedAt9Mode();
                AskPlayDroppedAT9();
                return;
            }

            if (extension == ".at3")
            {
                at3Controller.LoadFile(file);
                SelectEncodedAt3Mode();
                AskPlayDroppedAT3();
                return;
            }

            if (extension == ".wav")
            {
                SelectTargetCodec(
                    ConversionMode.WavToAt9,
                    ConversionMode.WavToAt3,
                    () => at9Controller.LoadFile(file),
                    () => at3Controller.LoadFile(file));
                return;
            }

            if (extension == ".mp3")
            {
                SelectTargetCodec(
                    ConversionMode.Mp3ToAt9,
                    ConversionMode.Mp3ToAt3,
                    () => at9Controller.LoadFile(file),
                    () => at3Controller.LoadFile(file));
                return;
            }

            mMessageBox(T("information"), T("invalid_audio_file"), SystemIcons.Information, true);
            ResetConversionUi();
        }

        private void SelectTargetCodec(ConversionMode at9Mode, ConversionMode at3Mode, Action loadAt9, Action loadAt3)
        {
            DialogResult result = DialogService.ShowTargetCodecDialog(this, T("question"), T("target_format_question"), SystemIcons.Question);
            if (result == DialogResult.OK)
            {
                loadAt9();
                SetConversionMode(at9Mode);
                ConfigureConsoleSelection(ConversionOptions.At9Consoles);
                return;
            }

            loadAt3();
            SetConversionMode(at3Mode);
            ConfigureConsoleSelection(ConversionOptions.At3Consoles);
        }

        private void SelectEncodedAt9Mode()
        {
            DialogResult result = DialogService.ShowFormatDialog(this, T("question"), T("format_question"), SystemIcons.Question);
            SetConversionMode(result == DialogResult.OK ? ConversionMode.At9ToMp3 : ConversionMode.At9ToWav);
            ConfigureConsoleSelection(ConversionOptions.At9Consoles);
        }

        private void SelectEncodedAt3Mode()
        {
            DialogResult result = DialogService.ShowFormatDialog(this, T("question"), T("format_question"), SystemIcons.Question);
            SetConversionMode(result == DialogResult.OK ? ConversionMode.At3ToMp3 : ConversionMode.At3ToWav);
            ConfigureConsoleSelection(ConversionOptions.At3Consoles);
        }

        private void AskPlayDroppedAT9()
        {
            ConfirmPlayDroppedAt9();
        }

        private void AskPlayDroppedAT3()
        {
            ConfirmPlayDroppedAt3();
        }

        private async void convertButton_Click(object sender, EventArgs e)
        {
            if (conversionInProgress)
                return;

            if (currentConversionMode == ConversionMode.None)
                return;

            if (!ConfirmOverwriteExistingFiles())
                return;

            conversionInProgress = true;
            FreezeConversionUi();

            try
            {
                await DoConversion();
            }
            finally
            {
                RestoreConversionUi();
                conversionInProgress = false;
            }
        }

        private bool ConfirmOverwriteExistingFiles()
        {
            if (ShouldConfirmIntermediateWav() && File.Exists(CurrentState.IntermediateWavFile))
            {
                if (Confirm(T("wav_exists_continue")) != DialogResult.OK)
                    return false;
            }

            string finalOutputFile = GetFinalOutputFile();
            if (File.Exists(finalOutputFile))
            {
                if (Confirm(T("file_exists_continue")) != DialogResult.OK)
                    return false;
            }

            return true;
        }

        private bool ShouldConfirmIntermediateWav()
        {
            return currentConversionMode == ConversionMode.Mp3ToAt9
                || currentConversionMode == ConversionMode.Mp3ToAt3
                || currentConversionMode == ConversionMode.At9ToMp3
                || currentConversionMode == ConversionMode.At3ToMp3;
        }

        private string GetFinalOutputFile()
        {
            if (currentConversionMode == ConversionMode.At9ToWav
                || currentConversionMode == ConversionMode.At3ToWav)
                return CurrentState.IntermediateWavFile;

            return CurrentState.FinalFile;
        }

        private async Task DoConversion()
        {
            if (filePathTextBox.Text == "")
                return;

            ConversionWorkflowResult result;
            if (ConversionModeInfo.IsAt9Mode(currentConversionMode))
                result = await at9Controller.ConvertAsync(GetAt9ConversionMode(), SetStatus);
            else
                result = await at3Controller.ConvertAsync(GetAt3ConversionMode(), consoleComboBox.Text, SetStatus);


            HandleWorkflowResult(result);
        }

        private At9ConversionMode GetAt9ConversionMode()
        {
            switch (currentConversionMode)
            {
                case ConversionMode.WavToAt9:
                    return At9ConversionMode.WavToAt9;
                case ConversionMode.At9ToWav:
                    return At9ConversionMode.At9ToWav;
                case ConversionMode.Mp3ToAt9:
                    return At9ConversionMode.Mp3ToAt9;
                default:
                    return At9ConversionMode.At9ToMp3;
            }
        }

        private At3ConversionMode GetAt3ConversionMode()
        {
            switch (currentConversionMode)
            {
                case ConversionMode.WavToAt3:
                    return At3ConversionMode.WavToAt3;
                case ConversionMode.At3ToWav:
                    return At3ConversionMode.At3ToWav;
                case ConversionMode.Mp3ToAt3:
                    return At3ConversionMode.Mp3ToAt3;
                default:
                    return At3ConversionMode.At3ToMp3;
            }
        }

        private void HandleWorkflowResult(ConversionWorkflowResult result)
        {
            if (!result.Succeeded)
            {
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    string message = string.IsNullOrEmpty(result.ErrorKey)
                        ? result.ErrorMessage
                        : T(result.ErrorKey) + " " + result.ErrorMessage;

                    mMessageBox(T("error"), message, SystemIcons.Error);
                }

                return;
            }

            if (result.AskDeleteWav)
                ConfirmDeleteWav();

            SetStatus("finish");

            if (result.AskPlay)
                ConfirmPlayFile();
        }

        private void SetStatus(string key)
        {
            currentStatusKey = key;
            statusLabel.Text = T(key);
            mainStatusStrip.Refresh();
        }

        private void ConfirmDeleteWav()
        {
            if (Confirm(T("delete_wav_question")) == DialogResult.OK)
            {
                conversionFileService.DeleteIntermediateWavAndMoveFinal(CurrentState);
                return;
            }

            conversionFileService.MoveFinalAndIntermediateFiles(CurrentState);
        }

        private void ConfirmPlayFile()
        {
            if (Confirm(T("play_file_question")) == DialogResult.OK)
                mPlayer();
        }

        private void SetConversionMode(ConversionMode mode)
        {
            currentConversionMode = mode;
            string modeText = ConversionModeInfo.GetDisplayText(mode);
            conversionModeLabel.Text = string.IsNullOrEmpty(modeText)
                ? T("conversion_mode_empty")
                : T("conversion_mode_prefix") + " " + modeText;
        }

        private void ConfigureConsoleSelection(string[] consoles)
        {
            consoleComboBox.Items.Clear();
            bitrateComboBox.Items.Clear();
            consoleComboBox.Enabled = ConversionModeInfo.NeedsConsole(currentConversionMode);
            bitrateComboBox.Enabled = false;
            SetToolOptionsEnabled(false);
            SetConvertButtonEnabled(false);
            FillComboBox(consoleComboBox, consoles, false);
            UpdateConsoleImages();
        }

        private void ResetConversionUi()
        {
            at9State.Reset();
            at3State.Reset();
            SetConversionMode(ConversionMode.None);
            filePathTextBox.Text = "";
            consoleComboBox.Items.Clear();
            bitrateComboBox.Items.Clear();
            consoleComboBox.Enabled = false;
            bitrateComboBox.Enabled = false;
            SetToolOptionsEnabled(false);
            SetConvertButtonEnabled(false);
            UpdateConsoleImages();
            SetStatus("ready");
        }

        private DialogResult Confirm(string promptText)
        {
            return DialogService.ShowYesNoDialog(this, T("question"), promptText, SystemIcons.Question, T("yes"), T("no"));
        }

        private void ConfirmPlayDroppedAt9()
        {
            if (Confirm(T("play_at9_question")) != DialogResult.OK)
                return;

            try
            {
                audioPlayerService.PlayAt9(at9State.SelectedFile);
                SetStatus("playing_at9");
                SetStopButtonEnabled(true);
            }
            catch (Exception ex)
            {
                mMessageBox(T("error"), T("at9_playback_error") + " " + ex.Message, SystemIcons.Error);
            }
        }

        private void ConfirmPlayDroppedAt3()
        {
            if (Confirm(T("play_at3_question")) == DialogResult.OK)
                PlayAT3(at3State.SelectedFile);
        }

        private DialogResult mMessageBox(string title, string promptText, Icon icon, bool isDigit = false)
        {
            return DialogService.ShowOkDialog(this, title, promptText, icon, T("ok"));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string[] versionpart = this.ProductVersion.Split('.');
            version = versionpart[0] + "." + versionpart[1];
            this.Text += MainForm.version;
            LoadLanguageList();

            dropLabel.AutoSize = false;
            dropLabel.TextAlign = ContentAlignment.MiddleCenter;
            ApplyDisabledConsoleImages();
            SetStopButtonEnabled(false);
            SetConvertButtonEnabled(false);
            SetToolOptionsEnabled(false);
        }

        private void ApplyDisabledConsoleImages()
        {
            UpdateConsoleImages();
        }

        private void UpdateConsoleImages()
        {
            string selectedConsole = consoleComboBox.Enabled ? consoleComboBox.Text : "";

            psvitaPictureBox.Image = selectedConsole == "PSVita"
                ? Resources.psvita
                : CreateDisabledImage(Resources.psvita);

            ps4PictureBox.Image = selectedConsole == "PS4"
                ? Resources.ps4
                : CreateDisabledImage(Resources.ps4);

            ps3PictureBox.Image = selectedConsole == "PS3"
                ? Resources.ps3
                : CreateDisabledImage(Resources.ps3);

            pspPictureBox.Image = selectedConsole == "PSP"
                ? Resources.psp
                : CreateDisabledImage(Resources.psp);
        }

        private static Bitmap CreateDisabledImage(Image image)
        {
            Bitmap disabledImage = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(disabledImage))
            {
                ControlPaint.DrawImageDisabled(graphics, image, 0, 0, Color.Transparent);
            }

            return disabledImage;
        }

        private void consoleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateConsoleImages();

            if (ConversionModeInfo.IsAt9Mode(currentConversionMode))
            {
                SelectAt9Console();
                return;
            }

            if (ConversionModeInfo.IsAt3Mode(currentConversionMode))
            {
                SelectAt3Console();
            }
        }

        private void SelectAt9Console()
        {
            if (consoleComboBox.Text == "PS4")
            {
                at9Controller.SelectConsole(consoleComboBox.Text);
                if (ConversionModeInfo.NeedsBitRate(currentConversionMode))
                    tabPage1PS4Combo();
                UpdateConvertButtonAfterConsole();
            }
            else if (consoleComboBox.Text == "PSVita")
            {
                at9Controller.SelectConsole(consoleComboBox.Text);
                if (ConversionModeInfo.NeedsBitRate(currentConversionMode))
                    tabPage1PSVitaCombo();
                UpdateConvertButtonAfterConsole();
                if (ConversionModeInfo.NeedsBitRate(currentConversionMode))
                    mMessageBox(T("information"), T("psvita_bitrate_info"), SystemIcons.Information, true);
            }
        }

        private void SelectAt3Console()
        {
            if (consoleComboBox.Text == "PSP")
            {
                at3Controller.SelectConsole(consoleComboBox.Text);
                if (ConversionModeInfo.NeedsBitRate(currentConversionMode))
                    tabPage2PSPCombo();
                UpdateConvertButtonAfterConsole();
            }
            else if (consoleComboBox.Text == "PS3")
            {
                at3Controller.SelectConsole(consoleComboBox.Text);
                if (ConversionModeInfo.NeedsBitRate(currentConversionMode))
                    tabPage2PS3Combo();
                UpdateConvertButtonAfterConsole();
            }
        }

        private void UpdateConvertButtonAfterConsole()
        {
            bitrateComboBox.Enabled = ConversionModeInfo.NeedsBitRate(currentConversionMode);
            if (!ConversionModeInfo.NeedsBitRate(currentConversionMode))
                bitrateComboBox.Items.Clear();

            SetToolOptionsEnabled(true);
            SetConvertButtonEnabled(!ConversionModeInfo.NeedsBitRate(currentConversionMode)
                || bitrateComboBox.SelectedIndex >= 0);
        }

        private void bitrateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConversionModeInfo.IsAt9Mode(currentConversionMode))
                at9Controller.SelectBitRate(bitrateComboBox.Text);
            else if (ConversionModeInfo.IsAt3Mode(currentConversionMode))
                at3Controller.SelectBitRate(bitrateComboBox.Text);

            SetConvertButtonEnabled(true);
        }

        private void toolOptionsButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(consoleComboBox.Text) || currentConversionMode == ConversionMode.None)
                return;

            using (ToolOptionsDialog dialog = new ToolOptionsDialog(consoleComboBox.Text, currentConversionMode, CurrentState.ToolSettings, T))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                CurrentState.ToolSettings.CopyFrom(dialog.Settings);
            }
        }

        private void tabPage1ConsoleCombo()
        {
            FillComboBox(consoleComboBox, ConversionOptions.At9Consoles, false);
        }

        private void tabPage1PSVitaCombo()
        {
            FillComboBox(bitrateComboBox, ConversionOptions.PsvitaBitRates, true);
        }

        private void tabPage1PS4Combo()
        {
            FillComboBox(bitrateComboBox, ConversionOptions.Ps4BitRates, true);
        }

        private void tabPage2ConsoleCombo()
        {
            FillComboBox(consoleComboBox, ConversionOptions.At3Consoles, false);
        }

        private void tabPage2PSPCombo()
        {
            FillComboBox(bitrateComboBox, ConversionOptions.PspBitRates, true);
        }

        private void tabPage2PS3Combo()
        {
            FillComboBox(bitrateComboBox, ConversionOptions.Ps3BitRates, true);
        }

        private void FillComboBox(ComboBox comboBox, string[] values, bool selectFirst)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(values);

            if (selectFirst && comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void bitrateInfoPictureBox_Click(object sender, EventArgs e)
        {
            if (!bitrateComboBox.Enabled)
                return;

            if (consoleComboBox.Text == "PS4")
            {
                Ps4BitrateInfo Ps4Load = new Ps4BitrateInfo();
                Ps4Load.Show();
            }
            else if (consoleComboBox.Text == "PSVita")
            {
                PsvitaBitrateInfo PsvitaLoad = new PsvitaBitrateInfo();
                PsvitaLoad.Show();
            }
            else if (consoleComboBox.Text == "PSP")
            {
                PspBitrateInfo PspLoad = new PspBitrateInfo();
                PspLoad.Show();
            }
            else if (consoleComboBox.Text == "PS3")
            {
                Ps3BitrateInfo Ps3Load = new Ps3BitrateInfo();
                Ps3Load.Show();
            }
        }

        private void mPlayer()
        {
            string fileToPlay = CurrentState.FinalFile;

            try
            {
                PlaybackFileKind playbackKind = audioPlayerService.PlayFile(fileToPlay, At3Player_Exited);
                ApplyPlaybackUi(playbackKind);
            }
            catch (FileNotFoundException ex)
            {
                mMessageBox(T("error"), T("file_not_found") + " " + ex.FileName, SystemIcons.Error);
            }
            catch (Exception ex)
            {
                mMessageBox(T("error"), T("playback_error") + " " + ex.Message, SystemIcons.Error);
            }
        }

        private void PlayAT3(string filePath)
        {
            try
            {
                audioPlayerService.PlayAt3(filePath, At3Player_Exited);
                ApplyPlaybackUi(PlaybackFileKind.At3);
            }
            catch (FileNotFoundException ex)
            {
                mMessageBox(T("error"), T("file_not_found") + " " + ex.FileName, SystemIcons.Error);
            }
            catch (Exception ex)
            {
                mMessageBox(T("error"), T("at3_playback_error") + " " + ex.Message, SystemIcons.Error);
            }
        }

        private void ApplyPlaybackUi(PlaybackFileKind playbackKind)
        {
            if (playbackKind == PlaybackFileKind.At9)
            {
                SetStatus("playing_at9");
                SetStopButtonEnabled(true);
            }
            else if (playbackKind == PlaybackFileKind.At3)
            {
                SetStatus("playing_at3");
                SetStopButtonEnabled(true);
            }
        }

        private void studioLinkStatusLabel_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(StudioGitHubUrl)
            {
                UseShellExecute = true
            });
        }

        private void stopPlaybackButton_Click(object sender, EventArgs e)
        {

            audioPlayerService.StopAt9();
            audioPlayerService.StopAt3();

            SetStatus("stop");
            SetStopButtonEnabled(false);

        }

        private void At3Player_Exited(object sender, EventArgs e)
        {
            DisableStopButtonWhenPlaybackEnds(stopPlaybackButton);
        }

        private void At9Player_PlaybackStopped(object sender, EventArgs e)
        {
            DisableStopButtonWhenPlaybackEnds(stopPlaybackButton);
        }

        private void DisableStopButtonWhenPlaybackEnds(Button stopButton)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    SetStopButtonEnabled(false);
                    SetStatus("stop");
                }));
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void LoadLanguageList()
        {
            languageComboBox.Items.Clear();

            languageService.EnsureEnglishTemplate(GetDefaultEnglishLanguage());
            IReadOnlyList<LanguageItem> languages = languageService.GetAvailableLanguages();

            if (languages.Count == 0)
            {
                LoadDefaultEnglishLanguage();
                ApplyLanguage();

                languageComboBox.Items.Clear();
                languageComboBox.Items.Add("English");
                languageComboBox.SelectedIndex = 0;
                languageComboBox.Enabled = false;

                return;
            }

            foreach (LanguageItem item in languages)
            {
                languageComboBox.Items.Add(item);
            }

            foreach (LanguageItem item in languageComboBox.Items)
            {
                if (item.Code.ToLower() == "en")
                {
                    languageComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SetStopButtonEnabled(bool enabled)
        {
            stopPlaybackButton.Enabled = enabled;
            stopPlaybackButton.BackColor = enabled ? activeButtonBackColor : disabledButtonBackColor;
            stopPlaybackButton.ForeColor = enabled ? Color.White : disabledButtonForeColor;
        }

        private void SetConvertButtonEnabled(bool enabled)
        {
            convertButton.Enabled = enabled;
            convertButton.BackColor = enabled ? activeButtonBackColor : disabledButtonBackColor;
            convertButton.ForeColor = enabled ? Color.White : disabledButtonForeColor;
        }

        private void FreezeConversionUi()
        {
            previousConvertButtonEnabled = convertButton.Enabled;
            previousConsoleComboBoxEnabled = consoleComboBox.Enabled;
            previousBitrateComboBoxEnabled = bitrateComboBox.Enabled;
            previousLanguageComboBoxEnabled = languageComboBox.Enabled;
            previousToolOptionsButtonEnabled = toolOptionsButton.Enabled;
            previousFilePathTextBoxEnabled = filePathTextBox.Enabled;
            previousBitrateInfoPictureBoxEnabled = bitrateInfoPictureBox.Enabled;
            previousDropLabelAllowDrop = dropLabel.AllowDrop;

            SetConvertButtonEnabled(false);
            consoleComboBox.Enabled = false;
            bitrateComboBox.Enabled = false;
            languageComboBox.Enabled = false;
            toolOptionsButton.Enabled = false;
            filePathTextBox.Enabled = false;
            bitrateInfoPictureBox.Enabled = false;
            dropLabel.AllowDrop = false;
        }

        private void RestoreConversionUi()
        {
            SetConvertButtonEnabled(previousConvertButtonEnabled);
            consoleComboBox.Enabled = previousConsoleComboBoxEnabled;
            bitrateComboBox.Enabled = previousBitrateComboBoxEnabled;
            languageComboBox.Enabled = previousLanguageComboBoxEnabled;
            toolOptionsButton.Enabled = previousToolOptionsButtonEnabled;
            filePathTextBox.Enabled = previousFilePathTextBoxEnabled;
            bitrateInfoPictureBox.Enabled = previousBitrateInfoPictureBoxEnabled;
            dropLabel.AllowDrop = previousDropLabelAllowDrop;
        }

        private void LoadLanguage(string langCode)
        {
            LanguageLoadResult result = languageService.LoadLanguage(langCode, GetDefaultEnglishLanguage());
            lang = result.Language;

            if (result.IsOutdated && !warnedOutdatedLanguages.Contains(langCode))
            {
                warnedOutdatedLanguages.Add(langCode);
                mMessageBox(T("information"), T("language_outdated_warning"), SystemIcons.Warning, true);
            }
        }

        private string T(string key)
        {
            return lang.ContainsKey(key) ? lang[key] : key;
        }

        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (languageComboBox.SelectedItem is LanguageItem selectedLanguage)
            {
                LoadLanguage(selectedLanguage.Code);
                ApplyLanguage();
            }
        }

        private void ApplyLanguage()
        {
            dropLabel.Text = T("drag_drop");
            convertButton.Text = T("convert");
            toolOptionsButton.Text = T("tool_options");
            stopPlaybackButton.Text = T("stop_playing");
            conversionGroupBox.Text = T("conversion_type");
            bitrateLabel.Text = T("bitrate");
            consoleLabel.Text = T("console_type");
            ApplyToolTips();
            SetConversionMode(currentConversionMode);
            SetStatus(currentStatusKey);
        }

        private void ApplyToolTips()
        {
            mainToolTip.SetToolTip(conversionGroupBox, T("tooltip_conversion_type"));
            mainToolTip.SetToolTip(consoleLabel, T("tooltip_console_type"));
            mainToolTip.SetToolTip(bitrateInfoPictureBox, T("tooltip_bitrate_info"));
            mainToolTip.SetToolTip(bitrateLabel, T("tooltip_bitrate"));
            mainToolTip.SetToolTip(dropLabel, T("tooltip_drag_drop"));
            mainToolTip.SetToolTip(convertButton, T("tooltip_convert"));
            mainToolTip.SetToolTip(filePathTextBox, T("tooltip_file_path"));
            mainToolTip.SetToolTip(toolOptionsButton, T("tooltip_tool_options"));
        }

        private void LoadDefaultEnglishLanguage()
        {
            lang = GetDefaultEnglishLanguage();
        }

        private Dictionary<string, string> GetDefaultEnglishLanguage()
        {
            return new Dictionary<string, string>
            {
                { "version", "1" },
                { "language", "English" }, 
                { "drag_drop", "Drag and drop your file here" },
                { "convert", "Convert" },
                { "tool_options", "Options..." },
                { "stop_playing", "Stop Playing" },
                { "conversion_type", "Conversion type" },
                { "bitrate", "BitRate [kbps]:" },
                { "console_type", "Console Type:" },               
                { "question", "Question" },
                { "information", "Information" },
                { "error", "Error" },
                { "yes", "Yes" },
                { "no", "No" },
                { "cancel", "Cancel" },
                { "ok", "OK" },
                { "tool_options_title", "Conversion options" },
                { "tool_options_mode", "Mode" },
                { "tool_options_encode_options", "Encode options" },
                { "tool_options_decode_options", "Decode options" },
                { "tool_options_expert_encode_switches", "Expert encode switches" },
                { "tool_options_expert_custom_arguments", "Expert custom arguments" },
                { "tool_options_loop", "Loop" },
                { "tool_options_loop_start", "Loop start" },
                { "tool_options_loop_end", "Loop end" },
                { "tool_options_repeat", "Repeat" },
                { "tool_options_wav_output", "WAV output" },
                { "tool_options_wave_extensible", "WAVE_FORMAT_EXTENSIBLE header" },
                { "tool_options_sampling_rate", "Sampling rate (-fs)" },
                { "tool_options_loop_list", "Loop list file" },
                { "tool_options_superframe", "Superframe" },
                { "tool_options_dual_mode", "Dual mode" },
                { "tool_options_quantized_bands", "Quantized bands" },
                { "tool_options_intensity_band", "Intensity band" },
                { "tool_options_gradient_mode", "Gradient mode" },
                { "tool_options_band_mode", "Band mode" },
                { "tool_options_band_mode_default", "Default" },
                { "tool_options_wide_band", "Wide band (-wband)" },
                { "tool_options_band_extension", "Band extension (-bex)" },
                { "tool_options_lfe_super_low_cut", "LFE super low cut (-slc)" },
                { "tool_options_encode", "Encode" },
                { "tool_options_decode", "Decode" },
                { "format_question", "What format do you want to convert it to?" },
                { "target_format_question", "What codec do you want to convert it to?" },
                { "conversion_mode_prefix", "Conversion:" },
                { "conversion_mode_empty", "Conversion:" },
                { "file_exists_continue", "File(s) already exist. Do you want to continue?" },
                { "wav_exists_continue", "The WAV file already exist. Do you want to overwrite it?" },
                { "delete_wav_question", "Do you want to delete the WAV file?" },
                { "play_file_question", "Do you want to play the file?" },
                { "play_at9_question", "Do you want to play this AT9 file?" },
                { "play_at3_question", "Do you want to play this AT3 file?" },
                { "invalid_audio_file", "Please select an MP3, WAV, AT9 or AT3 file." },
                { "resampling_wav", "Resampling WAV to 48000Hz..." },
                { "normalizing_psp", "Normalizing for PSP (44100Hz Stereo)..." },
                { "wav_progress", "WAV in progress..." },
                { "at9_progress", "AT9 in progress..." },
                { "at3_progress", "AT3 in progress..." },
                { "finish", "Finish!" },
                { "ready", "Ready!" },
                { "playing_at9", "Playing AT9..." },
                { "playing_at3", "Playing AT3..." },
                { "stop", "Stop!" },
                { "file_not_found", "File not found:" },
                { "playback_error", "Playback error:" },
                { "at9_playback_error", "AT9 playback error:" },
                { "at3_playback_error", "AT3 playback error:" },
                { "wav_preprocess_error", "Error during WAV preprocessing:" },
                { "psp_conversion_error", "PSP conversion error:" },
                { "conversion_log_error", "Error! Conversion failed" },
                { "expert_mode_warning_title", "Expert mode warning" },
                { "expert_mode_warning", "Expert mode is intended for advanced users.\r\n\r\nSome option combinations may fail depending on the Sony tool, bitrate, channels, sample rate or target console.\r\nUse it at your own risk." },
                { "invalid_options", "Invalid options" },
                { "custom_loop_end_error", "Custom loop end must be greater than loop start." },
                { "at3_custom_loop_samples_error", "AT3 custom loops need at least 6143 samples between start and end." },
                { "ps4_bex_wband_error", "PS4 -bex and -wband cannot be enabled together." },
                { "ps4_bex_sample_rate_error", "PS4 -bex requires 48000 Hz output." },
                { "ps4_bex_nbands_error", "With PS4 -bex, -nbands must be between 5 and 10." },
                { "loop_list_empty_error", "Loop list is enabled, but no loop list file is set." },
                { "loop_list_missing_error", "Loop list file was not found." },
                { "psvita_bitrate_info", "If you make theme for PSVita/TV use the BitRate 144 for more compatibility" },                        
                { "language_outdated_warning", "This translation file is older than the application language template. Missing translations will be displayed in English." },
                { "tooltip_conversion_type", "Selected conversion type" },
                { "tooltip_console_type", "Select your console type" },
                { "tooltip_bitrate_info", "Bitrate information" },
                { "tooltip_bitrate", "Select your bitrate" },
                { "tooltip_drag_drop", "Drag and drop your file here" },
                { "tooltip_convert", "Start conversion" },
                { "tooltip_file_path", "Selected file path" },
                { "tooltip_tool_options", "Open conversion options for the selected console and mode" },
                { "tooltip_options_mode", "Basic keeps safe defaults. Advanced exposes useful options. Expert exposes codec-specific switches." },
                { "tooltip_loop_mode", "Whole loop matches the current app default. No loop writes no loop metadata. Custom loop uses sample positions." },
                { "tooltip_loop_start", "Start sample for -loop S E." },
                { "tooltip_loop_end", "End sample for -loop S E. It must be greater than start and inside the source length." },
                { "tooltip_decode_repeat", "Used with -d. Sony tools default to 2, but this app defaults to 1 to avoid doubled output." },
                { "tooltip_wav_output", "PS4 AT9 decode only. 16-bit is the safest default." },
                { "tooltip_wave_extensible", "PS4 AT9 decode only. Adds -wext." },
                { "tooltip_sampling_rate", "AT9 encode only. Leave disabled unless you know the target sample-rate mode." },
                { "tooltip_loop_list", "Path passed to -looplist. Sony tools support up to 2 loops." },
                { "tooltip_superframe", "Controls AT9 superframe encoding. Leave on Default unless a target requires a specific mode." },
                { "tooltip_dual_mode", "Enables dual encode mode for AT9 tools. Use only when you know the target expects it." },
                { "tooltip_quantized_bands", "Sets -nbands. The valid range depends on the selected sampling rate and PS4 -bex mode." },
                { "tooltip_intensity_band", "Sets -isband. -1 disables it; otherwise use a band value supported by the selected tool." },
                { "tooltip_gradient_mode", "0-3 manual, 4 automatic. Lower values suit tonal sounds; higher values suit noisy sources." },
                { "tooltip_band_mode", "PS4 AT9 only. Select Default, -wband, or -bex. -wband and -bex cannot be combined." },
                { "tooltip_wide_band", "PS4 AT9 only. Enables -wband. Cannot be combined with -bex." },
                { "tooltip_band_extension", "PS4 only. Cannot be combined with -wband. Requires 48kHz output and nbands 5-10 when nbands is used." },
                { "tooltip_lfe_super_low_cut", "PS4 AT9 only. Enables LFE super low cut with -slc." },
                { "tooltip_custom_encode_args", "Appended to encode commands before input/output files. Use only options supported by the selected console tool." },
                { "tooltip_custom_decode_args", "Appended to decode commands before input/output files." }
            };
        }

        private void SetToolOptionsEnabled(bool enabled)
        {
            toolOptionsButton.Enabled = enabled;

            toolOptionsButton.BackColor = enabled ? activeButtonBackColor : disabledButtonBackColor;
            toolOptionsButton.ForeColor = enabled ? Color.White : disabledButtonForeColor;
        }
    }
}

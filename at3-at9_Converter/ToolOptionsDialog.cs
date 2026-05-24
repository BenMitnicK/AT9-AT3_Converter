using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace at3_at9_Converter
{
    public sealed class ToolOptionsDialog : Form
    {
        private readonly string consoleName;
        private readonly ConversionMode conversionMode;
        private readonly ConversionToolSettings settings;
        private readonly Func<string, string> translate;
        private readonly ToolTip toolTip = new ToolTip();
        private readonly ComboBox levelComboBox = new ComboBox();
        private readonly ComboBox loopModeComboBox = new ComboBox();
        private readonly NumericUpDown loopStartNumeric = new NumericUpDown();
        private readonly NumericUpDown loopEndNumeric = new NumericUpDown();
        private readonly NumericUpDown repeatNumeric = new NumericUpDown();
        private readonly CheckBox samplingRateCheckBox = new CheckBox();
        private readonly ComboBox samplingRateComboBox = new ComboBox();
        private readonly CheckBox loopListCheckBox = new CheckBox();
        private readonly TextBox loopListTextBox = new TextBox();
        private readonly ComboBox superframeComboBox = new ComboBox();
        private readonly CheckBox dualCheckBox = new CheckBox();
        private readonly CheckBox nbandsCheckBox = new CheckBox();
        private readonly NumericUpDown nbandsNumeric = new NumericUpDown();
        private readonly CheckBox isbandCheckBox = new CheckBox();
        private readonly NumericUpDown isbandNumeric = new NumericUpDown();
        private readonly CheckBox gradmodeCheckBox = new CheckBox();
        private readonly NumericUpDown gradmodeNumeric = new NumericUpDown();
        private readonly ComboBox bandModeComboBox = new ComboBox();
        private readonly CheckBox slcCheckBox = new CheckBox();
        private readonly CheckBox wextCheckBox = new CheckBox();
        private readonly ComboBox pcmFormatComboBox = new ComboBox();
        private readonly TextBox customEncodeTextBox = new TextBox();
        private readonly TextBox customDecodeTextBox = new TextBox();
        private readonly Panel contentPanel = new Panel();
        private readonly PictureBox modePictureBox = new PictureBox();
        private Image advancedModeImage;
        private Image expertModeImage;
        private ToolOptionLevel previousLevel = ToolOptionLevel.Basic;
        private bool expertWarningShown;
        private bool loadingSettings;

        public ToolOptionsDialog(string consoleName, ConversionMode conversionMode, ConversionToolSettings source, Func<string, string> translate)
        {
            this.consoleName = consoleName ?? "";
            this.conversionMode = conversionMode;
            this.translate = translate ?? (key => key);
            settings = source == null ? new ConversionToolSettings() : source.Clone();

            Text = T("tool_options_title") + " - " + this.consoleName;
            toolTip.IsBalloon = true;
            toolTip.ShowAlways = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(620, 570);

            BuildLayout();
            LoadSettings();
            RefreshOptionVisibility();
        }

        public ConversionToolSettings Settings
        {
            get { return settings; }
        }

        private bool IsEncodeMode
        {
            get
            {
                return conversionMode == ConversionMode.WavToAt9
                    || conversionMode == ConversionMode.WavToAt3
                    || conversionMode == ConversionMode.Mp3ToAt9
                    || conversionMode == ConversionMode.Mp3ToAt3;
            }
        }

        private bool IsDecodeMode
        {
            get
            {
                return conversionMode == ConversionMode.At9ToWav
                    || conversionMode == ConversionMode.At9ToMp3
                    || conversionMode == ConversionMode.At3ToWav
                    || conversionMode == ConversionMode.At3ToMp3;
            }
        }

        private bool IsAt9
        {
            get { return consoleName == "PS4" || consoleName == "PSVita"; }
        }

        private bool IsPs4
        {
            get { return consoleName == "PS4"; }
        }

        private void BuildLayout()
        {
            Label levelLabel = CreateLabel("tool_options_mode", "Mode", 12, 15, 80);
            levelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            levelComboBox.Items.AddRange(new object[] { "Basic", "Advanced", "Expert" });
            levelComboBox.Location = new Point(95, 12);
            levelComboBox.Size = new Size(130, 21);
            levelComboBox.SelectedIndexChanged += levelComboBox_SelectedIndexChanged;
            SetToolTip(T("tooltip_options_mode"), levelLabel, levelComboBox);

            contentPanel.Location = new Point(12, 45);
            contentPanel.Size = new Size(595, 470);
            contentPanel.AutoScroll = true;
            contentPanel.BorderStyle = BorderStyle.FixedSingle;

            modePictureBox.Location = new Point(430, 18);
            modePictureBox.Size = new Size(140, 140);
            modePictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            modePictureBox.Visible = false;
            contentPanel.Controls.Add(modePictureBox);

            Button okButton = new Button();
            okButton.Text = T("ok");
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(433, 530);
            okButton.Size = new Size(80, 27);
            okButton.Click += okButton_Click;

            Button cancelButton = new Button();
            cancelButton.Text = T("cancel");
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(525, 530);
            cancelButton.Size = new Size(80, 27);

            Controls.Add(levelLabel);
            Controls.Add(levelComboBox);
            Controls.Add(contentPanel);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            AcceptButton = okButton;
            CancelButton = cancelButton;

            BuildContent();
        }

        private void BuildContent()
        {
            int y = 12;

            AddSection("tool_options_encode_options", "Encode options", ref y);
            Label loopLabel = CreateLabel("tool_options_loop", "Loop", 14, y + 3, 120);
            AddControl(loopLabel);
            loopModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            loopModeComboBox.Items.AddRange(IsPs4
                ? new object[] { "Whole loop", "No loop", "Custom loop", "Default whole loop" }
                : new object[] { "Whole loop", "No loop", "Custom loop" });
            loopModeComboBox.Location = new Point(145, y);
            loopModeComboBox.Size = new Size(150, 21);
            loopModeComboBox.SelectedIndexChanged += delegate { RefreshOptionVisibility(); };
            SetToolTip(T("tooltip_loop_mode"), loopLabel, loopModeComboBox);
            AddControl(loopModeComboBox);
            y += 32;

            Label loopStartLabel = CreateLabel("tool_options_loop_start", "Loop start", 14, y + 3, 120);
            AddControl(loopStartLabel);
            ConfigureNumeric(loopStartNumeric, 0, 999999999, 0, 145, y);
            SetToolTip(T("tooltip_loop_start"), loopStartLabel, loopStartNumeric);
            AddControl(loopStartNumeric);
            y += 30;

            Label loopEndLabel = CreateLabel("tool_options_loop_end", "Loop end", 14, y + 3, 120);
            AddControl(loopEndLabel);
            ConfigureNumeric(loopEndNumeric, 1, 999999999, 1, 145, y);
            SetToolTip(T("tooltip_loop_end"), loopEndLabel, loopEndNumeric);
            AddControl(loopEndNumeric);
            y += 40;

            AddSection("tool_options_decode_options", "Decode options", ref y);
            Label repeatLabel = CreateLabel("tool_options_repeat", "Repeat", 14, y + 3, 120);
            AddControl(repeatLabel);
            ConfigureNumeric(repeatNumeric, 1, 99, 1, 145, y);
            SetToolTip(T("tooltip_decode_repeat"), repeatLabel, repeatNumeric);
            AddControl(repeatNumeric);
            y += 30;

            Label wavOutputLabel = CreateLabel("tool_options_wav_output", "WAV output", 14, y + 3, 120);
            AddControl(wavOutputLabel);
            pcmFormatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            pcmFormatComboBox.Items.AddRange(new object[] { "16-bit PCM", "24-bit PCM", "Float PCM" });
            pcmFormatComboBox.Location = new Point(145, y);
            pcmFormatComboBox.Size = new Size(150, 21);
            SetToolTip(T("tooltip_wav_output"), wavOutputLabel, pcmFormatComboBox);
            AddControl(pcmFormatComboBox);
            y += 30;

            wextCheckBox.Text = T("tool_options_wave_extensible");
            wextCheckBox.Location = new Point(145, y);
            wextCheckBox.Size = new Size(250, 22);
            SetToolTip(T("tooltip_wave_extensible"), wextCheckBox);
            AddControl(wextCheckBox);
            y += 40;

            AddSection("tool_options_expert_encode_switches", "Expert encode switches", ref y);
            samplingRateCheckBox.Text = T("tool_options_sampling_rate");
            samplingRateCheckBox.Location = new Point(14, y);
            samplingRateCheckBox.Size = new Size(125, 22);
            AddControl(samplingRateCheckBox);
            samplingRateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            samplingRateComboBox.Items.AddRange(new object[] { "12000", "24000", "48000" });
            samplingRateComboBox.Location = new Point(145, y);
            samplingRateComboBox.Size = new Size(90, 21);
            SetToolTip(T("tooltip_sampling_rate"), samplingRateCheckBox, samplingRateComboBox);
            AddControl(samplingRateComboBox);
            y += 30;

            loopListCheckBox.Text = T("tool_options_loop_list");
            loopListCheckBox.Location = new Point(14, y);
            loopListCheckBox.Size = new Size(125, 22);
            AddControl(loopListCheckBox);
            loopListTextBox.Location = new Point(145, y);
            loopListTextBox.Size = new Size(260, 20);
            SetToolTip(T("tooltip_loop_list"), loopListCheckBox, loopListTextBox);
            AddControl(loopListTextBox);
            y += 30;

            Label superframeLabel = CreateLabel("tool_options_superframe", "Superframe", 14, y + 3, 120);
            AddControl(superframeLabel);
            superframeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            superframeComboBox.Items.AddRange(new object[] { "Default", "On", "Off" });
            superframeComboBox.Location = new Point(145, y);
            superframeComboBox.Size = new Size(90, 21);
            SetToolTip(T("tooltip_superframe"), superframeLabel, superframeComboBox);
            AddControl(superframeComboBox);
            y += 30;

            dualCheckBox.Text = T("tool_options_dual_mode");
            dualCheckBox.Location = new Point(145, y);
            dualCheckBox.Size = new Size(120, 22);
            SetToolTip(T("tooltip_dual_mode"), dualCheckBox);
            AddControl(dualCheckBox);
            y += 30;

            nbandsCheckBox.Text = T("tool_options_quantized_bands");
            nbandsCheckBox.Location = new Point(14, y);
            nbandsCheckBox.Size = new Size(125, 22);
            AddControl(nbandsCheckBox);
            ConfigureNumeric(nbandsNumeric, 3, 18, 8, 145, y);
            SetToolTip(T("tooltip_quantized_bands"), nbandsCheckBox, nbandsNumeric);
            AddControl(nbandsNumeric);
            y += 30;

            isbandCheckBox.Text = T("tool_options_intensity_band");
            isbandCheckBox.Location = new Point(14, y);
            isbandCheckBox.Size = new Size(125, 22);
            AddControl(isbandCheckBox);
            ConfigureNumeric(isbandNumeric, -1, 18, -1, 145, y);
            SetToolTip(T("tooltip_intensity_band"), isbandCheckBox, isbandNumeric);
            AddControl(isbandNumeric);
            y += 30;

            gradmodeCheckBox.Text = T("tool_options_gradient_mode");
            gradmodeCheckBox.Location = new Point(14, y);
            gradmodeCheckBox.Size = new Size(125, 22);
            AddControl(gradmodeCheckBox);
            ConfigureNumeric(gradmodeNumeric, 0, 4, 4, 145, y);
            SetToolTip(T("tooltip_gradient_mode"), gradmodeCheckBox, gradmodeNumeric);
            AddControl(gradmodeNumeric);
            y += 30;

            Label bandModeLabel = CreateLabel("tool_options_band_mode", "Band mode", 14, y + 3, 120);
            AddControl(bandModeLabel);
            bandModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            bandModeComboBox.Items.AddRange(new object[] { T("tool_options_band_mode_default"), T("tool_options_wide_band"), T("tool_options_band_extension") });
            bandModeComboBox.Location = new Point(145, y);
            bandModeComboBox.Size = new Size(190, 21);
            SetToolTip(T("tooltip_band_mode"), bandModeLabel, bandModeComboBox);
            AddControl(bandModeComboBox);
            y += 30;

            slcCheckBox.Text = T("tool_options_lfe_super_low_cut");
            slcCheckBox.Location = new Point(145, y);
            slcCheckBox.Size = new Size(180, 22);
            SetToolTip(T("tooltip_lfe_super_low_cut"), slcCheckBox);
            AddControl(slcCheckBox);
            y += 36;

            AddSection("tool_options_expert_custom_arguments", "Expert custom arguments", ref y);
            Label customEncodeLabel = CreateLabel("tool_options_encode", "Encode", 14, y + 3, 120);
            AddControl(customEncodeLabel);
            customEncodeTextBox.Location = new Point(145, y);
            customEncodeTextBox.Size = new Size(260, 20);
            SetToolTip(T("tooltip_custom_encode_args"), customEncodeLabel, customEncodeTextBox);
            AddControl(customEncodeTextBox);
            y += 30;

            Label customDecodeLabel = CreateLabel("tool_options_decode", "Decode", 14, y + 3, 120);
            AddControl(customDecodeLabel);
            customDecodeTextBox.Location = new Point(145, y);
            customDecodeTextBox.Size = new Size(260, 20);
            SetToolTip(T("tooltip_custom_decode_args"), customDecodeLabel, customDecodeTextBox);
            AddControl(customDecodeTextBox);
        }

        private void LoadSettings()
        {
            loadingSettings = true;
            levelComboBox.SelectedIndex = (int)settings.Level;
            loopModeComboBox.SelectedIndex = GetLoopComboIndex(settings.LoopMode);
            loopStartNumeric.Value = Clamp(settings.LoopStart, loopStartNumeric.Minimum, loopStartNumeric.Maximum);
            loopEndNumeric.Value = Clamp(Math.Max(1, settings.LoopEnd), loopEndNumeric.Minimum, loopEndNumeric.Maximum);
            repeatNumeric.Value = Clamp(settings.DecodeRepeat, repeatNumeric.Minimum, repeatNumeric.Maximum);
            samplingRateCheckBox.Checked = settings.UseSamplingRate;
            samplingRateComboBox.SelectedItem = settings.SamplingRate.ToString();
            if (samplingRateComboBox.SelectedIndex < 0)
                samplingRateComboBox.SelectedIndex = 2;
            loopListCheckBox.Checked = settings.UseLoopList;
            loopListTextBox.Text = settings.LoopListPath;
            superframeComboBox.SelectedIndex = settings.SuperframeMode;
            dualCheckBox.Checked = settings.DualMode;
            nbandsCheckBox.Checked = settings.UseQuantizedBands;
            nbandsNumeric.Value = Clamp(settings.QuantizedBands, nbandsNumeric.Minimum, nbandsNumeric.Maximum);
            isbandCheckBox.Checked = settings.UseIntensityBand;
            isbandNumeric.Value = Clamp(settings.IntensityBand, isbandNumeric.Minimum, isbandNumeric.Maximum);
            gradmodeCheckBox.Checked = settings.UseGradientMode;
            gradmodeNumeric.Value = Clamp(settings.GradientMode, gradmodeNumeric.Minimum, gradmodeNumeric.Maximum);
            bandModeComboBox.SelectedIndex = GetBandModeComboIndex();
            slcCheckBox.Checked = settings.LfeSuperLowCut;
            wextCheckBox.Checked = settings.WaveExtensibleHeader;
            pcmFormatComboBox.SelectedIndex = (int)settings.PcmOutputFormat;
            customEncodeTextBox.Text = settings.CustomEncodeArgs;
            customDecodeTextBox.Text = settings.CustomDecodeArgs;
            previousLevel = settings.Level;
            loadingSettings = false;
        }

        private void levelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolOptionLevel newLevel = (ToolOptionLevel)Math.Max(0, levelComboBox.SelectedIndex);

            if (!loadingSettings)
            {
                bool leavingBasicEncode = IsEncodeMode
                    && previousLevel == ToolOptionLevel.Basic
                    && newLevel != ToolOptionLevel.Basic;

                if (leavingBasicEncode && loopModeComboBox.SelectedIndex == 0)
                    loopModeComboBox.SelectedIndex = 1;

                if (newLevel == ToolOptionLevel.Expert && !expertWarningShown)
                    ShowExpertWarning();

                previousLevel = newLevel;
            }

            RefreshOptionVisibility();
        }

        private void ShowExpertWarning()
        {
            expertWarningShown = true;
            DialogService.ShowOkDialog(this,
                T("expert_mode_warning_title"),
                T("expert_mode_warning"),
                SystemIcons.Warning,
                T("ok"));
        }

        private void RefreshOptionVisibility()
        {
            ToolOptionLevel level = (ToolOptionLevel)Math.Max(0, levelComboBox.SelectedIndex);
            bool advanced = level == ToolOptionLevel.Advanced || level == ToolOptionLevel.Expert;
            bool expert = level == ToolOptionLevel.Expert;
            bool customLoop = loopModeComboBox.SelectedIndex == 2;
            bool at9Expert = expert && IsAt9;
            bool ps4Expert = expert && IsPs4;
            bool ps4Decode = IsPs4 && IsDecodeMode;

            UpdateModeAppearance(level);

            foreach (Control control in contentPanel.Controls)
            {
                Label label = control as Label;
                if (label != null && label.Tag is string && ((string)label.Tag).StartsWith("label:", StringComparison.Ordinal))
                    label.Visible = false;
            }

            SetControlGroupVisible("tool_options_encode_options", IsEncodeMode && advanced);
            loopModeComboBox.Visible = IsEncodeMode && advanced;
            loopStartNumeric.Visible = IsEncodeMode && advanced && customLoop;
            loopEndNumeric.Visible = IsEncodeMode && advanced && customLoop;

            SetControlGroupVisible("tool_options_decode_options", IsDecodeMode && advanced);
            repeatNumeric.Visible = IsDecodeMode && advanced;
            pcmFormatComboBox.Visible = ps4Decode && advanced;
            wextCheckBox.Visible = ps4Decode && expert;

            SetControlGroupVisible("tool_options_expert_encode_switches", IsEncodeMode && expert && (at9Expert || ps4Expert));
            samplingRateCheckBox.Visible = IsEncodeMode && at9Expert;
            samplingRateComboBox.Visible = IsEncodeMode && at9Expert;
            loopListCheckBox.Visible = IsEncodeMode && at9Expert;
            loopListTextBox.Visible = IsEncodeMode && at9Expert;
            superframeComboBox.Visible = IsEncodeMode && at9Expert;
            dualCheckBox.Visible = IsEncodeMode && at9Expert;
            nbandsCheckBox.Visible = IsEncodeMode && at9Expert;
            nbandsNumeric.Visible = IsEncodeMode && at9Expert;
            isbandCheckBox.Visible = IsEncodeMode && at9Expert;
            isbandNumeric.Visible = IsEncodeMode && at9Expert;
            gradmodeCheckBox.Visible = IsEncodeMode && at9Expert;
            gradmodeNumeric.Visible = IsEncodeMode && at9Expert;
            bandModeComboBox.Visible = IsEncodeMode && ps4Expert;
            slcCheckBox.Visible = IsEncodeMode && ps4Expert;

            SetControlGroupVisible("tool_options_expert_custom_arguments", expert);
            customEncodeTextBox.Visible = IsEncodeMode && expert;
            customDecodeTextBox.Visible = IsDecodeMode && expert;

            LayoutVisibleOptions(advanced, expert, customLoop, at9Expert, ps4Expert, ps4Decode);
        }

        private void UpdateModeAppearance(ToolOptionLevel level)
        {
            if (level == ToolOptionLevel.Expert)
            {
                contentPanel.BackColor = Color.FromArgb(255, 244, 230);
                if (expertModeImage == null)
                    expertModeImage = ResizeModeImage(Properties.Resources.Expert);
                modePictureBox.Image = expertModeImage;
                modePictureBox.Visible = true;
            }
            else if (level == ToolOptionLevel.Advanced)
            {
                contentPanel.BackColor = Color.FromArgb(239, 246, 255);
                if (advancedModeImage == null)
                    advancedModeImage = ResizeModeImage(Properties.Resources.Advanced);
                modePictureBox.Image = advancedModeImage;
                modePictureBox.Visible = true;
            }
            else
            {
                contentPanel.BackColor = Color.FromArgb(249, 250, 251);
                modePictureBox.Image = null;
                modePictureBox.Visible = false;
            }
        }

        private Image ResizeModeImage(Image source)
        {
            Bitmap image = new Bitmap(modePictureBox.Width, modePictureBox.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(source, new Rectangle(0, 0, image.Width, image.Height));
            }

            return image;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (advancedModeImage != null)
                    advancedModeImage.Dispose();
                if (expertModeImage != null)
                    expertModeImage.Dispose();
            }

            base.Dispose(disposing);
        }

        private void LayoutVisibleOptions(bool advanced, bool expert, bool customLoop, bool at9Expert, bool ps4Expert, bool ps4Decode)
        {
            int y = 12;

            if (IsEncodeMode && advanced)
            {
                PlaceSection("tool_options_encode_options", ref y);
                PlaceRow("tool_options_loop", ref y, loopModeComboBox);

                if (customLoop)
                {
                    PlaceRow("tool_options_loop_start", ref y, loopStartNumeric);
                    PlaceRow("tool_options_loop_end", ref y, loopEndNumeric);
                }

                y += 8;
            }

            if (IsDecodeMode && advanced)
            {
                PlaceSection("tool_options_decode_options", ref y);
                PlaceRow("tool_options_repeat", ref y, repeatNumeric);

                if (ps4Decode)
                    PlaceRow("tool_options_wav_output", ref y, pcmFormatComboBox);

                if (ps4Decode && expert)
                    PlaceControlRow(ref y, wextCheckBox);

                y += 8;
            }

            if (IsEncodeMode && expert && (at9Expert || ps4Expert))
            {
                PlaceSection("tool_options_expert_encode_switches", ref y);

                if (at9Expert)
                {
                    PlaceControlRow(ref y, samplingRateCheckBox, samplingRateComboBox);
                    PlaceControlRow(ref y, loopListCheckBox, loopListTextBox);
                    PlaceRow("tool_options_superframe", ref y, superframeComboBox);
                    PlaceControlRow(ref y, dualCheckBox);
                    PlaceControlRow(ref y, nbandsCheckBox, nbandsNumeric);
                    PlaceControlRow(ref y, isbandCheckBox, isbandNumeric);
                    PlaceControlRow(ref y, gradmodeCheckBox, gradmodeNumeric);
                }

                if (ps4Expert)
                {
                    PlaceRow("tool_options_band_mode", ref y, bandModeComboBox);
                    PlaceControlRow(ref y, slcCheckBox);
                }

                y += 8;
            }

            if (expert && (IsEncodeMode || IsDecodeMode))
            {
                PlaceSection("tool_options_expert_custom_arguments", ref y);

                if (IsEncodeMode)
                    PlaceRow("tool_options_encode", ref y, customEncodeTextBox);

                if (IsDecodeMode)
                    PlaceRow("tool_options_decode", ref y, customDecodeTextBox);
            }

            contentPanel.AutoScrollMinSize = new Size(0, y + 14);
        }

        private void PlaceSection(string key, ref int y)
        {
            Label label = FindSectionLabel(key);
            if (label == null || !label.Visible)
                return;

            label.Top = y;
            y = label.Bottom + 10;
        }

        private void PlaceRow(string key, ref int y, params Control[] controls)
        {
            Label label = FindPlainLabel(key);
            if (label != null)
            {
                label.Visible = true;
                label.Top = y + 3;
            }

            PlaceControlRow(ref y, controls);
        }

        private void PlaceControlRow(ref int y, params Control[] controls)
        {
            int rowBottom = y;

            foreach (Control control in controls)
            {
                if (!control.Visible)
                    continue;

                control.Top = y;
                rowBottom = Math.Max(rowBottom, control.Bottom);
            }

            y = rowBottom + 8;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettingsFromControls();
            string validationError = ValidateSettings();
            if (!string.IsNullOrEmpty(validationError))
            {
                DialogService.ShowOkDialog(this, T("invalid_options"), validationError, SystemIcons.Warning, T("ok"));
                DialogResult = DialogResult.None;
            }
        }

        private void SaveSettingsFromControls()
        {
            settings.Level = (ToolOptionLevel)Math.Max(0, levelComboBox.SelectedIndex);
            settings.LoopMode = GetSelectedLoopMode();
            settings.LoopStart = (int)loopStartNumeric.Value;
            settings.LoopEnd = (int)loopEndNumeric.Value;
            settings.DecodeRepeat = (int)repeatNumeric.Value;
            settings.UseSamplingRate = samplingRateCheckBox.Checked;
            settings.SamplingRate = int.Parse((string)samplingRateComboBox.SelectedItem);
            settings.UseLoopList = loopListCheckBox.Checked;
            settings.LoopListPath = loopListTextBox.Text.Trim();
            settings.SuperframeMode = superframeComboBox.SelectedIndex;
            settings.DualMode = dualCheckBox.Checked;
            settings.UseQuantizedBands = nbandsCheckBox.Checked;
            settings.QuantizedBands = (int)nbandsNumeric.Value;
            settings.UseIntensityBand = isbandCheckBox.Checked;
            settings.IntensityBand = (int)isbandNumeric.Value;
            settings.UseGradientMode = gradmodeCheckBox.Checked;
            settings.GradientMode = (int)gradmodeNumeric.Value;
            settings.WideBand = bandModeComboBox.SelectedIndex == 1;
            settings.BandExtension = bandModeComboBox.SelectedIndex == 2;
            settings.LfeSuperLowCut = slcCheckBox.Checked;
            settings.WaveExtensibleHeader = wextCheckBox.Checked;
            settings.PcmOutputFormat = (ToolPcmOutputFormat)Math.Max(0, pcmFormatComboBox.SelectedIndex);
            settings.CustomEncodeArgs = customEncodeTextBox.Text.Trim();
            settings.CustomDecodeArgs = customDecodeTextBox.Text.Trim();
        }

        private string ValidateSettings()
        {
            if (settings.Level == ToolOptionLevel.Basic)
                return "";

            if (IsEncodeMode && settings.LoopMode == ToolLoopMode.CustomLoop)
            {
                if (settings.LoopEnd <= settings.LoopStart)
                    return T("custom_loop_end_error");

                if ((consoleName == "PSP" || consoleName == "PS3") && settings.LoopEnd < settings.LoopStart + 6143)
                    return T("at3_custom_loop_samples_error");
            }

            if (settings.Level != ToolOptionLevel.Expert)
                return "";

            if (settings.BandExtension && settings.WideBand)
                return T("ps4_bex_wband_error");

            if (settings.BandExtension && settings.UseSamplingRate && settings.SamplingRate != 48000)
                return T("ps4_bex_sample_rate_error");

            if (settings.BandExtension && settings.UseQuantizedBands
                && (settings.QuantizedBands < 5 || settings.QuantizedBands > 10))
                return T("ps4_bex_nbands_error");

            if (settings.UseLoopList && string.IsNullOrWhiteSpace(settings.LoopListPath))
                return T("loop_list_empty_error");

            if (settings.UseLoopList && !File.Exists(settings.LoopListPath))
                return T("loop_list_missing_error");

            return "";
        }

        private ToolLoopMode GetSelectedLoopMode()
        {
            switch (loopModeComboBox.SelectedIndex)
            {
                case 1:
                    return ToolLoopMode.NoLoop;
                case 2:
                    return ToolLoopMode.CustomLoop;
                case 3:
                    return ToolLoopMode.DefaultWholeLoop;
                default:
                    return ToolLoopMode.WholeLoop;
            }
        }

        private int GetLoopComboIndex(ToolLoopMode loopMode)
        {
            if (loopMode == ToolLoopMode.NoLoop)
                return 1;
            if (loopMode == ToolLoopMode.CustomLoop)
                return 2;
            if (loopMode == ToolLoopMode.DefaultWholeLoop && IsPs4)
                return 3;

            return 0;
        }

        private int GetBandModeComboIndex()
        {
            if (settings.WideBand)
                return 1;
            if (settings.BandExtension)
                return 2;

            return 0;
        }

        private void AddSection(string key, string fallback, ref int y)
        {
            Label label = CreateLabel(key, fallback, 8, y, 300);
            label.Font = new Font(Font, FontStyle.Bold);
            label.Tag = "section:" + key;
            contentPanel.Controls.Add(label);
            y += 25;
        }

        private Label CreateLabel(string key, string fallback, int x, int y, int width)
        {
            Label label = new Label();
            label.Text = T(key, fallback);
            label.Tag = "label:" + key;
            label.Location = new Point(x, y);
            label.Size = new Size(width, 18);
            return label;
        }

        private void AddControl(Control control)
        {
            contentPanel.Controls.Add(control);
        }

        private void SetToolTip(string text, params Control[] controls)
        {
            string compactText = WrapToolTipText(text, 48);
            foreach (Control control in controls)
            {
                if (control != null)
                    toolTip.SetToolTip(control, compactText);
            }
        }

        private static string WrapToolTipText(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLineLength)
                return text;

            string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            int lineLength = 0;

            foreach (string word in words)
            {
                if (lineLength > 0 && lineLength + word.Length + 1 > maxLineLength)
                {
                    builder.AppendLine();
                    lineLength = 0;
                }
                else if (lineLength > 0)
                {
                    builder.Append(' ');
                    lineLength++;
                }

                builder.Append(word);
                lineLength += word.Length;
            }

            return builder.ToString();
        }

        private void ConfigureNumeric(NumericUpDown numeric, decimal minimum, decimal maximum, decimal value, int x, int y)
        {
            numeric.Minimum = minimum;
            numeric.Maximum = maximum;
            numeric.Value = Clamp(value, minimum, maximum);
            numeric.Location = new Point(x, y);
            numeric.Size = new Size(100, 20);
        }

        private decimal Clamp(decimal value, decimal minimum, decimal maximum)
        {
            if (value < minimum)
                return minimum;
            if (value > maximum)
                return maximum;
            return value;
        }

        private void SetControlGroupVisible(string sectionName, bool visible)
        {
            Label label = FindSectionLabel(sectionName);
            if (label != null)
                label.Visible = visible;
        }

        private Label FindSectionLabel(string text)
        {
            foreach (Control candidate in contentPanel.Controls)
            {
                Label label = candidate as Label;
                if (label != null && label.Tag is string && (string)label.Tag == "section:" + text)
                    return label;
            }

            return null;
        }

        private Label FindPlainLabel(string text)
        {
            foreach (Control candidate in contentPanel.Controls)
            {
                Label label = candidate as Label;
                if (label == null)
                    continue;

                if (label.Tag is string && (string)label.Tag == "label:" + text)
                    return label;
            }

            return null;
        }

        private string T(string key)
        {
            return translate(key);
        }

        private string T(string key, string fallback)
        {
            string value = translate(key);
            return string.Equals(value, key, StringComparison.Ordinal) ? fallback : value;
        }
    }
}

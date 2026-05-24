namespace at3_at9_Converter
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.conversionPanel = new System.Windows.Forms.Panel();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.toolOptionsButton = new System.Windows.Forms.Button();
            this.stopPlaybackButton = new System.Windows.Forms.Button();
            this.conversionGroupBox = new System.Windows.Forms.GroupBox();
            this.conversionModeLabel = new System.Windows.Forms.Label();
            this.consoleComboBox = new System.Windows.Forms.ComboBox();
            this.consoleLabel = new System.Windows.Forms.Label();
            this.bitrateInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.bitrateLabel = new System.Windows.Forms.Label();
            this.bitrateComboBox = new System.Windows.Forms.ComboBox();
            this.dropLabel = new System.Windows.Forms.Label();
            this.convertButton = new System.Windows.Forms.Button();
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.psvitaPictureBox = new System.Windows.Forms.PictureBox();
            this.ps4PictureBox = new System.Windows.Forms.PictureBox();
            this.ps3PictureBox = new System.Windows.Forms.PictureBox();
            this.pspPictureBox = new System.Windows.Forms.PictureBox();
            this.mainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.studioLinkStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.conversionPanel.SuspendLayout();
            this.conversionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bitrateInfoPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.psvitaPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ps4PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ps3PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pspPictureBox)).BeginInit();
            this.mainStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // conversionPanel
            // 
            this.conversionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(197)))));
            this.conversionPanel.Controls.Add(this.languageComboBox);
            this.conversionPanel.Controls.Add(this.toolOptionsButton);
            this.conversionPanel.Controls.Add(this.stopPlaybackButton);
            this.conversionPanel.Controls.Add(this.conversionGroupBox);
            this.conversionPanel.Controls.Add(this.dropLabel);
            this.conversionPanel.Controls.Add(this.convertButton);
            this.conversionPanel.Controls.Add(this.filePathTextBox);
            this.conversionPanel.Controls.Add(this.psvitaPictureBox);
            this.conversionPanel.Controls.Add(this.ps4PictureBox);
            this.conversionPanel.Controls.Add(this.ps3PictureBox);
            this.conversionPanel.Controls.Add(this.pspPictureBox);
            this.conversionPanel.Location = new System.Drawing.Point(0, -2);
            this.conversionPanel.Name = "conversionPanel";
            this.conversionPanel.Size = new System.Drawing.Size(410, 372);
            this.conversionPanel.TabIndex = 0;
            // 
            // languageComboBox
            // 
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Location = new System.Drawing.Point(215, 17);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(104, 21);
            this.languageComboBox.TabIndex = 10;
            this.languageComboBox.SelectedIndexChanged += new System.EventHandler(this.languageComboBox_SelectedIndexChanged);
            // 
            // toolOptionsButton
            // 
            this.toolOptionsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.toolOptionsButton.Enabled = false;
            this.toolOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolOptionsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolOptionsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.toolOptionsButton.Location = new System.Drawing.Point(327, 15);
            this.toolOptionsButton.Name = "toolOptionsButton";
            this.toolOptionsButton.Size = new System.Drawing.Size(72, 25);
            this.toolOptionsButton.TabIndex = 16;
            this.toolOptionsButton.Text = "Options...";
            this.toolOptionsButton.UseVisualStyleBackColor = false;
            this.toolOptionsButton.Click += new System.EventHandler(this.toolOptionsButton_Click);
            // 
            // stopPlaybackButton
            // 
            this.stopPlaybackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.stopPlaybackButton.Enabled = false;
            this.stopPlaybackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopPlaybackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopPlaybackButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.stopPlaybackButton.Location = new System.Drawing.Point(215, 118);
            this.stopPlaybackButton.Name = "stopPlaybackButton";
            this.stopPlaybackButton.Size = new System.Drawing.Size(184, 34);
            this.stopPlaybackButton.TabIndex = 9;
            this.stopPlaybackButton.Text = "Stop Player";
            this.stopPlaybackButton.UseVisualStyleBackColor = false;
            this.stopPlaybackButton.Click += new System.EventHandler(this.stopPlaybackButton_Click);
            // 
            // conversionGroupBox
            // 
            this.conversionGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.conversionGroupBox.Controls.Add(this.conversionModeLabel);
            this.conversionGroupBox.Controls.Add(this.consoleComboBox);
            this.conversionGroupBox.Controls.Add(this.consoleLabel);
            this.conversionGroupBox.Controls.Add(this.bitrateInfoPictureBox);
            this.conversionGroupBox.Controls.Add(this.bitrateLabel);
            this.conversionGroupBox.Controls.Add(this.bitrateComboBox);
            this.conversionGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conversionGroupBox.ForeColor = System.Drawing.Color.White;
            this.conversionGroupBox.Location = new System.Drawing.Point(11, 9);
            this.conversionGroupBox.Name = "conversionGroupBox";
            this.conversionGroupBox.Size = new System.Drawing.Size(192, 146);
            this.conversionGroupBox.TabIndex = 8;
            this.conversionGroupBox.TabStop = false;
            this.conversionGroupBox.Text = "Conversion type";
            // 
            // conversionModeLabel
            // 
            this.conversionModeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.conversionModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conversionModeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.conversionModeLabel.Location = new System.Drawing.Point(8, 29);
            this.conversionModeLabel.Name = "conversionModeLabel";
            this.conversionModeLabel.Size = new System.Drawing.Size(176, 35);
            this.conversionModeLabel.TabIndex = 9;
            this.conversionModeLabel.Text = "Conversion:";
            this.conversionModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // consoleComboBox
            // 
            this.consoleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.consoleComboBox.Enabled = false;
            this.consoleComboBox.FormattingEnabled = true;
            this.consoleComboBox.Location = new System.Drawing.Point(102, 78);
            this.consoleComboBox.Name = "consoleComboBox";
            this.consoleComboBox.Size = new System.Drawing.Size(62, 21);
            this.consoleComboBox.TabIndex = 8;
            this.consoleComboBox.SelectedIndexChanged += new System.EventHandler(this.consoleComboBox_SelectedIndexChanged);
            // 
            // consoleLabel
            // 
            this.consoleLabel.AutoSize = true;
            this.consoleLabel.Location = new System.Drawing.Point(8, 81);
            this.consoleLabel.Name = "consoleLabel";
            this.consoleLabel.Size = new System.Drawing.Size(84, 13);
            this.consoleLabel.TabIndex = 7;
            this.consoleLabel.Text = "Console Type";
            // 
            // bitrateInfoPictureBox
            // 
            this.bitrateInfoPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bitrateInfoPictureBox.Image = global::at3_at9_Converter.Properties.Resources.info_bitrate;
            this.bitrateInfoPictureBox.Location = new System.Drawing.Point(168, 108);
            this.bitrateInfoPictureBox.Name = "bitrateInfoPictureBox";
            this.bitrateInfoPictureBox.Size = new System.Drawing.Size(18, 18);
            this.bitrateInfoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bitrateInfoPictureBox.TabIndex = 6;
            this.bitrateInfoPictureBox.TabStop = false;
            this.bitrateInfoPictureBox.Click += new System.EventHandler(this.bitrateInfoPictureBox_Click);
            // 
            // bitrateLabel
            // 
            this.bitrateLabel.AutoSize = true;
            this.bitrateLabel.Location = new System.Drawing.Point(8, 109);
            this.bitrateLabel.Name = "bitrateLabel";
            this.bitrateLabel.Size = new System.Drawing.Size(88, 13);
            this.bitrateLabel.TabIndex = 5;
            this.bitrateLabel.Text = "BitRate [kbps]";
            // 
            // bitrateComboBox
            // 
            this.bitrateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bitrateComboBox.Enabled = false;
            this.bitrateComboBox.FormattingEnabled = true;
            this.bitrateComboBox.Location = new System.Drawing.Point(102, 106);
            this.bitrateComboBox.Name = "bitrateComboBox";
            this.bitrateComboBox.Size = new System.Drawing.Size(62, 21);
            this.bitrateComboBox.TabIndex = 4;
            this.bitrateComboBox.SelectedIndexChanged += new System.EventHandler(this.bitrateComboBox_SelectedIndexChanged);
            // 
            // dropLabel
            // 
            this.dropLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.dropLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dropLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.dropLabel.Location = new System.Drawing.Point(13, 322);
            this.dropLabel.Name = "dropLabel";
            this.dropLabel.Size = new System.Drawing.Size(384, 31);
            this.dropLabel.TabIndex = 7;
            this.dropLabel.Text = "DragDrop Your File";
            this.dropLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // convertButton
            // 
            this.convertButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.convertButton.Enabled = false;
            this.convertButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.convertButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.convertButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.convertButton.Location = new System.Drawing.Point(215, 46);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(184, 64);
            this.convertButton.TabIndex = 6;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = false;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.BackColor = System.Drawing.Color.White;
            this.filePathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filePathTextBox.Location = new System.Drawing.Point(13, 293);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.Size = new System.Drawing.Size(384, 13);
            this.filePathTextBox.TabIndex = 4;
            // 
            // psvitaPictureBox
            // 
            this.psvitaPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.psvitaPictureBox.Image = global::at3_at9_Converter.Properties.Resources.psvita;
            this.psvitaPictureBox.Location = new System.Drawing.Point(17, 196);
            this.psvitaPictureBox.Name = "psvitaPictureBox";
            this.psvitaPictureBox.Size = new System.Drawing.Size(76, 54);
            this.psvitaPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.psvitaPictureBox.TabIndex = 11;
            this.psvitaPictureBox.TabStop = false;
            // 
            // ps4PictureBox
            // 
            this.ps4PictureBox.BackColor = System.Drawing.Color.Transparent;
            this.ps4PictureBox.Image = global::at3_at9_Converter.Properties.Resources.ps4;
            this.ps4PictureBox.Location = new System.Drawing.Point(113, 196);
            this.ps4PictureBox.Name = "ps4PictureBox";
            this.ps4PictureBox.Size = new System.Drawing.Size(76, 54);
            this.ps4PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ps4PictureBox.TabIndex = 12;
            this.ps4PictureBox.TabStop = false;
            // 
            // ps3PictureBox
            // 
            this.ps3PictureBox.BackColor = System.Drawing.Color.Transparent;
            this.ps3PictureBox.Image = global::at3_at9_Converter.Properties.Resources.ps3;
            this.ps3PictureBox.Location = new System.Drawing.Point(217, 196);
            this.ps3PictureBox.Name = "ps3PictureBox";
            this.ps3PictureBox.Size = new System.Drawing.Size(76, 54);
            this.ps3PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ps3PictureBox.TabIndex = 13;
            this.ps3PictureBox.TabStop = false;
            // 
            // pspPictureBox
            // 
            this.pspPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.pspPictureBox.Image = global::at3_at9_Converter.Properties.Resources.psp;
            this.pspPictureBox.Location = new System.Drawing.Point(317, 196);
            this.pspPictureBox.Name = "pspPictureBox";
            this.pspPictureBox.Size = new System.Drawing.Size(76, 54);
            this.pspPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pspPictureBox.TabIndex = 14;
            this.pspPictureBox.TabStop = false;
            // 
            // mainToolTip
            // 
            this.mainToolTip.IsBalloon = true;
            this.mainToolTip.ShowAlways = true;
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.studioLinkStatusLabel});
            this.mainStatusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 370);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(410, 22);
            this.mainStatusStrip.SizingGrip = false;
            this.mainStatusStrip.TabIndex = 1;
            this.mainStatusStrip.Text = "mainStatusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.statusLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(45, 17);
            this.statusLabel.Text = "Ready!";
            // 
            // studioLinkStatusLabel
            // 
            this.studioLinkStatusLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.studioLinkStatusLabel.IsLink = true;
            this.studioLinkStatusLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.studioLinkStatusLabel.LinkColor = System.Drawing.Color.Gray;
            this.studioLinkStatusLabel.Name = "studioLinkStatusLabel";
            this.studioLinkStatusLabel.Size = new System.Drawing.Size(72, 17);
            this.studioLinkStatusLabel.Text = "BMK-Studio";
            this.studioLinkStatusLabel.Click += new System.EventHandler(this.studioLinkStatusLabel_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 392);
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.conversionPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BMK AT9 & AT3 Converter  V";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.conversionPanel.ResumeLayout(false);
            this.conversionPanel.PerformLayout();
            this.conversionGroupBox.ResumeLayout(false);
            this.conversionGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bitrateInfoPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.psvitaPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ps4PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ps3PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pspPictureBox)).EndInit();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel conversionPanel;
        private System.Windows.Forms.Label dropLabel;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.GroupBox conversionGroupBox;
        private System.Windows.Forms.Label conversionModeLabel;
        private System.Windows.Forms.ToolTip mainToolTip;
        private System.Windows.Forms.Label bitrateLabel;
        private System.Windows.Forms.ComboBox bitrateComboBox;
        private System.Windows.Forms.PictureBox bitrateInfoPictureBox;
        private System.Windows.Forms.PictureBox psvitaPictureBox;
        private System.Windows.Forms.PictureBox ps4PictureBox;
        private System.Windows.Forms.PictureBox ps3PictureBox;
        private System.Windows.Forms.PictureBox pspPictureBox;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel studioLinkStatusLabel;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.Button stopPlaybackButton;
        private System.Windows.Forms.ComboBox consoleComboBox;
        private System.Windows.Forms.Label consoleLabel;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Button toolOptionsButton;
    }
}

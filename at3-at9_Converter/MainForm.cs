using at3_at9_Converter.Properties;
using NAudio;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace at3_at9_Converter
{

    public partial class MainForm : Form
    {

        private Dictionary<string, string> lang = new Dictionary<string, string>();
        private string languageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang");

        private At9Player at9Player = new At9Player();

        // ... (remaining existing variables) ...
        public static string dir = "", appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), version = "";
        // ... (keep all class variables here) ...

        Process playerProcess = new Process();
        ProcessStartInfo playerStartInfo = new ProcessStartInfo();
        public string FileNameSelected = "";
        public string FileNameConvert = "";
        public string FileNameFinal = "";
        public string FileNameFinal2 = "";
        public string VerifFileExtention = "";
        public string fFile = "";
        public string FileNameDrop = "";
        private string pPath = "";
        private string nFile = "";
        private string fFileOrig = "";
        private string at3tool = "";
        private string at9tool = "";
        private string at3bitRate = "";
        private string at9bitRate = "";

        string[] consoleListAt3 = new string[] { 
            "PSP",
            "PS3"
        };

        string[] consoleListAt9 = new string[] { 
            "PS4",
            "PSVita"
        };

        string[] pspList = new string[] { 
            "32",
            "48",
            "52",
            "64",
            "66",
            "96",
            "105",
            "128",
            "132" ,          
            "160",
            "192",
            "256",
            "320",
            "352"
        };

        string[] ps3List = new string[] { 
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

        string[] psvitaList = new string[] { 
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

        string[] ps4List = new string[] { 
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

        public MainForm()
        {
            InitializeComponent();
            this.tabPage1.AllowDrop = true;
            this.tabPage1.DragEnter += new DragEventHandler(tabPage1_DragEnter);
            this.tabPage1.DragDrop += new DragEventHandler(tabPage1_DragDrop);
            this.tabPage2.AllowDrop = true;
            this.tabPage2.DragEnter += new DragEventHandler(tabPage2_DragEnter);
            this.tabPage2.DragDrop += new DragEventHandler(tabPage2_DragDrop);
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
   
        }

        void tabPage1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void tabPage1_DragDrop(object sender, DragEventArgs e)
        {
            //var regex = new Regex(@"\s");
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //foreach (string file in files) Console.WriteLine(file);
            fFileOrig = "";
            pPath = "";
            textBox1.Text = "";
            VerifFileExtention = "";
            fFile = "";
            foreach (string file in files) {

                fFileOrig += Path.GetFileName(file);
                pPath = Path.GetDirectoryName(file);
                textBox1.Text = file;

            } 
            
                VerifFileExtention = textBox1.Text.Substring(textBox1.Text.LastIndexOf((".")));
                fFile = textBox1.Text;
                VerifExtention_at9();
            
        }

        void tabPage2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void tabPage2_DragDrop(object sender, DragEventArgs e)
        {
            //var regex = new Regex(@"\s");
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //foreach (string file in files) Console.WriteLine(file);
            fFileOrig = "";
            pPath = "";
            textBox2.Text = "";
            VerifFileExtention = "";
            fFile = "";
            foreach (string file in files)
            {
                fFileOrig += Path.GetFileName(file);
                pPath = Path.GetDirectoryName(file);
                textBox2.Text = file;

            } 
                VerifFileExtention = textBox2.Text.Substring(textBox2.Text.LastIndexOf((".")));
                fFile = textBox2.Text;
                VerifExtention_at3();            
        }

        private static String sReplace(String ffFile)
        {
           
            ffFile = ffFile.Replace(" ", "-");
            return ffFile;

        }

        private void rRename()
        {
            nFile = "";
            if (tabControl1.SelectedIndex == 0)
            {
            nFile = sReplace(fFileOrig);
            System.IO.File.Move(dir + "\\" + fFileOrig, dir + "\\" + nFile);
                fFile = dir + "\\" + nFile;
                fFileOrig = nFile;
                
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                nFile = sReplace(fFileOrig);
                System.IO.File.Move(textBox2.Text, pPath + "\\" + nFile);
                fFile = textBox2.Text;
                fFileOrig = nFile;
            }

        }

        private void VerifExtention_at9()
        {
            var regex = new Regex(@"\s");
            if (VerifFileExtention.Equals(".wav") || VerifFileExtention.Equals(".WAV"))
            {
                FileNameSelected = textBox1.Text;
                FileNameFinal = Path.ChangeExtension(FileNameSelected, ".at9");
                
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton1.Enabled = true;
                radioButton1.Checked = true;
                tabPage1ConsoleCombo();
                comboBox3.Items.Clear();
                comboBox4.Enabled = true;
            }
            else if (VerifFileExtention.Equals(".at9") || VerifFileExtention.Equals(".AT9"))
            {
                FileNameSelected = textBox1.Text;
                FileNameFinal2 = Path.ChangeExtension(FileNameSelected, ".wav");
                FileNameFinal = Path.ChangeExtension(FileNameSelected, ".mp3");             
                InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
                AskPlayDroppedAT9();
            }
            else if (VerifFileExtention.Equals(".mp3") || VerifFileExtention.Equals(".MP3"))
            {
                FileNameSelected = textBox1.Text;
                FileNameFinal2 = Path.ChangeExtension(FileNameSelected, ".wav");
                FileNameFinal = Path.ChangeExtension(FileNameSelected, ".at9");
                
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton4.Enabled = false;
                radioButton3.Enabled = true;
                radioButton3.Checked = true;
                tabPage1ConsoleCombo();
                comboBox3.Items.Clear();
                comboBox4.Enabled = true;
            }
            else
            {
                mMessageBox("Informations", "Please select MP3 Or Wav Or at9 File", SystemIcons.Information, true);
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                button2.Enabled = false;                      
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                textBox1.Text = "";
            }
        }

        private void AskPlayDroppedAT9()
        {
            DialogResult result = mInputBox(
                "Question",
                "Do you want to play this AT9 file?",
                SystemIcons.Question,
                true,
                8
            );
        }

        private void VerifExtention_at3()
        {
            var regex = new Regex(@"\s");
            if (VerifFileExtention.Equals(".wav") || VerifFileExtention.Equals(".WAV"))
            {
                FileNameSelected = textBox2.Text;
                FileNameFinal = Path.ChangeExtension(FileNameSelected, ".at3");
                
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                radioButton5.Enabled = true;
                radioButton5.Checked = true;
                tabPage2ConsoleCombo();
                comboBox2.Items.Clear();
                comboBox1.Enabled = true;
            }
            else if (VerifFileExtention.Equals(".at3") || VerifFileExtention.Equals(".AT3"))
            {
                FileNameSelected = textBox2.Text;
                FileNameFinal2 = Path.ChangeExtension(FileNameSelected, ".wav");
                FileNameFinal = Path.ChangeExtension(FileNameSelected, ".mp3");
                InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
                AskPlayDroppedAT3();
            }
            else if (VerifFileExtention.Equals(".mp3") || VerifFileExtention.Equals(".MP3"))
            {
                FileNameSelected = textBox2.Text;
                FileNameFinal2 = Path.ChangeExtension(FileNameSelected, ".wav");
                FileNameFinal = Path.ChangeExtension(FileNameSelected, ".at3");
                
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton8.Enabled = false;
                radioButton7.Enabled = true;
                radioButton7.Checked = true;
                tabPage2ConsoleCombo();
                comboBox2.Items.Clear();
                comboBox1.Enabled = true;
            }
            else
            {
                mMessageBox("Informations", "Please select MP3 Or Wav Or at3 File", SystemIcons.Information, true);
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                button4.Enabled = false;
                comboBox1.Enabled = false;               
                comboBox2.Enabled = false;                
                textBox2.Text = "";
            }
        }

        private void AskPlayDroppedAT3()
        {
            DialogResult result = mInputBox(
                "Question",
                "Do you want to play this AT3 file?",
                SystemIcons.Question,
                true,
                9
            );
        }

        private void button2_Click(object sender, EventArgs e)
        {
  
            if (radioButton3.Checked == true || radioButton4.Checked == true)
            {
            if (System.IO.File.Exists(FileNameFinal) || System.IO.File.Exists(FileNameFinal2))
            {
                at9DoProcessFileExist();
            }
            else
            {
                at9DoProcess();
            }
            }
            else if (radioButton1.Checked == true || radioButton2.Checked == true)
            {
                if (System.IO.File.Exists(FileNameFinal))
                {
                    at9DoProcessFileExist();
                }
                else
                {
                    at9DoProcess();
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (radioButton7.Checked == true || radioButton8.Checked == true)
            {
                if (System.IO.File.Exists(FileNameFinal) || System.IO.File.Exists(FileNameFinal2))
                {
                    at3DoProcessFileExist();
                }
                else
                {
                    at3DoProcess();
                }
            }
            else if (radioButton5.Checked == true || radioButton6.Checked == true)
            {
                if (System.IO.File.Exists(FileNameFinal))
                {
                    at3DoProcessFileExist();
                }
                else
                {
                    at3DoProcess();
                }
            }
        }

        private void at9DoProcessFileExist()
        {

            mInputBox("Question", "File(s) Exist Do you want to Continue ?", SystemIcons.Question, true, 1); 

        }

        private void at3DoProcessFileExist()
        {

            mInputBox("Question", "File(s) Exist Do you want to Continue ?", SystemIcons.Question, true, 2);

        }

        private void at9DoProcess()
        {
            if (radioButton1.Checked == true & textBox1.Text != "")
            {
                string wavToProcess = FileNameSelected;
                bool isTempWav = false;
                bool REP = false;
                try
                {
                    using (var reader = new WaveFileReader(FileNameSelected))
                    {
                        if (reader.WaveFormat.SampleRate != 48000)
                        {
                            toolStripStatusLabel1.Text = "Resampling WAV to 48000Hz...";
                            statusStrip1.Refresh();
                            wavToProcess = Path.Combine(Path.GetDirectoryName(FileNameSelected), "temp_normalized.wav");
                            using (var resampler = new WaveFormatConversionStream(new WaveFormat(48000, 16, reader.WaveFormat.Channels), reader))
                            {
                                WaveFileWriter.CreateWaveFile(wavToProcess, resampler);
                            }
                            isTempWav = true;
                        }
                    }

                    toolStripStatusLabel1.Text = "AT9 in Progress...";
                    statusStrip1.Refresh();
                    REP = RunExternalProcess(@"ATRAC\" + at9tool, " -e -br " + at9bitRate + " -wholeloop \"" + wavToProcess + "\" \"" + FileNameFinal + "\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during WAV preprocessing: " + ex.Message);
                }
                finally
                {
                    if (isTempWav && File.Exists(wavToProcess)) File.Delete(wavToProcess);
                }
                if (!REP)
                    return;

                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                PlayerFile();
            }
            else if (radioButton2.Checked == true & textBox1.Text != "")
            {
                bool REP = false;
                try
                {
                    toolStripStatusLabel1.Text = "Wav in Progress...";
                    statusStrip1.Refresh();
                    REP = RunExternalProcess(@"ATRAC\" + at9tool, " -d \"" + FileNameSelected + "\" \"" + FileNameFinal2 + "\"");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                if (!REP)
                    return;

                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }
            else if (radioButton3.Checked == true & textBox1.Text != "")
            {
                bool REP = false;
                try
                {
                    toolStripStatusLabel1.Text = "Wav in Progress...";
                    statusStrip1.Refresh();
                    using (Mp3FileReader mp3 = new Mp3FileReader(FileNameSelected))
                    {
                        using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(48000, 16, mp3.WaveFormat.Channels), mp3))
                        {
                            WaveFileWriter.CreateWaveFile(FileNameFinal2, pcm);
                        }
                    }
                    toolStripStatusLabel1.Text = "AT9 in Progress...";
                    statusStrip1.Refresh();
                    REP = RunExternalProcess(@"ATRAC\" + at9tool, " -e -br " + at9bitRate + " -wholeloop \"" + FileNameFinal2 + "\" \"" + FileNameFinal + "\"");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                DeleteFile();
                if (!REP)
                    return;

                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                PlayerFile();
            }
            else if (radioButton4.Checked == true & textBox1.Text != "")
            {              
                try {
                    bool REP = RunExternalProcess(@"ATRAC\" + at9tool, " -d \"" + FileNameSelected + "\" \"" + FileNameFinal2 + "\"");
                    if (!REP)
                        return;

                    REP = RunExternalProcess(@"LAME\lame.exe", "-V2 \"" + FileNameFinal2 + "\" \"" + FileNameFinal + "\"");
                    if (!REP)
                        return;

                    DeleteFile();
                    toolStripStatusLabel1.Text = "Finish!";
                    statusStrip1.Refresh();

                } catch (Exception ex) { MessageBox.Show(ex.Message); }             
            }
        }

        private void at3DoProcess()
        {
            if (radioButton5.Checked == true & textBox2.Text != "")
            {
                string wavToProcess = FileNameSelected;
                bool isTempWav = false;
                bool REP = false;
                try
                {
                    int targetRate = (comboBox1.Text == "PSP") ? 44100 : 48000;
                    int targetChannels = (comboBox1.Text == "PSP") ? 2 : 0; 

                    using (var reader = new WaveFileReader(FileNameSelected))
                    {
                        if (reader.WaveFormat.SampleRate != targetRate || (targetChannels == 2 && reader.WaveFormat.Channels != 2))
                        {
                            toolStripStatusLabel1.Text = "Normalizing for PSP (44100Hz Stereo)...";
                            statusStrip1.Refresh();
                            wavToProcess = Path.Combine(Path.GetDirectoryName(FileNameSelected), "temp_psp_norm.wav");
                            var outFormat = new WaveFormat(targetRate, 16, (targetChannels == 2) ? 2 : reader.WaveFormat.Channels);
                            using (var resampler = new WaveFormatConversionStream(outFormat, reader))
                            {
                                WaveFileWriter.CreateWaveFile(wavToProcess, resampler);
                            }
                            isTempWav = true;
                        }
                    }

                    toolStripStatusLabel1.Text = "AT3 in Progress...";
                    statusStrip1.Refresh();
                    REP = RunExternalProcess(@"ATRAC\" + at3tool, " -e -br " + at3bitRate + " -wholeloop \"" + wavToProcess + "\" \"" + FileNameFinal + "\"");
                }
                catch (Exception ex) { MessageBox.Show("PSP conversion error: " + ex.Message); }
                finally
                {
                    if (isTempWav && File.Exists(wavToProcess)) File.Delete(wavToProcess);
                }
                if (!REP)
                    return;

                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                PlayerFile();
            }
            else if (radioButton6.Checked == true & textBox2.Text != "")
            {
                bool REP = false;
                try { REP = RunExternalProcess(@"ATRAC\" + at3tool, " -d \"" + FileNameSelected + "\" \"" + FileNameFinal2 + "\""); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                if (!REP)
                    return;

                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }
            else if (radioButton7.Checked == true & textBox2.Text != "")
            {
                bool REP = false;
                try {
                    int targetRate = (comboBox1.Text == "PSP") ? 44100 : 48000;
                    using (Mp3FileReader mp3 = new Mp3FileReader(FileNameSelected)) {
                        using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(targetRate, 16, mp3.WaveFormat.Channels), mp3)) {
                            WaveFileWriter.CreateWaveFile(FileNameFinal2, pcm);
                        }
                    }
                    REP = RunExternalProcess(@"ATRAC\" + at3tool, " -e -br " + at3bitRate + " -wholeloop \"" + FileNameFinal2 + "\" \"" + FileNameFinal + "\"");
                } catch (Exception ex) { MessageBox.Show(ex.Message); }
                DeleteFile();
                if (!REP)
                    return;

                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                PlayerFile();
            }
            else if (radioButton8.Checked == true & textBox2.Text != "")
            {
                try {
                    bool REP = RunExternalProcess(@"ATRAC\" + at3tool, " -d \"" + FileNameSelected + "\" \"" + FileNameFinal2 + "\"");
                    if (!REP)
                        return;

                    REP = RunExternalProcess(@"LAME\lame.exe", "-V2 \"" + FileNameFinal2 + "\" \"" + FileNameFinal + "\"");
                    DeleteFile();
                    if (!REP)
                        return;

                    toolStripStatusLabel1.Text = "Finish!";
                    statusStrip1.Refresh();

                } catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void mMoveFile()
        {
            try
            {
                string fileName = Path.GetFileName(FileNameFinal);
                string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                string destPath = FileNameFinal; // FileNameFinal already contains the correct full path

                // If the file was created in the program folder but the destination is different
                if (File.Exists(sourcePath) && sourcePath.ToLower() != destPath.ToLower())
                {
                    if (File.Exists(destPath)) File.Delete(destPath);
                    File.Move(sourcePath, destPath);
                }
            }
            catch (Exception ex)
            {
                // Only log real errors to avoid bothering the user
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conversion_errors.log"), 
                    "\r\nError in mMoveFile: " + ex.Message);
            }
        }

        private void mMoveFile2()
        {
            // This function was called when the user chose NOT to delete the WAV file
            // Simply make sure the files are where they should be
            try
            {
                mMoveFile();
                // If a second final file exists, such as MP3 -> AT3 + WAV
                if (!string.IsNullOrEmpty(FileNameFinal2)) {
                    string fileName2 = Path.GetFileName(FileNameFinal2);
                    string sourcePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName2);
                    string destPath2 = FileNameFinal2;
                    if (File.Exists(sourcePath2) && sourcePath2.ToLower() != destPath2.ToLower()) {
                        if (File.Exists(destPath2)) File.Delete(destPath2);
                        File.Move(sourcePath2, destPath2);
                    }
                }
            } catch { }
        }

        private void DeleteFile()
        {
            mInputBox("Question", "Do you want to delete WAV file ?", SystemIcons.Question, true, 3); 
        }

        private void PlayerFile()
        {
            mInputBox("Question", "Do you want to play file ?", SystemIcons.Question, true, 7);
        }

        private void tabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {

            if (textBox1.Text != "" || textBox2.Text != "" || radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true || radioButton4.Checked == true || radioButton5.Checked == true || radioButton6.Checked == true || radioButton7.Checked == true || radioButton8.Checked == true)
            { 
                
                textBox1.Text = ""; 
                textBox2.Text = ""; 
                button2.Enabled = false;
                button4.Enabled = false; 
                radioButton1.Checked = false; 
                radioButton2.Checked = false; 
                radioButton3.Checked = false; 
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                toolStripStatusLabel1.Text = "Ready!";
                statusStrip1.Refresh();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
        }

        private DialogResult InputBox(string title, string promptText, Icon icon, bool isDigit = false)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonMP3 = new Button();   
            Button buttonWAV = new Button();
            PictureBox icon1 = new PictureBox();

            if (isDigit == true)

            form.Text = title;
            label.Text = promptText;

            buttonMP3.Text = "MP3";
            buttonWAV.Text = "WAV";
            buttonMP3.DialogResult = DialogResult.OK;
            buttonWAV.DialogResult = DialogResult.Cancel;
            icon1.Image = icon.ToBitmap();

            label.SetBounds(50, 22, 290, 17);
            icon1.SetBounds(15, 15, 35, 35);
            buttonMP3.SetBounds(24, 54, 140, 23);
            buttonWAV.SetBounds(172, 54, 140, 23);

            label.AutoSize = true;
            label.ForeColor = Color.DarkRed;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonMP3.ForeColor = Color.Green;
            buttonWAV.ForeColor = Color.Green;
            buttonMP3.Font = new Font("Arial", 8, FontStyle.Bold);
            buttonWAV.Font = new Font("Arial", 8, FontStyle.Bold);

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonMP3, buttonWAV });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonMP3;
            form.CancelButton = buttonWAV;

            

            DialogResult dialogResult = form.ShowDialog(this);
            switch (dialogResult)
            {
                case DialogResult.OK:
                    if (tabControl1.SelectedTab == tabPage1)
                    {
                        radioButton1.Enabled = false;
                        radioButton2.Enabled = false;
                        radioButton3.Enabled = false;
                        radioButton4.Enabled = true;
                        radioButton4.Checked = true;
                        comboBox3.Items.Clear();
                        comboBox4.Items.Clear();
                        comboBox3.Enabled = false;
                        comboBox4.Enabled = true;
                        tabPage1ConsoleCombo();
                        textBox1.Text = fFile;
                    }
                    else if (tabControl1.SelectedTab == tabPage2)
                    {
                        radioButton5.Enabled = false;
                        radioButton6.Enabled = false;
                        radioButton7.Enabled = false;
                        radioButton8.Enabled = true;
                        radioButton8.Checked = true;
                        comboBox1.Items.Clear();
                        comboBox2.Items.Clear();
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = false;
                        tabPage2ConsoleCombo();
                        textBox2.Text = fFile;
                    }
                    break;
                case DialogResult.Cancel:
                    if (tabControl1.SelectedTab == tabPage1)
                    {
                        radioButton1.Enabled = false;
                        radioButton3.Enabled = false;
                        radioButton4.Enabled = false;
                        radioButton2.Enabled = true;
                        radioButton2.Checked = true;
                        comboBox3.Items.Clear();
                        comboBox4.Items.Clear();
                        comboBox3.Enabled = false;
                        comboBox4.Enabled = true;
                        tabPage1ConsoleCombo();
                        textBox1.Text = fFile;
                    }
                    else if (tabControl1.SelectedTab == tabPage2)
                    {
                        radioButton5.Enabled = false;
                        radioButton7.Enabled = false;
                        radioButton8.Enabled = false;
                        radioButton6.Enabled = true;
                        radioButton6.Checked = true;
                        comboBox1.Items.Clear();
                        comboBox2.Items.Clear();
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = false;
                        tabPage2ConsoleCombo();
                        textBox2.Text = fFile;
                    }
                    break;               
            }
            return dialogResult;

        }

        private DialogResult mInputBox(string title, string promptText, Icon icon, bool isDigit = false, int i = 0)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonYes = new Button();
            Button buttonNo = new Button();
            PictureBox icon1 = new PictureBox();
            int z = i;

            if (isDigit == true)

                form.Text = title;
            label.Text = promptText;

            buttonYes.Text = "Yes";
            buttonNo.Text = "No";
            buttonYes.DialogResult = DialogResult.OK;
            buttonNo.DialogResult = DialogResult.Cancel;
            icon1.Image = icon.ToBitmap();

            label.SetBounds(50, 22, 290, 17);
            icon1.SetBounds(15, 15, 35, 35);
            buttonYes.SetBounds(24, 54, 140, 23);
            buttonNo.SetBounds(172, 54, 140, 23);

            label.AutoSize = true;
            label.ForeColor = Color.DarkRed;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonYes.ForeColor = Color.Green;
            buttonNo.ForeColor = Color.Green;
            buttonYes.Font = new Font("Arial", 8, FontStyle.Bold);
            buttonNo.Font = new Font("Arial", 8, FontStyle.Bold);

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonYes, buttonNo });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonYes;
            form.CancelButton = buttonNo;

            DialogResult dialogResult = form.ShowDialog(this);
            if (z == 1)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:                       
                            at9DoProcess(); 
                        break;
                    case DialogResult.Cancel:

                        break;
                }
               
            }else if (z == 2)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                            at3DoProcess();
                        break;
                    case DialogResult.Cancel:

                        break;
                }
               
            }else if (z == 3)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                            System.IO.File.Delete(FileNameFinal2);
                            mMoveFile();
                        break;
                    case DialogResult.Cancel:
                        mMoveFile2();
                        break;
                }
                
            }else if (z == 4)
            {
                
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        
                        string path1 = dir + "\\" + fFileOrig;
                        string path2 = pPath + "\\" + fFileOrig;
                        MessageBox.Show(dir + "\\" + fFileOrig + "\r\r" + pPath + "\\" + fFileOrig);
                        if (!pPath.Equals(dir))
                        {
                            MessageBox.Show("0");
                            if (File.Exists(path1))
                            {
                                MessageBox.Show("1");
                                MessageBox.Show(dir + "\\" + fFileOrig + "\r\r" + pPath + "\\" + fFileOrig);
                                File.Delete(path1);
                                File.Copy(path2, path1);
                                rRename();
                            }
                            else
                            {
                                MessageBox.Show("2");
                                File.Copy(path2, path1);
                                rRename();
                            }

                        }
                        
                        break;
                    case DialogResult.Cancel:

                        break;
                }
                
            }
            else if (z == 5)
            {
                string path = dir + "\\" + FileNameFinal;
                string path2 = pPath + "\\" + FileNameFinal;
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        File.Delete(path2);
                        File.Move(path, path2);
                        break;
                    case DialogResult.Cancel:

                        break;
                }

            }
            else if (z == 6)
            { 
                string path = dir + "\\" + FileNameFinal;
                string path1 = dir + "\\" + FileNameFinal2;
                string path2 = pPath + "\\" + FileNameFinal;
                string path3 = pPath + "\\" + FileNameFinal2;
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        if (File.Exists(path2))
                        {
                            File.Delete(path2);
                            File.Move(path, path2);
                        }
                        else
                        {
                            File.Move(path, path2);
                        }
                        if (File.Exists(path3))
                        {
                            File.Delete(path3);
                            File.Move(path1, path3);
                        }
                        else
                        {
                            File.Move(path1, path3);
                        }
                        break;
                    case DialogResult.Cancel:

                        break;
                }

            }
            else if (z == 7)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        mPlayer();
                        break;
                    case DialogResult.Cancel:

                        break;
                }

            }
            else if (z == 8)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        try
                        {
                            at9Player.Play(FileNameSelected);

                            toolStripStatusLabel1.Text = "Playing AT9...";
                            statusStrip1.Refresh();

                            button3.Enabled = true;
                            //button1.Enabled = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("AT9 playback error: " + ex.Message);
                        }
                        break;

                    case DialogResult.Cancel:
                        break;
                }
            }
            else if (z == 9)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        PlayAT3(FileNameSelected);
                        break;
                    case DialogResult.Cancel:

                        break;
                }

            }
            return dialogResult;
        }

        private DialogResult mMessageBox(string title, string promptText, Icon icon, bool isDigit = false)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonOK = new Button();
            PictureBox icon1 = new PictureBox();

            if (isDigit == true)

            form.Text = title;
            label.Text = promptText;

            buttonOK.Text = "OK";
            buttonOK.DialogResult = DialogResult.OK;
            icon1.Image = icon.ToBitmap();

            label.SetBounds(60, 22, 290, 17);
            icon1.SetBounds(15, 15, 35, 35);
            buttonOK.SetBounds(100, 54, 140, 23);

            label.AutoSize = true;
            label.ForeColor = Color.DarkRed;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonOK.ForeColor = Color.Green;
            buttonOK.Font = new Font("Arial", 8, FontStyle.Bold);

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonOK });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOK;



            DialogResult dialogResult = form.ShowDialog(this);
            return dialogResult;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string[] versionpart = this.ProductVersion.Split('.');
            version = versionpart[0] + "." + versionpart[1];
            this.Text += MainForm.version;
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "http://bmk.hamtek-solutions.com/";
            LoadLanguageList();

            label1.AutoSize = false;
            label1.TextAlign = ContentAlignment.MiddleCenter;

            label2.AutoSize = false;
            label2.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true || radioButton7.Checked == true)
            {
                if (comboBox1.Text == "PSP") 
                { 
                    at3tool = "PSP_at3tool.exe"; button4.Enabled = false; tabPage2PSPCombo(); comboBox2.Enabled = true; 
                }
                else if (comboBox1.Text == "PS3") 
                { 
                    at3tool = "PS3_at3tool.exe"; button4.Enabled = false; tabPage2PS3Combo(); comboBox2.Enabled = true; 
                }
            }
            else
            {
                if (comboBox1.Text == "PSP") 
                { 
                    at3tool = "PSP_at3tool.exe"; button4.Enabled = true; tabPage2PSPCombo();
                }
                else if (comboBox1.Text == "PS3") 
                { 
                    at3tool = "PS3_at3tool.exe"; button4.Enabled = true; tabPage2PS3Combo();
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true || radioButton3.Checked == true)
            {
                if (comboBox4.Text == "PS4") 
                { 
                    at9tool = "PS4_at9tool.exe"; button2.Enabled = false; tabPage1PS4Combo(); comboBox3.Enabled = true; 
                }
                else if (comboBox4.Text == "PSVita")
                { 
                    at9tool = "PSVita_at9tool.exe"; button2.Enabled = false; tabPage1PSVitaCombo(); comboBox3.Enabled = true;
                    MessageBox.Show("If you make theme for PSVita/TV use the BitRate 144 for more compatibility", "Informations", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); 
                }
            }
            else
            {
                if (comboBox4.Text == "PS4") 
                { 
                    at9tool = "PS4_at9tool.exe"; button2.Enabled = true; tabPage1PS4Combo(); 
                }
                else if (comboBox4.Text == "PSVita") 
                { 
                    at9tool = "PSVita_at9tool.exe"; button2.Enabled = true; tabPage1PSVitaCombo();
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            at3bitRate = comboBox2.Text;
            button4.Enabled = true;

            // Update final file name with the new bitrate
            if (!string.IsNullOrEmpty(FileNameSelected))
            {
                string dirPath = Path.GetDirectoryName(FileNameSelected);
                string fileNameOnly = Path.GetFileNameWithoutExtension(FileNameSelected);
                FileNameFinal = Path.Combine(dirPath, fileNameOnly + "_" + at3bitRate + "bit.at3");
                FileNameFinal2 = Path.Combine(dirPath, fileNameOnly + ".wav");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            at9bitRate = comboBox3.Text;
            button2.Enabled = true;

            // Update final file name with the new bitrate
            if (!string.IsNullOrEmpty(FileNameSelected))
            {
                string dirPath = Path.GetDirectoryName(FileNameSelected);
                string fileNameOnly = Path.GetFileNameWithoutExtension(FileNameSelected);
                FileNameFinal = Path.Combine(dirPath, fileNameOnly + "_" + at9bitRate + "bit.at9");
                FileNameFinal2 = Path.Combine(dirPath, fileNameOnly + ".wav");
            }
        }

        private void tabPage1ConsoleCombo()
        {
            comboBox4.Items.Clear();
            for (int i = 0; i < consoleListAt9.Length; i++) { comboBox4.Items.Add(consoleListAt9[i]); }
        }

        private void tabPage1PSVitaCombo()
        {
            comboBox3.Items.Clear();
            for (int i = 0; i < psvitaList.Length; i++) { comboBox3.Items.Add(psvitaList[i]); }
            if (comboBox3.Items.Count > 0) comboBox3.SelectedIndex = 0;
        }

        private void tabPage1PS4Combo()
        {
            comboBox3.Items.Clear();
            for (int i = 0; i < ps4List.Length; i++) { comboBox3.Items.Add(ps4List[i]); }
            if (comboBox3.Items.Count > 0) comboBox3.SelectedIndex = 0;
        }

        private void tabPage2ConsoleCombo()
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < consoleListAt3.Length; i++) { comboBox1.Items.Add(consoleListAt3[i]); }
        }

        private void tabPage2PSPCombo()
        {
            comboBox2.Items.Clear();
            for (int i = 0; i < pspList.Length; i++) { comboBox2.Items.Add(pspList[i]); } 
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
        }

        private void tabPage2PS3Combo()
        {
            comboBox2.Items.Clear();
            for (int i = 0; i < ps3List.Length; i++) { comboBox2.Items.Add(ps3List[i]); }
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (comboBox4.Text == "PS4" && comboBox3.Enabled == true)
            {
                Ps4BitrateInfo Ps4Load = new Ps4BitrateInfo();
                Ps4Load.Show();
            }
            else if (comboBox4.Text == "PSVita" && comboBox3.Enabled == true)
            {
                PsvitaBitrateInfo PsvitaLoad = new PsvitaBitrateInfo();
                PsvitaLoad.Show();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "PSP" && comboBox2.Enabled == true)
            {
                PspBitrateInfo PspLoad = new PspBitrateInfo();
                PspLoad.Show();
            }
            else if (comboBox1.Text == "PS3" && comboBox2.Enabled == true)
            {
                Ps3BitrateInfo Ps3Load = new Ps3BitrateInfo();
                Ps3Load.Show();
            }
        }

        private void mPlayer()
        {

            string fileToPlay = FileNameFinal;

            if (!File.Exists(fileToPlay))
            {
                MessageBox.Show("File not found: " + fileToPlay);
                return;
            }

            string ext = Path.GetExtension(fileToPlay).ToLower();

            if (ext == ".at9")
            {
                try
                {
                    at9Player.Play(fileToPlay);

                    toolStripStatusLabel1.Text = "Playing AT9...";
                    statusStrip1.Refresh();
                    button3.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("AT9 playback error: " + ex.Message);
                }

                return;
            }

            if (ext == ".at3")
            {
                try
                {
                    playerProcess = new Process();
                    playerStartInfo = new ProcessStartInfo();
                    playerStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    playerStartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PLAYER\MiniPlayer.exe");
                    playerStartInfo.Arguments = "\"" + FileNameFinal + "\"";
                    playerStartInfo.UseShellExecute = false;
                    playerStartInfo.CreateNoWindow = true;
                    playerStartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    playerProcess.StartInfo = playerStartInfo;
                    playerProcess.Start();

                    toolStripStatusLabel1.Text = "Playing...";
                    statusStrip1.Refresh();
                    button1.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Playback error: " + ex.Message);
                }
            }
        }

        private void PlayAT3(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File not found: " + filePath);
                    return;
                }

                playerProcess = new Process();
                playerStartInfo = new ProcessStartInfo();
                playerStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                playerStartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PLAYER\MiniPlayer.exe");
                playerStartInfo.Arguments = "\"" + filePath + "\"";
                playerStartInfo.UseShellExecute = false;
                playerStartInfo.CreateNoWindow = true;
                playerStartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

                playerProcess.StartInfo = playerStartInfo;
                playerProcess.Start();

                toolStripStatusLabel1.Text = "Playing AT3...";
                statusStrip1.Refresh();

                button1.Enabled = true;
                button3.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("AT3 playback error: " + ex.Message);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Send the URL to the operating system.
            Process.Start(e.Link.LinkData as string);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            playerProcess.Kill();

            toolStripStatusLabel1.Text = "Stop!";
            statusStrip1.Refresh();
            button1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            at9Player.Stop();

            toolStripStatusLabel1.Text = "Stop!";
            statusStrip1.Refresh();
            button3.Enabled = false;

        }

        private bool RunExternalProcess(string fileName, string arguments)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                
                process.StartInfo = startInfo;
                process.Start();
                
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                
                process.WaitForExit();

                if (process.ExitCode != 0 || !string.IsNullOrEmpty(error))
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conversion_errors.log");
                    string logEntry = string.Format("\r\n--- {0} ---\r\nFile: {1}\r\nCommand: {2} {3}\r\nOutput:\r\n{4}\r\nErrors:\r\n{5}\r\n--------------------------\r\n", 
                        DateTime.Now.ToString(), fileName, fileName, arguments, output, error);
                    
                    File.AppendAllText(logPath, logEntry);
                    
                    if (process.ExitCode != 0)
                    {
                         toolStripStatusLabel1.Text = "Error! Check conversion_errors.log";
                         statusStrip1.Refresh();
                    }
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conversion_errors.log");
                File.AppendAllText(logPath, "\r\nCRITICAL EXCEPTION: " + DateTime.Now.ToString() + " - " + ex.Message + "\r\n");
                MessageBox.Show("Critical error while executing the tool. Check conversion_errors.log", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadLanguageList()
        {
            comboBoxLanguage.Items.Clear();

            string langPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang");

            if (!Directory.Exists(langPath))
                Directory.CreateDirectory(langPath);

            string[] files = Directory.GetFiles(langPath, "*.json");

            if (files.Length == 0)
            {
                LoadDefaultEnglishLanguage();
                ApplyLanguage();

                comboBoxLanguage.Items.Clear();
                comboBoxLanguage.Items.Add("English");
                comboBoxLanguage.SelectedIndex = 0;
                comboBoxLanguage.Enabled = false;

                return;
            }

            foreach (string file in files)
            {
                string code = Path.GetFileNameWithoutExtension(file);
                string json = File.ReadAllText(file, Encoding.UTF8);

                Dictionary<string, string> tempLang =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                string languageName = tempLang.ContainsKey("language")
                    ? tempLang["language"]
                    : code;

                comboBoxLanguage.Items.Add(new LanguageItem
                {
                    Code = code,
                    Name = languageName
                });
            }

            foreach (LanguageItem item in comboBoxLanguage.Items)
            {
                if (item.Code.ToLower() == "en")
                {
                    comboBoxLanguage.SelectedItem = item;
                    break;
                }
            }
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            string langCode = comboBoxLanguage.Text;
            LoadLanguage(langCode);
            ApplyLanguage();
        }

        private void LoadLanguage(string langCode)
        {
            string filePath = Path.Combine(languageDir, langCode + ".json");

            if (!File.Exists(filePath))
                filePath = Path.Combine(languageDir, "en.json");

            string json = File.ReadAllText(filePath, Encoding.UTF8);

            lang = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        private string T(string key)
        {
            return lang.ContainsKey(key) ? lang[key] : key;
        }

        private void comboBoxLanguage_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBoxLanguage.SelectedItem is LanguageItem selectedLanguage)
            {
                LoadLanguage(selectedLanguage.Code);
                ApplyLanguage();
            }
        }

        private void ApplyLanguage()
        {
            tabPage1.Text = T("tab_at9");
            tabPage2.Text = T("tab_at3");
            label1.Text = T("drag_drop");
            label2.Text = T("drag_drop");
            button2.Text = T("convert");
            button4.Text = T("convert");
            button1.Text = T("stop_playing");
            button3.Text = T("stop_playing");
            groupBox1.Text = T("conversion_type");
            groupBox2.Text = T("conversion_type");
            label3.Text = T("bitrate");
            label4.Text = T("console_type");
            label5.Text = T("bitrate");
            label6.Text = T("console_type");
        }

        private void LoadDefaultEnglishLanguage()
        {
            lang = new Dictionary<string, string>
            {
                { "tab_at9", "AT9Tool PSVita/TV & P4" },
                { "tab_at3", "AT3Tool PSP & PS3" },
                { "drag_drop", "Drag and drop your file here" },
                { "convert", "Convert" },
                { "stop_playing", "Stop Playing" },
                { "conversion_type", "Select Type Convertion" },
                { "bitrate", "BitRate [kbps]:" },
                { "console_type", "Console Type:" },
                { "language", "English" }
            };
        }

        private class LanguageItem
        {
            public string Code { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}

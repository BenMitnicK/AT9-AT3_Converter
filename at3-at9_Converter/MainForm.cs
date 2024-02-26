using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime;
using NAudio.Wave;
using NAudio;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using at3_at9_Converter.Properties;
using System.Diagnostics;

namespace at3_at9_Converter
{

    public partial class MainForm : Form
    {

        //var dllDirectory = @"C:/some/path";
        //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory);
        public static string dir = "", appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), version = "";
        Process playerProcess = new Process();
        ProcessStartInfo playerStartInfo = new ProcessStartInfo();
        public string FileNameSelected = "";
        public string FileNameConvert = "";
        public string FileNameFinal = "";
        public string FileNameFinal2 = "";
        public string VerifFileExtention = "";
        public string fFile = "";
        public string FileNameDrop = "";
        private string tempFile = "";
        private string pPath = "";
        private string nFile = "";
        private string fFileOrig = "";
        private string at3tool = "";
        private string at9tool = "";
        private string at3bitRate = "";
        private string at9bitRate = "";
        private CustomToolTip tip;

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
            //textBox1.Text = pPath + "\\" + nFile;
            //VerifFileExtention = textBox1.Text.Substring(textBox1.Text.LastIndexOf((".")));
                fFile = dir + "\\" + nFile;
                fFileOrig = nFile;
                
            //VerifExtention_at9();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                nFile = sReplace(fFileOrig);
                System.IO.File.Move(textBox2.Text, pPath + "\\" + nFile);
                //textBox2.Text = pPath + "\\" + nFile;
                //VerifFileExtention = textBox2.Text.Substring(textBox2.Text.LastIndexOf((".")));
                fFile = textBox2.Text;
                fFileOrig = nFile;
                //VerifExtention_at3();
            }

        }

        private void VerifExtention_at9()
        {
            var regex = new Regex(@"\s");
            if (VerifFileExtention.Equals(".wav") || VerifFileExtention.Equals(".WAV"))
            {
                if (regex.IsMatch(fFileOrig))
                {
                    mInputBox("Your file can't convert because it\r contains space(s)", "     would you like to rename it ?", SystemIcons.Question, true, 4);
                }
                else
                {
                    //MessageBox.Show("M1 " + textBox1.Text);
                    FileNameDrop = textBox1.Text.Substring(textBox1.Text.LastIndexOf(("\\")));
                    FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "wav";
                    FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".at9";
                    radioButton2.Enabled = false;
                    radioButton3.Enabled = false;
                    radioButton4.Enabled = false;
                    //button2.Enabled = false;
                    radioButton1.Enabled = true;
                    radioButton1.Checked = true;
                    tabPage1ConsoleCombo();
                    comboBox3.Items.Clear();
                    textBox1.Text = fFile;
                    //comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                }
                if (!pPath.Equals(dir))
                {
                    //MessageBox.Show("M2 " + textBox1.Text);
                    FileNameDrop = textBox1.Text.Substring(textBox1.Text.LastIndexOf(("\\")));
                    FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "wav";
                    FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".at9";
                    radioButton2.Enabled = false;
                    radioButton3.Enabled = false;
                    radioButton4.Enabled = false;
                    //button2.Enabled = false;
                    radioButton1.Enabled = true;
                    radioButton1.Checked = true;
                    tabPage1ConsoleCombo();
                    comboBox3.Items.Clear();
                    textBox1.Text = fFile;
                    //comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                }
                else
                {
                    //MessageBox.Show("M3 " + textBox1.Text);
                    FileNameDrop = textBox1.Text.Substring(textBox1.Text.LastIndexOf(("\\")));
                    FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "wav";
                    FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".at9";
                    radioButton2.Enabled = false;
                    radioButton3.Enabled = false;
                    radioButton4.Enabled = false;
                    //button2.Enabled = false;
                    radioButton1.Enabled = true;
                    radioButton1.Checked = true;
                    tabPage1ConsoleCombo();
                    comboBox3.Items.Clear();
                    textBox1.Text = fFile;
                    //comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                }

            }
            else if (VerifFileExtention.Equals(".at9") || VerifFileExtention.Equals(".AT9"))
            {
                if (regex.IsMatch(fFileOrig))
                {
                    mInputBox("Your file can't convert because it\r contains space(s)", "     would you like to rename it ?", SystemIcons.Question, true, 4);
                    FileNameDrop = textBox1.Text.Substring(textBox1.Text.LastIndexOf(("\\")));
                    FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "at9";
                    FileNameFinal2 = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".wav";
                    FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".mp3";
                    InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
                }
                else
                {
                    FileNameDrop = textBox1.Text.Substring(textBox1.Text.LastIndexOf(("\\")));
                    FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "at9";
                    FileNameFinal2 = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".wav";
                    FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".mp3";
                    InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
                }          

            }
            else if (VerifFileExtention.Equals(".mp3") || VerifFileExtention.Equals(".MP3"))
            {

                FileNameDrop = textBox1.Text.Substring(textBox1.Text.LastIndexOf(("\\")));
                FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "mp3";
                FileNameFinal2 = sReplace(FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".wav");
                FileNameFinal = sReplace(FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".at9");
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton4.Enabled = false;
                //button2.Enabled = false;
                radioButton3.Enabled = true;
                radioButton3.Checked = true;
                tabPage1ConsoleCombo();
                comboBox3.Items.Clear();
                textBox1.Text = fFile;
                //comboBox3.Enabled = true;
                comboBox4.Enabled = true;

            }
            else
            {

                mMessageBox("Informations", "Please select MP3 Or Wav Or at9 File", SystemIcons.Information, true);
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                button2.Enabled = false;                      
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                textBox1.Text = "";

            }

        }

        private void VerifExtention_at3()
        {
            var regex = new Regex(@"\s");
            if (VerifFileExtention.Equals(".wav") || VerifFileExtention.Equals(".WAV"))
            {
                if (regex.IsMatch(fFileOrig))
                {
                    mInputBox("Your file can't convert because it\r contains space(s)", "     would you like to rename it ?", SystemIcons.Question, true, 4);
                }
                else
                {
                FileNameDrop = textBox2.Text.Substring(textBox2.Text.LastIndexOf(("\\")));
                FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "wav";
                FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".at3";
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                radioButton5.Enabled = true;
                radioButton5.Checked = true;
                tabPage2ConsoleCombo();
                comboBox2.Items.Clear();
                textBox2.Text = fFile;
                comboBox1.Enabled = true;
                //comboBox2.Enabled = true;
                }
                FileNameDrop = textBox2.Text.Substring(textBox2.Text.LastIndexOf(("\\")));
                FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "wav";
                FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".at3";
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                radioButton5.Enabled = true;
                radioButton5.Checked = true;
                tabPage2ConsoleCombo();
                comboBox2.Items.Clear();
                textBox2.Text = fFile;
                comboBox1.Enabled = true;
                //comboBox2.Enabled = true;

            }
            else if (VerifFileExtention.Equals(".at3") || VerifFileExtention.Equals(".AT3"))
            {
                if (regex.IsMatch(fFileOrig))
                {
                    mInputBox("Your file can't convert because it\r contains space(s)", "     would you like to rename it ?", SystemIcons.Question, true, 4);
                    FileNameDrop = textBox2.Text.Substring(textBox2.Text.LastIndexOf(("\\")));
                    FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "at3";
                    FileNameFinal2 = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".wav";
                    FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".mp3";
                    InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
                }
                else
                {
                    FileNameDrop = textBox2.Text.Substring(textBox2.Text.LastIndexOf(("\\")));
                    FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "at3";
                    FileNameFinal2 = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".wav";
                    FileNameFinal = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".mp3";
                    InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
                }   

            }
            else if (VerifFileExtention.Equals(".mp3") || VerifFileExtention.Equals(".MP3"))
            {

                FileNameDrop = textBox2.Text.Substring(textBox2.Text.LastIndexOf(("\\")));
                FileNameSelected = FileNameDrop.Substring(1, FileNameDrop.LastIndexOf(("."))) + "mp3";
                FileNameFinal2 = sReplace(FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".wav");
                FileNameFinal = sReplace(FileNameSelected.Substring(0, FileNameSelected.LastIndexOf(("."))) + ".at3");
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton8.Enabled = false;
                radioButton7.Enabled = true;
                radioButton7.Checked = true;
                tabPage2ConsoleCombo();
                comboBox2.Items.Clear();
                textBox2.Text = fFile;
                comboBox1.Enabled = true;
                //comboBox2.Enabled = true;

            }
            else
            {

                mMessageBox("Informations", "Please select MP3 Or Wav Or at3 File", SystemIcons.Information, true);
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
                button4.Enabled = false;
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();             
                comboBox1.Enabled = false;               
                comboBox2.Enabled = false;                
                textBox2.Text = "";

            }

        }

        /*private void button1_Click(object sender, EventArgs e)
        {
           
            if (radioButton1.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    //openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "wav files(*.wav)|*.wav";

                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal = FileNameConvert + ".at9";
                        button2.Enabled = true;
                    }
                }
            }
            else if (radioButton2.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "at9 files(*.at9)|*.at9";


                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal = FileNameConvert + ".wav";
                        button2.Enabled = true;
                    }
                }
            }
            else if (radioButton3.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "MP3 files(*.mp3)|*.mp3";


                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal2 = FileNameConvert + ".at9";
                        FileNameFinal = FileNameConvert + ".wav";
                        button2.Enabled = true;
                    }
                }
            }
            else if (radioButton4.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "at9 files(*.at9)|*.at9";


                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal2 = FileNameConvert + ".mp3";
                        FileNameFinal = FileNameConvert + ".wav";
                        button2.Enabled = true;
                    }
                }
            }
            else {

                mMessageBox("Informations", "Please select Convertion Type First!", SystemIcons.Information, true);                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (radioButton5.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    //openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "wav files(*.wav)|*.wav";

                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox2.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal = FileNameConvert + ".at3";
                        button4.Enabled = true;
                    }
                }
            }
            else if (radioButton6.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "at3 files(*.at3)|*.at3";


                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox2.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal = FileNameConvert + ".wav";
                        button4.Enabled = true;
                    }
                }
            }
            else if (radioButton7.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "MP3 files(*.mp3)|*.mp3";


                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox2.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal2 = FileNameConvert + ".at3";
                        FileNameFinal2 = sReplace(FileNameFinal2);
                        FileNameFinal = FileNameConvert + ".wav";
                        FileNameFinal = sReplace(FileNameFinal);
                        button4.Enabled = true;
                    }
                }
            }
            else if (radioButton8.Checked == true)
            {
                // Initialise une instance de OpenFileDialog  
                using (OpenFileDialog openfileDialog = new OpenFileDialog())
                {
                    openfileDialog.RestoreDirectory = true;
                    openfileDialog.Filter = "at3 files(*.at3)|*.at3";


                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox2.Text = openfileDialog.FileName;
                        FileNameSelected = openfileDialog.SafeFileName;
                        FileNameConvert = FileNameSelected.Substring(0, FileNameSelected.LastIndexOf((".")));
                        FileNameFinal2 = FileNameConvert + ".mp3";
                        FileNameFinal = FileNameConvert + ".wav";
                        button4.Enabled = true;
                    }
                }
            }
            else
            {

                mMessageBox("Informations", "Please select Convertion Type First!", SystemIcons.Information, true); 
            }
        }*/

        

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

                try
                {

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    startInfo.FileName = @"ATRAC\" + at9tool;
                    startInfo.Arguments = " -e -br " + at9bitRate + " -wholeloop " + FileNameSelected + " " + FileNameFinal;
                    //MessageBox.Show(startInfo.FileName + startInfo.Arguments);
                    process.StartInfo = startInfo;
                    process.Start();
                    //label1.SetBounds(32, 164, 234, 25);
                    //label1.Text = "AT9 in Progress...";                   
                    //label1.Refresh();
                    toolStripStatusLabel1.Text = "AT9 in Progress...";
                    statusStrip1.Refresh();
                    process.WaitForExit();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //mMoveFile();
                //label1.SetBounds(93, 164, 234, 25);
                //label1.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }
            else if (radioButton2.Checked == true & textBox1.Text != "")
            {

                try
                {

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.FileName = @"ATRAC\" + at9tool;
                    startInfo.Arguments = " -d " + FileNameSelected + " " + FileNameFinal2;
                    process.StartInfo = startInfo;
                    process.Start();
                    //label1.SetBounds(32, 164, 234, 25);
                    //label1.Text = "Wav in Progress...";                    
                    //label1.Refresh();
                    toolStripStatusLabel1.Text = "Wav in Progress...";
                    statusStrip1.Refresh();
                    process.WaitForExit();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //mMoveFile();
                //label1.SetBounds(93, 164, 234, 25);
                //label1.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                //System.Threading.Thread.Sleep(1000);
            }
            else if (radioButton3.Checked == true & textBox1.Text != "")
            {

                try
                {
                    try
                    {
                        
                        //label1.SetBounds(32, 164, 234, 25);
                        //label1.Text = "Wav in Progress...";                        
                        //label1.Refresh();
                        toolStripStatusLabel1.Text = "Wav in Progress...";
                        statusStrip1.Refresh();
                        
                        using (Mp3FileReader mp3 = new Mp3FileReader(FileNameSelected))
                        {
                            /*using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                            {
                                WaveFileWriter.CreateWaveFile(FileNameFinal2, pcm);
                            }*/
                            using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(48000, 16, 1), mp3))
                            {
                                WaveFileWriter.CreateWaveFile(FileNameFinal2, pcm);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    try
                    {

                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = @"ATRAC\" + at9tool;
                        startInfo.Arguments = " -e -br " + at9bitRate + " -wholeloop " + FileNameFinal2 + " " + FileNameFinal;
                        process.StartInfo = startInfo;
                        process.Start();
                        //label1.SetBounds(32, 164, 234, 25);
                        //label1.Text = "AT9 in Progress...";                       
                        //label1.Refresh();
                        toolStripStatusLabel1.Text = "AT9 in Progress...";
                        statusStrip1.Refresh();
                        process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DeleteFile();
                //label1.SetBounds(93, 164, 234, 25);
                //label1.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }
            else if (radioButton4.Checked == true & textBox1.Text != "")
            {

                try
                {
                    try
                    {
                        
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = @"ATRAC\" + at9tool;
                        startInfo.Arguments = " -d " + FileNameSelected + " " + FileNameFinal2;
                        process.StartInfo = startInfo;
                        process.Start();
                        //label1.SetBounds(32, 164, 234, 25);
                        //label1.Text = "Wav in Progress...";                        
                        //label1.Refresh();
                        toolStripStatusLabel1.Text = "Wav in Progress...";
                        statusStrip1.Refresh();
                        process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    try
                    {

                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                        psi.FileName = @"Lame\lame.exe";
                        psi.Arguments = "-V2 " + FileNameFinal2 + " " + FileNameFinal;
                        psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                        //label1.SetBounds(32, 164, 234, 25);
                        //label1.Text = "MP3 in Progress...";                       
                        //label1.Refresh();
                        toolStripStatusLabel1.Text = "MP3 in Progress...";
                        statusStrip1.Refresh();
                        p.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DeleteFile();
                //label1.SetBounds(93, 164, 234, 25);
                //label1.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }

        }

        private void at3DoProcess()
        {
 
            if (radioButton5.Checked == true & textBox2.Text != "")
            {

                try
                {

                    //System.Diagnostics.Process process = new System.Diagnostics.Process();
                    //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = @"ATRAC\" + at3tool;
                    startInfo.Arguments = " -e -br " + at3bitRate + " -wholeloop " + FileNameSelected + " " + FileNameFinal;
                    //MessageBox.Show(startInfo.FileName + startInfo.Arguments);
                    process.StartInfo = startInfo;
                    process.Start();
                    //label2.SetBounds(27, 179, 234, 25);
                    //label2.Text = "AT3 in Progress...";
                    //label2.Refresh();
                    toolStripStatusLabel1.Text = "AT3 in Progress...";
                    statusStrip1.Refresh();                    
                    process.WaitForExit();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //mMoveFile();
                //label2.SetBounds(93, 179, 234, 25);
                //label2.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                PlayerFile();
            }
            else if (radioButton6.Checked == true & textBox2.Text != "")
            {

                try
                {

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.FileName = @"ATRAC\" + at3tool;
                    startInfo.Arguments = " -d " + FileNameSelected + " " + FileNameFinal2;
                    process.StartInfo = startInfo;
                    process.Start();
                    //label2.SetBounds(27, 179, 234, 25);
                    //label2.Text = "Wav in Progress...";
                    //label2.Refresh();
                    toolStripStatusLabel1.Text = "Wav in Progress...";
                    statusStrip1.Refresh();
                    process.WaitForExit();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //mMoveFile();
                //label2.SetBounds(93, 179, 234, 25);
                //label2.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                //System.Threading.Thread.Sleep(1000);
            }
            else if (radioButton7.Checked == true & textBox2.Text != "")
            {

                try
                {
                    try
                    {

                        //label2.SetBounds(27, 179, 234, 25);
                        //label2.Text = "Wav in Progress...";
                        //label2.Refresh();
                        toolStripStatusLabel1.Text = "Wav in Progress...";
                        statusStrip1.Refresh();

                        using (Mp3FileReader mp3 = new Mp3FileReader(FileNameSelected))
                        {
                            if (comboBox1.Text == "PSP")
                            {
                                using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(44100, 16, 1), mp3))
                                {
                                    WaveFileWriter.CreateWaveFile(FileNameFinal2, pcm);
                                }
                            }
                            else
                            {
                                using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(48000, 16, 1), mp3))
                                {
                                    WaveFileWriter.CreateWaveFile(FileNameFinal2, pcm);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    try
                    {

                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = @"ATRAC\" + at3tool;
                        startInfo.Arguments = " -e -br " + at3bitRate + " -wholeloop " + FileNameFinal2 + " " + FileNameFinal;
                        process.StartInfo = startInfo;
                        process.Start();
                        //label2.SetBounds(27, 179, 234, 25);
                        //label2.Text = "AT3 in Progress...";
                        //label2.Refresh();
                        toolStripStatusLabel1.Text = "AT3 in Progress...";
                        statusStrip1.Refresh();
                        process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DeleteFile();
                //label2.SetBounds(93, 179, 234, 25);
                //label2.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
                PlayerFile();
            }
            else if (radioButton8.Checked == true & textBox2.Text != "")
            {

                try
                {
                    try
                    {

                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = @"ATRAC\" + at3tool;
                        startInfo.Arguments = " -d " + FileNameSelected + " " + FileNameFinal2;
                        process.StartInfo = startInfo;
                        process.Start();
                        //label2.SetBounds(27, 179, 234, 25);
                        //label2.Text = "Wav in Progress...";
                        //label2.Refresh();
                        toolStripStatusLabel1.Text = "Wav in Progress...";
                        statusStrip1.Refresh();
                        process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    try
                    {

                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                        psi.FileName = @"Lame\lame.exe";
                        psi.Arguments = "-V2 " + FileNameFinal2 + " " + FileNameFinal;
                        psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                        //label2.SetBounds(27, 179, 234, 25);
                        //label2.Text = "MP3 in Progress...";
                        //label2.Refresh();
                        toolStripStatusLabel1.Text = "MP3 in Progress...";
                        statusStrip1.Refresh();
                        p.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DeleteFile();
                //label2.SetBounds(93, 179, 234, 25);
                //label2.Text = "Finish!";
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }

        }

        private void mMoveFile()
        {

            string path = dir + "\\" + FileNameFinal;
            string path2 = pPath + "\\" + FileNameFinal;
            //MessageBox.Show("M3 " + path);
            //MessageBox.Show("M33 " + path2);
            try
            {
                /*if (!File.Exists(path))
                {
                    // This statement ensures that the file is created,
                    // but the handle is not kept.
                    using (FileStream fs = File.Create(path)) { }
                }*/

                // Ensure that the target does not exist.
                if (!pPath.Equals(dir))
                {
                    if (File.Exists(path2))
                    {
                        mInputBox("Move file to directory", "File(s) Exist Do you want to replace it ?", SystemIcons.Question, true, 5); //File.Delete(path2);
                    }

                    else
                    {

                        // Move the file.
                        File.Move(path, path2);

                    }

                    // See if the original exists now.
                    /*if (File.Exists(path))
                    {
                        Console.WriteLine("The original file still exists, which is unexpected.");
                    }
                    else
                    {
                        Console.WriteLine("The original file no longer exists, which is expected.");
                    }*/
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void mMoveFile2()
        {

            string path = dir + "\\" + FileNameFinal;
            string path1 = dir + "\\" + FileNameFinal2;
            string path2 = pPath + "\\" + FileNameFinal;
            string path3 = pPath + "\\" + FileNameFinal2;
            try
            {
                /*if (!File.Exists(path))
                {
                    // This statement ensures that the file is created,
                    // but the handle is not kept.
                    using (FileStream fs = File.Create(path)) { }
                }*/

                // Ensure that the target does not exist.
                if (!pPath.Equals(dir))
                {
                    if (File.Exists(path2) || File.Exists(path3))
                    {
                        mInputBox("Move file to directory", "File(s) Exist Do you want to replace it ?", SystemIcons.Question, true, 6); //File.Delete(path2);
                    }

                    else
                    {

                        // Move the file.
                        File.Move(path, path2);
                        File.Move(path1, path3);
                        Console.WriteLine("{0} was moved to {1}.", path, path2);
                    }

                    // See if the original exists now.
                    /*if (File.Exists(path))
                    {
                        Console.WriteLine("The original file still exists, which is unexpected.");
                    }
                    else
                    {
                        Console.WriteLine("The original file no longer exists, which is expected.");
                    }*/
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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
                //label1.SetBounds(32, 164, 234, 25); label1.Text = "DragDrop Your File";
                //label2.SetBounds(27, 179, 234, 25); label2.Text = "DragDrop Your File";
                toolStripStatusLabel1.Text = "Ready!";
                statusStrip1.Refresh();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                textBox1.Text = ""; button2.Enabled = false; comboBox3.Enabled = false; comboBox4.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label1.SetBounds(32, 164, 234, 25); label1.Text = "DragDrop Your File";*/
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "") { textBox1.Text = ""; button2.Enabled = false; comboBox4.Enabled = false; comboBox3.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label1.SetBounds(32, 164, 234, 25); label1.Text = "DragDrop Your File";*/ }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "") { textBox1.Text = ""; button2.Enabled = false; comboBox4.Enabled = false; comboBox3.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label1.SetBounds(32, 164, 234, 25); label1.Text = "DragDrop Your File";*/ }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "") { textBox1.Text = ""; button2.Enabled = false; comboBox4.Enabled = false; comboBox3.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label1.SetBounds(32, 164, 234, 25); label1.Text = "DragDrop Your File";*/ }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "") { textBox2.Text = ""; button4.Enabled = false; comboBox1.Enabled = false; comboBox2.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label2.SetBounds(27, 179, 234, 25); label2.Text = "DragDrop Your File";*/ }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "") { textBox2.Text = ""; button4.Enabled = false; comboBox1.Enabled = false; comboBox2.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label2.SetBounds(27, 179, 234, 25); label2.Text = "DragDrop Your File";*/ }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "") { textBox2.Text = ""; button4.Enabled = false; comboBox1.Enabled = false; comboBox2.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label2.SetBounds(27, 179, 234, 25); label2.Text = "DragDrop Your File";*/ }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "") { textBox2.Text = ""; button4.Enabled = false; comboBox1.Enabled = false; comboBox2.Enabled = false; toolStripStatusLabel1.Text = "Ready!"; statusStrip1.Refresh(); /*label2.SetBounds(27, 179, 234, 25); label2.Text = "DragDrop Your File";*/ }
        }

        private DialogResult InputBox(string title, string promptText, Icon icon, bool isDigit = false)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonMP3 = new Button();   
            Button buttonWAV = new Button();
            //Icon icon1;
            PictureBox icon1 = new PictureBox();
            //TextBox textBox = new TextBox();

            if (isDigit == true)
                //textBox.TypeNumricOnly = true; MessageBoxIcon.Exclamation; 

                //textBox.Width = 1000;

            form.Text = title;
            label.Text = promptText;
            //textBox.Text = value;

            buttonMP3.Text = "MP3";
            buttonWAV.Text = "WAV";
            buttonMP3.DialogResult = DialogResult.OK;
            buttonWAV.DialogResult = DialogResult.Cancel;
            icon1.Image = icon.ToBitmap();
            //icon1 = icon;

            label.SetBounds(50, 22, 290, 17);
            //textBox.SetBounds(12, 36, 372, 20);
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
            //textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            //buttonMP3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            //buttonWAV.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonMP3, buttonWAV });
            //form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
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
                        //button2.Enabled = true;
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
                        //button4.Enabled = true;
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
                        //button2.Enabled = true;
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
                        //button4.Enabled = true;
                    }
                    break;               
            }
            //value = textBox.Text;
            return dialogResult;

        }

        private DialogResult mInputBox(string title, string promptText, Icon icon, bool isDigit = false, int i = 0)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonYes = new Button();
            Button buttonNo = new Button();
            //Icon icon1;
            PictureBox icon1 = new PictureBox();
            int z = i;
            //TextBox textBox = new TextBox();

            if (isDigit == true)
                //textBox.TypeNumricOnly = true; MessageBoxIcon.Exclamation; 

                //textBox.Width = 1000;

                form.Text = title;
            label.Text = promptText;
            //textBox.Text = value;

            buttonYes.Text = "Yes";
            buttonNo.Text = "No";
            buttonYes.DialogResult = DialogResult.OK;
            buttonNo.DialogResult = DialogResult.Cancel;
            icon1.Image = icon.ToBitmap();
            //icon1 = icon;

            label.SetBounds(50, 22, 290, 17);
            //textBox.SetBounds(12, 36, 372, 20);
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
            //textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            //buttonMP3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            //buttonWAV.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonYes, buttonNo });
            //form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
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
            return dialogResult;
        }

        private DialogResult mMessageBox(string title, string promptText, Icon icon, bool isDigit = false)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonOK = new Button();
            //Button buttonWAV = new Button();
            //Icon icon1;
            PictureBox icon1 = new PictureBox();
            //TextBox textBox = new TextBox();

            if (isDigit == true)
                //textBox.TypeNumricOnly = true; MessageBoxIcon.Exclamation; 

                //textBox.Width = 1000;

            form.Text = title;
            label.Text = promptText;
            //textBox.Text = value;

            buttonOK.Text = "OK";
            //buttonWAV.Text = "WAV";
            buttonOK.DialogResult = DialogResult.OK;
            //buttonWAV.DialogResult = DialogResult.Cancel;
            icon1.Image = icon.ToBitmap();
            //icon1 = icon;

            label.SetBounds(60, 22, 290, 17);
            //textBox.SetBounds(12, 36, 372, 20);
            icon1.SetBounds(15, 15, 35, 35);
            buttonOK.SetBounds(100, 54, 140, 23);
            //buttonWAV.SetBounds(172, 54, 140, 23);

            label.AutoSize = true;
            label.ForeColor = Color.DarkRed;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonOK.ForeColor = Color.Green;
            //buttonWAV.ForeColor = Color.Green;
            buttonOK.Font = new Font("Arial", 8, FontStyle.Bold);
            //buttonWAV.Font = new Font("Arial", 8, FontStyle.Bold);
            //textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            //buttonMP3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            //buttonWAV.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonOK });
            //form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOK;
            //form.CancelButton = buttonWAV;



            DialogResult dialogResult = form.ShowDialog(this);
            /*switch (dialogResult)
            {
                case DialogResult.OK:
                    if (tabControl1.SelectedTab == tabPage1)
                    {
                        radioButton4.Checked = true;
                        textBox1.Text = fFile;
                        button2.Enabled = true;
                    }
                    else if (tabControl1.SelectedTab == tabPage2)
                    {
                        radioButton8.Checked = true;
                        textBox2.Text = fFile;
                        button4.Enabled = true;
                    }
                    break;
            }*/
            //value = textBox.Text;
            return dialogResult;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            StringBuilder newdir = new StringBuilder(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).CodeBase));
            dir = (newdir.Remove(0, 6).ToString());
            string[] versionpart = this.ProductVersion.Split('.');
            version = versionpart[0] + "." + versionpart[1];
            this.Text += MainForm.version;
            // Add a link to the LinkLabel.
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "http://bmk.hamtek-solutions.com/";
            linkLabel2.Links.Add(link);
            //new ToolTip().SetToolTip(pictureBox1, "The desired tool-tip text.");
            //this.tip = new CustomToolTip();
            //comboBox1.SelectedIndex = 0;
            //comboBox2.SelectedIndex = 3;
            //comboBox3.SelectedIndex = 3;
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
            //tabPage1PSVitaCombo();
            at3bitRate = comboBox2.Text;
            button4.Enabled = true;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //tabPage1PSVitaCombo();
            at9bitRate = comboBox3.Text;
            button2.Enabled = true;

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
        }

        private void tabPage1PS4Combo()
        {
            comboBox3.Items.Clear();
            for (int i = 0; i < ps4List.Length; i++) { comboBox3.Items.Add(ps4List[i]); }
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
        }

        private void tabPage2PS3Combo()
        {
            comboBox2.Items.Clear();
            for (int i = 0; i < ps3List.Length; i++) { comboBox2.Items.Add(ps3List[i]); }
        }

        /*private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            //ToolTip tt = new ToolTip();
            //tt.SetToolTip(this.pictureBox1, "Your username");
            //toolTip1.SetToolTip(pictureBox1, "Your username");
            //this.tip.SetToolTip(this.pictureBox1, "BitRate Channel\n36[kbps] 1ch");
            /*CustomizedToolTip myToolTip1 = new CustomizedToolTip();
            CustomizedToolTip.TOOLTIP_WIDTH = 371;
            CustomizedToolTip.TOOLTIP_HEIGHT = 600;
            myToolTip1.AutoSize = false;*/
            //myToolTip1.SetToolTip(pictureBox1, "");

            //myToolTip1.SetToolTip(button2, @"Button 2. Formatted string (w/o image) Created using the verbatim character '@'. End");
            //myToolTip1.SetToolTip(pictureBox1, @" ");
            //myToolTip1.SetToolTip(button3, "Button 3. ToolTip with Image is being developed by Kumar, Ravikant India");

            //button1.Tag = Resources.Image;
            //button3.Tag = Resources.Image;
            //pictureBox1.Tag = Resources.psvita_bitrate;
            
        //}

        /*private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            //ToolTip tt = new ToolTip();
            //tt.SetToolTip(this.pictureBox1, "Your username");
            //toolTip1.SetToolTip(pictureBox1, "Your username");
            //this.tip.SetToolTip(this.pictureBox1, "BitRate Channel\n36[kbps] 1ch");
            CustomizedToolTip myToolTip2 = new CustomizedToolTip();
            CustomizedToolTip.TOOLTIP_WIDTH = 701;
            CustomizedToolTip.TOOLTIP_HEIGHT = 700;
            myToolTip2.AutoSize = false;
            //myToolTip1.SetToolTip(pictureBox1, "");

            //myToolTip1.SetToolTip(button2, @"Button 2. Formatted string (w/o image) Created using the verbatim character '@'. End");
            myToolTip2.SetToolTip(pictureBox2, @" ");
            //myToolTip1.SetToolTip(button3, "Button 3. ToolTip with Image is being developed by Kumar, Ravikant India");

            //button1.Tag = Resources.Image;
            //button3.Tag = Resources.Image;
            pictureBox2.Tag = Resources.psp_ps3_bitrate;

        }*/

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

        private void mPlayer(){

            if (comboBox1.Text == "PSP")
            {
                try
                {

                    //System.Diagnostics.Process process = new System.Diagnostics.Process();
                    //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    playerStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    playerStartInfo.FileName = @"player\MiniPlayer.exe ";
                    playerStartInfo.Arguments = FileNameFinal;
                    playerProcess.StartInfo = playerStartInfo;
                    playerProcess.Start();
                    //label1.SetBounds(32, 164, 234, 25);
                    //label1.Text = "AT9 in Progress...";                       
                    //label1.Refresh();
                    toolStripStatusLabel1.Text = "Playing...";
                    statusStrip1.Refresh();
                    button1.Enabled = true;
                    //playerProcess.WaitForExit();
                    //toolStripStatusLabel1.Text = "Stop!";
                    //statusStrip1.Refresh();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
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

    }
}

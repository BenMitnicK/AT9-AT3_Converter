using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace at3_at9_Converter
{
    public partial class FormChooser : Form
    {
        public FormChooser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm ConvertLoad = new MainForm();
            ConvertLoad.Show();
            FormChooser ChooserLoad = new FormChooser();
            ChooserLoad.Dispose();
        }
    }
}

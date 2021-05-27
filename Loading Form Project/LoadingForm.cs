using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Loading_Form_Project
{
    public partial class LoadingForm : Form
    {
        private MainForm MainForm = null;

        public LoadingForm(MainForm main)
        {
            InitializeComponent();
            MainForm = main;
            Label label = new Label()
            {
                Text = "Loading..."
            };
            Controls.Add(label);
        }
        delegate void CloseDelegate();
        private void LoadingForm_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.Sleep(10000);
                Label label = new Label();
                label.Text = "Hello, world!";
                MainForm.Controls.Add(label);
                Invoke(new CloseDelegate(Close));
            }).Start();

        }
    }
}

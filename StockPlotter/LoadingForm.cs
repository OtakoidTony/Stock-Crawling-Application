using System;
using System.Threading;
using System.Windows.Forms;

namespace StockPlotter
{


    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
            CenterToScreen();
        }

        public LoadingForm(MainForm main) : this()
        {
            CheckForIllegalCrossThreadCalls = false;
            new Thread(() =>
            {
                textBox1.Text += "Loading Form Initializated." + Environment.NewLine;
                if (NetworkConnectionChecker.IsConnected())
                {
                    textBox1.Text += "Network is available." + Environment.NewLine;
                    Thread.Sleep(1000);

                    main.StockCodes = StockCode.GetStockCodes();
                    textBox1.Text += "Loaded stock codes from krx." + Environment.NewLine;
                    main.InitCodeListView();

                    textBox1.Text += "Initiated Code List View." + Environment.NewLine;
                    Thread.Sleep(1000);
                    Close();
                }
                else
                {
                    textBox1.Text += "Can't load stock codes from krx." + Environment.NewLine;
                    textBox1.Text += "Network Error: Please Connect to internet.";
                    Thread.Sleep(3000);
                    Application.ExitThread();
                    Environment.Exit(0);
                }
            }).Start();
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {

        }
    }
}

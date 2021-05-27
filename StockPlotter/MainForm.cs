using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace StockPlotter
{
    public partial class MainForm : Form
    {
        public Dictionary<int, List<FinanceData>> loadedDatas = new Dictionary<int, List<FinanceData>>();

        public List<StockCode> StockCodes = null;
        public Dictionary<string, List<FinanceData>> FinanceDataset = null;

        public ColumnHeader CompanyColumnHeader = new ColumnHeader()
        {
            Text = "회사명"
        };

        public ColumnHeader CodeColumnHeader = new ColumnHeader()
        {
            Text = "종목코드"
        };


        public MainForm()
        {
            InitializeComponent();
            CenterToScreen();

            StockCodes = new List<StockCode>();
            FinanceDataset = new Dictionary<string, List<FinanceData>>();

            codeListView.Columns.AddRange(new ColumnHeader[]
            {
                CompanyColumnHeader,
                CodeColumnHeader
            });

            StockListView.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader(){Text = "날짜"},
                new ColumnHeader(){Text = "종가"},
                new ColumnHeader(){Text = "전일비"},
                new ColumnHeader(){Text = "시가"},
                new ColumnHeader(){Text = "고가"},
                new ColumnHeader(){Text = "저가"},
                new ColumnHeader(){Text = "거래량"}
            });
        }

        public void DisplayFinanceDataToListView(List<FinanceData> datas)
        {
            StockListView.Items.Clear();
            foreach (var data in datas)
                StockListView.Items.Add(new ListViewItem(new[]
                {
                    data.date.ToString("yyyy-MM-dd"),
                    data.close +"",
                    data.diff+"",
                    data.open+"",
                    data.high+"",
                    data.low+"",
                    data.volume+""
                }));
        }

        public void InitCodeListView()
        {
            codeListView.Items.Clear();
            foreach (var code in StockCodes)
                codeListView.Items.Add(new ListViewItem(new[] { code.Company, code.Code + "" }));
        }

        public static readonly Converter<FinanceData, ScottPlot.OHLC> FinanceDataToScottPlotOHLCConverter
            = new Converter<FinanceData, ScottPlot.OHLC>((data) =>
        {
            return new ScottPlot.OHLC(
                data.open,
                data.high,
                data.low,
                data.close,
                data.date,
                new TimeSpan(1, 0, 0, 0, 0));
        });

        [Obsolete]
        private void MainForm_Load(object sender, EventArgs e)
        {
            //ScottPlot.OHLC[] ohlcs = ScottPlot.DataGen.RandomStockPrices(rand: null, pointCount: 60, deltaMinutes: 10);

            //var dataset = FinanceData.GetFinanceDatas(251270, 5);
            //List<ScottPlot.OHLC> my_ohlcs = dataset.ConvertAll(FinanceDataToScottPlotOHLCConverter);

            //formsPlot1.Plot.Title("주식차트");
            formsPlot1.Plot.YLabel("Stock Price (Won)");

            var colorUp = System.Drawing.ColorTranslator.FromHtml("#FF0000");
            var colorDown = System.Drawing.ColorTranslator.FromHtml("#0000FF");
            //formsPlot1.Plot.PlotCandlestick(my_ohlcs.ToArray(), colorUp, colorDown, autoWidth: false);
        }

        private void codeListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        [Obsolete]
        private void codeListView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                whenSelect();
            }
            catch (Exception ex)
            {

            }

        }

        public int SelectedCode = -1;

        [Obsolete]
        private void whenSelect()
        {
            if (codeListView.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = codeListView.SelectedItems;
                ListViewItem lvItem = items[0];
                SelectedCode = int.Parse(lvItem.SubItems[1].Text);

                List<FinanceData> dataset;

                if (loadedDatas.ContainsKey(SelectedCode))
                {
                    dataset = loadedDatas[SelectedCode];
                }
                else
                {
                    dataset = FinanceData.GetFinanceDatas(SelectedCode, 5);
                    loadedDatas[SelectedCode] = dataset;
                }

                List<ScottPlot.OHLC> my_ohlcs = dataset.ConvertAll(FinanceDataToScottPlotOHLCConverter);

                formsPlot1.Plot.Title(lvItem.SubItems[0].Text);
                formsPlot1.Plot.YLabel("Stock Price (Won)");

                var colorUp = System.Drawing.ColorTranslator.FromHtml("#FF0000");
                var colorDown = System.Drawing.ColorTranslator.FromHtml("#0000FF");

                formsPlot1.Plot.Clear();
                formsPlot1.Plot.PlotCandlestick(my_ohlcs.ToArray(), colorUp, colorDown, autoWidth: false);
                formsPlot1.Plot.XAxis.DateTimeFormat(true);
                formsPlot1.Render();

                DisplayFinanceDataToListView(loadedDatas[SelectedCode]);
            }
        }

        [Obsolete]
        private void codeListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    whenSelect();
                }
                catch (Exception ex)
                { }
            }
        }

        private void csvSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedCode == -1)
            {
                MessageBox.Show("종목을 선택해주십시오.", "에러 메시지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv) | *.csv";
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            string path = saveFileDialog.FileName;

            StringBuilder stringBuilder = new StringBuilder();
            List<FinanceData> dataset = loadedDatas[SelectedCode];
            stringBuilder.AppendLine("날짜,종가,전일비,시가,고가,저가,거래량");
            foreach (var data in dataset)
            {
                stringBuilder.AppendLine(string.Join(",", new[]
                {
                    data.date.ToString("yyyy-MM-dd"),
                    data.close +"",
                    data.diff+"",
                    data.open+"",
                    data.high+"",
                    data.low+"",
                    data.volume+""
                }));
            }
            System.IO.File.WriteAllText(path, stringBuilder.ToString(), Encoding.Default);
        }
    }
}

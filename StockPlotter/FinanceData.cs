using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockPlotter
{
    public class FinanceData
    {
        public DateTime date;
        public int close;
        public int diff;
        public int open;
        public int high;
        public int low;
        public int volume;

        public FinanceData(DateTime date, int close, int diff, int open, int high, int low, int volume)
        {
            this.date = date;
            this.close = close;
            this.diff = diff;
            this.open = open;
            this.high = high;
            this.low = low;
            this.volume = volume;
        }

        public FinanceData(String[] arr)
        {
            String[] temp = arr[0].Split('.');
            String year = temp[0];
            String month = temp[1];
            String day = temp[2];
            System.Diagnostics.Debug.WriteLine("arr[0]="+arr[0]);
            this.date = DateTime.Parse(string.Join("/", new string[] { month, day, year }));
            this.close = Int32.Parse(arr[1].Replace(",", ""));
            this.diff = Int32.Parse(arr[2].Replace(",", ""));
            this.open = Int32.Parse(arr[3].Replace(",", ""));
            this.high = Int32.Parse(arr[4].Replace(",", ""));
            this.low = Int32.Parse(arr[5].Replace(",", ""));
            this.volume = Int32.Parse(arr[6].Replace(",", ""));
        }

        public static List<FinanceData> GetFinanceDatas(int code, int pages)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<FinanceData> datas = new List<FinanceData>();
            
            for (int page = 1; page < pages + 1; page++)
            {
                var client = new RestClient("https://finance.naver.com/item/sise_day.nhn?code=" + code + "&page=" + page)
                {
                    Timeout = -1
                };
                var request = new RestRequest(Method.GET);
                request.AddHeader("DNT", "1");
                request.AddHeader("Upgrade-Insecure-Requests", "1");
                client.UserAgent = "Mozilla/5.0";
                request.AddHeader("Sec-Fetch-Site", "none");
                request.AddHeader("Sec-Fetch-Mode", "navigate");
                request.AddHeader("Sec-Fetch-User", "?1");
                request.AddHeader("Sec-Fetch-Dest", "document");

                var response = client.Execute(request);
                var resultHTML = Encoding.GetEncoding("euc-kr").GetString(response.RawBytes);
                const string selector = "tr[onmouseover=\"mouseOver(this)\"]";

                var resultDoc = new HtmlAgilityPack.HtmlDocument();
                resultDoc.LoadHtml(resultHTML);
                var result = resultDoc.QuerySelectorAll(selector);
                foreach (var element in result)
                {
                    var temp1 = new List<string>(element.InnerText.Split('\n'));
                    for (int i = 0; i < temp1.Count; i++)
                        temp1[i] = temp1[i].Trim();
                    temp1.RemoveAll((str) => str.Equals(""));

                    var temp = temp1.ToArray();
                    for (int i = 0; i < temp.Length; i++)
                        temp[i] = temp[i].Trim();
                    System.Diagnostics.Debug.WriteLine(string.Join("|", temp));
                    datas.Add(new FinanceData(temp));
                }
            }
            datas.Reverse();
            return datas;
        }
    }
}

using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockPlotter
{
    public class StockCode
    {
        /// <summary>
        /// 회사명
        /// </summary>
        public string Company;

        /// <summary>
        /// 종목코드
        /// </summary>
        public int Code;

        /// <summary>
        /// 업종
        /// </summary>
        public string Industry;

        /// <summary>
        /// 주요제품
        /// </summary>
        public string MajorProducts;

        /// <summary>
        /// 상장일
        /// </summary>
        public string ListingDate;

        /// <summary>
        /// 결산월
        /// </summary>
        public string SettlementMonth;

        /// <summary>
        /// 대표자명
        /// </summary>
        public string CEO;

        /// <summary>
        /// 홈페이지
        /// </summary>
        public string Homepage;

        /// <summary>
        /// 지역
        /// </summary>
        public string Area;

        private StockCode(String[] args)
        {
            Company = args[0];
            Code = Int32.Parse(args[1]);
            Industry = args[2];
            MajorProducts = args[3];
            ListingDate = args[4];
            SettlementMonth = args[5];
            CEO = args[6];
            Homepage = args[7];
            Area = args[8];
        }

        /// <summary>
        /// 상장법인목록을 krx에서 크롤링해 StockCode List로 반환하는 함수
        /// </summary>
        /// <returns>
        /// 상장법인목록
        /// </returns>
        public static List<StockCode> GetStockCodes()
        {
            var client = new RestClient("http://kind.krx.co.kr/corpgeneral/corpList.do?method=download&searchType=13");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var resultHTML = Encoding.GetEncoding("euc-kr").GetString(response.RawBytes);
            var resultDoc = new HtmlAgilityPack.HtmlDocument();
            resultDoc.LoadHtml(resultHTML);
            var resultTr = resultDoc.QuerySelectorAll("tr");
            var keysDoc = new HtmlAgilityPack.HtmlDocument();
            keysDoc.LoadHtml(resultTr[0].InnerHtml);
            List<StockCode> arr = new List<StockCode>();
            for (var j = 1; j < resultTr.Count; j++)
            {
                var tempDoc = new HtmlAgilityPack.HtmlDocument();
                tempDoc.LoadHtml(resultTr[j].InnerHtml);
                var temp = tempDoc.QuerySelectorAll("td");
                String[] cargs = new String[9];
                for (var i = 0; i < temp.Count; i++)
                    cargs[i] = temp[i].InnerHtml.Trim();
                arr.Add(new StockCode(cargs));
            }
            return arr;
        }
    }
}

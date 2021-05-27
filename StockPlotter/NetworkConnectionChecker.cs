using System.Runtime.InteropServices;

namespace StockPlotter
{
    static class NetworkConnectionChecker
    {
        #region 인터넷 연결 상태 구하기 - InternetGetConnectedState(description, reservedValue)
        /// <summary>
        /// 인터넷 연결 상태 구하기
        /// </summary>
        /// <param name="description">설명/param>
        /// <param name="reservedValue">예약 값</param>
        /// <returns>인터넷 연결 상태</returns>
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int description, int reservedValue);
        #endregion
        #region 연결 여부 구하기 - IsConnected()
        /// <summary>
        /// 연결 여부 구하기
        /// </summary>
        /// <returns>연결 여부</returns>
        public static bool IsConnected()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
        #endregion
    }
}

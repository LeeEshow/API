using System;
using System.Data.SqlClient;
using System.Reflection;

namespace Advantech.Database
{
    /// <summary>
    /// 靜態類別，SQL Server操作
    /// </summary>
    public static class MSSQL
    {
        #region 屬性

        // 安全性原則，連線資訊於該類別中不公開引用且無預設連線。
        #region 隱藏連線資訊

        private static string _IP { get; set; }

        private static string _ID { get; set; }

        private static string _PW { get; set; }

        private static string _Schema { get; set; }
        #endregion 隱藏連線資訊

        #endregion 屬性


        #region 行為
        /// <summary>
        /// 設定連線
        /// </summary>
        /// <param name="IP">主機位址</param>
        /// <param name="ID">使用者ID</param>
        /// <param name="Password">密碼</param>
        /// <param name="Schema">資料庫名稱</param>
        public static void Setting(string IP, string ID, string Password, string Schema)
        {
            _IP = IP;
            _ID = ID;
            _PW = Password;
            _Schema = Schema;
        }

        /// <summary>
        /// 建立 SQL 唯讀連線
        /// </summary>
        /// <returns></returns>
        public static SqlConnection ReadyOnly()
        {
            try
            {
                return new SqlConnection($@"Data Source={_IP};
                                            User ID={_ID};
                                            Password={_PW};
                                            Initial Catalog={_Schema};
                                            ApplicationIntent=ReadOnly");
            }
            catch (Exception ex)
            {
                Abject.APIExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }
        /// <summary>
        /// 建立 SQL 操作連線(讀/寫)，未實做!!
        /// </summary>
        /// <returns></returns>
        public static SqlConnection Connect()
        {
            try
            {
                return new SqlConnection($@"");
            }
            catch (Exception ex)
            {
                Abject.APIExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }

        /// <summary>
        /// 是否已經連線
        /// </summary>
        public static bool IsConnected()
        {
            try
            {
                using (var con = ReadyOnly())
                {
                    con.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Abject.APIExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }
        #endregion 行為

    }
}

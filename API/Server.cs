using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace API
{
    /// <summary>
    /// 伺服器。
    /// </summary>
    public static class Server
    {
        #region Server 方法例外事件
        /// <summary>
        /// 委派事件
        /// </summary>
        /// <param name="MethodBase">例外內容</param>
        /// <param name="ex">例外內容</param>
        public delegate void ServerMethodException(MethodBase MethodBase, Exception ex);
        /// <summary>
        /// Server 例外事件。
        /// </summary>
        public static event ServerMethodException ServerException;
        /// <summary>
        /// 規範所有 Server 行為/方法發生例外狀況必須觸發該事件。
        /// </summary>
        /// <param name="MethodBase"></param>
        /// <param name="ex"></param>
        private static void ServerExcepted(MethodBase MethodBase, Exception ex)
        {
            ServerException?.Invoke(MethodBase, ex);
        }
        #endregion Server 方法例外事件

        /// <summary>
        /// SQL Server 連線操作
        /// </summary>
        public static class MSSQL
        {
            // 安全性原則，連線資訊於該類別中不公開引用且無預設連線。
            #region 屬性

            private static string _IP { get; set; }

            private static string _ID { get; set; }

            private static string _Password { get; set; }

            private static string _Schema { get; set; }

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
                _Password = Password;
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
                                            Password={_Password};
                                            Initial Catalog={_Schema};
                                            ApplicationIntent=ReadOnly");
                }
                catch (Exception ex)
                {
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
                    return null;
                }
            }
            /// <summary>
            /// 建立 SQL 操作連線(讀/寫)，未實做!!
            /// </summary>
            /// <returns></returns>
            public static SqlConnection Writing()
            {
                try
                {
                    return new SqlConnection($@"Data Source={_IP};
                                            User ID={_ID};
                                            Password={_Password};
                                            Initial Catalog={_Schema};");
                }
                catch (Exception ex)
                {
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
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
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
                    return false;
                }
            }
            #endregion 行為
        }

        /// <summary>
        /// mySQL 連線操作
        /// </summary>
        public static class mySQL
        {
            // 安全性原則，連線資訊於該類別中不公開引用且無預設連線。
            #region 屬性
            private static string _IP { get; set; }

            private static string _ID { get; set; }

            private static string _Password { get; set; }

            private static string _Schema { get; set; }
            #endregion 屬性


            #region 行為
            /// <summary>
            /// 設定 mySQL 連線位址、使用者帳戶、使用者密碼。
            /// </summary>
            /// <param name="IP">連線位址</param>
            /// <param name="ID">使用者帳戶</param>
            /// <param name="Password">使用者密碼</param>
            /// <param name="Schema">預設資料庫</param>
            public static void Setting(string IP, string ID, string Password, string Schema)
            {
                _IP = IP;
                _ID = ID;
                _Password = Password;
                _Schema = Schema;
            }

            /// <summary>
            /// 建立連線，Return MySqlConnection Class。
            /// </summary>
            /// <returns></returns>
            public static MySqlConnection Connecting()
            {
                try
                {
                    return new MySqlConnection($@"server={_IP};
                                             user id={_ID};
                                             password={_Password};
                                             database={_Schema};
                                             pooling=false;
                                             CharSet=utf8;
                                             Convert Zero Datetime=True; 
                                             SslMode=none;
                                             Allow User Variables=True;
                                             Connect Timeout=180;");
                }
                catch (Exception ex)
                {
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
                    return null;
                }
            }

            /// <summary>
            /// 確認連線。可連線 Return true ；不可連線 Return false。
            /// </summary>
            /// <returns></returns>
            public static bool IsConnected()
            {
                try
                {
                    using (var con = Connecting())
                    {
                        con.Open();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
                    return false;
                }
            }

            /// <summary>
            /// 判斷欄位是否存在
            /// </summary>
            /// <param name="dataReader"></param>
            /// <param name="columnName"></param>
            /// <returns></returns>
            public static bool IsColumnExists(MySqlDataReader dataReader, string columnName)
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    if (dataReader.GetName(i).Equals(columnName))
                    {
                        return true;
                    };
                }
                return false;
            }
            #endregion 行為
        }

        /// <summary>
        /// Windows 遠端連線
        /// </summary>
        public static class WindowsUser
        {
            #region 屬性
            private static string _IP { get; set; }

            private static string _ID { get; set; }

            private static string _Password { get; set; }

            #region 引用參考屬性
            [DllImport("advapi32.dll", SetLastError = true)]
            private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool CloseHandle(IntPtr handle);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private extern static bool DuplicateToken(IntPtr existingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, ref IntPtr duplicateTokenHandle);

            // logon types
            const int LOGON32_LOGON_INTERACTIVE = 2;
            const int LOGON32_LOGON_NETWORK = 3;
            const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

            // logon providers
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            const int LOGON32_PROVIDER_WINNT40 = 2;
            const int LOGON32_PROVIDER_WINNT35 = 1;
            #endregion 引用參考屬性

            #endregion 屬性


            #region 行為
            /// <summary>
            /// 設定連線
            /// </summary>
            /// <param name="IP">主機位址</param>
            /// <param name="ID">使用者ID</param>
            /// <param name="Password">密碼</param>
            public static void Setting(string IP, string ID, string Password)
            {
                _IP = IP;
                _ID = ID;
                _Password = Password;
            }

            /// <summary>
            /// 建立 Windows 模擬使用者遠端連線，Return WindowsImpersonationContext Class。
            /// </summary>
            /// <returns></returns>
            public static WindowsImpersonationContext Connecting()
            {
                try
                {
                    #region LogonUser
                    IntPtr token = IntPtr.Zero;
                    bool isSuccess = LogonUser(_ID, _IP, _Password,
                                        LOGON32_LOGON_NEW_CREDENTIALS,
                                        LOGON32_PROVIDER_DEFAULT,
                                        ref token);
                    if (!isSuccess)
                    {
                        throw new ApplicationException("Failed to LogonUser, Code = " + Marshal.GetLastWin32Error());
                    }
                    #endregion LogonUser

                    #region DuplicateToken
                    IntPtr dupToken = IntPtr.Zero;
                    isSuccess = DuplicateToken(token, 2, ref dupToken);
                    if (!isSuccess)
                    {
                        throw new ApplicationException("Failed to DuplicateToken, Code = " + Marshal.GetLastWin32Error());
                    }
                    #endregion DuplicateToken

                    return new WindowsIdentity(dupToken).Impersonate();
                }
                catch (Exception ex)
                {
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
                    return null;
                }
            }

            #region 使用範例
            private static readonly string Def_Path = @"192.168.10.218";
            /// <summary>
            /// 將檔案由記憶體中存入 218 路徑
            /// </summary>
            /// <param name="Save_Path"></param>
            /// <param name="Stream"></param>
            /// <param name="FileMode"></param>
            /// <returns></returns>
            public static bool Upload(Stream Stream, string Save_Path, FileMode FileMode)
            {
                try
                {
                    // Save_Path 必須包含 IP，例如：192.168.10.218/FileServer
                    if ((FileMode == FileMode.Create || FileMode == FileMode.CreateNew) & Save_Path.Contains(Def_Path))
                    {
                        using (var wic = Connecting())
                        {
                            using (var fileStream = new FileStream(Save_Path, FileMode, FileAccess.Write))
                            {
                                Stream.CopyTo(fileStream);
                            }

                            if (File.Exists(Save_Path))
                            {
                                wic.Undo();
                                return true;
                            }
                            wic.Undo();
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
                    return false;
                }
            }

            /// <summary>
            /// 取得 218 路徑的指定路徑檔案
            /// </summary>
            /// <param name="File_Path"></param>
            /// <returns></returns>
            public static Stream Download(string File_Path)
            {
                try
                {
                    if (File_Path.Contains(Def_Path))
                    {
                        using (var wic = Connecting())
                        {
                            var data = File.OpenRead(File_Path);
                            wic.Undo();
                            return data;
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    ServerExcepted(MethodBase.GetCurrentMethod(), ex);
                    return null;
                }
            }
            #endregion 使用範例

            #endregion 行為
        }

    }

}


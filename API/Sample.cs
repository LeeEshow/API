using API.Struct;
using System;
using System.Data.SqlClient;
using System.Reflection;

namespace API.Model
{
    /// <summary>
    /// 繼承範例
    /// </summary>
    public class Sample : Model<Sample>
    {
        #region 屬性
        /// <summary>
        /// 產品條碼
        /// </summary>
        public override string ID { get; set; }
        /// <summary>
        /// 產品名稱
        /// </summary>
        public override string Name { get; set; }
        /// <summary>
        /// 類別
        /// </summary>
        public new API.Struct.Type Type { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public string Data { get; set; }
        #endregion 屬性


        #region 行為

        #region 編輯
        /// <summary>
        /// 註冊資料
        /// </summary>
        /// <returns></returns>
        public bool Registered()
        {
            using (var con = API.Server.MSSQL.Connecting())
            {
                con.Open();
                string str = $@"Insert into ....";

                var cmd = new SqlCommand(str, con);
                var Qty = cmd.ExecuteNonQuery();
                if (Qty > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            using (var con = API.Server.MSSQL.Connecting())
            {
                con.Open();
                string str = $@"Update ....";

                var cmd = new SqlCommand(str, con);
                var Qty = cmd.ExecuteNonQuery();
                if (Qty > 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion 編輯

        #region Find
        /// <summary>
        /// 取得資訊
        /// </summary>
        /// <returns></returns>
        public override Sample Find()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                using (var con = API.Server.MSSQL.ReadyOnly())
                {
                    con.Open();
                    string str = $@"SELECT * FROM ...";

                    var cmd = new SqlCommand(str, con);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        return Reading(reader);
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 取得資訊
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Sample Find(string ID)
        {
            this.ID = ID;
            return Find();
        }

        internal Sample Reading(SqlDataReader reader)
        {
            this.ID = reader["BARCODE"].ToString();
            this.Name = "XXX 產品";
            this.Data = reader["Data"].ToString();
            return this;
        }
        #endregion Find

        #endregion 行為
    }
}

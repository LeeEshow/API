using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace API
{
    /// <summary>
    /// 繼承範例
    /// </summary>
    public class Sample : Base
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
        public new Base Type;
        /// <summary>
        /// ???
        /// </summary>
        public string Extra_Barcode;
        /// <summary>
        /// ???
        /// </summary>
        public string Rule_Station;
        /// <summary>
        /// 工單號碼
        /// </summary>
        public string WIP_ID;

        /// <summary>
        /// 工單建立時間
        /// </summary>
        public DateTime Create_Date;
        /// <summary>
        /// 工單更新時間
        /// </summary>
        public DateTime Update_Date;
        #endregion 屬性


        #region 行為
        /// <summary>
        /// 取得資訊
        /// </summary>
        /// <returns></returns>
        public Sample Find()
        {
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    using (var con = API.Server.MSSQL.ReadyOnly())
                    {
                        con.Open();
                        string str = $@"SELECT * FROM TABLE
                                        where BARCODE_NO = '{this.ID}'";

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
            catch (Exception ex)
            {
                Base.APIExcepted(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
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
            this.ID = reader["BARCODE_NO"].ToString();
            this.Name = "XXX 產品";
            this.Extra_Barcode = reader["EXTRA_BARCODE_NO"].ToString();
            this.Rule_Station = reader["RULE_STATION_ID"].ToString();
            this.WIP_ID = reader["WIP_ID"].ToString();
            this.Create_Date = DateTime.Parse(reader["CREATE_DATE"].ToString());
            this.Update_Date = DateTime.Parse(reader["UPDATE_DATE"].ToString());

            return this;
        }

        /// <summary>
        /// 存入資料
        /// </summary>
        /// <returns></returns>
        public bool Insert()
        {
            try
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
            catch (Exception ex)
            {
                Base.APIExcepted(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }

        #endregion 行為
    }

}

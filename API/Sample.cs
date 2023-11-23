using Advantech.Structure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Advantech;
using System.Reflection;

namespace Advantech.Sample
{
    /// <summary>
    /// 工單
    /// </summary>
    public class WIP : Abject
    {
        #region 屬性
        /// <summary>
        /// 工單號碼
        /// </summary>
        public override string ID { get; set; } // 請注意繼承後 new/override 的差異。
        /// <summary>
        /// 工單名稱
        /// </summary>
        public override string Name { get; set; } // 請注意繼承後 new/override 的差異。
        /// <summary>
        /// 類別
        /// </summary>
        public new Abject Type; // 實作介面

        /// <summary>
        /// 生產計劃
        /// </summary>
        public Schedule Plan;

        #region 其它
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description;
        /// <summary>
        /// 工單建立時間
        /// </summary>
        public DateTime Create_Date;
        /// <summary>
        /// 工單更新時間
        /// </summary>
        public DateTime Update_Date;
        #endregion 其它

        #endregion 屬性


        #region 行為

        #region 增刪查改
        /// <summary>
        /// 取得相關物件資訊
        /// </summary>
        /// <returns></returns>
        public WIP Find()
        {
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    using (var con = Database.MSSQL.ReadyOnly())
                    {
                        con.Open();
                        string str = $@"Select TOP 1 t.WIP_ID, t.WIP_NO, t2.ITEM_NO, t.PLAN_QTY, t.WERKS, 
                                        (Select LINE_DESC FROM M9_MESDB.MES.LINE_INFO where LINE_ID = t.LINE_ID) as Line_No, 
                                        t.WIP_SCHEDULE_DATE, t.WIP_DUE_DATE, t.REMARKS, t.DESCRIPTION, t.CREATE_DATE, t.UPDATE_DATE From
                                        (SELECT TOP 1 * FROM M9_MESDB.MES.WIP_INFO Where WIP_NO = '{ID}' order by UPDATE_DATE desc) as t
                                        Inner Join M9_MESDB.MES.WIP_ATT as t2 on (t.WIP_NO = t2.WIP_NO) order by t.CREATE_DATE desc";

                        var cmd = new SqlCommand(str, con);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            this.ID = reader["WIP_NO"].ToString();
                            this.Name = "XXX工單";
                            this.Plan = new Schedule
                            {
                                ITEM_No = reader["ITEM_NO"].ToString(),
                                Plan_Qty = Convert.ToUInt32(reader["PLAN_QTY"].ToString()),
                                Factroy_No = reader["WERKS"].ToString(),
                                Line_No = reader["Line_No"].ToString(),
                                Schedule_Date = DateTime.TryParse(reader["WIP_SCHEDULE_DATE"].ToString(), out DateTime SD) ?
                                    SD : DateTime.MinValue,
                                Finish_Date = DateTime.TryParse(reader["WIP_DUE_DATE"].ToString(), out DateTime FD) ?
                                    FD : DateTime.MinValue,
                            };
                            this.Remark = reader["REMARKS"].ToString();
                            this.Description = reader["DESCRIPTION"].ToString();
                            this.Create_Date = DateTime.Parse(reader["CREATE_DATE"].ToString());
                            this.Update_Date = DateTime.Parse(reader["UPDATE_DATE"].ToString());
                            return this;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Abject.APIExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }
        /// <summary>
        /// 取得相關物件資訊
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public WIP Find(string ID)
        {
            this.ID = ID;
            return Find();
        }

        /// <summary>
        /// 更新物件資訊
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            try
            {
                using (var con = Database.MSSQL.Connect())
                {
                    con.Open();
                    string str = $@"";

                    var cmd = new SqlCommand(str, con);
                    var effect = cmd.ExecuteNonQuery();
                    if (effect > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Abject.APIExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }
        #endregion 增刪查改

        #region Get
        /// <summary>
        /// 取得工單 產品清單。
        /// </summary>
        /// <returns></returns>
        public List<Product> GetProducts()
        {
            try
            {
                List<Product> list = new List<Product>();
                if (!string.IsNullOrEmpty(ID))
                {
                    using (var con = Database.MSSQL.ReadyOnly())
                    {
                        con.Open();
                        string str = $@"Select t2.* From
                                        (SELECT * FROM [M9_MESDB].[MES].[WIP_INFO] where WIP_NO = '{this.ID}') as t
                                        Inner Join [M9_MESDB].[MES].[BARCODE_INFO] as t2 on (t.WIP_ID = t2.WIP_ID)";

                        var cmd = new SqlCommand(str, con);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var data = new Product().Reading(reader);
                            if (data != null)
                            {
                                list.Add(data);
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Abject.APIExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }
        /// <summary>
        /// 取得工單 產品清單。
        /// </summary>
        /// <param name="WIP_ID"></param>
        /// <returns></returns>
        public List<Product> GetProducts(string WIP_ID)
        {
            this.ID = WIP_ID;
            return GetProducts();
        }
        #endregion Get

        #endregion 行為
    }




    /// <summary>
    /// 成品/產品
    /// </summary>
    public class Product : Abject
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
        public new Abject Type;

        /// <summary>
        /// 狀態
        /// </summary>
        public Product_Status Status;
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
        /// 取得產品資訊
        /// </summary>
        /// <returns></returns>
        public Product Find()
        {
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    using (var con = Database.MSSQL.ReadyOnly())
                    {
                        con.Open();
                        string str = $@"SELECT * FROM [M9_MESDB].[MES].[BARCODE_INFO] 
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
                Abject.APIExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }
        /// <summary>
        /// 取得產品資訊
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Product Find(string ID)
        {
            this.ID = ID;
            return Find();
        }

        internal Product Reading(SqlDataReader reader)
        {
            this.ID = reader["BARCODE_NO"].ToString();
            this.Name = "XXX 產品";
            this.Status = Enum.TryParse(reader["STATUS_NO"].ToString(), out Product_Status result) ? 
                result : Product_Status.Default;
            this.Extra_Barcode = reader["EXTRA_BARCODE_NO"].ToString();
            this.Rule_Station = reader["RULE_STATION_ID"].ToString();
            this.WIP_ID = reader["WIP_ID"].ToString();
            this.Create_Date = DateTime.Parse(reader["CREATE_DATE"].ToString());
            this.Update_Date = DateTime.Parse(reader["UPDATE_DATE"].ToString());

            return this;
        }
        #endregion 行為
    }

}

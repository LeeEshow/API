using System;

// Advantech.Structure 作為共用命名空間
// 1. 負責管理公用類別 Class、結構 Structure、列舉 Eunm ...等共用元素
// 2. 開發規範定義在該命名空間內的類別 Class 只包含能包含屬性/欄位；不得包含行為/方法
namespace Advantech.Structure
{

    #region MoNo / WIP 
    /// <summary>
    /// 工單預期計畫
    /// </summary>
    public struct Schedule
    {
        /// <summary>
        /// 生產料號
        /// </summary>
        public string ITEM_No;
        /// <summary>
        /// 計畫生產數量
        /// </summary>
        public uint Plan_Qty;
        /// <summary>
        /// 廠別
        /// </summary>
        public string Factroy_No;
        /// <summary>
        /// 線別
        /// </summary>
        public string Line_No;
        /// <summary>
        /// 預期生產時間
        /// </summary>
        public DateTime Schedule_Date;
        /// <summary>
        /// 預期完成時間
        /// </summary>
        public DateTime Finish_Date;
    }
    #endregion MoNo / WIP 

    #region Product
    /// <summary>
    /// 產品/成品狀態，列舉
    /// </summary>
    public enum Product_Status
    {
        /// <summary>
        /// 預設
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        ASY = 1,
        /// <summary>
        /// 
        /// </summary>
        BAB = 2,
        /// <summary>
        /// 
        /// </summary>
        CPN = 3, 
        /// <summary>
        /// 
        /// </summary>
        DIP = 4,
        /// <summary>
        /// 
        /// </summary>
        FT = 5,
        /// <summary>
        /// 
        /// </summary>
        PKG = 6,
        /// <summary>
        /// 
        /// </summary>
        REW = 7,
        /// <summary>
        /// 
        /// </summary>
        RMA = 8,
        /// <summary>
        /// 
        /// </summary>
        SMT = 9,
        /// <summary>
        /// 
        /// </summary>
        TET = 10,
    }


    #endregion Product




}

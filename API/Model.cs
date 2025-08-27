using Newtonsoft.Json;
using System;
using System.Reflection;
using API.Struct;

namespace API
{
    /// <summary>
    /// 基礎父項類別 (泛型版)
    /// 提供所有物件的基本結構與共用方法，並透過泛型確保子類別回傳正確型別。
    /// </summary>
    /// <typeparam name="T">子類別型別，必須繼承 <see cref="Model{T}"/>，且具備無參數建構子</typeparam>
    public abstract class Model<T> : BaseModel, IBaseMethod<T> where T : Model<T>, new()
    {
        #region 屬性
        /// <summary>
        /// 物件識別碼
        /// </summary>
        public virtual string ID { get; set; }
        /// <summary>
        /// 物件名稱
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 物件類別，抽象屬性繼承時必須實作。
        /// </summary>
        public abstract object Type { get; set; }

        /// <summary>
        /// 類型完整名稱，唯獨。
        /// </summary>
        public sealed override string Class => this.GetType().FullName;
        #endregion 屬性


        #region 行為
        /// <summary>
        /// 查詢或建立物件的方法，由子類別實作。
        /// </summary>
        /// <returns>回傳子類別物件</returns>
        public abstract T Find();

        /// <summary>
        /// 檢查目前物件的公開屬性是否包含 null 或空字串。
        /// </summary>
        /// <returns></returns>
        public bool IsNullOrEmpty()
        {
            if (this == null)
            {
                return true;
            }
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    string value = (string)prop.GetValue(this);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
                else
                {
                    if (prop.GetValue(this) == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion 行為
    }
}

namespace API.Struct
{
    /// <summary>
    /// 基礎模型，提供類別完整名稱的唯讀屬性。
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 類型完整名稱
        /// </summary>
        public virtual string Class => GetType().FullName;

        #region API 方法例外事件       
        /// <summary>
        /// API 例外事件。當方法發生例外時會被觸發。
        /// </summary>
        public static event APIMethodException APIException;
        /// <summary>
        /// 觸發 API 例外事件，統一回報例外狀況。
        /// </summary>
        /// <param name="MethodBase">發生例外的方法資訊</param>
        /// <param name="ex">例外內容</param>
        protected internal static void APIExcepted(MethodBase MethodBase, Exception ex)
        {
            APIException?.Invoke(MethodBase, ex);
        }
        #endregion API 方法例外事件
    }

    /// <summary>
    /// 定義基本方法介面。
    /// </summary>
    /// <typeparam name="T">回傳的型別</typeparam>
    public interface IBaseMethod<T>
    {
        /// <summary>
        /// 查詢或建立物件的方法。
        /// </summary>
        /// <returns>指定型別的物件</returns>
        T Find();
    }


    /// <summary>
    /// 委派事件：用於 API 方法發生例外時的通知。
    /// </summary>
    /// <param name="MethodBase">例外發生的方法</param>
    /// <param name="ex">例外內容</param>
    public delegate void APIMethodException(MethodBase MethodBase, Exception ex);
}
using Newtonsoft.Json;
using System;
using System.Reflection;
using API.Struct;

namespace API
{
    /// <summary>
    /// 基礎父項類別
    /// </summary>
    public abstract class Model : BaseModel
    {
        // Base 作為所有物件的最原始父層結構，僅提供最基礎屬性 ID + Name + Class Name 與介面屬性 Type。
        // Node：請注意屬性上 virtual/new/override 應用與差異。
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


        // 行為中依照使用經驗將各繼承子物件常用的方法收入該父層結構。
        #region 行為
        /// <summary>
        /// 轉換物件
        /// </summary>
        /// <typeparam name="T">指定轉換物件</typeparam>
        /// <returns>指定轉換物件</returns>
        public T ConvertTo<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
            }
            catch (Exception ex)
            {
                APIExcepted(MethodBase.GetCurrentMethod(), ex);
                return default;
            }
        }

        /// <summary>
        /// 判斷公開的【屬性】內是否有 Value 為 null or empty。
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


        // 該事件處理所有物件 API 內方法例外統一回報窗口。因此規範所有必要方法處理時必須使用 try/catch，範例：
        #region API 方法例外事件       
        /// <summary>
        /// API 例外事件。
        /// </summary>
        public static event APIMethodException APIException;
        /// <summary>
        /// 規範所有 API 行為/方法發生例外狀況必須觸發該事件。
        /// </summary>
        /// <param name="MethodBase"></param>
        /// <param name="ex"></param>
        protected internal static void APIExcepted(MethodBase MethodBase, Exception ex)
        {
            APIException?.Invoke(MethodBase, ex);
        }
        #endregion API 方法例外事件
    }
}

namespace API.Struct
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string Class { get; set; }


    }

    /// <summary>
    /// 委派事件
    /// </summary>
    /// <param name="MethodBase">例外內容</param>
    /// <param name="ex">例外內容</param>
    public delegate void APIMethodException(MethodBase MethodBase, Exception ex);
}
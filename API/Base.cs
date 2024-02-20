using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Sample
{
    /// <summary>
    /// 基礎父項類別
    /// </summary>
    public class Base
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
        /// 物件分類
        /// </summary>
        public interface Type { }

        /// <summary>
        /// 完整類別名稱，唯獨。
        /// </summary>
        public string Class_Name
        {
            get
            {
                return this.GetType().FullName;
            }
        }
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
                ExceptedEvent(MethodBase.GetCurrentMethod(), ex);
                return default;
            }
        }

        /// <summary>
        /// 判斷公開的屬性內是否有 Value 為 null or empty。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsNullOrEmpty(object obj)
        {
            var p = obj.GetType().GetProperties();
            foreach (var pi in obj.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(obj);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
                else
                {
                    if (pi.GetValue(obj) == null)
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
        /// 委派事件
        /// </summary>
        /// <param name="MethodBase">例外內容</param>
        /// <param name="ex">例外內容</param>
        public delegate void APIMethodException(MethodBase MethodBase, Exception ex);
        /// <summary>
        /// API 例外事件。
        /// </summary>
        public static event APIMethodException APIException;
        /// <summary>
        /// 規範所有 API 行為/方法發生例外狀況必須觸發該事件。
        /// </summary>
        /// <param name="MethodBase"></param>
        /// <param name="ex"></param>
        protected internal static void ExceptedEvent(MethodBase MethodBase, Exception ex)
        {
            APIException?.Invoke(MethodBase, ex);
        }
        #endregion API 方法例外事件
    }




    #region 靜態擴充功能
    /// <summary>
    /// 靜態擴充功能
    /// </summary>
    public static class ExtensiveMethod
    {
        /// <summary>
        /// 深度複製物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this T obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return default(T);
            }
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), deserializeSettings);
        }

        /// <summary>
        /// 物件序列化成字串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
    #endregion 靜態擴充功能

}
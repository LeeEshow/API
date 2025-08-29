using API.Struct;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static API.AttributeBox;

namespace API
{
    #region 靜態擴充
    /// <summary>
    /// 靜態擴充
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 靜態擴充，物件序列化 >> JSON 字串
        /// </summary>
        /// <param name="obj">序列化物件</param>
        /// <param name="IgnoreNulls">是否隱藏 null、未填欄位</param>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool IgnoreNulls = true)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = { new StringEnumConverter() } // 列舉轉字串
            };

            if (IgnoreNulls)
            {
                settings.ContractResolver = new IgnoreNullsContractResolver();
                settings.NullValueHandling = NullValueHandling.Ignore;
            }

            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 靜態擴充，反序列字串 >> 物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string str)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter() }
            };

            return JsonConvert.DeserializeObject<T>(str, settings);
        }

        /// <summary>
        /// 靜態擴充，深度拷貝 + 物件轉換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ToObject<T>(this object obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 靜態擴充，判斷屬性是否為 null or 空值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="IncludeFields">判斷是否包含欄位</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsNullOrEmpty(this object obj, bool IncludeFields = false)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var result = new List<string>();
            var type = obj.GetType();

            // 屬性：Public 或 Internal
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(p => p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsAssembly));

            foreach (var prop in props)
            {
                var value = prop.GetValue(obj);
                if (IsNullOrDefault(value, prop.PropertyType))
                {
                    return true;
                }
            }

            if (IncludeFields)
            {
                // 欄位：Public 或 Internal
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .Where(f => f.IsPublic || f.IsAssembly);

                foreach (var field in fields)
                {
                    var value = field.GetValue(obj);
                    if (IsNullOrDefault(value, field.FieldType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private static bool IsNullOrDefault(object value, System.Type type)
        {
            if (value == null) return true;

            if (type == typeof(string))
                return string.IsNullOrWhiteSpace(value as string);

            var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
            return value.Equals(defaultValue);
        }

    }
    #endregion 靜態擴充

    #region 特性
    /// <summary>
    /// 特性容器
    /// </summary>
    public class AttributeBox
    {
        /// <summary>
        /// JSON 序列化時隱藏空值屬性
        /// </summary>
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
        public class IgnoreNullsAttribute : Attribute
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public class IgnoreNullsContractResolver : DefaultContractResolver
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <param name="memberSerialization"></param>
            /// <returns></returns>
            protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
            {
                var props = base.CreateProperties(type, memberSerialization);

                // 如果 Class/Struct 有 [IgnoreNulls] 特性
                if (type.GetCustomAttribute<IgnoreNullsAttribute>() != null)
                {
                    foreach (var prop in props)
                    {
                        prop.NullValueHandling = NullValueHandling.Ignore;
                        prop.DefaultValueHandling = DefaultValueHandling.Ignore;
                    }
                }

                return props;
            }
        }
    }
    #endregion 特性
}

// 結構分支
namespace API.Struct
{
    /// <summary>
    /// 基礎類別結構
    /// </summary>
    [IgnoreNulls]
    public struct Type
    {
        public int ID;

        public string No;

        public string Name;

        public string Remark;
    }



}

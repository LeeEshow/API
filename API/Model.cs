using static API.AttributeBox;

namespace API
{
    /// <summary>
    /// 基礎父項類別 (泛型版)
    /// 提供所有物件的基本結構與共用方法，並透過泛型確保子類別回傳正確型別。
    /// </summary>
    /// <typeparam name="T">子類別型別，必須繼承 <see cref="Model{T}"/>，且具備無參數建構子</typeparam>
    [IgnoreNulls]
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
        #endregion 行為
    }


    /// <summary>
    /// 基礎模型，提供類別完整名稱的唯讀屬性。
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 類型完整名稱
        /// </summary>
        public virtual string Class => GetType().FullName;
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

}

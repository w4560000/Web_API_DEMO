namespace BX.Service
{
    public interface IResultData<T> : IResult
    {
        /// <summary>
        /// 查詢資料
        /// </summary>
        T Data { get; set; }
    }
}
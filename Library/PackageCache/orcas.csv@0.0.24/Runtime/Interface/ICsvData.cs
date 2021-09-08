namespace Orcas.Csv
{
    public interface ICsvData<TKey>
    {
        TKey ID { get; set; }
        /// <summary>
        /// 存储时使用，没有特殊含义
        /// </summary>
        bool Saved { get; set; }
    }
}

using System;

public interface IExcelData<TKey> where TKey : IConvertible
{
    TKey ID { get; set; }
}

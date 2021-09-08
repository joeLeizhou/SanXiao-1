using System;
using System.Text;

namespace Orcas.Excel
{
    public interface IConverter
    {
        string Key { get; }
        Type KeyType { get; }
        object Covert2Value(string str);

        void Covert2String(object obj, StringBuilder output);
    }
}
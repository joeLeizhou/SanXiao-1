namespace Orcas.Data
{
    internal enum StorageType : byte
    {
        None,
        Int,
        Float,
        String,
        Node
    }
    
    public enum StorageVersionCheckResult : byte
    {
        /// <summary>
        /// 本地版本和服务器版本无冲突
        /// </summary>
        Access,
        /// <summary>
        /// 遇到冲突
        /// </summary>
        Crash,
        /// <summary>
        /// 遇到异常，建议以客户端的文件版本为准
        /// </summary>
        ExceptionSuggestLocal,
        /// <summary>
        /// 遇到异常，建议以服务器的存储版本为准
        /// </summary>
        ExceptionSuggestWeb,
        /// <summary>
        /// 遇到异常，无有效建议
        /// </summary>
        Exception
    }
}
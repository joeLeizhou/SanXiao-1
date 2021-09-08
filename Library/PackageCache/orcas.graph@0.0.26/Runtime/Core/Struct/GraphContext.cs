﻿using Unity.Mathematics;

namespace Orcas.Graph.Core
{
    public abstract class GraphContext
    {
        /// <summary>
        /// 数据存储ID
        /// </summary>
        public int GraphTempId;
        /// <summary>
        /// Graph Id
        /// </summary>
        public string Name;

        public abstract GraphContext Copy();
    }
}

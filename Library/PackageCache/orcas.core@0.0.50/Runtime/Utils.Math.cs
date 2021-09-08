using UnityEngine;

namespace Orcas.Core.Tools
{
    public static partial class Utils
    {
        /// <summary>
        /// 设置随机种子
        /// </summary>
        /// <param name="x"></param>
        public static void SetRandSeed(int x)
        {
            UnityEngine.Random.InitState(x);
        }
        
        /// <summary>
        /// 获得随机整数
        /// 包括a不包括b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int RandRange(int a, int b)
        {
            return UnityEngine.Random.Range(a, b);
        }
        
        /// <summary>
        /// 获得随机浮点数
        /// 包括a不包括b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float RandRange(float a, float b)
        {
            return UnityEngine.Random.Range(a, b);
        }

        /// <summary>
        /// 获得随机四元数
        /// </summary>
        /// <returns></returns>
        public static Quaternion RandQuaternion()
        {
            return UnityEngine.Random.rotation;
        }
    }
}
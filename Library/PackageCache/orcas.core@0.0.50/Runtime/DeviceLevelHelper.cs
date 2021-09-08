using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Core.Tools
{
    public enum DeviceLevel
    {
        High, 
        Medium,
        Low
    }
    
    public static class DeviceLevelHelper
    {
        /// <summary>
        /// 判断设备性能等级
        /// 测试数据基于2021年设备性能分布
        /// </summary>
        /// <returns>性能等级</returns>
        public static DeviceLevel JudgeDeviceLevel()
        {

            // 主频：单位GHz
            float cpuFrequency = SystemInfo.processorFrequency / 1024f;
            // 运行内存：单位GB
            float ram = SystemInfo.systemMemorySize / 1024f;
            // 显存：单位GB
            float graphicMemory = SystemInfo.graphicsMemorySize / 1024f;
            
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!SystemInfo.supportsInstancing || !SystemInfo.supportsShadows || !SystemInfo.supportsComputeShaders
                    || cpuFrequency <= 2.2f || SystemInfo.processorCount <= 4 || ram <= 4
                    || graphicMemory <= 1f)
                {
                    return DeviceLevel.Low;
                }

                if (cpuFrequency <= 2.6f || SystemInfo.processorCount <= 6 || ram <= 6)
                {
                    return DeviceLevel.Medium;
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                string type = SystemInfo.deviceModel.ToLower().Trim();
                string prefix = type.Substring(0, 3); 
                if (prefix == "iph")
                {
                    //iPhone
                    string strVersion = type.Substring(6);
                    var versionArr = strVersion.Split(',');
                    if (versionArr.Length > 0)
                    {
                        int versionCode;
                        if (int.TryParse(versionArr[0], out versionCode))
                        {
                            if (versionCode <= 9)
                            {
                                // iPhone 7 Plus及以下（iPhone9,4）
                                return DeviceLevel.Low;
                            }
                            
                            if (versionCode <= 10)
                            {
                                // iPhone X 及以下（iPhone10,6）
                                return DeviceLevel.Medium;
                            }
                            
                            return DeviceLevel.High;
                        }
                    }
                }
                else if (prefix == "ipad")
                {
                    // iPad
                    string strVersion = type.Substring(4);
                    var versionArr = strVersion.Split(',');
                    if (versionArr.Length > 1)
                    {
                        int versionCode1;
                        int versionCode2;
                        if (int.TryParse(versionArr[0], out versionCode1) && int.TryParse(versionArr[1], out versionCode2))
                        {
                            if (versionCode1 < 7 || (versionCode1 == 7 && versionCode2 <=6))
                            {
                                // iPad 2018及以前（iPad7,6）
                                return DeviceLevel.Low;
                            }
                            
                            if (versionCode1 < 8)
                            {
                                // iPad Pro 11-inch及以上（iPad8,1） 
                                return DeviceLevel.Medium;
                            }
                            
                            return DeviceLevel.High;
                        }
                    }
                }
                else if (prefix == "ipod")
                {
                    //iPod 暂时没有高端iPod
                    return DeviceLevel.Low;
                }
            }
            // 其他平台/解析失败暂时默认为高配
            return DeviceLevel.High;
        }
    }
}

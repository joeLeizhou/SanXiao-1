using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public class SpeedTestChooseAddressStrategy : IChooseAddressStrategy
    {
        public IPAddressInfo ChooseAddress(IPAddressInfo[] addressInfos)
        {
            if (addressInfos != null && addressInfos.Length > 0)
            {
                Array.Sort(addressInfos, (infoA, infoB) =>
                {
                    // 优先级1：失败次数少的
                    if (infoA.FailCount != infoB.FailCount)
                    {
                        return infoA.FailCount.CompareTo(infoB.FailCount);
                    }
                    
                    // 优先级2：测速快的
                    var typeA = (ConnectType) infoA.Type;
                    var typeB = (ConnectType) infoB.Type;
                    float avgTimeA = infoA.AvgTime;
                    float avgTimeB = infoB.AvgTime;
                    if (typeA != typeB)
                    {
                        // Kcp测数可接受多100ms的时间。因为经验表明Kcp更稳
                        avgTimeA = typeA == ConnectType.Kcp ? avgTimeA - 100 : avgTimeA;
                        avgTimeB = typeB == ConnectType.Kcp ? avgTimeB - 100 : avgTimeB;
                    }
                    return avgTimeA.CompareTo(avgTimeB);
                });
                return addressInfos[0];
            }
            return null; 
        }
    }
}

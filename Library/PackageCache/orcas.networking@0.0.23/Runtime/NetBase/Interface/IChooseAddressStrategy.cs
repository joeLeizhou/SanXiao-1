using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public interface IChooseAddressStrategy
    {
        IPAddressInfo ChooseAddress(IPAddressInfo[] addressInfos);
    }
}

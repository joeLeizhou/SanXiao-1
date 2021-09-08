using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Core.NativeUtils
{
    public interface IScreenRecordBridge
    {
        bool HasStart { get; set; }

        bool IsAvailable();

        void Start();

        void Stop();
    }
}


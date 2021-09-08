using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Orcas.Ecs.Fsm
{
    internal struct FsmInfoComponent : IComponentData
    {
        #if COLLECTION_10
        public FixedString32 FsmName;
        #else
        public NativeString32 FsmName;
        #endif
        public float SaveTime;
        public int Index;
    }
}
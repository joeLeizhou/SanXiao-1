using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft;

namespace Orcas.Graph.Core
{
    public abstract class OrModuleBase : ModuleBase
    {
        protected int EnabledSlotID;
        public virtual void EnableSlotByID(int slotID)
        {
            EnabledSlotID = slotID;
        }
        public OrModuleBase() : base()
        {

        }

#if UNITY_EDITOR
        public OrModuleBase(int id) : base(id)
        {
            EnabledSlotID = 0;
        }
#endif
    }
}

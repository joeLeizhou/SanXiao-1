using System;

namespace Orcas.Graph.Core
{
    internal interface IGraph
    {
#if UNITY_EDITOR
        void SetPath(string path);
        void Save();
        ModuleBase GetModuleById(int id);
        Slot GetSlotById(int id);
        ModuleBase CreateModule(Type moduleType);
        void RemoveModule(int id);
        void RegisterSlot(Slot slot);
#endif
    }
}

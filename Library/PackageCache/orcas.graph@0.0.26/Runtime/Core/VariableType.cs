using System;
namespace Orcas.Graph
{
    [Serializable]
    public enum VariableType
    {
        INT = 0,
        LONG = 1,
        FLOAT = 2,
        STRING = 3,
        OBJECT = 4,
        VECTOR3 = 5,
        VECTOR2 = 6,
        COLOR = 7,
        ENTITY = 8,
        FLOW = 9,
        BOOL = 10,
        ENTITIES = 11,
        ENUM = 12,
    }
}

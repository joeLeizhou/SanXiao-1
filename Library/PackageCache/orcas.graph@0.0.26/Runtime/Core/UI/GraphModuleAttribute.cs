using System;

namespace Orcas.Graph.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GraphModuleAttribute : Attribute
    {
        public readonly string[] Title;
        public readonly int Order;
        public GraphModuleAttribute(params string[] title) { this.Order = 0; this.Title = title; }
        public GraphModuleAttribute(int order, params string[] title) { this.Order = order; this.Title = title; }
    }


}

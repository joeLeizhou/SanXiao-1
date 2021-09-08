namespace Orcas.Networking
{
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class PropertyIndexAttribute : System.Attribute
    {
        public readonly int Index;
        public PropertyIndexAttribute(int index)
        {
            this.Index = index;
        }

    }
}
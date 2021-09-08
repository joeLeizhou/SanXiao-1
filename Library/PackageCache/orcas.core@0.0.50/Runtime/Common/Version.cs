using System;

namespace Orcas.Core
{
    public struct Version : IComparable, IComparable<Version>
    {
        private int v1, v2, v3;
        public Version(string vStr)
        {
            var vss = vStr.Split('.');
            v1 = int.Parse(vss[0]);
            v2 = int.Parse(vss[1]);
            v3 = vss.Length > 2 ? int.Parse(vss[2]) : 0;
        }

        public Version(string vStr, char split)
        {
            var vss = vStr.Split(split);
            v1 = int.Parse(vss[0]);
            v2 = int.Parse(vss[1]);
            v3 = vss.Length > 2 ? int.Parse(vss[2]) : 0;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is Version)
                return CompareTo((Version)obj);
            return -1;
        }

        public int CompareTo(Version other)
        {
            if (v1 == other.v1)
                if (v2 == other.v2)
                    return v3 - other.v3;
                else
                    return v2 - other.v2;
            else
                return v1 - other.v1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Version))
                return false;

            return GetHashCode() == obj.GetHashCode();
        }

        public bool Equals(Version obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return v1 << 20 | v2 << 10 | v3;
        }

        public override string ToString()
        {
            return $"{v1}.{v2}.{v3}";
        }
        public string ToStringShort()
        {
            return $"{v1}.{v2}";
        }
        public static bool operator >(Version lhs, Version rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator ==(Version lhs, Version rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }
        public static bool operator !=(Version lhs, Version rhs)
        {
            return lhs.CompareTo(rhs) != 0;
        }
        public static bool operator <(Version lhs, Version rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }
    }
}
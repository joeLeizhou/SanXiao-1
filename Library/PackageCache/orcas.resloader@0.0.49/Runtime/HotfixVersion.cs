using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Resources
{
    public enum HotfixWay
    {
        None,
        ForceDownload,
        NormalFix,
    }

    public struct HotfixVersion
    {
        public int VersionNumber1;
        public int VersionNumber2;
        public int VersionNumber3;

        public HotfixVersion(string version)
        {
            string[] arr = version.Split('.');
            VersionNumber1 = int.Parse(arr[0]);
            VersionNumber2 = int.Parse(arr[1]);
            VersionNumber3 = arr.Length > 2 ? int.Parse(arr[2]) : 0;
        }

        public static bool operator >(HotfixVersion lhs, HotfixVersion rhs)
        {
            if (lhs.VersionNumber1 > rhs.VersionNumber1)
            {
                return true;
            }
            else if (lhs.VersionNumber1 == rhs.VersionNumber1)
            {
                if (lhs.VersionNumber2 > rhs.VersionNumber2)
                {
                    return true;
                }
                else if (lhs.VersionNumber2 == rhs.VersionNumber2)
                {
                    return lhs.VersionNumber3 > rhs.VersionNumber3;
                }
            }
            return false;
        }

        public static bool operator <(HotfixVersion lhs, HotfixVersion rhs)
        {
            return rhs > lhs;
        }

        public static bool operator ==(HotfixVersion lhs, HotfixVersion rhs)
        {
            return (rhs.VersionNumber1 == lhs.VersionNumber1) && (rhs.VersionNumber2 == lhs.VersionNumber2) && (rhs.VersionNumber3 == lhs.VersionNumber3);
        }

        public static bool operator !=(HotfixVersion lhs, HotfixVersion rhs)
        {
            return (lhs == rhs) == false;
        }

        public override int GetHashCode()
        {
            return VersionNumber1 << 20 | VersionNumber2 << 10 | VersionNumber3;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || (obj is HotfixVersion) == false)
                return false;
            return this.Equals((HotfixVersion)obj);
        }

        public bool Equals(HotfixVersion v)
        {
            return this.GetHashCode() == v.GetHashCode();
        }

        public override string ToString()
        {
            return $"{VersionNumber1}.{VersionNumber2}.{VersionNumber3}";
        }
        public string ToStringShort()
        {
            return $"{VersionNumber1}.{VersionNumber2}";
        }
    }
}
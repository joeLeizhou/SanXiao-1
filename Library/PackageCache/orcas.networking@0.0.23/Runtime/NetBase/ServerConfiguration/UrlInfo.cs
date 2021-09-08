using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    [Serializable]
    public class UrlInfo
    {
        [Serializable]
        public enum UrlType
        {
            Normal,
            SandBox,
            Test,
            Develop,
            Custom
        }

        [SerializeField]
        private string _url;

        [SerializeField]
        private UrlType _urlType;

        [SerializeField]
        private string _urlTypeName;

        [SerializeField]
        private int _port = 80;

        [SerializeField]
        private int _id;

        [HideInInspector]
        public bool _visible = true;

        public UrlType GetUrlType()
        {
            return _urlType;
        }

        public string GetUrlTypeName()
        {
            if (_urlType == UrlType.Normal)
            {
                return "Normal";
            }
            else if (_urlType == UrlType.Develop)
            {
                return "Develop";
            }
            else if (_urlType == UrlType.SandBox)
            {
                return "SandBox";
            }
            else if (_urlType == UrlType.Test)
            {
                return "Test";
            }
            else
            {
                return _urlTypeName;
            }

        }

        public string GetUrl()
        {
            return _url;
        }

        public int GetPort()
        {
            return _port;
        }

        public int GetUrlID()
        {
            return _id;
        }
    }
}
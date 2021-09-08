﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orcas.Core
{
    public abstract class SingletonBase<T> where T : class, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Orcas.Fsm{
	public class StateFactory{
		
		private static StateFactory _instance;
		public static StateFactory Instance
		{
			get
			{
				if (_instance == null) _instance = new StateFactory();
				return _instance;
			}
		}
		private Dictionary<Type, ConstructorInfo> constructorsDict = new Dictionary<Type, ConstructorInfo>();
		
		
		/// <summary>
		/// 添加状态定义
		/// </summary>
		/// <param name="overwrite"></param>
		/// <typeparam name="T"></typeparam>
		public void AddState<T>(bool overwrite = true) where T : StateBase
		{
			AddState(typeof(T), overwrite);
		}
		
		public void AddState(Type stateType, bool overwrite = true)
		{
			if (constructorsDict.ContainsKey(stateType))
			{
				if (overwrite)
				{
					constructorsDict[stateType] = stateType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
				}
			}
			else
			{
				constructorsDict.Add(stateType, stateType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null));
			}
		}

		
		/// <summary>
		/// 创建状态
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public StateBase CreateState<T>()
		{
			return CreateState(typeof(T));
		}
		
		public StateBase CreateState(Type stateType)
		{
			if (constructorsDict.ContainsKey(stateType) == false) return null;
			var typeConstructor = constructorsDict[stateType];
			StateBase ret = typeConstructor.Invoke(new object[0]) as StateBase;
			return ret;
		}

		public bool ContainsState(Type type)
		{
			return constructorsDict.ContainsKey(type);
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Orcas.Lua.Core
{
	public class Localization : MonoBehaviour {

	    public string key;

		// Use this for initialization
		void Start () {
	        Text font = this.transform.GetComponent<Text>();
	        if(font != null && !string.IsNullOrEmpty(key))
	        {
	            font.text = CLanguageManager.GetStringByKey(key);
	        }
		}
	}
}

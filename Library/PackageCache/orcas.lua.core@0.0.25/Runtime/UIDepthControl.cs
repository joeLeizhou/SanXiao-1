using UnityEngine;
using System.Collections;

using UnityEngine.UI;

namespace Orcas.Lua.Core
{
    
    public class UIDepthControl : MonoBehaviour
    {
        /// <summary>
        /// 深度，默认为零
        /// </summary>
        public int depth;
        /// <summary>
        /// 是否是UI
        /// </summary>
        public bool isUI = true;

        // Use this for initialization
        void Start()
        {
            this.refreshDepth();
        }

        void OnEnable()
        {
            this.refreshDepth();
        }

        private void refreshDepth()
        {
            if (isUI)
            {
                Canvas canvas = this.GetComponent<Canvas>();
                GraphicRaycaster raycaster = this.GetComponent<GraphicRaycaster>();
                if (canvas == null) canvas = this.gameObject.AddComponent<Canvas>();
                if (raycaster == null) raycaster = this.gameObject.AddComponent<GraphicRaycaster>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = this.depth;
            }
            else
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>();

                foreach (Renderer render in renders)
                {
                    render.sortingOrder = this.depth;
                }
            }
        }

        public void SetDepth(int depth)
        {
            this.depth = depth;
            refreshDepth();
        }
    }
}


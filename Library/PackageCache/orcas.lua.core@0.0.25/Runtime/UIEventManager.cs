using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.EventSystems;

namespace Orcas.Lua.Core
{
    public class UIEventManager : CustomEventTrigger
    {
        public delegate void VoidDelegate(GameObject go);

        public delegate void TouchDelegate(Vector3 position, Vector3 dis, bool touchEnd);

        public static bool disableSingleClickForall = false;
        public bool disableSingleClick = false;
        public VoidDelegate onClickDelegate;
        public VoidDelegate onLongClickDelegate;
        public VoidDelegate onPointerDownDelegate;
        public VoidDelegate onPointerUpDelegate;
        public VoidDelegate onDragDelegate;
        public VoidDelegate onEndDragDelegate;
        public VoidDelegate onDropDelegate;

        private bool startTouch;
        public TouchDelegate onTouchDelegate;
        private Vector3 lastMousePos;
        private Canvas UICanvas_2D, UICanvas_3D;
        private bool _pressed;
        private bool _hasLongPressed;
        private float _pressedTime;
        public float LongPressDuration = 0.2f;

        /// <summary>
        /// 新手引导用
        /// </summary>
        private LuaFunction clickCallback;

        public static UIEventManager Get(GameObject go)
        {
            if (go == null)
            {
                Debugger.LogError("对象为空");
                return null;
            }

            UIEventManager listener = go.GetComponent<UIEventManager>();
            if (listener == null) listener = go.AddComponent<UIEventManager>();

            return listener;
        }

        void Awake()
        {
            //this.UICanvas_2D = UIManager.UICanvasObj_2D.GetComponent<Canvas>();
            //this.UICanvas_3D = UIManager.UICanvasObj_3D.GetComponent<Canvas>();
        }

        void Update()
        {
            // longpress event 
            if (_pressed && this.onLongClickDelegate != null)
            {
                if (Time.realtimeSinceStartup - _pressedTime >= LongPressDuration && !_hasLongPressed)
                {
                    _hasLongPressed = true;
                    _pressedTime = Time.realtimeSinceStartup;
                    if (this.onLongClickDelegate != null) this.onLongClickDelegate(this.gameObject);
                }
            }

            // touch event
            if (this.onTouchDelegate == null) return;

            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                if (IsPointerOverGameObject(this.UICanvas_2D, Input.mousePosition) ||
                    IsPointerOverGameObject(this.UICanvas_3D, Input.mousePosition))
                {
                    this.lastMousePos = Input.mousePosition;
                    startTouch = true;
                    this.onTouchDelegate(Input.mousePosition, Input.mousePosition - lastMousePos, false);
                    //DebugerGUI.Log("当前触摸在UI上");
                    //DebugHelper.Log("当前触摸在UI上");
                }
                else
                {
                    //DebugerGUI.Log("当前没有触摸在UI上");
                    //DebugHelper.Log("当前没有触摸在UI上");
                }
            }

            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                if (startTouch)
                {
                    startTouch = false;
                    this.onTouchDelegate(Input.mousePosition, Input.mousePosition - lastMousePos, true);
                }
            }

            if (this.startTouch && Input.mousePosition != this.lastMousePos)
            {
                this.onTouchDelegate(Input.mousePosition, Input.mousePosition - lastMousePos, false);
                this.lastMousePos = Input.mousePosition;
            }
        }

        //通过画布上的 GraphicRaycaster 组件发射射线  
        private bool IsPointerOverGameObject(Canvas canvas, Vector2 screenPosition)
        {
            //实例化点击事件  
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            //将点击位置的屏幕坐标赋值给点击事件  
            eventDataCurrentPosition.position = screenPosition;
            //获取画布上的 GraphicRaycaster 组件  
            GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();

            List<RaycastResult> results = new List<RaycastResult>();
            // GraphicRaycaster 发射射线  
            uiRaycaster.Raycast(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject == this.gameObject) return true;
            }

            return false;
            //return results.Count > 0;
        }


        /// <summary>
        /// 添加点击事件
        /// </summary>
        /// <param name="btnObj"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool onClick(LuaFunction fun)
        {
            this.onClickDelegate = delegate(GameObject obj) { fun.Call(obj); };
            this.clickCallback = fun;
            return true;
        }

        /// <summary>
        /// 添加点击事件
        /// </summary>
        /// <param name="btnObj"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool onLongClick(LuaFunction fun)
        {
            this.onLongClickDelegate = delegate(GameObject obj) { fun.Call(obj); };
            // this.clickCallback = fun;
            return true;
        }

        /// <summary>
        /// 给ScrollView用的
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public void OnClickInScroll(LuaFunction fun)
        {
            this.gameObject.GetComponent<Button>().onClick.AddListener(delegate() { fun.Call(this.gameObject); }
            );

            this.clickCallback = fun;

            this.enabled = false;
        }

        /// <summary>
        /// 返回点击事件
        /// </summary>
        /// <returns></returns>
        public LuaFunction getCkickFun()
        {
            return this.clickCallback;
        }

        /// <summary>
        /// 删除点击事件
        /// </summary>
        /// <returns></returns>
        public bool removeClick()
        {
            this.onClickDelegate = null;

            return true;
        }

        /// <summary>
        /// 删除长按点击事件
        /// </summary>
        /// <returns></returns>
        public bool removeLongClick()
        {
            this.onLongClickDelegate = null;

            return true;
        }


        /// <summary>
        /// 事件响应
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            bool singleClick =
                eventData.pointerId == 0 ||
                eventData.pointerId ==
                -1; //https://docs.unity3d.com/ScriptReference/EventSystems.PointerEventData-pointerId.html
            // DebugHelper.Log("pointerId "+eventData.pointerId.ToString()+ " single:"+singleClick.ToString());
            if (this.onClickDelegate != null && (singleClick || disableSingleClickForall || disableSingleClick))
                this.onClickDelegate(this.gameObject);
        }

        //=====================================================================================
        public bool onDown(LuaFunction fun)
        {
            this.onPointerDownDelegate = delegate(GameObject obj) { fun.Call(obj); };
            this.clickCallback = fun;
            return true;
        }

        public bool removeDown()
        {
            this.onPointerDownDelegate = null;
            return true;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (this.onPointerDownDelegate != null) this.onPointerDownDelegate(this.gameObject);
            _pressedTime = Time.realtimeSinceStartup;
            _pressed = true;
            _hasLongPressed = false;
        }

        //====================================================================================
        public bool onUp(LuaFunction fun)
        {
            this.onPointerUpDelegate = delegate(GameObject obj) { fun.Call(obj); };
            return true;
        }

        public bool removeUp()
        {
            this.onPointerUpDelegate = null;

            return true;
        }

        public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (this.onPointerUpDelegate != null) this.onPointerUpDelegate(this.gameObject);
            _pressed = false;
        }

        //================================================================================
        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            _pressed = false;
        }

        //================================================================================
        public bool onDrop(LuaFunction fun)
        {
            this.onDragDelegate = delegate(GameObject obj) { fun.Call(obj); };
            return true;
        }

        public bool removeDrop()
        {
            this.onDragDelegate = null;

            return true;
        }

        public override void OnDrop(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (this.onDropDelegate != null) this.onDropDelegate(this.gameObject);
        }

        //=============================================================================== 
        public bool onValueChanged(LuaFunction fun)
        {
            if (this.gameObject.GetComponent<InputField>() != null)
            {
                this.gameObject.GetComponent<InputField>().onValueChanged.AddListener((obj) => fun.Call(obj));
                return true;
            }

            return false;
        }

        public bool removeValueChanged()
        {
            if (this.gameObject.GetComponent<InputField>() != null)
            {
                this.gameObject.GetComponent<InputField>().onValueChanged.RemoveAllListeners();
                return true;
            }

            return false;
        }


        public bool onEndEdit(LuaFunction fun)
        {
            if (this.gameObject.GetComponent<InputField>() != null)
            {
                this.gameObject.GetComponent<InputField>().onEndEdit.AddListener((obj) => fun.Call(obj));
                return true;
            }

            return false;
        }

        public bool removeEndEdit()
        {
            if (this.gameObject.GetComponent<InputField>() != null)
            {
                this.gameObject.GetComponent<InputField>().onEndEdit.RemoveAllListeners();
                return true;
            }

            return false;
        }

        //==============================================================================
        public bool onTouch(LuaFunction fun)
        {
            this.onTouchDelegate = delegate(Vector3 vec, Vector3 dis, bool touchEnd) { fun.Call(vec, dis, touchEnd); };
            return true;
        }


        /// <summary>
        /// 删除触摸事件
        /// </summary>
        /// <returns></returns>
        public bool removeTouch()
        {
            this.onTouchDelegate = null;

            return true;
        }

        //==============================================================================
        /// <summary>
        /// Toggle 状态改变事件
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool ToggleOnValueChanged(LuaFunction fun)
        {
            if (this.gameObject.GetComponent<Toggle>() != null)
            {
                this.gameObject.GetComponent<Toggle>().onValueChanged
                    .AddListener((isOn) => fun.Call(this.gameObject, isOn));
                return true;
            }

            return false;
        }

        public bool RemoveToggleOnValueChanged()
        {
            if (this.gameObject.GetComponent<Toggle>() != null)
            {
                this.gameObject.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                return true;
            }

            return false;
        }

        //=============================================================================== 
        /// <summary>
        /// Slider 状态改变事件
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool SliderOnValueChanged(LuaFunction fun)
        {
            if (this.gameObject.GetComponent<Slider>() != null)
            {
                this.gameObject.GetComponent<Slider>().onValueChanged.AddListener((obj) => fun.Call(obj));
                return true;
            }

            return false;
        }

        public bool RemoveSliderOnValueChanged()
        {
            if (this.gameObject.GetComponent<Slider>() != null)
            {
                this.gameObject.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
                return true;
            }

            return false;
        }

        //=============================================================================== 
        /// <summary>
        /// ScrollRect 状态改变事件
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool ScrollOnValueChanged(LuaFunction fun)
        {
            if (this.gameObject.GetComponent<ScrollRect>() != null)
            {
                this.gameObject.GetComponent<ScrollRect>().onValueChanged.AddListener((obj) => fun.Call(obj));
                return true;
            }

            return false;
        }

        public bool RemoveScrollOnValueChanged()
        {
            if (this.gameObject.GetComponent<ScrollRect>() != null)
            {
                this.gameObject.GetComponent<ScrollRect>().onValueChanged.RemoveAllListeners();
                return true;
            }

            return false;
        }

        //==============================================================================
        /// <summary>
        /// 删除按钮上的所有事件
        /// </summary>
        /// <returns></returns>
        public bool removeAllEvent()
        {
            this.onClickDelegate = null;
            this.onLongClickDelegate = null;
            this.onPointerDownDelegate = null;
            this.onPointerUpDelegate = null;
            this.onDragDelegate = null;
            this.onEndDragDelegate = null;
            this.onDropDelegate = null;
            this.onTouchDelegate = null;
            this.removeValueChanged();
            this.RemoveToggleOnValueChanged();
            this.RemoveSliderOnValueChanged();
            return true;
        }
    }
}
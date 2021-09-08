using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LuaInterface;
using System;

namespace Orcas.Lua.Core
{
    public class UIDragManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IScrollHandler
    {        
        #region 内部状态
        enum DragState
        {
            None = 0,               
            Draging = 1,
            DoubleTouching = 2,            
        }
        
        #endregion

        #region 回调定义

        public delegate void DragDelegate(UnityEngine.EventSystems.PointerEventData eventData);
        public delegate void DoubleTouchDelegate(Vector2 pos1, Vector2 pos2);
        public delegate void MouseScrollDelegate(float valueY);
        
        #endregion


        #region 参数区
        
        /// <summary>
        /// 点击的停留时间阈值
        /// </summary>
        public float ClickTimeThreshold = 0.2f;
        
        /// <summary>
        /// 长按点击的停留事件阈值
        /// </summary>
        public float LongPressClickTimeThreshold = 0.2f;
        
        #endregion

        #region 内部参数
        private const int NullPointerId = -10086;
        private DragDelegate _beginDragDelegate;
        private DragDelegate _dragDelegate;
        private DragDelegate _dragForceDelegate;
        private DragDelegate _endDragDelegate;
        
        private DragDelegate _clickDelegate;
        private DragDelegate _longPressClickDelegate;

        private DoubleTouchDelegate _doubleTouchDelegate;
        private MouseScrollDelegate _mouseScrollDelegate;
        
        private DragState _state = DragState.None;
        private PointerEventData _pointDownEventData1;
        private PointerEventData _pointDownEventData2;
        private Vector2 _doubleTouchPos1;
        private Vector2 _doubleTouchPos2;
        private int _touchId1 = NullPointerId;
        private int _touchId2 = NullPointerId;
        private float _touchDownTime;
        private bool _exceedClickTime = false;
        #endregion

        public static UIDragManager Get(GameObject go)
        {
            if (go == null)
            {
                Debugger.LogError("对象为空");
                return null;
            }

            UIDragManager listener = go.GetComponent<UIDragManager>();
            if (listener == null) listener = go.AddComponent<UIDragManager>();
            return listener;
        }
        
        private void Start()
        {
            _state = DragState.None;
            _exceedClickTime = false;
        }
        
        void Update()
        {
            if (_state == DragState.Draging)
            {                
                if (Time.time > _touchDownTime + ClickTimeThreshold)
                {
                    _exceedClickTime = true;
                }

                if(_pointDownEventData1 != null)
                {
                    _dragForceDelegate?.Invoke(_pointDownEventData1);
                }

            }
        }

        #region EventSystem事件

        public void OnScroll(PointerEventData eventData)
        {
            _mouseScrollDelegate?.Invoke(eventData.scrollDelta.y);
        }
        
        
        public virtual void OnPointerDown(PointerEventData eventData)
        {            
            if (_touchId1 == NullPointerId)
            {
                _touchId1 = eventData.pointerId;
                _pointDownEventData1 = eventData;
                _doubleTouchPos1 = eventData.position;
                _state = DragState.Draging;
                _touchDownTime = Time.time;
                _beginDragDelegate?.Invoke(_pointDownEventData1);   
                return;
            }

            if (_touchId1 != NullPointerId && _touchId2 == NullPointerId)
            {
                _touchId2 = eventData.pointerId;
                _pointDownEventData2 = eventData;
                _doubleTouchPos2 = eventData.position;
                _state = DragState.DoubleTouching;
            }
        }
        
        public virtual void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(_state != DragState.Draging && _state != DragState.DoubleTouching) return;
            
            // 手指1的拖动
            if (_touchId1 == eventData.pointerId)
            {
                _pointDownEventData1 = eventData;
                _doubleTouchPos1 = eventData.position;
                if (_state == DragState.DoubleTouching)
                {
                    _doubleTouchDelegate?.Invoke(_doubleTouchPos1, _doubleTouchPos2);
                }
                else
                {
                    _dragDelegate?.Invoke(_pointDownEventData1);
                } 
            }

            if (_touchId2 == eventData.pointerId)
            {
                _pointDownEventData2 = eventData;
                _doubleTouchPos2 = eventData.position;
                if (_state == DragState.DoubleTouching)
                {
                    _doubleTouchDelegate?.Invoke(_doubleTouchPos1, _doubleTouchPos2);
                }   
            }
        }
        
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (_state == DragState.DoubleTouching)
            {
                _endDragDelegate?.Invoke(_pointDownEventData1);
                // 双指拖动时，松开一只手指的时候还能拖动
                if (eventData.pointerId == _touchId2)
                {
                    _touchId2 = NullPointerId;
                }
                else
                {
                    _touchId1 = _touchId2;
                    _pointDownEventData1 = _pointDownEventData2;
                    _doubleTouchPos1 = _doubleTouchPos2;
                    _touchId2 = NullPointerId;
                }
                _state = DragState.Draging;
                _beginDragDelegate?.Invoke(_pointDownEventData1);
            }            
            else if (_state == DragState.Draging)
            {
                if (_exceedClickTime == false)
                {
                    // 短按Click
                    _clickDelegate?.Invoke(eventData);
                }
                else
                {
                    // 长按Long Press Click
                    _longPressClickDelegate?.Invoke(eventData);
                }
                _exceedClickTime = false;

                _touchId1 = NullPointerId;
                _touchId2 = NullPointerId;
                _state = DragState.None;
                _endDragDelegate?.Invoke(eventData);
                // _state = DragState.EndDrag;
            }
            _touchDownTime = 0f;
        }

        #endregion
        

        #region 事件注册

        public void onBeginDrag(LuaFunction fun)
        {
            _beginDragDelegate = delegate (UnityEngine.EventSystems.PointerEventData eventData) { fun.Call(eventData); };
        }

        public void onDrag(LuaFunction fun)
        {
            _dragDelegate = delegate (UnityEngine.EventSystems.PointerEventData eventData) { fun.Call(eventData); };
        }

        //每帧都会回调
        public void onForceDrag(LuaFunction fun)
        {
            _dragForceDelegate = delegate (UnityEngine.EventSystems.PointerEventData eventData) { fun.Call(eventData); };
        }

        public void onEndDrag(LuaFunction fun)
        {
            _endDragDelegate = delegate (UnityEngine.EventSystems.PointerEventData eventData) { fun.Call(eventData); };
        }
        

        public void onDoubleTouch(LuaFunction fun)
        {
            _doubleTouchDelegate = (Vector2 posA, Vector2 posB) => { fun.Call(posA, posB); };
        }

        public void onClick(LuaFunction fun)
        {
            _clickDelegate = (PointerEventData eventData) => { fun.Call(eventData); };
        }

        public void onLongPressClick(LuaFunction fun)
        {
            _longPressClickDelegate = (PointerEventData eventData) => { fun.Call(eventData); };
        }
        
        public void onMouseScroll(LuaFunction fun)
        {
            _mouseScrollDelegate = (float y) => { fun.Call(y); };
        }
        
        public bool removeDrag()
        {
            _beginDragDelegate = null;
            _dragDelegate = null;
            _endDragDelegate = null;
            _doubleTouchDelegate = null;
            _clickDelegate = null;
            _longPressClickDelegate = null;
            _mouseScrollDelegate = null;
            return true;
        }

        #endregion
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus == false)
           {               
               if (_state == DragState.Draging)
               {
                   _touchId1 = NullPointerId;
                   _touchId2 = NullPointerId;
                   _state = DragState.None;
                   _endDragDelegate?.Invoke(_pointDownEventData1);
               }
           }
            
        }
        
        private void OnDisable()
        {
            if (_state == DragState.Draging)
            {
                _touchId1 = NullPointerId;
                _touchId2 = NullPointerId;
                _state = DragState.None;
                _endDragDelegate?.Invoke(_pointDownEventData1);
            }
        }
    }
    
}

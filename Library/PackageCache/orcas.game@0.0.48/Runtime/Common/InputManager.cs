using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Orcas.Core;
using UnityEngine;

namespace Orcas.Game.Common
{
    public class InputKeyData
    {
        internal KeyCode KeyCode;
        public int Down;
        public int Up;
        public bool Hold;
        public bool FinalHold;

        internal void Reset()
        {
            Down = Up = 0;
            Hold = false;
        }
    }

    public class MouseData
    {
        internal bool First = true;
        public int Down;
        public int Up;
        public bool Hold;
        public bool FinalHold;
        public Vector3 Position;
        public Vector3 DeltaPosition;
        
        internal void Reset()
        {
            Down = Up = 0;
            Hold = false;
        }
    }

    public struct FingerData
    {
        public int Id;
        public int Down;
        public int Up;
        public bool Hold;
        public bool FinalHold;
        public Vector2 DeltaPosition;
        public Vector2 Position;
        public int TapCount;

        internal void Reset()
        {
            Id = -1;
            Down = Up = 0;
            Hold = false;
        }

        internal void SetFingerDataWithTouch(in Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Down++;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Up++;
                    break;
            }

            Hold = FinalHold = true;
            TapCount = touch.tapCount;
            Position = touch.position;
            DeltaPosition = touch.deltaPosition;
            
        }
    }

    public class TouchData
    {
        public int FingerCount;
        public FingerData[] Finger = new FingerData[InputManager.MaxTouchFinger];
        internal void Reset()
        {
            FingerCount = 0;
        }

        internal void Reset(int index)
        {
            Finger[index].Reset();
        }
    }
    
    internal class InputData : MonoBehaviour
    {
        private Dictionary<KeyCode, InputKeyData> _registerEvents;
        private TouchData _touchData;
        private List<InputKeyData> _data;
        private MouseData[] _mouseData;

        private void Awake()
        {
            _registerEvents = new Dictionary<KeyCode, InputKeyData>();
            _touchData = new TouchData();
            _data = new List<InputKeyData>();
            _mouseData = new MouseData[3];
            for (var i = 0; i < 3; i++)
            {
                _mouseData[i] = new MouseData();
            }
        }

        internal void RegisterKeyCodeEvent(in KeyCode keyCode)
        {
            if (_registerEvents.ContainsKey(keyCode) == false)
            {
                var data = new InputKeyData()
                {
                    KeyCode = keyCode
                };
                _data.Add(data);
                _registerEvents.Add(keyCode, data);
            }
        }

        internal void RemoveKeyCodeEvent(in KeyCode keyCode)
        {
            if (_registerEvents.ContainsKey(keyCode) == false) return;
            _data.Remove(_registerEvents[keyCode]);
            _registerEvents.Remove(keyCode);
        }

        private void Update()
        {
            for (var i = 0; i < _data.Count; i++)
            {
                if (Input.GetKey(_data[i].KeyCode))
                {
                    _data[i].FinalHold = _data[i].Hold = true;
                    if (Input.GetKeyDown(_data[i].KeyCode))
                    {
                        _data[i].Down ++;
                    }
                }
                else
                {
                    _data[i].FinalHold = false;
                    if (Input.GetKeyUp(_data[i].KeyCode))
                    {
                        _data[i].Up ++;
                    }
                }
            }

            if (InputManager.EnableMouse)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButton(i))
                    {
                        _mouseData[i].FinalHold = _mouseData[i].Hold = true;
                        if (Input.GetMouseButtonDown(i))
                        {
                            _mouseData[i].Down++;
                        }
                    }
                    else
                    {
                        _mouseData[i].FinalHold = false;
                        if (Input.GetMouseButtonUp(i))
                        {
                            _mouseData[i].Up++;
                        }
                    }

                    if (_mouseData[i].First)
                    {
                        _mouseData[i].First = false;
                    }
                    else
                    {
                        _mouseData[i].DeltaPosition = Input.mousePosition - _mouseData[i].Position;
                    }

                    _mouseData[i].Position = Input.mousePosition;
                }
            }

            if (InputManager.EnableTouch)
            {
                var touches = Input.touches;
                for (var i = 0; i < Input.touchCount; i++)
                {
                    var found = false;
                    for (var j = 0; j < _touchData.FingerCount; j++)
                    {
                        if (_touchData.Finger[j].Id != touches[i].fingerId) continue;
                        found = true;
                        _touchData.Finger[j].SetFingerDataWithTouch(touches[i]);
                        break;
                    }

                    if (found) continue;
                    if (_touchData.FingerCount == InputManager.MaxTouchFinger) continue;
                    _touchData.Reset(_touchData.FingerCount);
                    _touchData.Finger[_touchData.FingerCount].SetFingerDataWithTouch(touches[i]);
                    _touchData.FingerCount++;
                }

                for (var j = 0; j < _touchData.FingerCount; j++)
                {
                    var found = false;
                    for (var i = 0; i < Input.touchCount; i++)
                    {
                        if (_touchData.Finger[j].Id != touches[i].fingerId) continue;
                        found = true;
                        break;
                    }

                    if (found) continue;
                    _touchData.Finger[j].Hold = false;
                }
            }
        }

        internal InputKeyData GetKeyData(in KeyCode keyCode)
        {
            if (_registerEvents.ContainsKey(keyCode) != false) return _registerEvents[keyCode];
            Debug.LogError("需要先用InputManagerd的RegisterKeyCodeEvent函数注册按键再访问数据!");
            return null;
        }

        internal MouseData GetMouseData(int index)
        {
            return index >= 3 ? null : _mouseData[index];
        }

        internal TouchData GetTouchData()
        {
            return _touchData;
        }

        internal void Reset()
        {
            for (var i = 0; i < _data.Count; i++)
            {
                _data[i].Reset();
            }

            if (InputManager.EnableMouse)
            {
                for (var i = 0; i < 3; i++)
                {
                    _mouseData[i].Reset();
                }
            }

            if (InputManager.EnableTouch)
            {
                _touchData.Reset();
            }
        }
    }
    
    public class InputManager : IManager
    {
        public static int MaxTouchFinger = 5;
        public static bool EnableTouch = false;
        public static bool EnableMouse = true;
        private InputData _inputData;
        
        public void RegisterKeyCodeEvent(KeyCode keyCode)
        {
            _inputData.RegisterKeyCodeEvent(keyCode);
        }

        public void RemoveKeyCodeEvent(KeyCode keyCode)
        {
            _inputData.RemoveKeyCodeEvent(keyCode);
        }

        public MouseData GetMouseData(int index)
        {
            return _inputData.GetMouseData(index);
        }
        
        public InputKeyData GetKeyData(KeyCode code)
        {
            return _inputData.GetKeyData(code);
        }

        public TouchData GetTouchData()
        {
            return _inputData.GetTouchData();
        }
        
        public void Init()
        {
            var obj = new GameObject("InputData");
            _inputData = obj.AddComponent<InputData>();
            Object.DontDestroyOnLoad(obj);
        }

        public void SetLooper(GameLogicLooper logicLooper)
        {
            logicLooper.AfterUpdateQueue += _inputData.Reset;
        }

        public void Update(uint currentFrameCount)
        {
            
        }

        public void OnPause()
        {
        }

        public void OnResume()
        {
        }

        public void OnDestroy()
        {
        }
    }
}
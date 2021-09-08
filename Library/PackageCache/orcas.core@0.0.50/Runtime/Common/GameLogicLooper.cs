using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Orcas.Core
{
    public abstract class GameLogicLooper
    {
        public delegate void UpdateDelegate();
        /// <summary>
        /// 当前帧数
        /// </summary>
        public uint FrameCount { get; private set; }

        /// <summary>
        /// 开启后会补帧
        /// 默认关闭
        /// </summary>
        public bool SpeedMode = false;
        protected abstract float DeltaTime { get; }
        private float lastUpdateTime = 0;
        public bool Enable = true;
        public UpdateDelegate PreUpdateQueue = delegate {  };
        public UpdateDelegate AfterUpdateQueue = delegate {  };

        /// <summary>
        /// 是否开启分帧模式：如果开启了分帧模式，会把PreUpdate、Update、LateUpdate均摊到几个渲染帧里(除非DeltaTime小于3帧拆不开)
        /// </summary>
        public bool EnableSplitFrame = false;

        private bool hasCallPreUpdate = false;
        private bool hasCallUpdate = false;
        private bool hasCallLateUpdate = false;
        
        public void Update()
        {
            if (SpeedMode == false)
            {
                if (EnableSplitFrame)
                {
                    // 分帧模式
                    if (lastUpdateTime + DeltaTime <= Time.time)
                    {
                        lastUpdateTime += DeltaTime;
                        FrameCount++;
                        hasCallPreUpdate = false;
                        hasCallUpdate = false;
                        hasCallLateUpdate = false;
                    }
                    
                    if (Enable == false) return;
                    if (!hasCallPreUpdate &&
                        lastUpdateTime <= Time.time)
                    {
                        hasCallPreUpdate = true;
                        PreUpdateQueue();
                    }

                    if (!hasCallUpdate && lastUpdateTime + DeltaTime / 3f <= Time.time)
                    {
                        hasCallUpdate = true;
                        LogicUpdate();
                    }
                        
                    if (!hasCallLateUpdate && lastUpdateTime + 2f * DeltaTime / 3f <= Time.time)
                    {
                        hasCallLateUpdate = true;
                        AfterUpdateQueue();
                    }
                }
                else
                {
                    // 非分帧模式
                    if (lastUpdateTime + DeltaTime > Time.time)
                    {
                        return;
                    }
                    
                    lastUpdateTime += DeltaTime;
                    if (Enable == false) return;
                    FrameCount++;
                    PreUpdateQueue();
                    LogicUpdate();
                    AfterUpdateQueue();
                }
            }
            else
            {
                while (lastUpdateTime + DeltaTime <= Time.time)
                {
                    lastUpdateTime += DeltaTime;
                    if (Enable == false) return;
                    FrameCount++;
                    PreUpdateQueue();
                    LogicUpdate();
                    AfterUpdateQueue();
                }
            }
        }
        
        public void ForceUpdate()
        {
            lastUpdateTime = Time.time;
            FrameCount++;
            PreUpdateQueue();
            LogicUpdate();
            AfterUpdateQueue();
        }

        protected abstract void LogicUpdate();
    }
}
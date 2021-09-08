using System.Collections.Generic;
using Orcas.Core;

namespace Orcas.Graph.Core
{
    public class GraphManager : IManager
    {
        private class RunTimeGraph
        {
            public GraphContext Context;
            public Graph Graph;
        }
        public static int MaxStorageData = 100;
        public static GraphManager Instance { get; private set; }
        public IGraphEventHandler GraphEventHandler { get; set; } = null;
        public GraphContext CurrentData;
        internal bool EnableUpdate;

        private readonly Queue<RunTimeGraph> _skillGraphRuns;
        private readonly HashSet<int> _interruptIds;
        private readonly GraphContext[] _contexts;
        private readonly Queue<GraphContext> _initDataQueue;
        
        private int _topTempId;
        private Graph _runningGraph;
        private GraphContext _runningGraphData;
        private uint _lastPauseFrame;
        private uint _currentFrame;
        private uint _pauseDeltaFrameCount;

        public GraphManager()
        {
            Instance = this;
            EnableUpdate = true;
            _contexts = new GraphContext[MaxStorageData];
            _skillGraphRuns = new Queue<RunTimeGraph>();
            _interruptIds = new HashSet<int>();
            _initDataQueue = new Queue<GraphContext>();
            PreLoad();
        }

        /// <summary>
        /// 预加载
        /// </summary>
        private static void PreLoad()
        {
            
        }

        private RunTimeGraph InitRunTimeGraph(GraphContext option)
        {
            var graph = GraphPool.GetGraph(option.Name);
            graph.Init();
            graph.ResetVariable();
            return new RunTimeGraph() { Context = option, Graph = graph };
        }

        public void Update(uint currentFrameCount)
        {
            _currentFrame = currentFrameCount;
            while (_initDataQueue.Count > 0)
            {
                var runGraph = InitRunTimeGraph(_initDataQueue.Dequeue());
                _skillGraphRuns.Enqueue(runGraph);
            }
            var count = _skillGraphRuns.Count;
            for (var i = 0; i < count; i++)
            {
                var runGraph = _skillGraphRuns.Dequeue();
                if (CheckRuntimeGraphInterrupt(runGraph.Context.GraphTempId))
                {
                    GraphPool.CollectGraph(runGraph.Context.Name, runGraph.Graph);
                }
                else
                {
                    CurrentData = runGraph.Context;
                    if (runGraph.Graph.UpdateRun(runGraph.Context))
                        _skillGraphRuns.Enqueue(runGraph);
                    else
                    {
                        _interruptIds.Remove(runGraph.Context.GraphTempId);
                        GraphPool.CollectGraph(runGraph.Context.Name, runGraph.Graph);
                    }
                }
            }
        }

        public void Init()
        {
        }
        
        public void OnPause()
        {
            if (!EnableUpdate) return;
            EnableUpdate = false;
            _lastPauseFrame = _currentFrame;
        }

        public void OnResume()
        {
            if (EnableUpdate) return;
            EnableUpdate = true;
            _pauseDeltaFrameCount += _currentFrame - _lastPauseFrame;
        }

        public void OnDestroy()
        {
            Instance = null;
            GraphPool.ClearAll();
        }

        // public static void PauseGraph(int graphTempId)
        // {
        //     
        // }
        /// <summary>
        /// 获取graph上下文信息
        /// </summary>
        /// <param name="graphTempId"></param>
        /// <returns></returns>
        public static GraphContext GetGraphContext(int graphTempId)
        {
            return Instance._contexts[graphTempId];
        }
        /// <summary>
        /// 打断技能释放
        /// </summary>
        /// <param name="graphTempId"></param>
        /// <param name="changeState"></param>
        public static void InterruptGraph(int graphTempId, bool changeState = true)
        {
            Instance._interruptIds.Add(graphTempId);

            var context = Instance._contexts[graphTempId];
            Instance.GraphEventHandler?.OnInterruput(context);
        }
        /// <summary>
        /// 检查graph是否处于需要被打断的状态
        /// </summary>
        /// <param name="graphTempId"></param>
        /// <returns></returns>
        public static bool CheckRuntimeGraphInterrupt(int graphTempId)
        {
            if (!Instance._interruptIds.Contains(graphTempId)) return false;
            Instance._interruptIds.Remove(graphTempId);
            return true;
        }

        /// <summary>
        /// 通过技能id执行技能函数
        /// 这里会返回一个临时ID，表示技能实例的ID
        /// 在打断施法的时候需要用到临时ID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int RunGraph(GraphContext context)
        {
            context.GraphTempId = Instance._topTempId % MaxStorageData;
            Instance._topTempId++;
            Instance._contexts[context.GraphTempId] = context;
            Instance._initDataQueue.Enqueue(context);
            Instance.GraphEventHandler?.OnRunGraph(context);
            return context.GraphTempId;
        }
    }
}

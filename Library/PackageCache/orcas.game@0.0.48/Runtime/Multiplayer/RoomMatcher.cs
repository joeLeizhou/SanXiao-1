using System.Collections;
using Orcas.Core;
using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;
using UnityEngine;

namespace Orcas.Game.Multiplayer
{
    public abstract class RoomMatcher : IMatcher
    {
        public Timer Timer { get; private set; }
        public MatchState State { get; private set; }

        public int MatchingType { get; private set; }
        public int MatchingStage { get; private set; }

        private readonly IClient _server;
        private float _overTime;
        private Coroutine _coroutine;
        private MultiPlayerManager _multiPlayer;
        private static short _matchCount = 0;

        protected RoomMatcher(IClient server, float overTime = 60)
        {
            _server = server;
            State = MatchState.None;
            _overTime = overTime;
            Timer = new Timer();
            MatchingType = 0;
            MatchingStage = 0;
        }

        protected virtual ReqMatch GetMatchProto(int type, int stage)
        {
            MatchingType = type;
            MatchingStage = stage;
            return new DefaultReqMatch {MatchId = ++_matchCount, MatchType = (byte)type, Stage = stage};
        }

        protected virtual ReqCancelMatch GetCancelProto()
        {
            return new DefaultReqCancelMatch {MatchId = _matchCount};
        }

        IEnumerator Matching()
        {
            while (Timer.RealTime <= _overTime)
            {
                yield return null;
            }
            //TODO: 看具体需求

            SetMatchOver();
            OnMatchOvered(MatchEvent.ClientOverTime, null);
        }

        void IMatcher.SetMultiplayer(MultiPlayerManager multiPlayer)
        {
            _multiPlayer = multiPlayer;
        }

        void IMatcher.Match(int type, int stage)
        {
            Debug.Log("match " + State);
            if (State != MatchState.None && State != MatchState.Over) return; 
            Timer.Start();
            State = MatchState.Matching;
            _server.SendMessage(GetMatchProto(type, stage));
            _coroutine = CoroutineManager.Instance.StartCoroutine(Matching());
        }

        void IMatcher.Cancel()
        {
            if (State != MatchState.Matching) return;
            State = MatchState.Canceling;
            if (_coroutine != null) CoroutineManager.Instance.StopCoroutine(_coroutine);
            _server.SendMessage(GetCancelProto());
        }

        void IMatcher.Reset()
        {
            if (State == MatchState.Over)
                State = MatchState.None;
        }

        /// <summary>
        /// 加入匹配队列结果
        /// </summary>
        /// <param name="matchEvent"></param>
        /// <param name="serverResult"></param>
        protected abstract void OnMatchOvered(MatchEvent matchEvent, RltMatch serverResult);
        /// <summary>
        /// 匹配取消或者服务端取消结果
        /// </summary>
        /// <param name="matchEvent"></param>
        /// <param name="serverResult"></param>
        protected abstract void OnMatchCanceled(MatchEvent matchEvent, RltCancelMatch serverResult);

        /// <summary>
        /// 匹配到对手的结果
        /// </summary>
        /// <param name="matchEvent"></param>
        /// <param name="serverResult"></param>
        protected abstract void OnMatchInfoed(MatchEvent matchEvent, RltMatchInfo serverResult, IRoom room);

        protected virtual Room GetRoom(long roomId, int stage, int scene)
        {
            return new DefaultRoom(roomId, stage, scene);
        }
        
        private void SetMatchOver()
        {
            Timer.Stop();
            if (_coroutine != null) CoroutineManager.Instance.StopCoroutine(_coroutine);
            State = MatchState.Over;
        }

        #region net message
        public void OnMatch(RltMatch serverResult)
        {
            var matchEvent = MatchEvent.Matching;
            if(serverResult.Code != 1)
            {
                SetMatchOver();
                matchEvent = MatchEvent.NoMatch;
            }
            OnMatchOvered(matchEvent, serverResult);
        }

        public void OnMatchCancel(RltCancelMatch serverResult)
        {
            SetMatchOver();
            OnMatchCanceled(MatchEvent.ClientCanceled, serverResult);
        }

        public void OnMatchInfo(RltMatchInfo serverResult)
        {
            SetMatchOver();

            var room = GetRoom(serverResult.RoomID, MatchingStage, serverResult.Scene);
            _multiPlayer.SetRoom(room);
            room.EnterGame(serverResult);

            OnMatchInfoed(MatchEvent.Success, serverResult, room);
        }
        #endregion
    }
}
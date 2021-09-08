using System;
using System.Collections.Generic;
using Orcas.Core;
using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;
using Orcas.Game.Common;

namespace Orcas.Game.Multiplayer
{
    public class MultiPlayerManager : IManager
    {
        private IMatcher _matcher;
        private IRoom _room;
        private Timer _timer;
        public int MyServerId;

        private MultiPlayerManager()
        {
            _timer = new Timer();
            _timer.Start();
        }
        /// <summary>
        /// 创建多人游戏
        /// </summary>
        /// <param name="matcher">推荐继承抽象类RoomMatcher</param>
        /// <param name="room">推荐继承抽象类Room</param>
        /// <returns></returns>
        public static MultiPlayerManager Create(IMatcher matcher, int myServerId)
        {
            var ret = new MultiPlayerManager();
            matcher.SetMultiplayer(ret);
            ret.MyServerId = myServerId;
            ret._room = null;
            ret._matcher = matcher;
            ret.AddProtos();
            return ret;
        }

        internal void AddProtos()
        {
            var factory = ProtocolFactory.Instance;
            factory.AddProto<DefaultReqMatch>(CommonProtoId.ReqMatch, false);
            factory.AddProto<DefaultRltMatch>(CommonProtoId.RltMatch, false);
            factory.AddProto<DefaultReqCancelMatch>(CommonProtoId.ReqCancelMatch, false);
            factory.AddProto<DefaultRltCancelMatch>(CommonProtoId.RltCancelMatch, false);
            factory.AddProto<DefaultRltMatchInfo>(CommonProtoId.RltMatchInfo, false);
        }

        internal void SetRoom(IRoom room)
        {
            room?.SetMultiplayer(this);
            _room = room;
        }

        public void Init()
        {
        }

        public void Update(uint currentFrameCount)
        {
            if (_room != null)
                _room.OnUpdate();

        }

        public IMatcher GetMatcher()
        {
            return _matcher;
        }

        public IRoom GetRoom()
        {
            return _room;
        }

        public void OnPause()
        {
            if (_timer.Stopped) return;
            _timer.Stop();
        }

        public void OnResume()
        {
            if (_timer.Stopped == false) return;
            _timer.ReStart();
        }

        public void OnDestroy()
        {
        }
    }
}
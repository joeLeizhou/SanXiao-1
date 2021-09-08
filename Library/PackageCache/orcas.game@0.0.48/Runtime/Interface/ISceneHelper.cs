using System;
using Orcas.Game.Multiplayer;

namespace Orcas.Game
{
    public interface ISceneHelper
    {
        SceneLoadState LoadState { get; }
        void LoadScene(int state, int scene);
    }
}
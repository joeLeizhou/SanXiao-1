using Orcas.Core;
using UnityEngine;

public class GameLooper : GameLogicLooper
{
    public const float DELTA_TIME = 0.02f;
    public static GameLogicLooper Instance;

    public GameLooper()
    {
        Time.fixedDeltaTime = DELTA_TIME;
        Instance = this;
    }
    protected override void LogicUpdate()
    {
    }

    protected override float DeltaTime { get; } = DELTA_TIME;
}
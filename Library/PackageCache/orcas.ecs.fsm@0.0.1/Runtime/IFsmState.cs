using Unity.Entities;

namespace Orcas.Ecs.Fsm
{
    public interface IFsmState : IComponentData
    {
        float StateEnterTime { get; set; }
        /// <summary>
        /// 记录数据是否变成了脏数据，
        /// 值和帧数相同时表示数据变为了脏数据
        /// </summary>
        uint DirtyFlag { get; set; }
        /// <summary>
        /// 使得数据变成脏数据的System的index,
        /// 值为max时表示从外部修改为脏数据(当外部改变后执行时）
        /// </summary>
        uint DirtySystemIndex { get; set; }
    }
}
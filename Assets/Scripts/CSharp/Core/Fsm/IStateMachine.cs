namespace SanXiao.Core
{
    public interface IStateMachine
    {
        void Update();
        void UpdateEndOfFrame();
    }
}
namespace SanXiao.Core
{
    public abstract class StateBase<T>
    {
        protected T Target;
        internal string Key;
        internal void BindTarget(T target)
        {
            Target = target;
        }
        public abstract void Enter(object[] parameters);
        public abstract void Update();
        public abstract void End();
    }
}
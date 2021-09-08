namespace Orcas.Graph.Core
{
    public interface IGraphEventHandler
    {
        void OnInterruput(GraphContext context);
        
        void OnRunGraph(GraphContext context);
    }
}
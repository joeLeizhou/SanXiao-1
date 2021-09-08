using Orcas.Graph.Core;
using Orcas.Graph.Variables;
using UnityEngine;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("测试(debug)", "创建Cube")]
    public class TestCreateCube : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public TestCreateCube()
        {

        }
#if UNITY_EDITOR
        public TestCreateCube(int id) : base(id)
        {
            Name = "CreateCube";
            Inputs = new Slot[2] { new Slot(id, true, "位置", new VVector3()), new Slot(id, true, "scale", new VVector3()) };
        }
#endif

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            GameObject gameObject = new GameObject("Cube");
            var mf = gameObject.AddComponent<MeshFilter>();
            var mr = gameObject.AddComponent<MeshRenderer>();
            var mesh = new Mesh();
            mf.sharedMesh = mesh;
            mesh.SetVertices(new System.Collections.Generic.List<Vector3>() {
                new Vector3(-0.5f,-0.5f,-0.5f),
            new Vector3(-0.5f,-0.5f,0.5f),
                new Vector3(-0.5f,0.5f,-0.5f),
                new Vector3(0.5f,-0.5f,-0.5f),
                new Vector3(-0.5f,0.5f,0.5f),
                new Vector3(0.5f,-0.5f,0.5f),
                new Vector3(0.5f,0.5f,-0.5f),
            new Vector3(0.5f,0.5f,0.5f)});
            mesh.SetTriangles(new int[36] { 0, 1, 2, 2, 1, 4, 0, 2, 3, 2, 6, 3, 2, 4, 6, 4, 7, 6, 1, 5, 4, 5, 7, 4, 0, 3, 1, 3, 5, 1, 6, 7, 3, 7, 5, 3 }, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            gameObject.transform.position = Inputs[0].GetFloat3Variable();
            gameObject.transform.localScale = Inputs[1].GetFloat3Variable();
            return frameCount;
        }
    }
}
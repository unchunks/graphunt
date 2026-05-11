using System.IO;
using System.Text.Json;

public class JsonStageRepository : IStageRepository
{
    private readonly string _filePath;

    public JsonStageRepository(string filePath = "Assets/Stages/stage_01.json")
    {
        _filePath = filePath;
    }

    public GraphModel Load()
    {
        if (!File.Exists(_filePath))
        {
            UnityEngine.Debug.LogError($"ステージファイルが見つかりません: {_filePath}");
            return new GraphModel();
        }

        string jsonString = File.ReadAllText(_filePath);
        StageData stage = JsonSerializer.Deserialize<StageData>(jsonString);
        return ConvertToGraphModel(stage);
    }

    // 注意: ここではグラフのデータのみを構築しており、Viewは処理されていない（ViewはGraphViewで生成）
    private GraphModel ConvertToGraphModel(StageData stage)
    {
        GraphModel graph = new GraphModel();

        // ノードの追加
        for (int i = 0; i < stage.NodeCount; i++)
        {
            NodeType type = (i == stage.GoalNode) ? NodeType.Goal : NodeType.Normal;
            graph.AddNode(new NodeData(i, type));
        }

        // エッジの追加
        foreach (var edge in stage.Edges)
        {
            graph.AddEdge(new EdgeData(edge[0], edge[1]));
        }

        // プレイヤーの初期位置設定
        graph.InitializePlayerPositions(stage.RabbitStart, stage.WolfAStart, stage.WolfBStart);
        return graph;
    }
}

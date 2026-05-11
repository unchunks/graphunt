using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum StageType
    {
        Stage_01,
        Stage_02,
        //Stage_03
    }

    [SerializeField] private TurnController _turnController;
    [SerializeField] private InputController _inputController;

    [Header("Stage Settings")]
    [Tooltip("プレイするステージを選択してください")]
    [SerializeField] private StageType _selectedStage = StageType.Stage_01;

    private void Awake()
    {
        RuleEngine rule = new RuleEngine();

        string stageFilePath = GetStageFilePath(_selectedStage);
        IStageRepository repo = new JsonStageRepository(stageFilePath);

        IPlayerInputStrategy rabbitStrategy = null;
        IPlayerInputStrategy wolfStrategy = null;

        // TODO: PVPやPVEなどモードに応じたプレイヤー入力戦略の初期化
        rabbitStrategy = new HumanInputStrategy(_inputController, rule);
        wolfStrategy = new HumanInputStrategy(_inputController, rule);

        _turnController.Initialize(rule, repo, rabbitStrategy, wolfStrategy);
    }

    /// <summary>
    /// 選択されたenumの値からJSONファイルのパスを生成する
    /// </summary>
    private string GetStageFilePath(StageType stageType)
    {
        string fileName = stageType.ToString().ToLower();
        return $"Assets/Stages/{fileName}.json";
    }
}

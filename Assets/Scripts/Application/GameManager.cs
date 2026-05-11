using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TurnController _turnController;
    [SerializeField] private InputController _inputController;

    private void Awake()
    {
        RuleEngine rule = new RuleEngine();
        IStageRepository repo = new JsonStageRepository();
        IPlayerInputStrategy rabbitStrategy = null;
        IPlayerInputStrategy wolfStrategy = null;

        // TODO: PVPやPVEなどモードに応じたプレイヤー入力戦略の初期化
        rabbitStrategy = new HumanInputStrategy(_inputController, rule);
        wolfStrategy = new HumanInputStrategy(_inputController, rule);

        _turnController.Initialize(rule, repo, rabbitStrategy, wolfStrategy);
    }
}

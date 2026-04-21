using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TurnController _turnController;

    private void Awake()
    {
        RuleEngine rule = new RuleEngine();
        IStageRepository repo = new JsonStageRepository();
        IPlayerInputStrategy rabbitStrategy = null;
        IPlayerInputStrategy wolfStrategy = null;

        // TODO: PVPやPVEなどモードに応じたプレイヤー入力戦略の初期化
        rabbitStrategy = new HumanInputStrategy();
        wolfStrategy = new HumanInputStrategy();

        _turnController.Initialize(rule, repo, rabbitStrategy, wolfStrategy);
    }
}

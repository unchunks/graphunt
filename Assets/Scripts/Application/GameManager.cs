using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    [SerializeField] private TurnController _turnController;

    private void Awake()
    {
        var graph = new GraphModel();
        var rule = new RuleEngine();
        IStageRepository repo = new JsonStageRepository();

        _turnController.Initialize(graph, rule, repo);
    }
}

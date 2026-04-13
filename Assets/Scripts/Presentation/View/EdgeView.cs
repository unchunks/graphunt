using System;
using UnityEngine;

public class EdgeView : MonoBehaviour
{
    public void Initialize(GraphModel model)
    {
        model.OnPlayerMoved += HandlePlayerMoved;
        model.OnEdgeAdded += HandleEdgeAdded;
        model.OnEdgeRemoved += HandleEdgeRemoved;
    }

    private void HandlePlayerMoved(PlayerID playerId, int to)
    {
        //if (playerId == _playerId && to == _to)
        //    PlayMoveAnimation(playerId, to);
    }
    private void HandleEdgeAdded(EdgeData edge)
    {
        //if (edge == _edge)
        //    PlayConnectAnimation(edge);
    }
    private void HandleEdgeRemoved(EdgeData edge)
    {
        //if (edge == _edge)
        //    PlayDisconnectAnimation(edge);
    }
}

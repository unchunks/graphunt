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
        if (playerId == _playerId && to == _to)
            PlayMoveAnimation(playerId, to);
    }
    private void HandleEdgeAdded(int from, int to)
    {
        if (from == _from && to == _to)
            PlayConnectAnimation(from, to);
    }
    private void HandleEdgeRemoved(int from, int to)
    {
        if (from == _from && to == _to)
            PlayDisconnectAnimation(from, to);
    }
}

public enum GameState
{
    Setup,      // ステージ読み込み中
    RabbitTurn, // ウサギの入力待ち
        //SelectingAction
        //MoveMode
        //DisconnectMode
        //ConnectMode
    WolfTurn,   // オオカミの入力待ち（またはAI思考中）
    Result,     // 決着・リザルト表示
    //Replay
}

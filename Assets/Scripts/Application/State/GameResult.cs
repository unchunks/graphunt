// 勝利結果の種類
public enum GameResult
{
    None,
    RabbitReachedGoal,   // ウサギがゴールに到達
    RabbitLooped,        // 千日手（ウサギ勝利）
    WolfCaught,          // オオカミがウサギを捕獲
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ゲーム全体の状態を管理するフラグ
    public static bool gameIsPaused; // 一時停止中か
    public static bool isReading;    // ノートやマップなど閲覧中か
    public static bool isAlive;      // プレイヤーが生存しているか

    // 最初に一度だけ呼ばれる
    void Start()
    {
        Application.targetFrameRate = 60; // FPS上限を設定

        gameIsPaused = false; // 初期状態：非ポーズ
        isAlive = true;       // プレイヤーは生きている
        isReading = false;    // 閲覧中ではない
        Time.timeScale = 1.0f; // ゲームの時間を通常通りに進行

    }
}

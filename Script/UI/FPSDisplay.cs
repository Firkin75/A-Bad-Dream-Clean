using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private Text fpsText;       // FPSを表示するTextコンポーネント
    private float deltaTime;    // フレーム間の経過時間（平滑化用）

    void Start()
    {
        Application.targetFrameRate = 60;    // フレームレート上限を60に設定
        fpsText = GetComponent<Text>();      // Textコンポーネント取得
    }

    void Update()
    {
        // ゲームが一時停止中・読書UI中・プレイヤー死亡時はカウント停止
        if (GameManager.gameIsPaused || GameManager.isReading || !GameManager.isAlive)
            return;

        else
        {
            // FPSの平滑化計算（Moving Average）
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;

            // FPSを整数にして表示
            fpsText.text = fps.ToString("F0") + " FPS";
        }
    }
}

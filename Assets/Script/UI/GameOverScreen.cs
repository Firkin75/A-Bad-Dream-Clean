using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    void Start()
    {
        // カーソルを表示し、ロックを解除する（ゲームオーバー画面ではUI操作が必要なため）
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ゲームを再開（直前のゲームプレイシーンを再読み込み）
    public void reStart()
    {
        // 最後にプレイしていたシーンのインデックスを取得（デフォルトは1）
        int lastSceneIndex = PlayerPrefs.GetInt("LastScene", 1);

        // そのシーンを読み込むことでゲームを再スタート
        SceneManager.LoadScene(lastSceneIndex);
    }

    // メインメニューに戻る（通常はタイトル画面）
    public void backToMenu()
    {
        // シーンインデックス0を読み込む（タイトル画面など）
        SceneManager.LoadScene(0);
    }

    // ゲームを終了（ビルド版でのみ有効）
    public void Quit()
    {
        // アプリケーションを終了（Unityエディタでは無効）
        Application.Quit();
    }
}

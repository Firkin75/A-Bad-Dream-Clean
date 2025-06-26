using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    void Start()
    {
        // カーソルを表示＆ロック解除
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ゲームを再開（シーン1を再読み込み）
    public void reStart()
    {
        SceneManager.LoadSceneAsync(1); // ゲームプレイシーンのインデックスが1
    }

    // メインメニューに戻る（シーン0）
    public void backToMenu()
    {
        SceneManager.LoadScene(0); // タイトル画面
    }

    // ゲーム終了（ビルド版で有効）
    public void Quit()
    {
        Application.Quit(); // エディタでは動作しません
    }
}

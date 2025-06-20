using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteScreen : MonoBehaviour
{
    // 初期化：カーソルを表示・ロック解除
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // メインメニュー（シーン0）に戻る
    public void backToMenu()
    {
        SceneManager.LoadScene(0);
    }

    // ゲームを終了する（ビルド版でのみ有効）
    public void exitTheGame()
    {
        Application.Quit();
    }
}

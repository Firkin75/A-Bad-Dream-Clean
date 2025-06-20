using Unity.VisualScripting;
using UnityEngine;

public class NoteMenu : MonoBehaviour
{
    private bool wasReading = false; // 直前の状態が「読書中」だったか

    void Update()
    {
        // ノートUIが表示されているかどうか
        bool isNoteActive = UIManager.Instance.noteUI.activeInHierarchy;

        // ノートを開いたときの処理
        if (isNoteActive && !wasReading)
        {
            Cursor.lockState = CursorLockMode.None; // カーソルを解放
            Cursor.visible = true;                 // カーソル表示
            Time.timeScale = 0f;                   // ゲーム停止
            GameManager.isReading = true;          // 読書フラグON
            wasReading = true;                     // 状態記録
        }
        // ノートを閉じたときの処理
        else if (!isNoteActive && wasReading)
        {
            Cursor.lockState = CursorLockMode.Locked; // カーソルをロック
            Cursor.visible = false;                   // カーソル非表示
            Time.timeScale = 1f;                      // ゲーム再開
            GameManager.isReading = false;            // 読書フラグOFF
            wasReading = false;                       // 状態記録
        }
    }

    // ボタンなどから呼び出す：ノートUIを閉じる
    public void CloseNoteUI()
    {
        UIManager.Instance.noteUI.SetActive(false);
    }
}

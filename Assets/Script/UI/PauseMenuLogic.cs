using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuLogic : MonoBehaviour
{
    // AudioMixer（ミキサー）への参照。音量調整に使用
    public AudioMixer audioMixer;

    // 音量調整用のスライダーUI
    public Slider volumeSlider;

    // 現在ポーズ状態かどうかのフラグ
    private bool isPaused = false;

    void Start()
    {
        // 保存されている音量を読み込み（デフォルト値は0dB）
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);

        // オーディオミキサーに音量を適用
        audioMixer.SetFloat("MasterVol", savedVolume);

        // スライダーにも値を反映（UI初期化）
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
    }

    void Update()
    {
        // 読書中または死亡中はポーズできないようにする
        if (!GameManager.isAlive || GameManager.isReading) return;

        // ESCキーが押されたらポーズのオン・オフを切り替える
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame(); // ポーズ解除
            else
                PauseGame();  // ポーズ開始
        }
    }

    // ゲームを最初からやり直す（タイトル画面へ遷移）
    public void reStart()
    {
        SceneManager.LoadSceneAsync(0); // シーン番号0を非同期で読み込み
    }

    // メインメニューへ戻る
    public void backToMenu()
    {
        SceneManager.LoadScene(0); // シーン番号0を同期的に読み込み
    }

    // ゲーム終了（ビルド版で有効。エディタでは反応しない）
    public void exitTheGame()
    {
        Application.Quit(); // アプリケーション終了
    }

    // ポーズ処理（ゲーム時間停止＋UI表示）
    public void PauseGame()
    {
        GameManager.gameIsPaused = true;                 // グローバルポーズフラグON
        UIManager.Instance.ShowPauseMenu(true);          // ポーズメニューを表示

        Time.timeScale = 0f;                             // ゲーム内時間を停止（物理・アニメ等）
        Cursor.lockState = CursorLockMode.None;          // カーソルを解放
        Cursor.visible = true;                           // カーソルを表示

        isPaused = true;                                 // 内部フラグ更新
    }

    // スライダー操作時に呼ばれる音量設定関数
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", volume);        // 即時反映
        PlayerPrefs.SetFloat("MasterVolume", volume);    // 永続保存
        PlayerPrefs.Save();
    }

    // ポーズ解除処理（ゲーム再開）
    public void ResumeGame()
    {
        UIManager.Instance.ShowPauseMenu(false);         // ポーズメニューを非表示
        Time.timeScale = 1f;                             // ゲーム内時間を再開

        Cursor.lockState = CursorLockMode.Locked;        // カーソルを画面中央に固定
        Cursor.visible = false;                          // カーソルを非表示

        GameManager.gameIsPaused = false;                // グローバルポーズフラグOFF
        isPaused = false;                                // 内部フラグ更新
    }
}

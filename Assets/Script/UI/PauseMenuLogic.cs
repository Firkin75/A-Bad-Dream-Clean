using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuLogic : MonoBehaviour
{
    public AudioMixer audioMixer;     // オーディオミキサーへの参照（音量調整用）
    public Slider volumeSlider;       // 音量調整スライダー

    private bool isPaused = false;    // ポーズ中かどうかのフラグ

    void Start()
    {
        // 保存された音量を取得（デフォルトは0 dB）
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        audioMixer.SetFloat("MasterVol", savedVolume);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
    }

    void Update()
    {
        // 読書中 or 死亡中はポーズ無効
        if (!GameManager.isAlive || GameManager.isReading) return;

        // ESCキーでポーズのオン/オフ切り替え
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // ゲームを最初からやり直す（シーン0を再読み込み）
    public void reStart()
    {
        SceneManager.LoadSceneAsync(0);
    }

    // メインメニューへ戻る（シーン0を読み込み）
    public void backToMenu()
    {
        SceneManager.LoadScene(0);
    }

    // ゲームを終了（ビルド版でのみ有効）
    public void exitTheGame()
    {
        Application.Quit();
    }

    // ポーズ処理
    public void PauseGame()
    {
        GameManager.gameIsPaused = true;
        UIManager.Instance.ShowPauseMenu(true); // ポーズメニューを表示

        Time.timeScale = 0f;                    // ゲーム時間停止
        Cursor.lockState = CursorLockMode.None; // カーソル解放
        Cursor.visible = true;

        isPaused = true;
    }

    // 音量設定と保存
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", volume);      // 即時反映
        PlayerPrefs.SetFloat("MasterVolume", volume);  // 保存
        PlayerPrefs.Save();
    }

    // ポーズ解除処理
    public void ResumeGame()
    {
        UIManager.Instance.ShowPauseMenu(false);   // メニューを非表示
        Time.timeScale = 1f;                       // ゲーム再開

        Cursor.lockState = CursorLockMode.Locked;  // カーソル固定
        Cursor.visible = false;

        GameManager.gameIsPaused = false;
        isPaused = false;
    }
}

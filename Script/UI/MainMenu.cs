using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // 各種メニュー画面のUI参照
    public GameObject settingMenu;  // 設定画面
    public GameObject mainMenu;     // メインメニュー
    public GameObject extra;        // エクストラ画面
    public AudioMixer audioMixer;   // オーディオミキサー
    public Slider volumeSlider;     // 音量スライダー

    // 最初に一度だけ呼ばれる
    void Start()
    {
        // 保存された音量設定を読み込み
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        volumeSlider.value = savedVolume;
        audioMixer.SetFloat("MasterVol", savedVolume);

        // フレームレート設定
        Application.targetFrameRate = 60;

        // メニュー表示状態初期化
        mainMenu.SetActive(true);
        settingMenu.SetActive(false);
        extra.SetActive(false);

        // カーソル表示＆ロック解除
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 「スタート」ボタンが押されたとき
    public void gameStart()
    {
        SceneManager.LoadScene(1); // ゲームシーン（インデックス1）を読み込む
        mainMenu.SetActive(false);
        settingMenu.SetActive(false);
        extra.SetActive(false);
    }

    // エクストラ画面へ移動
    public void toExtra()
    {
        extra.SetActive(true);
        mainMenu.SetActive(false);
        settingMenu.SetActive(false);
    }

    // 設定画面を表示
    public void ToSettingMenu()
    {
        settingMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    // ゲームを終了（ビルド版で有効）
    public void exitTheGame()
    {
        Application.Quit();
    }

    // メインメニューへ戻る
    public void backToMainMenu()
    {
        mainMenu.SetActive(true);
        settingMenu.SetActive(false);
        extra.SetActive(false);
    }

    // 音量を設定し、保存
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", volume);     // 音量をリアルタイムで設定
        PlayerPrefs.SetFloat("MasterVolume", volume); // 音量を保存
        PlayerPrefs.Save();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // シングルトンインスタンス

    [Header("UI Elements")]
    public GameObject weaponUI;       // 武器UI
    public GameObject keyDisplayUI;   // 鍵の取得UI
    public GameObject deadScrn;       // 死亡画面
    public GameObject pm;             // ポーズメニュー
    public GameObject noteUI;         // ノート表示UI
    public GameObject miniMap;        // ミニマップUI

    [Header("Tutorial UI")]
    public GameObject moveTutorial;       // 移動チュートリアル
    public GameObject interact;           // インタラクトヒントUI
    public GameObject combatTutorial;     // 戦闘チュートリアル

    void Awake()
    {
        // シングルトンの初期化処理
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // 各UIを初期状態で非表示に設定
        pm.SetActive(false);
        keyDisplayUI.SetActive(false);
        noteUI.SetActive(false);
        miniMap.SetActive(false);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ステージ1では移動チュートリアルを表示し、武器UIは非表示
        if (currentSceneIndex == 1)
        {
            ShowMovementTutorial(true);
            weaponUI.SetActive(false);
        }
        else
        {
            weaponUI.SetActive(true);
        }
    }

    // --------------------------------------------------
    // 他スクリプトから呼び出すUI制御メソッド群
    // --------------------------------------------------

    public void ShowWeaponUI(bool show)
    {
        weaponUI.SetActive(show); // 武器UI表示切替
    }

    public void ShowPauseMenu(bool show)
    {
        pm.SetActive(show); // ポーズメニュー表示切替
    }

    public void ShowMiniMap(bool show)
    {
        miniMap.SetActive(show); // ミニマップ表示切替
    }

    public void ShowNoteUI(bool show)
    {
        noteUI.SetActive(show); // ノートUI表示切替
    }

    public void ShowKeyUI(bool show)
    {
        keyDisplayUI.SetActive(show); // 鍵UI表示切替
    }

    public void ShowDeathScreen(bool show)
    {
        deadScrn.SetActive(show); // 死亡画面表示切替
    }

    public void ShowMovementTutorial(bool show)
    {
        Time.timeScale = 0f;                     // ゲーム停止
        GameManager.isReading = true;
        Cursor.lockState = CursorLockMode.None;  // カーソル解放
        Cursor.visible = true;
        moveTutorial.SetActive(true);            // 移動チュートリアル表示
    }

    public void CloseMovementTutorial()
    {
        Debug.Log("clicked");
        moveTutorial.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // カーソル固定
        Cursor.visible = false;
        GameManager.isReading = false;
        Time.timeScale = 1f;                      // ゲーム再開
    }

    public void ShowCombatTutorial(bool show)
    {
        Time.timeScale = 0f;
        GameManager.isReading = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        combatTutorial.SetActive(true);           // 戦闘チュートリアル表示
    }

    public void CloseCombatTutorial()
    {
        Debug.Log("clicked");
        combatTutorial.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.isReading = false;
        Time.timeScale = 1f;
    }

    public void ShowInteractUI(bool show)
    {
        interact.SetActive(show); // インタラクトUI表示切替
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // シングルトンインスタンス

    [Header("UI Elements")]
    public GameObject weaponUI;       // 冷匂UI
    public GameObject keyDisplayUI;   // �Iの函誼UI
    public GameObject deadScrn;       // 棒蘭鮫中
    public GameObject pm;             // ポ�`ズメニュ�`
    public GameObject noteUI;         // ノ�`ト燕幣UI
    public GameObject miniMap;        // ミニマップUI

    [Header("Tutorial UI")]
    public GameObject moveTutorial;       // 卞�咼船紿`トリアル
    public GameObject interact;           // インタラクトヒントUI
    public GameObject combatTutorial;     // �蜉Lチュ�`トリアル

    void Awake()
    {
        // シングルトンの兜豚晒�I尖
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
        // 光UIを兜豚彜�Bで掲燕幣に�O協
        pm.SetActive(false);
        keyDisplayUI.SetActive(false);
        noteUI.SetActive(false);
        miniMap.SetActive(false);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ステ�`ジ1では卞�咼船紿`トリアルを燕幣し、冷匂UIは掲燕幣
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
    // 麿スクリプトから柵び竃すUI崙囮メソッド蛤
    // --------------------------------------------------

    public void ShowWeaponUI(bool show)
    {
        weaponUI.SetActive(show); // 冷匂UI燕幣俳紋
    }

    public void ShowPauseMenu(bool show)
    {
        pm.SetActive(show); // ポ�`ズメニュ�`燕幣俳紋
    }

    public void ShowMiniMap(bool show)
    {
        miniMap.SetActive(show); // ミニマップ燕幣俳紋
    }

    public void ShowNoteUI(bool show)
    {
        noteUI.SetActive(show); // ノ�`トUI燕幣俳紋
    }

    public void ShowKeyUI(bool show)
    {
        keyDisplayUI.SetActive(show); // �IUI燕幣俳紋
    }

    public void ShowDeathScreen(bool show)
    {
        deadScrn.SetActive(show); // 棒蘭鮫中燕幣俳紋
    }

    public void ShowMovementTutorial(bool show)
    {
        Time.timeScale = 0f;                     // ゲ�`ム唯峭
        GameManager.isReading = true;
        Cursor.lockState = CursorLockMode.None;  // カ�`ソル盾慧
        Cursor.visible = true;
        moveTutorial.SetActive(true);            // 卞�咼船紿`トリアル燕幣
    }

    public void CloseMovementTutorial()
    {
        Debug.Log("clicked");
        moveTutorial.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // カ�`ソル耕協
        Cursor.visible = false;
        GameManager.isReading = false;
        Time.timeScale = 1f;                      // ゲ�`ム壅�_
    }

    public void ShowCombatTutorial(bool show)
    {
        Time.timeScale = 0f;
        GameManager.isReading = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        combatTutorial.SetActive(true);           // �蜉Lチュ�`トリアル燕幣
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
        interact.SetActive(show); // インタラクトUI燕幣俳紋
    }
}

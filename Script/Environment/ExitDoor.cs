using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour, IInteractable
{
    private AudioSource lockedSound;               // 鍵がない場合のサウンド
    private PlayerInventory playerInventory;       // プレイヤーの所持品情報

    void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>(); // プレイヤーのインベントリ取得
        lockedSound = GetComponent<AudioSource>();                  // AudioSource取得
    }

    public void Interact()
    {
        if (playerInventory.hasKey)
        {
            // 鍵を所持していれば次のシーンへ（シーン3）
            SceneManager.LoadScene(3);
        }
        else
        {
            // 鍵がない場合は音と警告メッセージを表示
            lockedSound.Play();
            MessageManager.Instance.ShowWarningMessage("A key is required");
        }
    }
}

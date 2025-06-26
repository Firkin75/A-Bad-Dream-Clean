using UnityEngine;

public class PCInteract : MonoBehaviour, IInteractable
{
    public bool isNote;        // ノート（繰り返し使用可）
    public bool isMap;         // マップ表示（1回のみ）
    public bool isSecurity;    // セキュリティキー取得端末（1回のみ）

    public GameObject enemySet;     // 交互后激活的敌人组
    public GameObject indicator;    // 提示UI（交互后隐藏）

    [Header("Security Light")]
    public Light securityLight;     // セキュリティライト（緑色に変化）

    private bool hasInteracted = false; // 一度限りの処理に使用（ノート以外）

    public void Interact()
    {
        // ノート以外は一度限り
        if (!isNote && hasInteracted) return;

        // 共通：音效播放
        MessageManager.Instance.PlayPickupSound();

        // -------- ノート --------（繰り返し可）
        if (isNote)
        {
            UIManager.Instance.ShowNoteUI(true);
            if (enemySet != null) enemySet.SetActive(true);
        }

        // -------- マップ --------（1回のみ）
        if (isMap && !hasInteracted)
        {
            UIManager.Instance.ShowMiniMap(true);
            MessageManager.Instance.ShowWarningMessage("You got the map for this area");
        }

        // -------- セキュリティキー --------（1回のみ）
        if (isSecurity && !hasInteracted)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.hasKey = true;
                    UIManager.Instance.ShowKeyUI(true);
                    MessageManager.Instance.ShowPickupMessage("The door is now unlocked");
                }
                else
                {
                    Debug.LogWarning("PlayerInventory コンポーネントが見つかりません！");
                }
            }
            else
            {
                Debug.LogWarning("プレイヤーが見つかりません！");
            }

            if (securityLight != null)
                securityLight.color = Color.green;
        }

        // 共通処理：指示アイコン非表示、敵出現
        if (indicator != null)
            indicator.SetActive(false);

        if (enemySet != null)
            enemySet.SetActive(true);

        // ノート以外なら一度限り
        if (!isNote)
            hasInteracted = true;
    }

    public bool IsInteractable()
    {
        // ノート以外は再使用不可
        return isNote || !hasInteracted;
    }
}

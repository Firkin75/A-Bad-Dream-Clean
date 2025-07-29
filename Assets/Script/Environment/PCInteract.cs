using UnityEngine;

// プレイヤーがインタラクト可能なPC端末（ノート、マップ、セキュリティ）の処理
public class PCInteract : MonoBehaviour, IInteractable
{
    public bool isNote;        // ノートかどうか（複数回使用可）
    public bool isMap;         // マップ端末かどうか（1回限り）
    public bool isSecurity;    // セキュリティキー端末かどうか（1回限り）

    public GameObject enemySet;     // インタラクト後に出現させる敵オブジェクト（任意）
    public GameObject indicator;    // プレイヤーへのUIインジケーター（インタラクト後に非表示）

    [Header("Security Light")]
    public Light securityLight;     // セキュリティキー取得後に緑に変えるライト

    private bool hasInteracted = false; // すでに使用されたか（ノート以外に適用）

    // インタラクト時の処理（プレイヤーが使用したときに呼ばれる）
    public void Interact()
    {
        // ノート以外は一度しか使用できない
        if (!isNote && hasInteracted) return;

        // 共通：効果音を再生（ピックアップ音など）
        MessageManager.Instance.PlayPickupSound();

        // -------- ノート処理（何度でも使用可能） --------
        if (isNote)
        {
            UIManager.Instance.ShowNoteUI(true);              // ノートUIを表示
            if (enemySet != null) enemySet.SetActive(true);   // 敵があれば出現
        }

        // -------- マップ取得処理（一度限り） --------
        if (isMap && !hasInteracted)
        {
            UIManager.Instance.ShowMiniMap(true);                             // マップUIを表示
            MessageManager.Instance.ShowWarningMessage("You got the map for this area"); // メッセージ表示
        }

        // -------- セキュリティキー取得処理（一度限り） --------
        if (isSecurity && !hasInteracted)
        {
            // プレイヤーオブジェクトを取得
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.hasKey = true;                                  // 鍵フラグをONに
                    UIManager.Instance.ShowKeyUI(true);                       // UI表示
                    MessageManager.Instance.ShowPickupMessage("The door is now unlocked"); // メッセージ表示
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
                securityLight.color = Color.green; // ライトを緑色に変更
        }

        // -------- 共通処理 --------

        // インジケーターUIを非表示（アイテム取得済みの演出）
        if (indicator != null)
            indicator.SetActive(false);

        // 敵を出現させる（任意設定）
        if (enemySet != null)
            enemySet.SetActive(true);

        // ノート以外は一度使用されたことにする
        if (!isNote)
            hasInteracted = true;
    }

    // インタラクト可能かどうかを返す
    public bool IsInteractable()
    {
        // ノートは常に使用可、それ以外は未使用時のみ
        return isNote || !hasInteracted;
    }
}

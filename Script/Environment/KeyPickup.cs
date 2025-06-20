using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject uiKeyDisplay;  // 鍵を取得したときに表示するUI
    public GameObject enemySet;      // 鍵取得後に出現させる敵

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーが触れた場合のみ反応
        if (other.CompareTag("Player"))
        {
            // 敵セットを出現させる
            if (enemySet != null)
            {
                enemySet.SetActive(true);
            }

            // プレイヤーに鍵を持たせる
            other.GetComponent<PlayerInventory>().hasKey = true;

            // UI表示（例：画面に鍵アイコンを表示）
            uiKeyDisplay.SetActive(true);

            // このオブジェクト（鍵）を削除
            Destroy(gameObject);
        }
    }
}

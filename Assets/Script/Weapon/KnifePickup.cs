using UnityEngine;

public class KnifePickup : MonoBehaviour
{
    public string weaponName = "Knife";     // 武器の名前（武器管理に登録する用）
    public int slotIndex = 0;               // 武器スロット番号
    public GameObject weaponUI;             // 武器UI（例：画面表示）
    public GameObject enemySet;             // ナイフ取得後に出現させる敵セット

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーが接触したか確認
        if (other.CompareTag("Player"))
        {
            weaponUI.SetActive(true); // 武器UIを表示

            // WeaponManagerのインスタンスを探す
            WeaponManager weaponManager = FindFirstObjectByType<WeaponManager>();
            if (weaponManager != null)
            {
                // 武器を登録
                weaponManager.PickupWeapon(weaponName, slotIndex);
                
                // 敵を出現させる
                if (enemySet != null)
                {
                    enemySet.SetActive(true);
                }

                // このオブジェクトを削除
                Destroy(gameObject);

                // メッセージ表示と効果音
                MessageManager.Instance.ShowPickupMessage("Picked up a knife");
                MessageManager.Instance.PlayPickupSound();

                // UI
                UIManager.Instance.ShowCombatTutorial(true);


            }
        }
    }
}

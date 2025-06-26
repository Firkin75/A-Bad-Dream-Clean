using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int amountOfHealth; // 回復される体力の量

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーと接触したか確認
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            // すでに体力が満タンなら何もしない
            if (PlayerHealth.currentHealth >= PlayerHealth.maxHealth)
            {
                return;
            }

            // 体力を回復し、メッセージを表示
            health.AddHealth(amountOfHealth);
            MessageManager.Instance.ShowPickupMessage("Picked up a medic kit");

            // 効果音を再生
            MessageManager.Instance.PlayPickupSound();

            // この回復アイテムを消去
            Destroy(gameObject);
        }
    }
}

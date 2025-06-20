using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private DoorScript doorScript; // ドアの制御スクリプト参照

    void Start()
    {
        // 親オブジェクトから DoorScript を取得
        doorScript = GetComponentInParent<DoorScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 敵が範囲に入ったらドアに通知
        if (other.CompareTag("Enemy"))
        {
            if (doorScript != null)
            {
                doorScript.OnEnemyEnter(); // ドアに「敵が接近」通知
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 敵が範囲から出たらドアに通知
        if (other.CompareTag("Enemy"))
        {
            if (doorScript != null)
            {
                doorScript.OnEnemyExit(); // ドアに「敵が離れた」通知
            }
        }
    }
}

using UnityEngine;

public class Shotgun : Weapon
{
    // 自動照準の視野角度（正面方向から左右何度まで敵を狙えるか）
    [SerializeField]
    private float autoAimAngle;

    // 武器の攻撃処理（親クラスWeaponのFireをオーバーライド）
    protected override void Fire()
    {
        // 攻撃アニメーションを再生（Animatorの"Fire"トリガーを設定）
        gunAnim.SetTrigger("Fire");

        // 武器の発射音を再生（命中の有無に関係なく、攻撃時に再生）
        WeaponSound.Play();

        // soundRangeの範囲内にいる敵を取得し、「銃声を聞いた」ことを通知する
        Collider[] enemyColliders = Physics.OverlapSphere(player.position, soundRange, enemyLayerMask);
        foreach (var enemyCollider in enemyColliders)
        {
            EnemyController ai = enemyCollider.GetComponent<EnemyController>();
            if (ai != null) ai.OnHeardGunshot(); // 敵AIにアグロ状態へ移行させる
        }

        // カメラの位置（origin）と水平方向のforwardベクトルを取得
        Vector3 origin = fpsCam.transform.position;
        Vector3 forward = fpsCam.transform.forward;
        forward.y = 0;             // 上下方向を無視し、水平方向のみに限定
        forward.Normalize();       // 単位ベクトルに正規化

        // 射程範囲（gunRange）内に存在する敵（enemyLayerMask）のコライダーを取得
        Collider[] allHits = Physics.OverlapSphere(origin, gunRange, enemyLayerMask);

        // 自動照準によってロックオンすべき最も近い敵を記録する変数
        Collider bestTarget = null;
        float bestDistance = float.MaxValue; // 初期値は最大距離

        // 検出された全ての敵コライダーをチェック
        foreach (var col in allHits)
        {
            // "Enemy"タグを持たないオブジェクトは無視
            if (!col.CompareTag("Enemy")) continue;

            // プレイヤーから敵の中心位置へのベクトルと距離を取得
            Vector3 toTarget = col.bounds.center - origin;
            float distance = toTarget.magnitude;

            // 水平面上での方向ベクトル（Y軸成分を無視）
            Vector3 toTargetFlat = toTarget;
            toTargetFlat.y = 0;

            // forward（視線方向）との角度を計算
            float angle = Vector3.Angle(forward, toTargetFlat);

            // autoAimAngleより外にいる敵は自動照準の対象外
            if (angle > autoAimAngle) continue;

            // プレイヤーと敵の間に障害物があるか確認（Triggerは無視）
            if (Physics.Raycast(origin, toTarget.normalized, out RaycastHit obstacleHit, distance, ~0, QueryTriggerInteraction.Ignore))
            {
                // 敵以外のオブジェクトで遮られている場合は無視
                if (!obstacleHit.collider.CompareTag("Enemy")) continue;
            }

            // 最も近い敵をbestTargetとして保存
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = col;
            }
        }

        // ロックオンされた敵がいればダメージ処理を実行
        if (bestTarget != null)
        {
            EnemyController enemy = bestTarget.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 敵にダメージを与える
                enemy.TakeDamage(gunDamage);

                // デバッグ用にログ出力
                Debug.Log("Auto-aim hit enemy: " + enemy.name);
            }
        }

        // 命中しない場合でも、発射音は既に再生されているため、追加処理は行わない
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Knife : Weapon
{
    // 敵に命中した時に再生される効果音（ヒット音）
    public AudioSource hitSound;

    // 攻撃処理（基底クラスWeaponのFireメソッドをオーバーライド）
    protected override void Fire()
    {
        // 攻撃アニメーションを再生（AnimatorのTriggerを使って"Fire"を実行）
        gunAnim.SetTrigger("Fire");

        // DOOM風の垂直オートエイム：
        // プレイヤーの照準は左右（水平）のみ動かせるが、
        // 攻撃判定は垂直方向も含めて自動で補正されるようにする

        // カメラ（プレイヤー視点）の位置を原点とする
        Vector3 origin = fpsCam.transform.position;

        // forwardベクトルを水平面に制限（上下方向は除外）
        Vector3 horizontalDirection = fpsCam.transform.forward;
        horizontalDirection.y = 0;             // Y成分を0にして上下方向を除去
        horizontalDirection.Normalize();       // 単位ベクトルに正規化

        // SphereCastAllを使って前方の攻撃判定を実行
        // 半径0.5、長さgunRangeの円柱状の範囲で敵を検出
        RaycastHit[] hits = Physics.SphereCastAll(origin, 0.5f, horizontalDirection, gunRange, enemyLayerMask);

        // 最も近く、かつ視線が通っている敵を記録する変数
        RaycastHit? bestTarget = null;
        float bestDistance = float.MaxValue;

        // ヒットしたすべてのオブジェクトを調査
        foreach (RaycastHit h in hits)
        {
            // タグが"Enemy"でないものは無視
            if (h.collider.CompareTag("Enemy"))
            {
                // 命中音（ヒット音）を先に再生することで打撃感を演出
                hitSound.Play();

                // プレイヤーから敵中心へのベクトルと距離を計算
                Vector3 toTarget = h.collider.bounds.center - origin;
                Ray rayToTarget = new Ray(origin, toTarget.normalized);
                float distanceToTarget = toTarget.magnitude;

                // プレイヤーと敵の間に障害物があるか確認（Triggerコライダーは無視）
                if (Physics.Raycast(rayToTarget, out RaycastHit obstacleHit, distanceToTarget, ~0, QueryTriggerInteraction.Ignore))
                {
                    // 敵以外の物体にぶつかっていた場合はスキップ
                    if (!obstacleHit.collider.CompareTag("Enemy"))
                    {
                        continue;
                    }
                }

                // 最も近い敵をbestTargetとして記録
                if (distanceToTarget < bestDistance)
                {
                    bestDistance = distanceToTarget;
                    bestTarget = h;
                }
            }
        }

        // ロックオンされた敵がいればダメージを与える
        if (bestTarget.HasValue)
        {
            // EnemyControllerを取得してダメージ処理を実行
            EnemyController enemy = bestTarget.Value.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(gunDamage); // ダメージ適用
                Debug.Log("Auto-aim hit enemy: " + enemy.name); // デバッグログ出力
            }
        }
        else
        {
            // 敵がいなかった場合は空振り音を再生（攻撃が外れた時のフィードバック）
            WeaponSound.Play();
            Debug.Log("No enemy hit by auto-aim.");
        }
    }
}

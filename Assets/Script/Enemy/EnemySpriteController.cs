using UnityEngine;

/// <summary>
/// 敵キャラのスプライトアニメーション制御用スクリプト（向きや反転の調整）
/// </summary>
public class EnemySpriteController : MonoBehaviour
{
    private Transform player;                        // プレイヤーのTransform参照
    private EnemyController enemyController;         // 敵AI制御スクリプトの参照
    private Animator animator;                       // アニメーション用Animator
    private SpriteRenderer spriteRenderer;           // スプライトの反転用SpriteRenderer

    void Start()
    {
        // プレイヤーのオブジェクトをタグから取得
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 各コンポーネントを取得
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<EnemyController>();
    }

    void Update()
    {
        // 敵の移動方向を取得
        Vector3 moveDirection = enemyController.MoveDirection;

        // スプライトアニメーションの向きを更新
        UpdateAnimation(moveDirection);
    }

    void LateUpdate()
    {
        // 常にプレイヤーの方向を向く（ビルボード処理）
        if (player != null)
        {
            transform.LookAt(player);
        }
    }

    /// <summary>
    /// アニメーションの向きやスプライトの反転を更新する処理
    /// </summary>
    void UpdateAnimation(Vector3 moveDirection)
    {
        // 敵の移動方向に基づく角度（XZ平面）
        float enemyAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        // プレイヤーへの方向ベクトルと角度
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float playerAngle = Mathf.Atan2(toPlayer.x, toPlayer.z) * Mathf.Rad2Deg;

        // 敵の向きとプレイヤーの位置の角度差を計算（0～360度に正規化）
        float angleDifference = (enemyAngle - playerAngle + 360f) % 360f;

        // 向きとスプライト反転の設定を取得
        int directionIndex;
        bool flipX;
        GetDirectionAndFlip(angleDifference, out directionIndex, out flipX);

        // アニメーションパラメータに設定
        animator.SetFloat("directionIndex", directionIndex);
        spriteRenderer.flipX = flipX;
    }

    /// <summary>
    /// 角度差に応じた方向インデックスと反転フラグの設定
    /// </summary>
    void GetDirectionAndFlip(float angleDifference, out int directionIndex, out bool flipX)
    {
        flipX = false;

        if (angleDifference >= 337.5f || angleDifference < 22.5f)
        {
            directionIndex = 0; // 正面
        }
        else if (angleDifference >= 22.5f && angleDifference < 67.5f)
        {
            directionIndex = 1; // 斜め前（左反転）
            flipX = true;
        }
        else if (angleDifference >= 67.5f && angleDifference < 112.5f)
        {
            directionIndex = 2; // 横（左反転）
            flipX = true;
        }
        else if (angleDifference >= 112.5f && angleDifference < 157.5f)
        {
            directionIndex = 3; // 斜め後ろ（左反転）
            flipX = true;
        }
        else if (angleDifference >= 157.5f && angleDifference < 202.5f)
        {
            directionIndex = 4; // 背面
        }
        else if (angleDifference >= 202.5f && angleDifference < 247.5f)
        {
            directionIndex = 3; // 斜め後ろ（右向き）
        }
        else if (angleDifference >= 247.5f && angleDifference < 292.5f)
        {
            directionIndex = 2; // 横（右向き）
        }
        else if (angleDifference >= 292.5f && angleDifference < 337.5f)
        {
            directionIndex = 1; // 斜め前（右向き）
        }
        else
        {
            directionIndex = 0; // デフォルト（正面）
        }
    }
}

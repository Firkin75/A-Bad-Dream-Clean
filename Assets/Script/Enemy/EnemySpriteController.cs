using UnityEngine;

/// <summary>
/// 敵キャラのスプライトアニメーション制御用スクリプト（プレイヤーの位置に応じて向きや反転を調整）
/// </summary>
public class EnemySpriteController : MonoBehaviour
{
    private Transform player;                        // プレイヤーのTransform参照
    private EnemyController enemyController;         // 敵の動作を制御するスクリプト
    private Animator animator;                       // アニメーション制御用のAnimator
    private SpriteRenderer spriteRenderer;           // スプライトの反転を行うためのSpriteRenderer

    void Start()
    {
        // プレイヤーのオブジェクトをタグ"Player"から取得
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 自身に付いている各コンポーネントを取得
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<EnemyController>();
    }

    void Update()
    {
        // 敵の移動方向を取得（EnemyControllerから受け取る）
        Vector3 moveDirection = enemyController.MoveDirection;

        // アニメーションとスプライト反転を更新
        UpdateAnimation(moveDirection);
    }

    void LateUpdate()
    {
        // 敵キャラが常にプレイヤーの方向を向くようにする（ビルボード処理）
        if (player != null)
        {
            transform.LookAt(player);
        }
    }

    /// <summary>
    /// アニメーションとスプライト反転設定を更新する処理
    /// </summary>
    void UpdateAnimation(Vector3 moveDirection)
    {
        // 敵の移動方向からXZ平面の角度を求める
        float enemyAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        // プレイヤーへの方向ベクトルと角度を計算
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float playerAngle = Mathf.Atan2(toPlayer.x, toPlayer.z) * Mathf.Rad2Deg;

        // プレイヤーと敵の向きの角度差を求める（0〜360度に正規化）
        float angleDifference = (enemyAngle - playerAngle + 360f) % 360f;

        // 角度差に応じた向き（directionIndex）と反転（flipX）を取得
        int directionIndex;
        bool flipX;
        GetDirectionAndFlip(angleDifference, out directionIndex, out flipX);

        // アニメーションとスプライトに反映
        animator.SetFloat("directionIndex", directionIndex);  // アニメーション用パラメータ
        spriteRenderer.flipX = flipX;                          // 左右反転設定
    }

    /// <summary>
    /// 角度差に基づいてアニメーションの向きとスプライト反転設定を決定
    /// </summary>
    void GetDirectionAndFlip(float angleDifference, out int directionIndex, out bool flipX)
    {
        flipX = false;

        // 0度付近（正面）→インデックス0
        if (angleDifference >= 337.5f || angleDifference < 22.5f)
        {
            directionIndex = 0; // 正面
        }
        // 22.5〜67.5度：斜め前（左側）、反転
        else if (angleDifference >= 22.5f && angleDifference < 67.5f)
        {
            directionIndex = 1; // 斜め前
            flipX = true;
        }
        // 67.5〜112.5度：左（完全横向き）、反転
        else if (angleDifference >= 67.5f && angleDifference < 112.5f)
        {
            directionIndex = 2; // 横
            flipX = true;
        }
        // 112.5〜157.5度：斜め後ろ（左）、反転
        else if (angleDifference >= 112.5f && angleDifference < 157.5f)
        {
            directionIndex = 3; // 斜め後ろ
            flipX = true;
        }
        // 157.5〜202.5度：背面
        else if (angleDifference >= 157.5f && angleDifference < 202.5f)
        {
            directionIndex = 4; // 背面
        }
        // 202.5〜247.5度：斜め後ろ（右）
        else if (angleDifference >= 202.5f && angleDifference < 247.5f)
        {
            directionIndex = 3; // 斜め後ろ
        }
        // 247.5〜292.5度：右（完全横向き）
        else if (angleDifference >= 247.5f && angleDifference < 292.5f)
        {
            directionIndex = 2; // 横
        }
        // 292.5〜337.5度：斜め前（右）
        else if (angleDifference >= 292.5f && angleDifference < 337.5f)
        {
            directionIndex = 1; // 斜め前
        }
        else
        {
            directionIndex = 0; // 万が一のときは正面に
        }
    }
}

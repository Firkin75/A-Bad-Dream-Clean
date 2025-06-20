using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Vector3 MoveDirection { get; private set; } // アニメーション用の移動方向
    public bool isDeaf;                                // 銃声を無視するか
    public AudioSource footStep;
    public LayerMask Player;
    public float health;

    public GameObject defaultDropItem;                 // 死亡時にドロップするアイテム
    public Transform[] patrolPoints;                   // 巡回ポイント
    private int currentPatrolIndex = 0;

    public int damage;
    public Transform firePoint;                        // 攻撃時のRay起点
    public Transform dropPoint;                        // ドロップ位置
    public AudioSource atkSound;

    public GameObject corpsePrefab;                    // 死亡後に出現する死体プレハブ
    public Transform corpseSpawnPoint;                 // 死体の出現位置（足元）

    private Transform player;
    private bool isAlive = true;
    private NavMeshAgent agent;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color flashColor = Color.red;               // 被弾時のフラッシュカラー
    public float flashDuration;
    public int flashCount = 1;
    public bool isAggro;                               // プレイヤーを発見したか

    private Coroutine chaseCoroutine;
    private float chaseUpdateInterval = 0.3f;

    // 視認・攻撃範囲
    public float sightRange, attackRange;
    protected bool playerInSightRange, playerInAttackRange;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found!");

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;  // 回転制御は無効（スプライト用）
        agent.updateUpAxis = false;    // 上方向の制御も無効

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70); // 敵の詰まりを軽減
    }

    void Update()
    {
        if (!isAlive) return;

        anim.SetBool("isMoving", agent.velocity.sqrMagnitude > 0.01f);

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        float distanceToPlayer = Vector3.Distance(firePoint.position, player.position);

        // 視認範囲チェック（Raycastで遮蔽物確認）
        playerInSightRange = false;
        if (distanceToPlayer <= sightRange)
        {
            int visionMask = LayerMask.GetMask("Player", "Environment");
            if (Physics.Raycast(firePoint.position, directionToPlayer, out RaycastHit hit, sightRange, visionMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerInSightRange = true;
                    isAggro = true;
                }
            }
        }

        // 攻撃範囲チェック
        playerInAttackRange = false;
        if (distanceToPlayer <= attackRange)
        {
            if (Physics.Raycast(firePoint.position, directionToPlayer, out RaycastHit attackHit, attackRange))
            {
                if (attackHit.collider.CompareTag("Player"))
                {
                    playerInAttackRange = true;
                }
            }
        }

        // AIステート制御
        if (isAggro && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
        else if (!isAggro)
        {
            Patrolling();
        }

        MoveDirection = agent.isStopped ? Vector3.zero : agent.velocity.normalized;
    }

    // 巡回行動
    protected virtual void Patrolling()
    {
        if (patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        anim.SetBool("isAttacking", false);
        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    // 追跡行動（コルーチンで遅延更新）
    protected virtual void ChasePlayer()
    {
        if (!isAlive) return;

        if (chaseCoroutine == null)
        {
            chaseCoroutine = StartCoroutine(ChasePlayerRoutine());
        }

        anim.SetBool("isAttacking", false);
    }

    // 攻撃行動
    private void AttackPlayer()
    {
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }

        agent.SetDestination(transform.position); // 停止

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);

        anim.SetBool("isAttacking", true);
    }

    // アニメーションイベントから呼ばれる攻撃処理
    public void raycastAttack()
    {
        Debug.Log("Attack triggered!");

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        if (Physics.Raycast(firePoint.position, directionToPlayer, out RaycastHit hit, attackRange))
        {
            atkSound?.Play();

            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    // 足音再生（アニメーションイベントから呼ばれる）
    public void playFootstepSound()
    {
        if (footStep != null)
        {
            footStep.Play();
        }
    }

    // ダメージ処理
    public virtual void TakeDamage(float damage)
    {
        if (!isAlive) return;

        StartCoroutine(FlashRed());
        health -= damage;

        isAggro = true;
        playerInSightRange = true;
        agent.isStopped = false;

        if (health <= 0)
        {
            Die();
        }
    }

    // 被弾フラッシュ（赤点滅）
    public IEnumerator FlashRed()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    // 死亡処理
    public virtual void Die()
    {
        if (!isAlive) return;

        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null) box.enabled = false;

        isAlive = false;
        agent.enabled = false;

        anim.SetBool("isAttacking", false);
        anim.SetBool("isMoving", false);
        anim.ResetTrigger("Die");
        anim.SetTrigger("Die");

        DropItem(); // アイテムドロップ処理
    }

    // 死体生成と敵オブジェクトの削除（アニメーションイベントから呼ばれる）
    public virtual void DestroyGameObj()
    {
        if (corpsePrefab != null && corpseSpawnPoint != null)
        {
            Instantiate(corpsePrefab, corpseSpawnPoint.position, corpseSpawnPoint.rotation);
        }
        Destroy(gameObject);
    }

    // アイテムドロップ
    private void DropItem()
    {
        if (defaultDropItem != null)
        {
            Vector3 dropPosition = dropPoint.position;
            GameObject dropItem = Instantiate(defaultDropItem, dropPosition, Quaternion.identity);

            // Rigidbody があれば物理演算で落とす
            Rigidbody rb = dropItem.GetComponent<Rigidbody>();
        }
    }

    // 銃声を聞いた時に呼ばれる
    public void OnHeardGunshot()
    {
        if (!isAlive) return;
        if (!isDeaf)
        {
            isAggro = true;
        }
    }

    // ドロップアイテム取得（拡張可能）
    public virtual GameObject GetDropItem()
    {
        return defaultDropItem;
    }

    // プレイヤー追跡コルーチン
    private IEnumerator ChasePlayerRoutine()
    {
        while (isAggro && !playerInAttackRange && isAlive)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            yield return new WaitForSeconds(chaseUpdateInterval);
        }

        chaseCoroutine = null; // 停止時にnullに戻す
    }

    // デバッグ用：視認/攻撃範囲の可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

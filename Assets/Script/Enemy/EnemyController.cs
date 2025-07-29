using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // アニメーション制御用：移動方向（外部から読み取り可能）
    public Vector3 MoveDirection { get; private set; }

    // 銃声に反応しないかどうか（例：ロボットやゾンビ等）
    public bool isDeaf;

    // 足音の再生用AudioSource（アニメーションイベント経由）
    public AudioSource footStep;

    // プレイヤー用レイヤーマスク（Raycastなどで使用）
    public LayerMask Player;

    // 現在の体力
    public float health;

    // 死亡時にドロップするアイテム
    public GameObject defaultDropItem;

    // 巡回ポイントの配列（Waypoints）
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    // 攻撃力
    public int damage;

    // 攻撃時のRay発射起点（敵の前方など）
    public Transform firePoint;

    // ドロップアイテムの出現位置
    public Transform dropPoint;

    // 攻撃時の効果音
    public AudioSource atkSound;

    // 死体生成用のプレハブ
    public GameObject corpsePrefab;

    // 死体の出現位置（通常は足元）
    public Transform corpseSpawnPoint;

    private Transform player;
    private bool isAlive = true;
    private NavMeshAgent agent;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // ダメージを受けた時の一時的なフラッシュ色
    public Color flashColor = Color.red;
    public float flashDuration;
    public int flashCount = 1;

    // プレイヤーを視認しているかどうか（戦闘状態に入ったか）
    public bool isAggro;

    private Coroutine chaseCoroutine;
    private float chaseUpdateInterval = 0.3f;

    // 視認・攻撃判定用の距離
    public float sightRange, attackRange;
    protected bool playerInSightRange, playerInAttackRange;

    void Start()
    {
        // プレイヤーオブジェクトの取得（Tagで検索）
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found!");

        // 各種コンポーネントの初期化
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // NavMeshAgentの回転と上下方向制御を無効（2Dスプライト向け）
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // 障害物回避を高品質設定にし、優先度もランダムで被りにくくする
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70);
    }

    void Update()
    {
        if (!isAlive) return;

        // 移動アニメーションON/OFF
        anim.SetBool("isMoving", agent.velocity.sqrMagnitude > 0.01f);

        // プレイヤーとの方向・距離を計算
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        float distanceToPlayer = Vector3.Distance(firePoint.position, player.position);

        // プレイヤーが視認範囲内か判定（Raycastで遮蔽物チェック）
        playerInSightRange = false;
        if (distanceToPlayer <= sightRange)
        {
            int visionMask = LayerMask.GetMask("Player", "Environment");
            if (Physics.Raycast(firePoint.position, directionToPlayer, out RaycastHit hit, sightRange, visionMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerInSightRange = true;
                    isAggro = true; // 戦闘状態に移行
                }
            }
        }

        // プレイヤーが攻撃範囲内か判定
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

        // AI状態遷移（巡回・追跡・攻撃）
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

        // 現在の移動方向を記録（アニメーション制御などに使用）
        MoveDirection = agent.isStopped ? Vector3.zero : agent.velocity.normalized;
    }

    // 巡回行動（パトロール）
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

    // プレイヤー追跡処理（コルーチンで間引き）
    protected virtual void ChasePlayer()
    {
        if (!isAlive) return;

        if (chaseCoroutine == null)
        {
            chaseCoroutine = StartCoroutine(ChasePlayerRoutine());
        }

        anim.SetBool("isAttacking", false);
    }

    // 攻撃アクション（移動停止＋アニメーション）
    private void AttackPlayer()
    {
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }

        // プレイヤーの方向に向く
        agent.SetDestination(transform.position);
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);

        anim.SetBool("isAttacking", true);
    }

    // アニメーションイベント経由で呼ばれる攻撃Raycast処理
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

    // ダメージを受けた時の処理
    public virtual void TakeDamage(float damage)
    {
        if (!isAlive) return;

        StartCoroutine(FlashRed());  // 赤く点滅
        health -= damage;

        isAggro = true;
        playerInSightRange = true;
        agent.isStopped = false;

        if (health <= 0)
        {
            Die();
        }
    }

    // 被弾時に赤く点滅させるコルーチン
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

    // 死亡処理（HP0到達時）
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

    // 死体プレハブの生成と自身の削除（アニメーションイベントから呼ばれる）
    public virtual void DestroyGameObj()
    {
        if (corpsePrefab != null && corpseSpawnPoint != null)
        {
            Instantiate(corpsePrefab, corpseSpawnPoint.position, corpseSpawnPoint.rotation);
        }
        Destroy(gameObject);
    }

    // アイテムドロップ処理
    private void DropItem()
    {
        if (defaultDropItem != null)
        {
            Vector3 dropPosition = dropPoint.position;
            GameObject dropItem = Instantiate(defaultDropItem, dropPosition, Quaternion.identity);

            // Rigidbodyがあれば物理演算で落とす（未使用ならそのまま）
            Rigidbody rb = dropItem.GetComponent<Rigidbody>();
        }
    }

    // 銃声を聞いた時に呼ばれる（遠くにいても反応）
    public void OnHeardGunshot()
    {
        if (!isAlive) return;
        if (!isDeaf)
        {
            isAggro = true;
        }
    }

    // ドロップアイテムを取得する（オーバーライド可能）
    public virtual GameObject GetDropItem()
    {
        return defaultDropItem;
    }

    // プレイヤー追跡コルーチン（一定間隔で目的地を更新）
    private IEnumerator ChasePlayerRoutine()
    {
        while (isAggro && !playerInAttackRange && isAlive)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            yield return new WaitForSeconds(chaseUpdateInterval);
        }

        // 追跡終了時はコルーチン参照をクリア
        chaseCoroutine = null;
    }

    // デバッグ表示：視認範囲と攻撃範囲をSceneビューに描画
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

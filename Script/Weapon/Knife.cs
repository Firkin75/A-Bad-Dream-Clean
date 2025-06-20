using UnityEngine;
using UnityEngine.UI;

public class Knife : MonoBehaviour
{
    public float gunDamage = 1;           // 1回の攻撃で与えるダメージ
    public float gunRange = 2;            // 攻撃可能な距離
    public float fireRate = 2f;           // 1秒あたりの攻撃回数（クールダウン）
    public float soundRange = 2;          // 敵に聞こえる範囲（サウンド半径）

    public AudioSource hitSound;          // 敵に命中した時の音
    public AudioSource knifeSound;        // 空振り時の音
    public Camera fpsCam;                 // FPS視点のカメラ
    public LayerMask enemyLayerMask;      // 敵検出用レイヤー

    private Animator gunAnim;             // ナイフのアニメーター
    private Transform player;             // プレイヤーのTransform
    private float nextTimeToFire = 0;     // 次の攻撃可能時間

    void Start()
    {
        gunAnim = GetComponent<Animator>();

        // プレイヤーオブジェクトをタグで探す
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found! 'Player'タグが設定されているか確認してください。");
        }
    }

    void Update()
    {
        if (GameManager.isReading || GameManager.gameIsPaused || !GameManager.isAlive) return;

        // 左クリックで攻撃（クールダウン中でなければ）
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Fire();
        }
    }

    // 攻撃処理
    void Fire()
    {
        gunAnim.SetTrigger("Fire"); // アニメーション再生

        // サウンド範囲内の敵に気づかせる
        Collider[] enemyColliders = Physics.OverlapSphere(player.position, soundRange, enemyLayerMask);
        foreach (var enemyCollider in enemyColliders)
        {
            EnemyController ai = enemyCollider.GetComponent<EnemyController>();
            if (ai != null)
            {
                ai.OnHeardGunshot(); // 銃声（ナイフでも）を聞いた敵をアグロ状態に
            }
        }

        // DOOM風の垂直オートエイム：プレイヤーの照準は水平方向のみで、垂直方向は自動で命中判定
        Vector3 origin = fpsCam.transform.position;
        Vector3 horizontalDirection = fpsCam.transform.forward;
        horizontalDirection.y = 0;
        horizontalDirection.Normalize();

        // SphereCastで複数ヒット検出
        RaycastHit[] hits = Physics.SphereCastAll(origin, 0.5f, horizontalDirection, gunRange, enemyLayerMask);

        RaycastHit? bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (RaycastHit h in hits)
        {
            if (h.collider.CompareTag("Enemy"))
            {
                hitSound.Play(); // ヒット音を先に鳴らす（実際は後で判定）

                Vector3 toTarget = h.collider.bounds.center - origin;
                Ray rayToTarget = new Ray(origin, toTarget.normalized);
                float distanceToTarget = toTarget.magnitude;

                // 障害物チェック（Trigger無視）
                if (Physics.Raycast(rayToTarget, out RaycastHit obstacleHit, distanceToTarget, ~0, QueryTriggerInteraction.Ignore))
                {
                    if (!obstacleHit.collider.CompareTag("Enemy"))
                    {
                        continue; // 敵以外で遮られているならスキップ
                    }
                }

                // 視線が通っている敵として採用（最も近い対象を優先）
                if (distanceToTarget < bestDistance)
                {
                    bestDistance = distanceToTarget;
                    bestTarget = h;
                }
            }
        }

        // ダメージ処理
        if (bestTarget.HasValue)
        {
            EnemyController enemy = bestTarget.Value.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(gunDamage);
                Debug.Log("Auto-aim hit enemy: " + enemy.name);
            }
        }
        else
        {
            knifeSound.Play(); // 空振り
            Debug.Log("No enemy hit by auto-aim.");
        }
    }
}

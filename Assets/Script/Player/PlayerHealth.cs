using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // プレイヤーが生きているかどうかの状態フラグ
    public bool isPlayerAlive = true;

    // プレイヤーの最大HP
    public static int maxHealth = 100;

    // 現在のHP（グローバルに共有されるstatic変数）
    public static int currentHealth;

    // ダメージを受けた時に画面に赤くフラッシュするオーバーレイUI
    public Image redScreenOverlay;

    // 赤フラッシュが完全に消えるまでの時間（秒）
    public float fadeDuration = 1.5f;

    // HPを表示するUIテキスト
    public Text hpText;

    // 死亡時に非表示にするUI（HPや装備表示など）
    public GameObject ui;

    // プレイヤーがダメージを受けたときの効果音
    public AudioSource hitAudio;

    // プレイヤーが死亡したときの効果音
    public AudioSource dieSound;

    // 被弾音が連続して鳴りすぎないようにするためのクールダウン時間
    [SerializeField]
    private float hitSoundCooldown = 2f;

    // 最後に被弾音を鳴らした時刻（Time.time基準）
    private float lastHitTime = -1f;

    // ゲーム開始時にHPを初期化
    void Start()
    {
        currentHealth = maxHealth;
    }

    // 毎フレーム呼び出される処理
    void Update()
    {
        // 現在のHPをUIに表示（毎フレーム更新）
        hpText.text = currentHealth.ToString();
    }

    // ダメージを受けたときに呼び出す処理
    public void TakeDamage(int damage)
    {
        int remainingDamage = damage;

        // 被弾音を鳴らす条件（前回再生から一定時間経っているか）
        if (Time.time - lastHitTime >= hitSoundCooldown)
        {
            hitAudio.Play();             // 効果音再生
            lastHitTime = Time.time;     // 最終再生時間を更新
        }

        // HPを減らす処理
        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;

            // HPが0以下になったら死亡処理へ
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    // 回復処理（最大HPを超えないように制限）
    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    // 死亡処理（HPが0以下になったときに実行）
    private void Die()
    {
        Debug.Log("Player has died.");
        dieSound.Play();                // 死亡効果音を再生

        GameManager.isAlive = false;   // ゲーム全体の生死状態を更新
        ui.SetActive(false);           // UIを非表示にする
        Time.timeScale = 0f;           // ゲーム全体を一時停止（ポーズ）

        // 死亡演出（赤フェードとGameOver画面への遷移）を開始
        StartCoroutine(FadeToRedWhilePaused());
    }

    // 死亡時：赤画面にフェード → 数秒待機 → GameOverシーンへ遷移
    private IEnumerator FadeToRedWhilePaused()
    {
        float timer = 0f;
        float fadeDuration = 2f;

        // フェードの開始色（透明）と終了色（真っ赤）
        Color startColor = new Color(1, 0, 0, 0);
        Color endColor = new Color(1, 0, 0, 1);

        // 赤色フェードを徐々に進行（Time.timeScale=0でも動くようにunscaledDeltaTime使用）
        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            redScreenOverlay.color = Color.Lerp(startColor, endColor, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // 最終的な色を真っ赤に固定
        redScreenOverlay.color = endColor;

        // 少し赤画面を維持してから遷移（演出用）
        float waitTime = 1f;
        float waitTimer = 0f;
        while (waitTimer < waitTime)
        {
            waitTimer += Time.unscaledDeltaTime;
            yield return null;
        }

        // 一時停止を解除
        Time.timeScale = 1f;

        // 現在のシーン番号を保存（リトライ用）
        PlayerPrefs.SetInt("LastScene", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();

        // GameOver画面に遷移
        SceneManager.LoadScene("GameOverScreen");
    }
}

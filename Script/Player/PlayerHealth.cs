using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public bool isPlayerAlive = true;              // プレイヤーが生きているかどうか
    public static int maxHealth = 100;             // 最大HP
    public static int currentHealth;               // 現在のHP（グローバル共有）

    public Image redScreenOverlay;                 // ダメージ時の赤フラッシュ
    public float fadeDuration = 1.5f;              // 赤画面のフェード時間

    public Text hpText;                            // HP表示UI
    public GameObject ui;                          // 死亡時に非表示にするUI
    public AudioSource hitAudio;                   // 被弾サウンド
    public AudioSource dieSound;                   // 死亡サウンド

    [SerializeField]
    private float hitSoundCooldown = 2f;           // 被弾音のクールダウン
    private float lastHitTime = -1f;               // 最後に被弾音を鳴らした時間

    void Start()
    {
        currentHealth = maxHealth;                 // HP初期化
    }

    void Update()
    {
        // HPテキスト更新（毎フレーム）
        hpText.text = currentHealth.ToString();
    }

    // ダメージ処理
    public void TakeDamage(int damage)
    {
        int remainingDamage = damage;

        // 被弾音のクールダウン判定
        if (Time.time - lastHitTime >= hitSoundCooldown)
        {
            hitAudio.Play();
            lastHitTime = Time.time;
        }

        // HPを減らす
        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;

            // 死亡判定
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    // 回復処理
    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    // 死亡処理
    private void Die()
    {
        Debug.Log("Player has died.");
        dieSound.Play();

        GameManager.isAlive = false;   // グローバル状態を更新
        ui.SetActive(false);           // ゲームUIを非表示
        Time.timeScale = 0f;           // ゲーム停止

        StartCoroutine(FadeToRedWhilePaused());
    }

    // 死亡時：赤フェード→一定時間後にGameOver画面へ
    private IEnumerator FadeToRedWhilePaused()
    {
        float timer = 0f;
        float fadeDuration = 2f;
        Color startColor = new Color(1, 0, 0, 0);
        Color endColor = new Color(1, 0, 0, 1);

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            redScreenOverlay.color = Color.Lerp(startColor, endColor, t);
            timer += Time.unscaledDeltaTime; // ポーズ中でも進む時間
            yield return null;
        }

        redScreenOverlay.color = endColor;

        // 赤画面を少し維持
        float waitTime = 1f;
        float waitTimer = 0f;
        while (waitTimer < waitTime)
        {
            waitTimer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f; // ゲームを再開
        SceneManager.LoadScene(2); // GameOverシーンへ遷移
    }
}

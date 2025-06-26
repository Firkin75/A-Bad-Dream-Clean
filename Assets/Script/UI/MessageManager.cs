using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; } // シングルトンインスタンス

    [Header("UI Components")]
    public Text pickupText;    // 例：右下に「〇〇を取得した」と表示
    public Text warningText;   // 例：画面中央に「ドアがロックされている」と表示

    [Header("Display Settings")]
    public float pickupDuration = 2f;   // 取得メッセージの表示時間
    public float warningDuration = 2f;  // 警告メッセージの表示時間

    [Header("SFX")]
    public AudioSource pickupAudioSource; // 取得時の効果音

    private Coroutine pickupCoroutine;
    private Coroutine warningCoroutine;

    void Awake()
    {
        // シングルトンの設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        pickupText.text = "";   // 初期化：取得メッセージを空に
        warningText.text = "";  // 初期化：警告メッセージを空に
    }

    // 取得メッセージを表示
    public void ShowPickupMessage(string message)
    {
        if (pickupCoroutine != null)
        {
            StopCoroutine(pickupCoroutine); // 既存のコルーチンを停止
        }
        pickupCoroutine = StartCoroutine(ShowTemporaryMessage(pickupText, message, pickupDuration));
    }

    // 警告メッセージを表示
    public void ShowWarningMessage(string message)
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine); // 既存のコルーチンを停止
        }
        warningCoroutine = StartCoroutine(ShowTemporaryMessage(warningText, message, warningDuration));
    }

    // 取得時の効果音を再生
    public void PlayPickupSound()
    {
        pickupAudioSource.Play();
    }

    // 一定時間メッセージを表示し、非表示にする共通処理
    private IEnumerator ShowTemporaryMessage(Text textComponent, string message, float duration)
    {
        textComponent.text = message;
        yield return new WaitForSecondsRealtime(duration); // 指定秒数待機（ゲーム停止中もカウント）
        textComponent.text = "";
    }
}

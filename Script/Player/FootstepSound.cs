using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioSource footstepAudioSource; // 足音のAudioSource
    public float minSpeed;                  // 足音が鳴るための最小移動速度
    public float footstepDelay;             // 足音の間隔（秒）

    private CharacterController characterController;
    private Vector3 previousPosition;       // 前フレームの位置
    private float currentSpeed;             // 現在の移動速度
    private float timeSinceLastFootstep;    // 前回の足音からの経過時間

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (footstepAudioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!"); // AudioSourceが未設定の場合エラー
        }

        previousPosition = transform.position;         // 初期位置を記録
        timeSinceLastFootstep = footstepDelay;         // タイマー初期化
    }

    void Update()
    {
        // 現在の移動速度を計算
        Vector3 positionDelta = transform.position - previousPosition;
        currentSpeed = positionDelta.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        // 接地状態でかつ速度が一定以上の場合、足音を再生
        if (characterController.isGrounded && currentSpeed > minSpeed)
        {
            timeSinceLastFootstep += Time.deltaTime;

            if (timeSinceLastFootstep >= footstepDelay)
            {
                PlayFootstep();                  // 足音を再生
                timeSinceLastFootstep = 0f;      // タイマーリセット
            }
        }
        else
        {
            // 移動していない、または空中の場合、足音を停止
            if (footstepAudioSource.isPlaying)
            {
                return;
            }
        }
    }

    // 足音を再生（すでに再生中でなければ）
    void PlayFootstep()
    {
        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Play();
        }
    }
}

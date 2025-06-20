using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 4f;              // プレイヤーの移動速度
    public float mouseSensitivity = 100f;     // マウス感度（視点移動）
    public Slider sensitivitySlider;          // UIスライダー（感度調整用）

    private CharacterController controller;   // CharacterControllerコンポーネント
    private Transform cameraTransform;        // メインカメラ参照
    private Vector3 velocity;                 // 重力用の速度ベクトル
    private float xRotation = 0f;             // 垂直方向の視点回転
    private float gravity = 9.81f;            // 重力値
    private const string SENSITIVITY_KEY = "MouseSensitivity"; // PlayerPrefsのキー

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;


        // 保存された感度設定を読み込む（なければデフォルト100）
        mouseSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 100f);

        // スライダーがあればUIにも反映
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = mouseSensitivity;
        }
    }

    void Update()
    {
        // 読書中、ポーズ中、または死亡中は操作不可
        if (GameManager.isReading || GameManager.gameIsPaused || !GameManager.isAlive)
        {
            return;
        }

        // ----------- プレイヤー移動処理 -----------
        float moveX = Input.GetAxis("Horizontal"); // A/Dキー or ←→
        float moveZ = Input.GetAxis("Vertical");   // W/Sキー or ↑↓

        // 視点方向に基づく移動ベクトル
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 移動を適用
        controller.Move(move * walkSpeed * Time.deltaTime);

        // 重力を適用
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ----------- マウス視点操作処理 -----------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // 水平方向のみ回転（垂直は固定）
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // 感度をスライダーから設定・保存する
    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value); // 設定を保存
        PlayerPrefs.Save();                           // 即座に保存を反映
    }
}

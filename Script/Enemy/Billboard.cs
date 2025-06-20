using UnityEngine;

/// <summary>
/// 常にカメラの方向を向くビルボード処理
/// </summary>
public class Billboard : MonoBehaviour
{
    private Transform cameraTransform; // メインカメラのTransformへの参照

    void Start()
    {
        // メインカメラのTransformを取得
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            // カメラが見つからない場合はエラーメッセージを出力
            Debug.LogError("Billboard: メインカメラが見つかりません！");
        }
    }

    void LateUpdate()
    {
        // カメラが未設定の場合は処理を行わない
        if (cameraTransform == null) return;

        // カメラの位置とこのオブジェクトの位置の差分ベクトルを計算（＝カメラの方向）
        Vector3 direction = cameraTransform.position - transform.position;

        // Y軸の回転を固定（縦方向の追従を防ぐ、横方向のみ回転）
        direction.y = 0;

        // カメラの方向に向くようにオブジェクトを回転
        transform.rotation = Quaternion.LookRotation(direction);

        // オブジェクトがカメラに背を向けないように180度回転（スプライトが逆になるのを防ぐ）
        transform.Rotate(0, 180f, 0);
    }
}

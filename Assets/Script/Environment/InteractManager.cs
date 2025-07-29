using UnityEngine;

// インタラクト（プレイヤーが使用できる物体）を管理するクラス
public class InteractManager : MonoBehaviour
{
    public Transform InteractorSource;   // インタラクト起点（通常はカメラなどの視線方向）
    public float InteractRange = 3f;     // インタラクト可能な距離（単位：メートル）

    private IInteractable currentInteractable;  // 現在注目しているインタラクト対象（Eキーで使用）

    void Start()
    {
        // 0.2秒ごとにインタラクト可能なオブジェクトを探索
        InvokeRepeating(nameof(CheckForInteractable), 0f, 0.2f);
    }

    void Update()
    {
        // 注目中のインタラクト対象が存在し、Eキーが押されたら使用処理を呼び出す
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact(); // 対象に対してインタラクト実行
        }
    }

    // インタラクト可能なオブジェクトをRaycastで検出する処理
    void CheckForInteractable()
    {
        // カメラ（視線）から前方に向かってRayを飛ばす
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        // Rayが何かに当たった場合の処理
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
        {
            // 当たったオブジェクトに IInteractable インターフェースがあるか確認
            if (hitInfo.collider.TryGetComponent(out IInteractable interactObject))
            {
                // 対象が現在インタラクト可能かどうか判定
                if (interactObject.IsInteractable())
                {
                    currentInteractable = interactObject;              // 有効な対象を記憶
                    UIManager.Instance.ShowInteractUI(true);           // インタラクトUI表示（例：Eキーアイコン）
                    return;
                }
            }
        }

        // 対象がいない、または無効だった場合、UIを非表示＆リセット
        currentInteractable = null;
        UIManager.Instance.ShowInteractUI(false);
    }
}

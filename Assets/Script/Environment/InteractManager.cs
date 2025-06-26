using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public Transform InteractorSource;   // プレイヤーの視線（例：カメラ）
    public float InteractRange = 3f;     // インタラクト可能な最大距離

    private IInteractable currentInteractable;  // 現在注目しているインタラクト対象

    void Start()
    {
        // 0.2秒ごとにインタラクト可能なオブジェクトをチェック
        InvokeRepeating(nameof(CheckForInteractable), 0f, 0.2f);
    }

    void Update()
    {
        // インタラクト対象があり、Eキーが押された場合、Interactを実行
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    void CheckForInteractable()
    {
        // 視線方向にレイを飛ばす
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
        {
            // ヒットしたオブジェクトに IInteractable が付いているか確認
            if (hitInfo.collider.TryGetComponent(out IInteractable interactObject))
            {
                // 対象が現在インタラクト可能か判定
                if (interactObject.IsInteractable()) 
                {
                    currentInteractable = interactObject;
                    UIManager.Instance.ShowInteractUI(true);  // UIを表示
                    return;
                }
            }
        }

        // インタラクトできるものがなければ、対象とUIをリセット
        currentInteractable = null;
        UIManager.Instance.ShowInteractUI(false);
    }
}

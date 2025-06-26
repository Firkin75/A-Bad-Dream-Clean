using UnityEngine;

public class DoorScript : MonoBehaviour, IInteractable
{
    public GameObject enemySet;            // 扉を開けたとき出現する敵セット
    public AudioSource doorSound;          // 開閉時のサウンド
    public AudioSource lockedSound;        // 鍵が必要な場合のロック音
    public bool requiresKey = false;       // 鍵が必要かどうか

    [SerializeField] private float moveSpeed = 2f;             // 扉の開閉速度
    [SerializeField] private float autoCloseDelay = 5f;        // 自動で閉まるまでの時間
    [SerializeField] private Vector3 openOffset = new Vector3(3, 0, 0); // 扉の開く距離

    private Transform doorTransform;        // 実際に動かす扉オブジェクト
    private Vector3 closedPosition;         // 閉まっている位置
    private Vector3 openPosition;           // 開いている位置
    private PlayerInventory playerInventory;
    private bool isOpen = false;            // 扉が開いているかどうか
    private bool isEnemyInRange = false;    // 敵が付近にいるかどうか
    private float lastOpenTime = -999f;     // 最後に開いた時間（自動閉鎖用）

    void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();

        Transform parent = transform.parent;
        if (parent != null)
        {
            doorTransform = parent.Find("Door"); // "Door"という名前の子を探す
        }

        if (doorTransform == null)
        {
            Debug.LogWarning("Door object not found under parent.");
            return;
        }

        closedPosition = doorTransform.position;
        openPosition = closedPosition + doorTransform.right * openOffset.magnitude;
    }

    void Update()
    {
        if (doorTransform == null) return;

        // 一定時間後に自動で閉まる処理（敵がいないときのみ）
        if (isOpen && !isEnemyInRange && Time.time - lastOpenTime >= autoCloseDelay)
        {
            CloseDoor();
        }

        // 現在の目標位置へ移動（滑らかな開閉）
        Vector3 target = isOpen ? openPosition : closedPosition;
        doorTransform.position = Vector3.MoveTowards(doorTransform.position, target, moveSpeed * Time.deltaTime);
    }

    // プレイヤーがEキーでインタラクトしたときの処理
    public void Interact()
    {
        if (requiresKey)
        {
            if (playerInventory.hasKey)
            {
                OpenDoor();
                if (enemySet != null) enemySet.SetActive(true);
            }
            else
            {
                if (lockedSound != null) lockedSound.Play();
                MessageManager.Instance.ShowWarningMessage("A key is required");
                Debug.Log("Key Required!");
            }
        }
        else
        {
            OpenDoor();
            if (enemySet != null) enemySet.SetActive(true);
        }
    }

    // 扉を開く処理
    private void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        lastOpenTime = Time.time;

        if (doorSound != null) doorSound.Play();
    }

    // 扉を閉じる処理
    private void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;

        if (doorSound != null) doorSound.Play();
    }

    // 敵が扉のセンサーに入った時
    public void OnEnemyEnter()
    {
        isEnemyInRange = true;
        OpenDoor();
    }

    // 敵が扉のセンサーから離れた時
    public void OnEnemyExit()
    {
        isEnemyInRange = false;
    }
}

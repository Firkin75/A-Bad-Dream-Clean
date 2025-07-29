using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasKey = false; // 鍵を所持しているかどうか

    void Start()
    {
        hasKey = false; // ゲーム開始時は鍵を持っていない
    }
}

using UnityEngine;

public class FloatingItems : MonoBehaviour
{
    public float amplitude;  // 上下移動の高さ（振幅）
    public float frequency;  // 上下移動の速さ（周波数）

    private Vector3 startPos; // 初期位置の記録

    void Start()
    {
        startPos = transform.position; // 初期位置を記録
    }

    void Update()
    {
        // 時間に応じてY座標を上下させる
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;

        // 元の位置にY方向のオフセットを加えて更新
        transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
    }
}

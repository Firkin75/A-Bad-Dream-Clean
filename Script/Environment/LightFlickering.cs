using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    public float minTime;  // 次の切り替えまでの最小時間
    public float maxTime;  // 次の切り替えまでの最大時間

    private float timer;   // タイマー
    private Light lightOBJ; // 対象のライトコンポーネント

    void Start()
    {
        // Lightコンポーネント取得
        lightOBJ = GetComponent<Light>();
        // 最初のランダムタイマーを設定
        timer = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        LightFlicker(); // 毎フレーム点滅チェック
    }

    void LightFlicker()
    {
        // タイマーを減らす
        if (timer > 0)
            timer -= Time.deltaTime;

        // 時間切れになったらライトのON/OFFを切り替え、次のタイマーを設定
        if (timer <= 0)
        {
            lightOBJ.enabled = !lightOBJ.enabled;
            timer = Random.Range(minTime, maxTime);
        }
    }
}

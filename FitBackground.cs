using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
    public Camera targetCamera; // Main CameraをInspectorで割り当て
    public float depth = 5f;    // 背景をカメラの前(手前/奥)どこに置くか。Z

    void Start()
    {
        Fit();
    }

    void Fit()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        var sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        // カメラの見えてる高さ・幅（ワールド座標単位）
        float camHeight = targetCamera.orthographicSize * 2f; // 縦の長さ
        float camWidth = camHeight * targetCamera.aspect;     // 横の長さ

        // スプライトのサイズ（ワールド単位）
        // sprite.bounds.size は、現在のPixels Per Unitを考慮した大きさ
        Vector2 spriteSize = sr.sprite.bounds.size;

        // どれくらい拡大すればカメラの幅・高さを埋められるか？
        float scaleX = camWidth / spriteSize.x;
        float scaleY = camHeight / spriteSize.y;

        // ちょうど画面を覆いたいので、XとYそれぞれ合わせる
        transform.localScale = new Vector3(scaleX, scaleY, 1f);

        // カメラの中心に背景を配置（XY同期）
        transform.position = new Vector3(
            targetCamera.transform.position.x,
            targetCamera.transform.position.y,
            targetCamera.transform.position.z + depth // カメラより奥に置く
        );
    }
}

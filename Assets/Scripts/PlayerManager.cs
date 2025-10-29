using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    // --- インスペクタから調整する項目 ---
    public float moveSpeed = 7f;
    public float jumpForce = 10f;

    // ★★★ 1. ジョイスティックの参照を追加 ★★★
    public Joystick joystick;

    [Header("接地判定の設定")]
    [SerializeField]
    private float groundCheckRadius = 0.3f;
    [SerializeField]
    private float groundCheckOffsetY = -0.8f;
    [SerializeField]
    private float groundCastDistance = 0.5f;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("ジャンプの設定")]
    public int maxJumps = 2;
    private int currentJumps = 0;

    // --- 内部で使う変数 ---
    private Rigidbody2D rigidbody2D;
    private float moveInputX; // OnMove (キーボード/ゲームパッド) からの入力
    private bool isGrounded;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        // キーボード/ゲームパッドの入力を保存
        moveInputX = value.Get<Vector2>().x;
    }

    // スペースキーとタッチの両方から呼ばれる共通のジャンプ処理
    public void TryJump()
    {
        // "かつ" 現在のジャンプ回数が最大回数未満なら
        if (currentJumps < maxJumps)
        {
            // (現在のX速度は維持し、Y速度だけをjumpForceにする)
            rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, jumpForce);

            // ジャンプカウンターを 1 増やす
            currentJumps++;
        }
    }

    public void OnJump(InputValue value)
    {
        // ★★★ 2. OnJumpの中身を書き換える ★★★
        // もしボタンが押された瞬間なら
        if (value.isPressed)
        {
            // 新しく作ったTryJump()を呼ぶ
            TryJump();
        }
    }

    void FixedUpdate()
    {
        // --- 接地判定 ---
        Vector2 castPosition = (Vector2)transform.position + new Vector2(0, groundCheckOffsetY);
        RaycastHit2D hit = Physics2D.CircleCast(
            castPosition,
            groundCheckRadius,
            Vector2.down,
            groundCastDistance,
            groundLayer
        );
        isGrounded = hit.collider != null;

        // --- ★★★ 3段ジャンプ修正 ★★★ ---
        // 接地していて、"かつ" 落下中（または静止）の時だけカウンターをリセット
        if (isGrounded && rigidbody2D.linearVelocity.y <= 0.01f) // 0.01f は誤差を考慮
        {
            currentJumps = 0;
        }

        // --- ★★★ 2. ジョイスティックとキーボード入力を統合 ★★★ ---
        // 最終的に使用する水平入力を格納する変数
        float finalMoveInput = moveInputX; // デフォルトはキーボード入力

        // もしジョイスティックが設定されていて、かつ操作されている場合
        if (joystick != null && joystick.Horizontal != 0)
        {
            // ジョイスティックの入力を優先する
            finalMoveInput = joystick.Horizontal;
        }
        // --- (統合完了) ---


        // --- 移動処理 ---
        // (★ finalMoveInput を使って移動)
        // Y速度はジャンプ処理（OnJump）か物理演算（重力）に任せる
        rigidbody2D.linearVelocity = new Vector2(finalMoveInput * moveSpeed, rigidbody2D.linearVelocity.y);

        // --- 向きの変更処理 ---
        // (★ finalMoveInput を使って向きを変更)
        if (finalMoveInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (finalMoveInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // (おまけ) 判定範囲をSceneビューで見えるようにする
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 castPosition = (Vector2)transform.position + new Vector2(0, groundCheckOffsetY);
        Vector2 endPosition = castPosition + (Vector2.down * groundCastDistance);
        Gizmos.DrawWireSphere(castPosition, groundCheckRadius);
        Gizmos.DrawWireSphere(endPosition, groundCheckRadius);
    }
}




//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerManager : MonoBehaviour
//{
//    // --- インスペクタから調整する項目 ---
//    public float moveSpeed = 7f;
//    public float jumpForce = 10f;

//    [Header("接地判定の設定")]
//    [SerializeField]
//    private float groundCheckRadius = 0.3f;
//    [SerializeField]
//    private float groundCheckOffsetY = -0.8f;
//    [SerializeField]
//    private float groundCastDistance = 0.5f;
//    [SerializeField]
//    private LayerMask groundLayer;

//    [Header("ジャンプの設定")]
//    public int maxJumps = 2;
//    private int currentJumps = 0;

//    // --- 内部で使う変数 ---
//    private Rigidbody2D rigidbody2D;
//    private float moveInputX;
//    private bool isGrounded;

//    void Start()
//    {
//        rigidbody2D = GetComponent<Rigidbody2D>();
//    }

//    public void OnMove(InputValue value)
//    {
//        moveInputX = value.Get<Vector2>().x;
//    }

//    public void OnJump(InputValue value)
//    {
//        // もしボタンが押された瞬間で、"かつ" 現在のジャンプ回数が最大回数未満なら
//        if (value.isPressed && currentJumps < maxJumps)
//        {
//            // --- ★★★ ジャンプ力修正 ★★★ ---
//            // AddForceの代わりにVelocityを直接設定し、毎回同じ力で飛ぶようにする
//            // (現在のX速度は維持し、Y速度だけをjumpForceにする)
//            rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, jumpForce);

//            // ジャンプカウンターを 1 増やす
//            currentJumps++;
//        }
//    }

//    void FixedUpdate()
//    {
//        // --- 接地判定 ---
//        Vector2 castPosition = (Vector2)transform.position + new Vector2(0, groundCheckOffsetY);
//        RaycastHit2D hit = Physics2D.CircleCast(
//            castPosition,
//            groundCheckRadius,
//            Vector2.down,
//            groundCastDistance,
//            groundLayer
//        );
//        isGrounded = hit.collider != null;

//        // --- ★★★ 3段ジャンプ修正 ★★★ ---
//        // 接地していて、"かつ" 落下中（または静止）の時だけカウンターをリセット
//        if (isGrounded && rigidbody2D.linearVelocity.y <= 0.01f) // 0.01f は誤差を考慮
//        {
//            currentJumps = 0;
//        }

//        // --- 移動処理 ---
//        // Y速度はジャンプ処理（OnJump）か物理演算（重力）に任せる
//        rigidbody2D.linearVelocity = new Vector2(moveInputX * moveSpeed, rigidbody2D.linearVelocity.y);

//        // --- 向きの変更処理 ---
//        if (moveInputX > 0)
//        {
//            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
//        }
//        else if (moveInputX < 0)
//        {
//            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
//        }
//    }

//    // (おまけ) 判定範囲をSceneビューで見えるようにする
//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Vector2 castPosition = (Vector2)transform.position + new Vector2(0, groundCheckOffsetY);
//        Vector2 endPosition = castPosition + (Vector2.down * groundCastDistance);
//        Gizmos.DrawWireSphere(castPosition, groundCheckRadius);
//        Gizmos.DrawWireSphere(endPosition, groundCheckRadius);
//    }
//}
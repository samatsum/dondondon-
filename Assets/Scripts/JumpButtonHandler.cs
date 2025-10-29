using UnityEngine;
using UnityEngine.EventSystems; // UIイベントを使うために必要

// IPointerDownHandler を実装すると、UIがタッチされた瞬間を検知できる
public class JumpButtonHandler : MonoBehaviour, IPointerDownHandler
{
    // プレイヤーの参照をインスペクターから設定する
    public PlayerManager playerManager;

    // このUIオブジェクトがタッチされた時に呼ばれる
    public void OnPointerDown(PointerEventData eventData)
    {
        // プレイヤーが設定されていれば、ジャンプ処理を呼ぶ
        if (playerManager != null)
        {
            playerManager.TryJump(); // PlayerManager側で新しく作るTryJump()を呼ぶ
        }
    }
}
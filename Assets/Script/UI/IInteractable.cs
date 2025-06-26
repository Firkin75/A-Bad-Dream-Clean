// インタラクト可能なオブジェクトに共通するインターフェース
public interface IInteractable
{
    // プレイヤーが「E」キーなどで呼び出す操作
    void Interact();

    // 現在インタラクト可能かどうか（デフォルトは true）
    bool IsInteractable() => true;
}

public static class GLOBAL
{
    #region Constants

    public const int MAX_OPER_TILE_COUNT = 256;
    public const int MAX_SWAP_TILE_COUNT = 32;
    public const int MAX_FIREWALL_COUNT = 16;
    public const int MAX_OBSERVER_COUNT = 16;
    public const int MAX_BOSSLASER_COUNT = 32;

    #endregion

    #region Enums

    public enum Character
    {
        None = -1,
        Zero, Pixel,
        Parser,
    }

    public enum Operation
    {
        None = -1,
        Add, Subtract, Multiply, Divide,
        Equal, NotEqual, Greater, Less,
        Portal
    }

    //�������߰�
    public enum ObjectID
    {
        None = -1,
        OperationTile, SwapTile,
        Firewall, Observer, BossLaser
    }

    //�����߰�
    public enum SceneID
    {
        None = -1,
        Title, Office, Matrix,
        Boss
    }

    public enum EventID
    {
        None = -1,
        PlayerMove, PlayerDie,
        NextStage
    }

    public enum AudioChannel
    {
        None = -1,
        BGM, SFX, UI,
    }

    public enum BGM_ID
    {
        None = -1,
        Title, Matrix,
    }

    public enum LoopType
    {
        None, All, Single,
    }

    public enum TraverseType
    {
        None, Linear, Random,
    }

    public enum SFX_ID
    {
        None = -1,
        PlayerMove, PlayerRespawn, PlayerHoldBox, PlayerReleaseBox,
        ObserverDetect,
        EnterPortal, OutPortal,
        ParserAttack, LaserExplosion,
    }

    public enum UI_SFX_ID
    {
        None = -1,
        StartButtonClick, ButtonClick,
        PanelOpen, PanelClose,
        DialogTyping,
    }

    #endregion
}
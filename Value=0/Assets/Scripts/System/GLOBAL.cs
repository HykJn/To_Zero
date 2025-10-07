public static class GLOBAL
{
    #region Constants

    public const int MAX_OPER_TILE_COUNT = 256;
    public const int MAX_SWAP_TILE_COUNT = 256;
    public const int MAX_FIREWALL_COUNT = 256;
    public const int MAX_OBSERVER_COUNT = 128;

    #endregion

    #region Enums

    public enum Character
    {
        None,
        Zero, Pixel,
        Parser,
    }

    public enum Operation
    {
        None,
        Add, Subtract, Multiply, Divide,
        Equal, NotEqual, Greater, Less
    }

    public enum ObjectID
    {
        None,
        OperationTile, SwapTile,
        Firewall, Observer,
    }

    public enum SceneID
    {
        None,
        Title, Office, Matrix,
    }

    public enum EventID
    {
        None,
        PlayerMove, PlayerDie,
        NextStage
    }

    public enum AudioChannel
    {
        None,
        BGM, SFX, UI,
    }

    public enum BGM_ID
    {
        None,
        Title, Office, Matrix,
        BattleParser,
        Ending,
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
        None,
        PlayerMove, PlayerRespawn, PlayerHoldBox, PlayerReleaseBox,
        ObserverDetect,
        ParserAttack, LaserExplosion,
        EnterPortal, OutPortal,
    }

    public enum UI_SFX_ID
    {
        None,
        StartButtonClick, ButtonClick,
        PanelOpen, PanelClose,
        DialogTyping,
    }

    #endregion
}
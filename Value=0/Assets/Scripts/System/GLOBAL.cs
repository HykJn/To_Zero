public static class GLOBAL
{
    #region Constants

    public const int MAX_TILE_COUNT = 1024;
    public const int MAX_WALL_COUNT = 512;
    public const int MAX_FIREWALL_COUNT = 512;
    public const int MAX_OBSERVER_COUNT = 256;

    #endregion

    #region Enums

    public enum Character
    {
        None,
        Zero, Pixel,
        Parser
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
        Tile, Wall, Firewall, Observer,
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

    public enum BGM_ID
    {
        None,
        Title, Office, Matrix,
        BattleParser,
        Ending,
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
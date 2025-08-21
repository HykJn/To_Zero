public static class GlobalDefines
{
    public const int MAX_TILE_COUNT = 1024;
    public const int MAX_WALL_COUNT = 512;
    public const int MAX_BOX_COUNT = 512;
    public const int MAX_DRONE_COUNT = 256;

    public enum Character
    {
        Value, System, Unknown
    }


    public enum TileType
    {
        None, Start, Portal,

        // Basic arithmetic operations
        Add, Sub, Mul, Div,

        // Conditional operations
        Equal, Not, Greater, Less,
    }

    public enum ObjectID
    {
        None,
        Tile, Wall, Box,
        Drone
    }

    public enum EventID
    {
        PlayerMove, NextStage,
        PlayerDieByDrone,
        PlayerDieByMoves,
        PlayerDieBySystem,
    }

    public enum BGM_ID
    {
        Title, InGame, BossRoom, Ending,
    }

    public enum SFX_ID
    {
        PlayerMove, PlayerRespawn, PlayerHoldBox, PlayerReleaseBox,
        DroneDetect,
        SystemAttack, SystemDie,
        PortalIn, PortalOut
    }

    public enum UI_SFX_ID
    {
        StartButtonClick,
        ButtonClick,
        PanelOpen, PanelClose,
        NextDialog,
    }

    public enum SceneID
    {
        Title, InGame,
    }
}
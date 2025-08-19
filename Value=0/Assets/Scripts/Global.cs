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
    OperationTile, SwapTile, Wall, Box,
    Drone
}

public enum EventID
{
    PlayerMove, NextStage,
    PlayerDieByDrone,
    PlayerDieByMoves,
    PlayerDieBySystem,
}

public enum BGMID
{
    Title, InGame, BossRoom, Ending,
}
public enum SFXID
{
    PlayerMove, PlayerRespawn, PlayerHoldBox, PlayerUnholdBox,
    DroneDetect,
    SystemAttack, SystemDie,
    PortalIn, PortalOut
}
public enum UISFXID
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
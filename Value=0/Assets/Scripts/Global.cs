public enum Operator
{
    None,
    // Basic arithmetic operations
    Add, Sub, Mul, Div,
    // Conditional operation
    Equal, Not, Greater, Less,
    Portal
}

public enum ObjectID
{
    None,
    OperationTile, SwapTile, Wall, Box,
    Drone
}

public enum EventID
{
    PlayerMove, PlayerDie, NextStage,
}

public enum BGMID
{
    Title, InGame, BossRoom, Ending,
}
public enum SFXID
{
    PlayerMove, PlayerDie, PlayerRespawn, PlayerRestart, PlayerHoldBox, PlayerUnholdBox,
    DroneMove, DroneDetect,
    SystemAttack, SystemDie,
    StageTransition
}
public enum UISFXID
{
    StartButtonClick,
    ButtonClick, ButtonHover,
    PanelOpen, PanelClose,
    NextDialog,
}
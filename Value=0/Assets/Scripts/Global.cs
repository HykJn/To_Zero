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
    OperationTile, SwapTile, Wall,
    Box,
    Portal,
}

public enum EventID
{
    PlayerMove, PlayerDie, NextStage,
}

public enum BGMID { }
public enum SFXID { }
public enum UISFXID { }
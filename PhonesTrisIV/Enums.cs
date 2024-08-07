using System;
namespace PhonesTrisIV
{
    public enum PieceType
    {
        I = 0,
        O = 1,
        T = 2,
        L = 3,
        J = 4,
        S = 5,
        Z = 6,
    }
    public enum Direction
    {
        DOWN,
        LEFT,
        RIGHT,
    }
    public enum ButtonTypes
    {
        MOVE_LEFT = 0,
        MOVE_RIGHT = 1,
        ROT_LEFT = 2,
        ROT_RIGHT= 3,
    }
    public enum StateType
    {
        PLAY,
        MENU
    }
    public enum RotationDirection
    {
        CW = 90,
        CCW = -90,
        NONE = 0
    }
    public enum Rotation
    {
        ROT0,
        ROT90,
        ROT180,
        ROT270,
        NONE
    }
    public enum TileType
    {
        IOT = 0,
        JS = 1,
        LZ = 2,
        GRAY = 3,
        BLANK = 4,
    }
    public enum ControlType
    {
        LEFT = 0,
        RIGHT = 1,
        DOWN = 2,
        CCW = 3,
        CW = 4,
        PAUSE = 5,
        QUIT = 6
    }
    public enum EventType
    {
        KEY_UP = 0,
        KEY_DOWN = 1,
        SPECIAL = 2
    }
    public enum MoveBlockage
    {
        NONE,
        BOARD,
        PIECE,
    }
}


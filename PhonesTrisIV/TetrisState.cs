using System;
namespace PhonesTrisIV
{
    public enum TetrisState
    {
        SPAWN_DELAY = 0,
        SPAWN = 1,
        PLAY = 2,
        CHECK_CLEAR = 3,
        CLEAR = 4,
        DIE = 5,
        GAME_OVER = 6
    }
}


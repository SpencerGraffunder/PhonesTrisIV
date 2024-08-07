using System;
using System.Collections.Generic;

namespace PhonesTrisIV
{
    public class PlayerInput
    {
        List<Event> Events;
        bool NewGame = false;
        public int StartingLevel { get; private set; }
        public int PlayerCount { get; private set; }
        bool Pause = false;
        bool Resume = false;
        int PlayerNumber;
        public PlayerInput(int playerNumber)
        {
            PlayerNumber = playerNumber;
            StartingLevel = 5;
            PlayerCount = 2;
        }
    }
}

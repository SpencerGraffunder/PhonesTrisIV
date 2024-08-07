using System;
using System.Collections.Generic;

namespace PhonesTrisIV
{
    public class Player
    {
        public int PlayerNumber { get; private set; }

        // piece given
        public Piece ActivePiece;
        public Piece NextPiece;
        public PieceType? NextPieceType = null;

        // clear/spawn
        public int ClearAnimationCounter = 0;
        public int SpawnDelayCounter = 0;
        public int SpawnDelayThreshold = 100;
        public bool DoneClearing = true;

        // piece movement
        public int FallCounter = 0;
        public int DasCounter = 0;
        public int DasThreshold = 0;
        public int DownCounter = 0;

        // controls
        public bool[] ButtonStates;
        public Button[] Buttons;

        // state
        public TetrisState State = TetrisState.SPAWN;
        public List<ClearingLine> LinesToClear;

        public int SpawnColumn;

        public Player(int playerNumber)
        {
            this.PlayerNumber = playerNumber;
            ButtonStates = new bool[4];
        }

        public void UpdateButtons(List<Tile[]> board, Players[] players)
        {
            if (Buttons[(int)ButtonTypes.ROT_LEFT].WasJustPressed()) // ccw
            {
                if (ActivePiece != null && ActivePiece.CanRotate(board, players, RotationDirection.CCW))
                {
                    ActivePiece.Rotate(RotationDirection.CCW);
                }
            }
            if (Buttons[(int)ButtonTypes.ROT_RIGHT].WasJustPressed()) // cw
            {
                if (ActivePiece != null && ActivePiece.CanRotate(board, players, RotationDirection.CW))
                {
                    ActivePiece.Rotate(RotationDirection.CW);
                }
            }
            if (Buttons[(int)ButtonTypes.MOVE_LEFT].WasJustPressed())
            {
                ButtonStates[(int)ButtonTypes.MOVE_LEFT] = true;
                DasThreshold = 0;
                DasCounter = 0;
                ButtonStates[(int)ButtonTypes.MOVE_RIGHT] = false;
            }
            if (Buttons[(int)ButtonTypes.MOVE_RIGHT].WasJustPressed())
            {
                ButtonStates[(int)ButtonTypes.MOVE_RIGHT] = true;
                DasThreshold = 0;
                DasCounter = 0;
                ButtonStates[(int)ButtonTypes.MOVE_LEFT] = false;
            }
            //if (Buttons[p * 4 + (int)ButtonTypes.MOVE_DOWN].WasJustPressed())
            //{
            //    buttonStates[(int)ButtonTypes.MOVE_DOWN] = true;
            //    DownCounter = 0;
            //}

            if (Buttons[(int)ButtonTypes.MOVE_LEFT].WasJustReleased())
            {
                ButtonStates[(int)ButtonTypes.MOVE_LEFT] = false;
            }
            if (Buttons[(int)ButtonTypes.MOVE_RIGHT].WasJustReleased())
            {
                ButtonStates[(int)ButtonTypes.MOVE_RIGHT] = false;
            }
            //if (Buttons[p * 4 + (int)ButtonTypes.MOVE_DOWN].WasJustReleased())
            //{
            //    buttonStates[(int)ButtonTypes.MOVE_DOWN] = false;
            //}
        }
    }
}

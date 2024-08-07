using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace PhonesTrisIV
{

    class PlayState : State
    {
        private readonly Dictionary<int, int> FALL_DELAY_VALUES;
        private readonly Dictionary<int, int[]> DAS_VALUES; 
        private const int BOARD_HEIGHT_BUFFER = 2;
        private readonly Dictionary<int, int> SCORING_VALUES;

        Texture2D LeftRotateButtonTexture;
        Texture2D RightArrowButtonTexture;
        Texture2D RightRotateButtonTexture;
        Texture2D LeftArrowButtonTexture;

        int BoardWidth = 10;
        readonly int BoardHeight = 20;
        List<Tile[]> Board = new List<Tile[]>();
        Player[] Players;
        int CurrentLevel = 0;
        bool GameOver = false;
        int PlayerCount = 2;
        bool IsPaused = false;
        int FallThreshold;
        int LinesCleared = 0;
        int DieCounter = 0;
        int FallCounter = 0;
        bool TimeToMove = false;
        int Score = 0;
        bool TimeToRotate = false;
        List<ClearingLine> ClearingLines = new List<ClearingLine>();
        bool ClearFlag = false;
        bool HasLeveledUp = false;
        Dictionary<TileType, Texture2D> TileSurfaces = new Dictionary<TileType, Texture2D>();
        int w;
        int h;
        //Button[] MahButtons = new Button[11];
        double TileSize;
        double centeringOffset;
        int maxButtonWidth;
        int maxButtonHeight;
        int ButtonDisplaySize;
        int[] ButtonXs = new int[4];
        int[] ButtonYs = new int[2];

        public PlayState()
        {
            w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            BoardWidth = (4 * PlayerCount) + 6;
            TileSize = h / BoardWidth;

            // display stuff
            centeringOffset = (w - (TileSize * BoardHeight)) / 2;
            maxButtonWidth = h / 4;
            maxButtonHeight = (int)((w - BoardHeight * TileSize) / 2);
            ButtonDisplaySize = Math.Min(maxButtonHeight, maxButtonWidth) / 2;

            for (int i = 0; i < 4; i++)
            {
                ButtonXs[i] = maxButtonWidth / 2 + maxButtonWidth * i;
            }
            ButtonYs[0] = maxButtonHeight / 2;
            ButtonYs[1] = maxButtonHeight + (int)(BoardHeight * TileSize) + maxButtonHeight / 2;

            FALL_DELAY_VALUES = new Dictionary<int, int>()
            {
                [0] = 48,
                [1] = 43,
                [2] = 38,
                [3] = 33,
                [4] = 28,
                [5] = 23,
                [6] = 18,
                [7] = 13,
                [8] = 8,
                [9] = 6,
                [10] = 5,
                [13] = 4,
                [16] = 3,
                [19] = 2,
                [29] = 1
            };

            DAS_VALUES = new Dictionary<int, int[]>()
            {
                [0] = new int[] { 3-1, 8-1 },
                [1] = new int[] { 6-1, 16-1 },
            };

            SCORING_VALUES = new Dictionary<int, int>
            {
                [0] = 0,
                [1] = 40,
                [2] = 100,
                [3] = 300,
                [4] = 1200,
            };

            Reset(2, 5);
        }

        private int GetFallThreshold()
        {
            for (int i = CurrentLevel; i >= 0; i--)
            {
                if (FALL_DELAY_VALUES.ContainsKey(i))
                {
                    return FALL_DELAY_VALUES[i];
                }
            }
            throw new Exception();
        }

        public void Reset(int playerCount, int startingLevel)
        {
            CurrentLevel = startingLevel;
            LinesCleared = 10 * CurrentLevel;
            DieCounter = 0;
            FallCounter = 0;
            TimeToMove = false;
            Score = 0;
            IsPaused = false;
            ClearingLines = new List<ClearingLine>();
            ClearFlag = false;
            HasLeveledUp = false;
            PlayerCount = playerCount;

            BoardWidth = (4 * PlayerCount) + 6;
            TileSize = h / BoardWidth;

            // Fill board with empty tiles
            Board = new List<Tile[]>();
            for (int row = 0; row < BoardHeight + BOARD_HEIGHT_BUFFER; row++)
            {
                Board.Add(new Tile[BoardWidth]);
                for (int col = 0; col < BoardWidth; col++)
                {
                    Board[row][col] = new Tile();
                }
            }

            Players = new Player[PlayerCount];
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = new Player(i);
            }

            // find the greatest level less than CURRENT_LEVEL in FALL_DELAY_VALUES and set the speed to that level's speed
            FallThreshold = GetFallThreshold();

            /*
            split the board into PLAYER_COUNT equal sections (using floats), find the middle of the section we care about
            using the average, and round down for the left half, and round up for the right half
            that way the pieces hopefully don't overlap spawn positions and things stay symmetrical
            somehow it seems to work this way between 1-5 players (6 has some odd stuff; I'd bet it's some number theory
            throwing it off, and perhaps it'll be really difficult and different stuff must be implemented),
            so hopefully that will port well into 7+ after it's fixed in a general way for 6 player
            this is all based on I piece spawn position, to help make all the 4 columns 'allocated' to each player be as
            evenly spread as possible
            */
            if (PlayerCount > 1)
            {
                for (int i = 0; i < Players.Length; i++)
                {
                    Players[i].SpawnColumn = (int)Math.Round(((BoardWidth - 1.0) / PlayerCount * (i) + BoardWidth / (double)PlayerCount * (i + 1.0)) / 2.0, MidpointRounding.AwayFromZero);
                }
            }
            else if (PlayerCount == 1)
            {
                Players[0].SpawnColumn = BoardWidth / 2;
            }

            // buttons
            foreach (Player p in Players)
            {
                p.Buttons[(int)ButtonTypes.MOVE_RIGHT] = new Button(new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[0] - ButtonDisplaySize / 2, ButtonDisplaySize, ButtonDisplaySize),
                    new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[0] - ButtonDisplaySize / 2, maxButtonHeight, maxButtonWidth),
                    new Vector2(ButtonYs[p.PlayerNumber], ButtonXs[0]), ButtonTypes.MOVE_RIGHT);
                p.Buttons[(int)ButtonTypes.ROT_RIGHT] = new Button(new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[1] - ButtonDisplaySize / 2, ButtonDisplaySize, ButtonDisplaySize),
                    new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[1] - ButtonDisplaySize / 2, maxButtonHeight, maxButtonWidth),
                    new Vector2(ButtonYs[p.PlayerNumber], ButtonXs[0]), ButtonTypes.ROT_RIGHT);
                p.Buttons[(int)ButtonTypes.ROT_LEFT] = new Button(new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[2] - ButtonDisplaySize / 2, ButtonDisplaySize, ButtonDisplaySize),
                    new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[2] - ButtonDisplaySize / 2, maxButtonHeight, maxButtonWidth),
                    new Vector2(ButtonYs[p.PlayerNumber], ButtonXs[0]), ButtonTypes.ROT_LEFT);
                p.Buttons[(int)ButtonTypes.MOVE_LEFT] = new Button(new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[3] - ButtonDisplaySize / 2, ButtonDisplaySize, ButtonDisplaySize),
                    new Rectangle(ButtonYs[p.PlayerNumber] - ButtonDisplaySize / 2, ButtonXs[3] - ButtonDisplaySize / 2, maxButtonHeight, maxButtonWidth),
                    new Vector2(ButtonYs[p.PlayerNumber], ButtonXs[0]), ButtonTypes.MOVE_LEFT);
            }

        }

        public void DoEvent(Event e, int playerNumber) {
            if (e.Control == ControlType.QUIT)
            {
                State.SwitchState(StateType.MENU);
            }
        }

        public override void LoadContent(ContentManager Content, SpriteBatch spriteBatch)
        {
            foreach (TileType t in Enum.GetValues(typeof(TileType))) {
                if (t == TileType.BLANK)
                {
                    TileSurfaces.Add(t, Content.Load<Texture2D>("background"));
                }
                else
                {
                    TileSurfaces.Add(t, Content.Load<Texture2D>(((int)t).ToString()));
                }
            }
            LeftRotateButtonTexture = Content.Load<Texture2D>("leftRotate");
            RightRotateButtonTexture = Content.Load<Texture2D>("rightRotate");
            LeftArrowButtonTexture = Content.Load<Texture2D>("leftArrow");
            RightArrowButtonTexture = Content.Load<Texture2D>("rightArrow");

            foreach (Player p in Players)
            {
                p.Buttons[(int)ButtonTypes.MOVE_RIGHT].Texture = RightArrowButtonTexture;
                p.Buttons[(int)ButtonTypes.ROT_RIGHT].Texture = RightRotateButtonTexture;
                p.Buttons[(int)ButtonTypes.ROT_LEFT].Texture = LeftRotateButtonTexture;
                p.Buttons[(int)ButtonTypes.MOVE_LEFT].Texture = LeftArrowButtonTexture;
            }
        }

        public void ClearLines()
        {
            // Split the lines into two lists: those being cleared this frame, and others.
            List<ClearingLine> futureClearingLines = new List<ClearingLine>();
            List<ClearingLine> presentClearingLines = new List<ClearingLine>();
            foreach (var line in ClearingLines)
            {
                line.DecrementCounter();
                if (line.Counter <= 0)
                {
                    presentClearingLines.Add(line);
                }
                else
                {
                    futureClearingLines.Add(line);
                }
            }

            // Shift all the future lines down if they're above the present line
            foreach (var clearingLine in presentClearingLines)
            {
                foreach (var shiftingLine in futureClearingLines)
                {
                    if (clearingLine.BoardIndex > shiftingLine.BoardIndex)
                    {
                        shiftingLine.BoardIndex += 1;
                    }
                }
            }

            // Clear the lines from the board
            foreach (var line in presentClearingLines)
            {
                // pop the cleared line from the board
                Board.RemoveAt(line.BoardIndex);
                // create new line of blank tiles
                Tile[] newLine = new Tile[BoardWidth];
                for (int i = 0; i < BoardWidth; i++)
                {
                    newLine[i] = new Tile();
                }
                // append the new line to the beginnign of the board
                Board.Insert(0, newLine);
            }

            // set clearingLines to the list that doesn't contain cleared lines
            ClearingLines = futureClearingLines;

            // update score
            int presentLinesCleared = presentClearingLines.Count;
            Score = SCORING_VALUES[Math.Clamp(presentLinesCleared, 0, 4)] * (CurrentLevel + 1);

            // update lines cleared and level
            if (!HasLeveledUp)
            {
                if (LinesCleared + presentLinesCleared >= 10 * (Math.Min(PlayerCount, 3) + 2 * CurrentLevel))
                {
                    CurrentLevel++;
                    HasLeveledUp = true;
                    // set speed
                    FallThreshold = GetFallThreshold();
                }
            }
            else
            {
                if (LinesCleared % (10 * Math.Min(3, PlayerCount)) + presentLinesCleared >= 10 * Math.Min(3, PlayerCount)) {
                    CurrentLevel++;
                    // set speed
                    FallThreshold = GetFallThreshold();
                }
            }

            LinesCleared += presentLinesCleared;
        }

        public void LockPiece(int playerNumber)
        {
            bool pieceLockedIntoAnotherPiece = false;
            int maxRowIndex = 0;
            TileType tileType = PlayerCount == 1 ? Players[playerNumber].ActivePiece.TileType : (TileType)playerNumber;

            foreach (var location in Players[playerNumber].ActivePiece.Locations)
            {
                // Check if locked into another piece
                if (Board[location.Row][location.Col].TileType != TileType.BLANK)
                {
                    pieceLockedIntoAnotherPiece = true;
                }
                Board[location.Row][location.Col] = new Tile(tileType);
                maxRowIndex = Math.Max(location.Row, maxRowIndex);
            }
            // this was some weird frame data stuff based on emulating the 1989 NES version of Tetris (the wiki didn't go quite in-depth enough)
            if (Players[playerNumber].ActivePiece.Type == PieceType.I)
            {
                Players[playerNumber].SpawnDelayThreshold = ((maxRowIndex + 2) / 4) * 2 + 10;
            }
            else
            {
                Players[playerNumber].SpawnDelayThreshold = ((maxRowIndex + 3) / 4) * 2 + 10;
            }
            // Keep track of if a line can be cleared to set the player's state
            bool wasLineAddedToList = false;

            // Add all clearable lines to list
            for (int row = 0; row < Board.Count; row++)
            {
                bool canClear = true;
                foreach (var tile in Board[row])
                {
                    if (tile.TileType == TileType.BLANK)
                    {
                        canClear = false;
                    }
                }
                if (canClear)
                {
                    wasLineAddedToList = true;
                    bool isLineInClearingLines = false;
                    foreach (var line in ClearingLines)
                    {
                        if (line.BoardIndex == row)
                        {
                            isLineInClearingLines = true;
                        }
                    }
                    if (!isLineInClearingLines)
                    {
                        ClearingLines.Add(new ClearingLine(playerNumber, row, 20));
                    }
                }
            }
            // if a piece was added to ClearingLines
            if (wasLineAddedToList)
            {
                Players[playerNumber].State = TetrisState.CLEAR;
            }
            else
            {
                Players[playerNumber].State = TetrisState.SPAWN_DELAY;
            }

            Players[playerNumber].ActivePiece = null;
            if (pieceLockedIntoAnotherPiece)
            {
                for (int i = 0; i < Players.Length; i++)
                {
                    Players[i].State = TetrisState.DIE;
                }
            }
        }

        private void HandleTouchInput()
        {
            TouchCollection touches = TouchPanel.GetState();
            foreach (Player p in Players)
            {
                foreach (Button b in p.Buttons)
                {
                    b?.HandleTouch(touches);
                }
            }
        }
       
        public override void Update(GameTime gameTime)
        {
            if (Board[0][0] == null) return;
            HandleTouchInput();

            // move button presses to pieces and players
            foreach (Player p in Players)
            {
                p.UpdateButtons();
            }

            // increment das_counter if a move key is pressed (it's reset to zero each time a key is pressed down)
            for (int p = 0; p < PlayerCount; p++)
            {
                if (Players[p].ButtonStates[(int)ButtonTypes.MOVE_LEFT] || Players[p].ButtonStates[(int)ButtonTypes.MOVE_RIGHT])
                {
                    Players[p].DasCounter += 1;
                }
            }

            ClearLines();

            for (int p = 0; p < PlayerCount; p++)
            {
                Player player = Players[p];

                if (player.State == TetrisState.SPAWN)
                {
                    Random random = new Random();
                    // if the game just started or the next piece can spawn into the board(only checking other players' pieces, not the piece tiles on the board)
                    if (player.NextPiece == null || player.NextPiece.CanMove(Board, Players) == MoveBlockage.NONE)
                    {
                        player.SpawnDelayCounter += 1;

                        PieceType? spawnPieceType;
                        // spwan piece
                        if (player.NextPieceType == null) // probably only for the first piece
                        {
                            spawnPieceType = (PieceType)random.Next(Enum.GetNames(typeof(PieceType)).Length);
                        }
                        else
                        {
                            spawnPieceType = player.NextPieceType;
                        }
                        if (player.ActivePiece == null)
                        {
                            player.ActivePiece = new Piece(spawnPieceType, p, player.SpawnColumn, PlayerCount);
                        }
                        else
                        {
                            player.ActivePiece.Type = spawnPieceType;
                        }

                        // Roll the next piece type, do it twice if it's the same as the last one
                        for (int i = 0; i < 2; i++)
                        {
                            player.NextPieceType = (PieceType)random.Next(Enum.GetNames(typeof(PieceType)).Length);
                            if (player.NextPieceType != player.ActivePiece.Type) break;
                        }

                        player.ActivePiece = new Piece(player.ActivePiece.Type, p, player.SpawnColumn, PlayerCount);
                        player.NextPiece = new Piece(player.NextPieceType, p, player.SpawnColumn, PlayerCount); // this puts the next piece in the next piece box
                        player.State = TetrisState.PLAY;
                        player.FallCounter = 0;
                        player.SpawnDelayCounter = 0;
                    }
                }

                if (player.State == TetrisState.PLAY)
                {
                    // move piece logic
                    if (player.buttonStates[(int)ButtonTypes.MOVE_LEFT] || player.buttonStates[(int)ButtonTypes.MOVE_RIGHT])
                    {
                        /*
                        see if the das counter is above the das threshold, which is one of three values given the player count based on if the key was just pressed(das_threshold == 0), else
                        { if first das threshold was passed for that l/ r keypress(das_threshold == DAS_VALUES[self.state.player_count][1]), else (das_threshold == DAS_VALUES[self.state.player_count][0])}
                        this way the first time the button is pressed, the piece moves(and if there's a piece in the way and then there isn't, it moves)
                        furthermore, once the initial nonzero das_threshold(DAS_VALUES[self.state.player_count][1]) is passed, then das_threshold becomes smaller(DAS_VALUES[self.state.player_count][0])
                        so that the piece moves faster after the player surely wants the game to start moving the piece for them

                        special case: if a direction is held for a long time and the active piece is against another piece(either placed or another player's piece), the das_counter shouldn't be reset
                        if the piece can move again because the input is being made for auto shift, so if das_counter == 0, we want to just subtract 1 off das_threshold unless it comes within
                        DAS_VALUES[self.state.player_count][0] of DAS_VALUES[self.state.player_count][1], in which case we want it to be DAS_VALUES[self.state.player_count][1] -DAS_VALUES[self.state.player_count][0]
                        */

                        // the special case's code also makes it work to buffer auto shift during a piece's spawn delay time or a line clear animation
                        if (player.DasCounter > player.DasThreshold)
                        {
                            Direction? MoveDirection = null;
                            if (player.buttonStates[(int)ButtonTypes.MOVE_LEFT]) MoveDirection = Direction.LEFT;
                            else if (player.buttonStates[(int)ButtonTypes.MOVE_RIGHT]) MoveDirection = Direction.RIGHT;

                            // if the piece can move, move it and do DAS stuff
                            if (player.ActivePiece.CanMove(Board, Players, MoveDirection) == MoveBlockage.NONE)
                            {
                                player.ActivePiece.Move((Direction)MoveDirection);
                                // make sure das_threshold is no longer zero for this move input and set das_counter back accordingly
                                if (player.DasThreshold == 0)
                                {
                                    player.DasThreshold = DAS_VALUES[p][1];
                                    // set das_counter as explained in the special case
                                    if (player.DasCounter + DAS_VALUES[p][0] > DAS_VALUES[p][1])
                                    {
                                        player.DasCounter = DAS_VALUES[p][1] - DAS_VALUES[p][0];
                                    }
                                    else
                                    {
                                        player.DasCounter -= 1;
                                    }
                                }
                                else
                                {
                                    player.DasThreshold = DAS_VALUES[p][0];
                                    player.DasCounter = 0;
                                }
                            }
                        }
                    }
                    //if (player.buttonStates[(int)ButtonTypes.MOVE_DOWN])
                    //{
                    //    player.DownCounter += 1;

                    //    if (player.DownCounter > 2)
                    //    {
                    //        if (player.ActivePiece.CanMove(Board, Players, Direction.DOWN) == MoveBlockage.NONE)
                    //        {
                    //            player.ActivePiece.Move(Direction.DOWN);
                    //            player.FallCounter = 0;
                    //        }
                    //        else if (player.ActivePiece.CanMove(Board, Players, Direction.DOWN) == MoveBlockage.BOARD)
                    //        {
                    //            LockPiece(p);
                    //        }
                    //        else if (player.ActivePiece.CanMove(Board, Players, Direction.DOWN) == MoveBlockage.PIECE)
                    //        {

                    //        }
                    //        player.DownCounter = 0;
                    //    }
                    //}
                    player.FallCounter += 1;

                    if (player.FallCounter >= FallThreshold && player.ActivePiece != null)
                    {
                        MoveBlockage canMove = player.ActivePiece.CanMove(Board, Players, Direction.DOWN);
                        if (canMove == MoveBlockage.BOARD)
                        {
                            LockPiece(p);
                        }
                        else if (canMove == MoveBlockage.NONE)
                        {
                            player.ActivePiece.Move(Direction.DOWN);
                        }
                        //else if (player.ActivePiece.CanMove(Board, Players, Direction.DOWN) == MoveBlockage.PIECE)
                        //{

                        //}
                        player.FallCounter = 0;
                    }
                }
                if (player.State == TetrisState.CLEAR)
                {
                    bool isBeingCleared = false;
                    foreach (ClearingLine line in ClearingLines)
                    {
                        if (line.PlayerNumber == player.PlayerNumber)
                        {
                            isBeingCleared = true;
                        }
                    }
                    if (!isBeingCleared)
                    {
                        player.State = TetrisState.SPAWN_DELAY;
                    }
                }
                else if (player.State == TetrisState.SPAWN_DELAY)
                {
                    player.SpawnDelayCounter += 1;

                    //player.IsMoveDownPressed = false;

                    if (player.SpawnDelayCounter > player.SpawnDelayThreshold)
                    {
                        player.State = TetrisState.SPAWN;
                    }
                }
                if (player.State == TetrisState.DIE)
                {
                    DieCounter += 1;
                    // wait 2 seconds
                    if (DieCounter >= 120)
                    {
                        for (int i = 0; i < Players.Length; i++)
                        {
                            player.State = TetrisState.GAME_OVER;
                        }
                    }
                }
                if (player.State == TetrisState.GAME_OVER)
                {
                    GameOver = true;
                    State.SwitchState(StateType.MENU);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            
            // board
            for (int row = 2; row < BoardHeight+2; row++)
            {
                for (int col = 0; col < BoardWidth; col++)
                {
                    Texture2D tileTexture = TileSurfaces[Board[row][col].TileType];
                    _spriteBatch.Draw(tileTexture, new Rectangle((int)((row-2) * TileSize + centeringOffset), (int)(h - ((col + 1) * TileSize)), (int)TileSize, (int)TileSize), Color.White);
                }
            }

            // active pieces and next piece
            foreach (var p in Players)
            {
                // draw active piece
                if (p.ActivePiece != null)
                {
                    foreach (var location in p.ActivePiece.Locations)
                    {
                        var tileTexture = TileSurfaces[p.ActivePiece.TileType];
                        _spriteBatch.Draw(tileTexture, new Rectangle((int)((location.Row-2) * TileSize + centeringOffset), (int)(h - ((location.Col + 1) * TileSize)), (int)TileSize, (int)TileSize), Color.White);
                    }
                }
                if (p.NextPiece != null)
                {
                    // DrawNextPiece()
                }
            }

            foreach (var button in MahButtons)
            {
                if (button?.Texture != null)
                {
                    _spriteBatch.Draw(button.Texture, button.DisplayRectangle, null, Color.White);
                }
                
            }

            base.Draw(_spriteBatch);
        }

    }
}

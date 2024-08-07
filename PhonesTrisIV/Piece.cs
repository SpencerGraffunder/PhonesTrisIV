using System;
using System.Collections.Generic;

namespace PhonesTrisIV
{
    public class Piece
    {
        public TileType TileType = 0;
        Rotation Rotation = 0;
        public BoardCoordinates[] Locations = new BoardCoordinates[4];
        public PieceType? Type;
        int PlayerNumber;

        public Piece(PieceType? pieceType, int playerNumber, int spawnColumn, int playerCount)
        {
            Type = pieceType;

            switch (Type) {
                case PieceType.I:
                    Locations[0] = new BoardCoordinates(spawnColumn - 2, 2);
                    Locations[1] = new BoardCoordinates(spawnColumn - 1, 2);
                    Locations[2] = new BoardCoordinates(spawnColumn    , 2);
                    Locations[3] = new BoardCoordinates(spawnColumn + 1, 2);
                    TileType = TileType.IOT;
                    break;
                case PieceType.O:
                    Locations[0] = new BoardCoordinates(spawnColumn - 1, 2);
                    Locations[1] = new BoardCoordinates(spawnColumn    , 2);
                    Locations[2] = new BoardCoordinates(spawnColumn - 1, 3);
                    Locations[3] = new BoardCoordinates(spawnColumn    , 3);
                    TileType = TileType.IOT;
                    break;
                case PieceType.T:
                    Locations[0] = new BoardCoordinates(spawnColumn - 1, 2);
                    Locations[1] = new BoardCoordinates(spawnColumn    , 2);
                    Locations[2] = new BoardCoordinates(spawnColumn + 1, 2);
                    Locations[3] = new BoardCoordinates(spawnColumn    , 3);
                    TileType = TileType.IOT;
                    break;
                case PieceType.L:
                    Locations[0] = new BoardCoordinates(spawnColumn - 1, 2);
                    Locations[1] = new BoardCoordinates(spawnColumn    , 2);
                    Locations[2] = new BoardCoordinates(spawnColumn + 1, 2);
                    Locations[3] = new BoardCoordinates(spawnColumn - 1, 3);
                    TileType = TileType.LZ;
                    break;
                case PieceType.J:
                    Locations[0] = new BoardCoordinates(spawnColumn - 1, 2);
                    Locations[1] = new BoardCoordinates(spawnColumn    , 2);
                    Locations[2] = new BoardCoordinates(spawnColumn + 1, 2);
                    Locations[3] = new BoardCoordinates(spawnColumn + 1, 3);
                    TileType = TileType.JS;
                    break;
                case PieceType.Z:
                    Locations[0] = new BoardCoordinates(spawnColumn - 1, 2);
                    Locations[1] = new BoardCoordinates(spawnColumn    , 2);
                    Locations[2] = new BoardCoordinates(spawnColumn    , 3);
                    Locations[3] = new BoardCoordinates(spawnColumn + 1, 3);
                    TileType = TileType.LZ;
                    break;
                case PieceType.S:
                    Locations[0] = new BoardCoordinates(spawnColumn    , 2);
                    Locations[1] = new BoardCoordinates(spawnColumn + 1, 2);
                    Locations[2] = new BoardCoordinates(spawnColumn - 1, 3);
                    Locations[3] = new BoardCoordinates(spawnColumn    , 3);
                    TileType = TileType.JS;
                    break;
            }

            if (playerCount != 1)
            {
                TileType = (TileType)playerNumber;
            }

            PlayerNumber = playerNumber;
        }

        public void Move(Direction direction, BoardCoordinates[] locations = null)
        {
            if (locations == null)
            {
                locations = Locations;
            }
            for (int i = 0; i < 4; i++)
            {
                locations[i].Move(direction);
            }
        }

        // Direction is null for spawning piece
        public MoveBlockage CanMove(List<Tile[]> board, Player[] players, Direction? direction = null)
        {
            BoardCoordinates[] testLocations = new BoardCoordinates[4];
            for (int i = 0; i < 4; i++)
            {
                testLocations[i] = new BoardCoordinates(Locations[i]);
            }

            // if not spawning piece
            if (direction != null)
            {
                Move((Direction)direction, testLocations);
            }

            foreach (BoardCoordinates location in testLocations)
            {
                if (location.Row >= board.Count ||
                    location.Col < 0 ||
                    location.Col >= board[0].Length)
                {
                    return MoveBlockage.BOARD;
                }

                // if not spawning piece
                if (direction != null)
                {
                    if (board[location.Row][location.Col].TileType != TileType.BLANK)
                    {
                        return MoveBlockage.BOARD;
                    }
                }

                foreach (Player player in players)
                {
                    if (player.PlayerNumber != PlayerNumber && player.ActivePiece != null)
                    {
                        foreach (BoardCoordinates otherLocation in player.ActivePiece?.Locations)
                        {
                            if (otherLocation == location)
                            {
                                return MoveBlockage.PIECE;
                            }
                        }
                    }
                }
            }
            return MoveBlockage.NONE;
        }
        public void Rotate(RotationDirection rotationDirection, BoardCoordinates[] hypotheticalLocations = null, Rotation hypotheticalCurrentRotation = Rotation.NONE)
        {
            // Use hypothetical if they exist, otherwise use this's
            BoardCoordinates[] locations = hypotheticalLocations == null ? Locations : hypotheticalLocations;
            Rotation rotation = hypotheticalCurrentRotation == Rotation.NONE ? Rotation : hypotheticalCurrentRotation;
            Rotation newRotation = Rotation.NONE;

            BoardCoordinates pivot = new BoardCoordinates(0, 0); // dummy assignment

            switch (Type)
            {
                case PieceType.I:
                case PieceType.S://col,row
                case PieceType.Z:
                    RotationDirection directionToRotate = RotationDirection.NONE;
                    switch (Type)
                    {
                        case PieceType.I:
                            pivot = locations[2];
                            break;
                        case PieceType.S:
                            pivot = locations[0];
                            break;
                        case PieceType.Z:
                            pivot = locations[1];
                            break;
                    }
                    if (rotation == Rotation.ROT0 || rotation == Rotation.ROT180)
                    {
                        for (int i = 0; i < locations.Length; i++)
                        {
                            locations[i] = new BoardCoordinates((locations[i].Row - pivot.Row) + pivot.Col, (pivot.Col - locations[i].Col) + pivot.Row);
                        }
                        newRotation = (Rotation)(((int)directionToRotate - 90) % 360);
                    }
                    else
                    {
                        for (int i = 0; i < locations.Length; i++)
                        {
                            locations[i] = new BoardCoordinates((pivot.Row - locations[i].Row) + pivot.Col, (locations[i].Col - pivot.Col) + pivot.Row);
                        }
                        newRotation = (Rotation)(((int)directionToRotate + 90) % 360);
                    }
                    break;
                case PieceType.T:
                case PieceType.L:
                case PieceType.J:
                    pivot = new BoardCoordinates(locations[1]);
                    if (rotationDirection == RotationDirection.CW)
                    {
                        for (int i = 0; i < locations.Length; i++)
                        {
                            locations[i] = new BoardCoordinates((pivot.Row - locations[i].Row) + pivot.Col, (locations[i].Col - pivot.Col) + pivot.Row);
                        }
                        newRotation = (Rotation)(((int)rotation + 90) % 360);
                    }
                    else if (rotationDirection == RotationDirection.CCW)
                    {
                        for (int i = 0; i < locations.Length; i++)
                        {
                            locations[i] = new BoardCoordinates((locations[i].Row - pivot.Row) + pivot.Col, (pivot.Col - locations[i].Col) + pivot.Row);
                        }
                        newRotation = (Rotation)(((int)rotation - 90) % 360);
                    }
                    break;
            }

            // if not using hypothetical, save the new rotation to this
            if (hypotheticalLocations == null)
            {
                Rotation = newRotation;
            }
        }

        public bool CanRotate(List<Tile[]> board, Player[] players, RotationDirection rotationDirection)
        {
            BoardCoordinates[] testLocations = new BoardCoordinates[4];
            for (int i = 0; i < Locations.Length; i++)
            {
                testLocations[i] = new BoardCoordinates(Locations[i]);
            }
            Rotation testRotation = Rotation;

            Rotate(rotationDirection, testLocations, testRotation);

            foreach (BoardCoordinates location in testLocations) // col,row
            {
                if (location.Row >= board.Count || location.Col < 0 || location.Col >= board[0].Length)
                {
                    return false;
                }
                if (board[location.Row][location.Col].TileType != TileType.BLANK)
                {
                    return false;
                }

                foreach (Player player in players)
                {
                    if (player.ActivePiece != null)
                    {
                        if (player.ActivePiece.PlayerNumber != PlayerNumber)
                        {
                            foreach (BoardCoordinates otherLocation in player.ActivePiece.Locations)
                            {
                                if (otherLocation == location)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}

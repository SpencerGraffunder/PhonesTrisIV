using System;
namespace PhonesTrisIV
{
    public struct BoardCoordinates
    {
        public int Col { get; private set; }
        public int Row { get; private set; }
        public BoardCoordinates(int col, int row)
        {
            this.Col = col;
            this.Row = row;
        }
        public BoardCoordinates(BoardCoordinates original)
        {
            this.Col = original.Col;
            this.Row = original.Row;
        }
        public static bool operator ==(BoardCoordinates a, BoardCoordinates b)
        {
            return ((a.Col == b.Col) && (a.Row == b.Row));
        }
        public override bool Equals(object other)
        {
            return this == (BoardCoordinates)other;
        }
        public static bool operator !=(BoardCoordinates a, BoardCoordinates b)
        {
            return !(a == b);
        }
        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    this.Row += 1;
                    break;
                case Direction.LEFT:
                    this.Col -= 1;
                    break;
                case Direction.RIGHT:
                    this.Col += 1;
                    break;
            }
        }
    }
}

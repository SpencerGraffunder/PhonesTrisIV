namespace PhonesTrisIV
{
    public class ClearingLine
    {
        public int PlayerNumber;
        public int BoardIndex;
        public int Counter;

        public ClearingLine(int playerNumber, int boardIndex, int counter)
        {
            PlayerNumber = playerNumber;
            BoardIndex = boardIndex;
            Counter = counter;
        }

        public void DecrementCounter()
        {
            Counter -= 1;
        }

    }
}
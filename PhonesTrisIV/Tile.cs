namespace PhonesTrisIV
{
    public class Tile
    {
        public TileType TileType { get; private set; }
        bool IsActive;

        public Tile(TileType tileType = TileType.BLANK, bool isActive = false)
        {
            this.TileType = tileType;
            this.IsActive = isActive;
        }
    }
}
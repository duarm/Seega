namespace Seega.Types
{
    public struct Coordinates
    {
        public int x;
        public int y;

        public Coordinates up => new Coordinates (x + 1, y);
        public Coordinates right => new Coordinates (x, y + 1);
        public Coordinates down => new Coordinates (x - 1, y);
        public Coordinates left => new Coordinates (x, y - 1);

        public Coordinates[] cardinal4 => new Coordinates[4] { up, right, down, left };
        public static Coordinates zero => new Coordinates (0, 0);

        public Coordinates (int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinates (TileField tile)
        {
            this.x = tile.Coordinates.x;
            this.y = tile.Coordinates.y;
        }

        public override string ToString ()
        {
            return $"({x}, {y})";
        }
    }
}
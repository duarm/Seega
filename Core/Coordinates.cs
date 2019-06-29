public struct Coordinates
{

    public int x;
    public int y;

    public Coordinates up
    {
        get
        {
            return new Coordinates (x + 1, y);
        }
    }

    public Coordinates right
    {
        get
        {
            return new Coordinates (x, y + 1);
        }
    }

    public Coordinates down
    {
        get
        {
            return new Coordinates (x - 1, y);
        }
    }

    public Coordinates left
    {
        get
        {
            return new Coordinates (x, y - 1);
        }
    }

    public static Coordinates zero
    {
        get
        {
            return new Coordinates (0, 0);
        }
    }

    public Coordinates (int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Coordinates (TileField tile)
    {
        this.x = tile.coordinates.x;
        this.y = tile.coordinates.y;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}
namespace Seega.Types
{
    public class Movement
    {
        private int x;
        private int y;

        public bool Vertical { get { return x > 0 || x < 0; } }
        public bool Horizontal { get { return y > 0 || y < 0; } }

        public Movement (TileField currentField, TileField lastField)
        {
            x = currentField.Coordinates.x > lastField.Coordinates.x ? 1 : -1;
            y = currentField.Coordinates.y > lastField.Coordinates.y ? 1 : -1;
        }
    }
}
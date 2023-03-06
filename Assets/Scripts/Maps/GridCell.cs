namespace Maps
{
    public class GridCell<T>
    {
        public int X { get; }
        public int Y { get; }
        public T Value { get; }

        public GridCell(int x, int y, T value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }
}
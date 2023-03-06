using Extensions;
using UnityEngine;

namespace Maps
{
    public class Attic
    {
        private readonly int _maxWidth;
        private readonly int _maxHeight;
        public Grid<GridCell<bool>> Grid { get; }

        public Attic(int maxWidth, int maxHeight)
        {
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
            Grid = new Grid<GridCell<bool>>(maxWidth, maxHeight, InitializeAtticCell);
        }

        private static GridCell<bool> InitializeAtticCell(int x, int y)
        {
            return new GridCell<bool>(x, y, true);
        }

        public void DigCorridors(int cellsToRemove)
        {
            Vector2Int walkerPosition = new Vector2Int(_maxWidth / 2, _maxHeight / 2);
            while (cellsToRemove > 0)
            {
                Direction randomDirection = RandomUtils.GetRandomEnumValue<Direction>();
                Vector2Int newWalkerPosition = walkerPosition + randomDirection.ToCoordinates();
                if (!Grid.AreCoordinatesValid(newWalkerPosition, true)) continue;

                GridCell<bool> cell = Grid.Get(newWalkerPosition);
                if (cell.Value)
                {
                    Grid.Set(newWalkerPosition, new GridCell<bool>(cell.X, cell.Y, false));
                    cellsToRemove--;
                }

                walkerPosition = newWalkerPosition;
            }
        }
    }
}
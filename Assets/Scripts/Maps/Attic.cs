using System.Linq;
using Extensions;
using UnityEngine;
using Grid = Maps.Grid<Maps.GridCell<bool>>;

namespace Maps
{
    public class Attic
    {
        private readonly int _maxWidth;
        private readonly int _maxHeight;
        public Grid<GridCell<bool>> Grid { get; private set; }

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

        public void Shrink()
        {
          GridCell<bool>[] emptyCells = Grid.Cells.Where(c => !c.Value).ToArray();
          
          int minX = emptyCells.Min(c => c.X);
          int maxX = emptyCells.Max(c => c.X);
          int shrinkWidth = maxX - minX + 3;
          
          int minY = emptyCells.Min(c => c.Y);
          int maxY = emptyCells.Max(c => c.Y);
          int shrinkHeight = maxY - minY + 3;

          Grid newGrid = new Grid<GridCell<bool>>(shrinkWidth, shrinkHeight, InitializeAtticCell);
          for (int x = minX - 1; x <= maxX; x++)
          {
              for (int y = minY - 1; y <= maxY; y++)
              {
                  GridCell<bool> value = Grid.Get(x, y);
                  newGrid.Set(x - minX + 1, y - minY + 1, value);
              }
          }

          Grid = newGrid;
        }
    }
}
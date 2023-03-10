using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Grid = Maps.Grid<Maps.GridCell<bool>> ;

namespace Maps
{
    public class AtticGenerator : MonoBehaviour
    {
        [Header("Tiles")]
        [SerializeField] private Tilemap _wallsTilemap;
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private RuleTile _wallTiles;
        [SerializeField] private RuleTile _groundTiles;

        [Header("Dimensions (for testing only)")]
        [SerializeField] private int _maxWidth;
        [SerializeField] private int _maxHeight;
        [SerializeField] private int _cellsToRemove;

        [SerializeField] private bool _shrink;
        
        public Attic Attic { get; private set; }
        
        [ContextMenu("Generate Attic")]
        public void Generate()
        {
            _wallsTilemap.ClearAllTiles();
            _groundTilemap.ClearAllTiles();
            Attic = new Attic(_maxWidth, _maxHeight);
            Attic.DigCorridors(Mathf.Clamp(_cellsToRemove, 1, (_maxWidth * _maxHeight) - (_maxWidth + _maxWidth + _maxHeight - 2 + _maxHeight - 2)));
            if (_shrink) Attic.Shrink();
            foreach (GridCell<bool> cell in Attic.Grid.Cells)
            {
                if (cell.Value)
                {
                    _wallsTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), _wallTiles);
                }
                else
                {
                    _groundTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), _groundTiles);
                }
            }
        }
        
        public void Generate(int width, int height, int cellsToRemove)
        {
            _wallsTilemap.ClearAllTiles();
            _groundTilemap.ClearAllTiles();
            Attic = new Attic(width, height);
            Attic.DigCorridors(Mathf.Clamp(cellsToRemove, 1, (width * height) - (width + width + height - 2 + height - 2)));
            if (_shrink) Attic.Shrink();
            foreach (GridCell<bool> cell in Attic.Grid.Cells)
            {
                if (cell.Value)
                {
                    _wallsTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), _wallTiles);
                }
                else
                {
                    _groundTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), _groundTiles);
                }
            }
        }

        public void PrintGrid()
        {
            Grid g = Attic.Grid;
            String[] lines = new String[g.Height];
            for (int i = 0; i < g.Height * g.Width; i++)
            {
                lines[i / g.Height] += g.Get(i).Value ? "1" : "0";
            }

            Array.Reverse(lines);
            
            String toPrint = "";

            foreach (String s in lines)
            {
                toPrint += s;
                toPrint += "\n";
            }
            
            Debug.Log(toPrint);
        }
        
        public String StringGrid()
        {
            Grid g = Attic.Grid;
            String[] lines = new String[g.Height];
            for (int i = 0; i < g.Height * g.Width; i++)
            {
                lines[i / g.Height] += g.Get(i).Value ? "1" : "0";
            }

            Array.Reverse(lines);
            
            String toPrint = "";

            foreach (String s in lines)
            {
                toPrint += s;
                toPrint += "\n";
            }

            return toPrint;
        }
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

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
            Attic = new Attic(_maxWidth, _maxHeight);
            Attic.DigCorridors(_cellsToRemove);
            if (_shrink) Attic.Shrink();
            _wallsTilemap.ClearAllTiles();
            _groundTilemap.ClearAllTiles();
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
            Attic = new Attic(width, height);
            Attic.DigCorridors(cellsToRemove);
            if (_shrink) Attic.Shrink();
            _wallsTilemap.ClearAllTiles();
            _groundTilemap.ClearAllTiles();
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
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Maps
{
    public class AtticGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap _wallsTilemap;
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private TileBase[] _wallTiles;
        [SerializeField] private TileBase[] _groundTiles;

        [SerializeField] private int _maxWidth;
        [SerializeField] private int _maxHeight;
        [SerializeField] private int _cellsToRemove;

        [SerializeField] private bool _shrink;
        
        [ContextMenu("Generate Attic")]
        public void Generate()
        {
            Attic attic = new Attic(_maxWidth, _maxHeight);
            attic.DigCorridors(_cellsToRemove);
            if (_shrink) attic.Shrink();
            _wallsTilemap.ClearAllTiles();
            _groundTilemap.ClearAllTiles();
            foreach (GridCell<bool> cell in attic.Grid.Cells)
            {
                if (cell.Value)
                {
                    _wallsTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), _wallTiles[0]);
                }
                else
                {
                    _groundTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), _groundTiles[0]);
                }
            }
        }
    }
}
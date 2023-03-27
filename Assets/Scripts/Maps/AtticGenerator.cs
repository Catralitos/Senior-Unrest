using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Grid = Maps.Grid<Maps.GridCell<bool>>;

namespace Maps
{
    /// <summary>
    /// Generates a map. Adapted from https://www.youtube.com/watch?v=u2i8Ga4phBA
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class AtticGenerator : MonoBehaviour
    {
        /// <summary>
        /// The walls tilemap
        /// </summary>
        [Header("Tiles")] public Tilemap wallsTilemap;
        /// <summary>
        /// The ground tilemap
        /// </summary>
        public Tilemap groundTilemap;
        /// <summary>
        /// The wall rule tiles
        /// </summary>
        public RuleTile wallTiles;
        /// <summary>
        /// The ground rule tiles
        /// </summary>
        public RuleTile groundTiles;
        
        /// <summary>
        /// Gets the attic/map generated.
        /// </summary>
        /// <value>
        /// The attic.
        /// </value>
        public Attic Attic { get; private set; }
        
        /// <summary>
        /// Generates a map.
        /// </summary>
        /// <param name="width">The map width.</param>
        /// <param name="height">The map height.</param>
        /// <param name="cellsToRemove">The cells to remove from the grid.</param>
        public void Generate(int width, int height, int cellsToRemove)
        {
            //We clear the tilemaps
            wallsTilemap.ClearAllTiles();
            groundTilemap.ClearAllTiles();
            //Create an attic and dig the corridors
            Attic = new Attic(width, height);
            Attic.DigCorridors(
                Mathf.Clamp(cellsToRemove, 1, width * height - (width + width + height - 2 + height - 2)));
            //For each cell we set the correct tile graphic
            foreach (var cell in Attic.Grid.Cells)
                if (cell.Value)
                    wallsTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), wallTiles);
                else
                    groundTilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), groundTiles);
        }
    }
}
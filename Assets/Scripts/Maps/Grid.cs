using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Maps
{
  public class Grid<T>
  {
    public T[] Cells { get; }
    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
      Cells = new T[width * height];
      Width = width;
      Height = height;
    }

    public int CoordinatesToIndex(int x, int y)
    {
      return y * Width + x;
    }
  
    public int CoordinatesToIndex(Vector2Int coordinates)
    {
      return coordinates.y * Width + coordinates.x;
    }

    public Vector2Int IndexToCoords(int index)
    {
      return new Vector2Int(index % Width, index / Width);
    }

    public void Set(int x, int y, T value)
    {
      Cells[CoordinatesToIndex(x, y)] = value;
    }

    public void Set(Vector2Int coordinates, T value)
    {
      Cells[CoordinatesToIndex(coordinates.x, coordinates.y)] = value;
    }

    public void Get(int index, T value)
    {
      Cells[index] = value;
    }
  
    public T Get(int x, int y)
    {
      return Cells[CoordinatesToIndex(x, y)];
    }

    public T Get(Vector2Int coordinates)
    {
      return Cells[CoordinatesToIndex(coordinates.x, coordinates.y)];
    }

    public T Get(int index)
    {
      return Cells[index];
    }

    public bool AreCoordinatesValid(int x, int y, bool safeWalls = false)
    {
      return safeWalls ? 
        x > 0 && x < Width - 1 && y > 0 && y < Height - 1 : 
        x >= 0 && x < Width && y >= 0 && y < Height;
    }
  
    public bool AreCoordinatesValid(Vector2Int coordinates, bool safeWalls = false)
    {
      return AreCoordinatesValid(coordinates.x, coordinates.y, safeWalls);
    }

    public Vector2Int GetCoordinates(T value)
    {
      int i = Array.IndexOf(Cells, value);
      if (i == -1)
      {
        throw new ArgumentException();
      }

      return IndexToCoords(i);
    }

    public List<T> GetNeighbours(int x, int y, bool safeWalls = false)
    {
      Direction[] directions = (Direction[]) Enum.GetValues(typeof(Direction));
      List<T> neighbours = new List<T>();

      foreach (Direction direction in directions)
      {
        Vector2Int neighbourCoordinates = new Vector2Int(x, y) + direction.ToCoordinates();
        if (AreCoordinatesValid(neighbourCoordinates, safeWalls))
        {
          neighbours.Add(Get(neighbourCoordinates));
        }
      }

      return neighbours;
    }
  
    public List<T> GetNeighbours(Vector2Int coordinates, bool safeWalls = false)
    {
      return GetNeighbours(coordinates.x, coordinates.y, safeWalls);
    }
  }
}

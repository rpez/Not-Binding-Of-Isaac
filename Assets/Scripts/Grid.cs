using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class Grid
{
  private int[,] m_grid;
  private Vector3 origoPos = new Vector3(-GRID_MAX_X / 2, -GRID_MAX_Y / 2);

  public Grid()
  {
    int[,] m_grid = new int[GRID_MAX_X, GRID_MAX_Y];
  }

  public Vector3 GridToWorldCoordinates(int x, int y, Vector3 roomPos)
  {
    return roomPos + origoPos * GRID_SIZE + new Vector3(x, y) * GRID_SIZE;
  }

  public (int, int) WorldToGridCoordinates(Vector3 worldPos, Vector3 roomPos)
  {
    Vector3 coords = worldPos * (1/GRID_SIZE) - roomPos * (1/GRID_SIZE) - origoPos;

    // I would think that one would use FloorToInt here, but for some reason RoundToInt gives the correct answers...
    (int, int) gridCoords = (Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));

    return gridCoords;
  }
}

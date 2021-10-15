using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class Grid
{
  private Vector3 origoPos = new Vector3(-GRID_MAX_X / 2, -GRID_MAX_Y / 2);

  public Vector3 GridToWorldCoordinates(int x, int y, Vector3 roomPos)
  {
    return roomPos + origoPos * GRID_SIZE + new Vector3(x, -y + GRID_MAX_Y - 1) * GRID_SIZE;
  }

  public (int, int) WorldToGridCoordinates(Vector3 worldPos, Vector3 roomPos)
  {
    Vector3 coords = worldPos * (1/GRID_SIZE) - roomPos * (1/GRID_SIZE) - origoPos;

    // I would think that one would use FloorToInt here, but for some reason RoundToInt gives the correct answers...
    (int, int) gridCoords = (Mathf.RoundToInt(coords.x), Mathf.RoundToInt(GRID_MAX_Y - coords.y - 1));

    // Clamp the values here to be extra sure we never go over the maximum grid
    return (Mathf.Clamp(gridCoords.Item1, 0, GRID_MAX_X - 1), Mathf.Clamp(gridCoords.Item2, 0, GRID_MAX_Y - 1));
  }
}

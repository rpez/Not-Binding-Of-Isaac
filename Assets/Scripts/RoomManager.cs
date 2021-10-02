using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    public int m_roomAmount;
    public GameObject m_roomPrefab;

    private List<Room> m_rooms = new List<Room>();
    private Dictionary<(int, int), Room> m_coordsToRoom = new Dictionary<(int, int), Room>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateNewFloor(10);
        foreach (Room room in m_rooms)
        {
            GameObject roomObj = GameObject.Instantiate(m_roomPrefab, new Vector3(room.m_coordinates.Item1 * 19.2f, room.m_coordinates.Item2 * 10.8f, 0), Quaternion.identity);
            roomObj.GetComponent<RoomObject>().Init(room.m_neigbours.Select(x => x != null).ToArray());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateNewFloor(int rooms)
    {
        List<(int, int)> grid = new List<(int, int)>();

        grid.Add((0, 0));
        int createdRooms = 1;

        m_rooms.Add(new Room((0, 0)));
        m_coordsToRoom.Add((0, 0), m_rooms[0]);
        while (createdRooms < rooms)
        {
            int rIndex = Random.Range(0, grid.Count);
            (int, int) coords = grid[rIndex];
            Room selected = m_coordsToRoom[coords];
            if (selected.m_neighbourAmount >= 4) continue;

            int rDir = Random.Range(0, 4);
            if (selected.m_neigbours[rDir] != null) continue;
            (int, int) newCoords = coords;
            switch (rDir)
            {
                case 0:
                    newCoords = (coords.Item1, coords.Item2 + 1);
                    break;
                case 1:
                    newCoords = (coords.Item1 - 1, coords.Item2);
                    break;
                case 2:
                    newCoords = (coords.Item1, coords.Item2 - 1);
                    break;
                case 3:
                    newCoords = (coords.Item1 + 1, coords.Item2);
                    break;
                default:
                    Debug.LogError("Wrong direction while creating rooms.");
                    break;
            }
            if (grid.Contains(newCoords)) continue;

            Room newRoom = new Room(newCoords);
            selected.AttachRoom(rDir, newRoom);
            newRoom.AttachRoom((rDir + 2) % 4, selected);
            m_rooms.Add(newRoom);
            m_coordsToRoom.Add(newCoords, newRoom);
            grid.Add(newCoords);

            createdRooms++;
        }
    }
}

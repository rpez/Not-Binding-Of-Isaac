using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    public int m_roomAmount;
    public int m_itemRoomAmount;
    public GameObject m_camera;
    public GameObject[] m_roomPool;
    public GameObject[] m_bossRoomPool;
    public GameObject[] m_itemRoomPool;

    private List<FloorNode> m_rooms = new List<FloorNode>();
    private int m_bossRoomIndex = int.MaxValue;
    private List<Room> m_roomObjs = new List<Room>();
    private Dictionary<(int, int), FloorNode> m_coordsToRoom = new Dictionary<(int, int), FloorNode>();

    private bool m_cameraMoving = false;
    private float m_cameraMoveDuration = 0.3f;
    private float m_cameraMoveTime;
    private Vector3 m_oldCameraPosition;
    private Vector3 m_newCameraPosition;

    private FloorNode m_currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        GenerateNewFloor(m_roomAmount);

        int[] itemRooms = new int[m_itemRoomAmount];
        List<int> freeRooms = new List<int>();
        // Find all room ids that can be converted to item rooms
        for (int i = 0; i < m_roomAmount; i++)
        {
            if (i != 0 && i != m_bossRoomIndex) freeRooms.Add(i);
        }
        // Pick some of the to actually be item rooms
        for (int i = 0; i < m_itemRoomAmount; i++)
        {
            int id = Random.Range(0, freeRooms.Count);
            itemRooms[i] = freeRooms[id];
            freeRooms.RemoveAt(id);
        }
        // Spawn rooms
        for (int i = 0; i < m_rooms.Count; i++)
        {
            FloorNode room = m_rooms[i];
            GameObject roomPrefab;
            if (i == 0)
            {
                roomPrefab = m_roomPool[0];
            }
            else if (i == m_bossRoomIndex) roomPrefab = m_bossRoomPool[Random.Range(0, m_bossRoomPool.Length)];
            else if (itemRooms.Contains(i)) roomPrefab = m_itemRoomPool[Random.Range(0, m_itemRoomPool.Length)];
            else roomPrefab = m_roomPool[Random.Range(1, m_roomPool.Length)];
            GameObject roomObj = GameObject.Instantiate(roomPrefab, new Vector2(room.m_coordinates.Item1 * 19.2f, room.m_coordinates.Item2 * 10.8f), Quaternion.identity);
            Room script = roomObj.GetComponent<Room>();
            room.m_room = script;
            m_roomObjs.Add(script);

            // Kinda stupid way to handle this, but the floors are small so not that big of a deal
            int bossDir = -1;
            for (int k = 0; k < room.m_neigbours.Length; k++)
            {
                if (room.m_neigbours[k] == m_rooms[m_bossRoomIndex]) bossDir = k;
            }

            script.Init(room.m_neigbours.Select(x => x != null).ToArray(), ChangeRoom, bossDir);
        }
        m_roomObjs[0].OnRoomEnter();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_cameraMoving)
        {
            m_cameraMoveTime += Time.deltaTime;
            m_camera.transform.position = Vector3.Lerp(m_oldCameraPosition, m_newCameraPosition, m_cameraMoveTime / m_cameraMoveDuration);
            if (m_cameraMoveTime > m_cameraMoveDuration)
            {
                m_camera.transform.position = m_newCameraPosition;
                m_cameraMoving = false;
                m_cameraMoveTime = 0f;
            }
        }
    }

    public void ChangeRoom(int dir)
    {
        m_currentRoom.m_room.OnRoomLeave();
        m_currentRoom.m_room.CloseAllDoors();
        m_oldCameraPosition = m_camera.transform.position;
        FloorNode room = m_currentRoom.m_neigbours[dir];
        m_currentRoom = room;
        m_currentRoom.m_room.OnRoomEnter();
        m_newCameraPosition = new Vector3(room.m_coordinates.Item1 * 19.2f, room.m_coordinates.Item2 * 10.8f, -10f);
        m_cameraMoving = true;
    }

    public void GenerateNewFloor(int rooms)
    {
        List<(int, int)> grid = new List<(int, int)>();

        grid.Add((0, 0));
        int createdRooms = 1;

        m_rooms.Add(new FloorNode((0, 0)));
        m_currentRoom = m_rooms[0];
        m_coordsToRoom.Add((0, 0), m_rooms[0]);
        int longestDistance = 0;
        while (createdRooms < rooms)
        {
            int rIndex = Random.Range(0, grid.Count);
            // Prevent any additional doors from boss room
            if (rIndex == m_bossRoomIndex) continue;
            (int, int) coords = grid[rIndex];
            FloorNode selected = m_coordsToRoom[coords];
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

            FloorNode newRoom = new FloorNode(newCoords);
            selected.AttachRoom(rDir, newRoom);
            newRoom.AttachRoom((rDir + 2) % 4, selected);
            m_rooms.Add(newRoom);
            m_coordsToRoom.Add(newCoords, newRoom);
            grid.Add(newCoords);
            // Determine boss room coords by taxi distance
            // NOTE: taxi distance isn't always the longest path from the start
            // Couldn't bother writing a pathfinding algorithm
            int taxiDistance = Mathf.Abs(newCoords.Item1) + Mathf.Abs(newCoords.Item2);
            if (taxiDistance > longestDistance)
            {
                longestDistance = taxiDistance;
                m_bossRoomIndex = grid.Count() - 1;
            }

            createdRooms++;
        }
    }
}

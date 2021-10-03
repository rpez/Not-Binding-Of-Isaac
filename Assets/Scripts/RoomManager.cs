using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    public int m_roomAmount;
    public GameObject m_roomPrefab;
    public GameObject m_camera;

    private List<FloorNode> m_rooms = new List<FloorNode>();
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
        GenerateNewFloor(10);
        foreach (FloorNode room in m_rooms)
        {
            GameObject roomObj = GameObject.Instantiate(m_roomPrefab, new Vector2(room.m_coordinates.Item1 * 19.2f, room.m_coordinates.Item2 * 10.8f), Quaternion.identity);
            Room script = roomObj.GetComponent<Room>();
            room.m_room = script;
            m_roomObjs.Add(script);
            script.Init(room.m_neigbours.Select(x => x != null).ToArray(), ChangeRoom, LockRoom);
        }
        m_rooms[0].m_room.OpenAllDoors();
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
        m_currentRoom.m_room.CloseAllDoors();
        m_oldCameraPosition = m_camera.transform.position;
        FloorNode room = m_currentRoom.m_neigbours[dir];
        m_currentRoom = room;
        m_currentRoom.m_room.OpenAllDoors();
        m_newCameraPosition = new Vector3(room.m_coordinates.Item1 * 19.2f, room.m_coordinates.Item2 * 10.8f, -10f);
        m_cameraMoving = true;
    }

    public void LockRoom()
    {
        m_currentRoom.m_room.CloseAllDoors();
    }

    public void GenerateNewFloor(int rooms)
    {
        List<(int, int)> grid = new List<(int, int)>();

        grid.Add((0, 0));
        int createdRooms = 1;

        m_rooms.Add(new FloorNode((0, 0)));
        m_currentRoom = m_rooms[0];
        m_coordsToRoom.Add((0, 0), m_rooms[0]);
        while (createdRooms < rooms)
        {
            int rIndex = Random.Range(0, grid.Count);
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

            createdRooms++;
        }
    }
}

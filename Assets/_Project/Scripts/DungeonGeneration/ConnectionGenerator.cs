using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;
using Delaunay.Geo;

namespace SacredTreeStudios.DungeonGeneration
{
    public class ConnectionGenerator : MonoBehaviour {

        private List<LineSegment> roomConnectionTree;
        private List<LineSegment> delaunayTriangulation;
        private Dictionary<Vector2, Room> connectionDictionary = new Dictionary<Vector2, Room>();

        private DungeonGenerator dungeonGenerator;

        private bool debugMode;

        void Awake()
        {
            dungeonGenerator = GetComponent<DungeonGenerator>();
        }

        public void ConnectRooms()
        {
            debugMode = dungeonGenerator.debugMode;
            TriangulateAndBuildTree();
            CreateRoomConnections();
        }

        private void TriangulateAndBuildTree()
        {
            List<uint> colors = new List<uint>();
            List<Vector2> points = new List<Vector2>();
            roomConnectionTree = new List<LineSegment>();
            delaunayTriangulation = new List<LineSegment>();
            connectionDictionary = new Dictionary<Vector2, Room>();
            foreach (Room room in dungeonGenerator.dungeonRooms)
            {
                connectionDictionary.Add(room.GetCenter(), room);
                points.Add(room.GetCenter());
                colors.Add(0);
            }
            Voronoi voroni = new Voronoi(points, colors, new Rect(0, 0, 50, 50));
            roomConnectionTree = voroni.SpanningTree(KruskalType.MINIMUM);
            delaunayTriangulation = voroni.DelaunayTriangulation();
        }

        private void CreateRoomConnections()
        {
            System.Random pseudoRNG = new System.Random(dungeonGenerator.seed);

            List<int> range = new List<int>();
            for (int i = 0; i < delaunayTriangulation.Count; i++)
            {
                range.Add(i);
            }

            for (int i = 0; i < delaunayTriangulation.Count * dungeonGenerator.corridorDensity; i++)
            {
                int idx = pseudoRNG.Next(0, range.Count);
                int value = range[idx];
                range.RemoveAt(idx);
                if (Vector2.Distance((Vector2)delaunayTriangulation[value].p0, (Vector2)delaunayTriangulation[value].p1) < 20f)
                {
                    roomConnectionTree.Add(delaunayTriangulation[value]);
                }
            }

            for (int i = 0; i < roomConnectionTree.Count; i++)
            {
                Vector3 roomA = new Vector3(roomConnectionTree[i].p0.Value.x, 0f, roomConnectionTree[i].p0.Value.y);
                Vector3 roomB = new Vector3(roomConnectionTree[i].p1.Value.x, 0f, roomConnectionTree[i].p1.Value.y);
                if (!connectionDictionary[roomConnectionTree[i].p0.Value].connectedRooms.Contains(connectionDictionary[roomConnectionTree[i].p1.Value]))
                {
                    connectionDictionary[roomConnectionTree[i].p0.Value].connectedRooms.Add(connectionDictionary[roomConnectionTree[i].p1.Value]);
                }

                if (debugMode)
                {
                    GameObject line = GameObject.Instantiate(Resources.Load("Line") as GameObject);
                    line.GetComponent<LineRenderer>().SetPosition(0, roomA);
                    line.GetComponent<LineRenderer>().SetPosition(1, roomB);
                }
            }
        }
    }
}
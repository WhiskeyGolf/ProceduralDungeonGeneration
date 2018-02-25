using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SacredTreeStudios.DungeonGeneration
{
    public class RoomSorter : IComparer {
        int IComparer.Compare(System.Object x, System.Object y)
        {
            return((new CaseInsensitiveComparer()).Compare(((Collider)x).gameObject.name, ((Collider)y).gameObject.name));
        }
    }

    public class RoomGenerator : MonoBehaviour
    {
        private GameObject dungeonParent;

        public GameObject roomPrefab;

        private bool debugMode;

        public float maxX { get; private set; }
        public float maxY { get; private set; }

        public bool roomGenerationComplete { get; private set; }

        private DungeonGenerator dungeonGenerator;

        void Awake()
        {
            dungeonGenerator = GetComponent<DungeonGenerator>();
            roomGenerationComplete = false;
        }

        public void GenerateRooms()
        {
            roomGenerationComplete = false;
            debugMode = dungeonGenerator.debugMode;
            dungeonParent = new GameObject("Dungeon");
            dungeonParent.tag = "DungeonParent";
            int[] roomSizeDistribution = GetRoomSizeDistribution();
            System.Random pseudoRNG = new System.Random(dungeonGenerator.seed);
            for (int i = 0; i < dungeonGenerator.numberOfRooms; i++)
            {
                int roomWidth = roomSizeDistribution[pseudoRNG.Next(0, roomSizeDistribution.Length)];
                int roomLength = roomSizeDistribution[pseudoRNG.Next(0, roomSizeDistribution.Length)];
                GameObject room = Instantiate(roomPrefab, DungeonUtilities.CalculatePointInCircle(dungeonGenerator.roomSpawnRadius, pseudoRNG), Quaternion.identity, dungeonParent.transform) as GameObject;
                if (debugMode)
                {
                    room.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    room.GetComponent<Renderer>().material.mainTextureScale = new Vector2(roomWidth, roomLength);
                }
                room.transform.localScale = new Vector3(roomWidth, .05f, roomLength);
                room.name = "Room " + i;
                room.GetComponent<Room>().roomID = i;
                dungeonGenerator.dungeonRooms.Add(room.GetComponent<Room>());
            }
            StartCoroutine(SeperateRooms());
        }

        private IEnumerator SeperateRooms()
        {
            List<Collider> colliders = new List<Collider>();
            for (int i = 0; i < dungeonParent.transform.childCount; i++)
            {
                colliders.Add(dungeonParent.transform.GetChild(i).GetComponent<Collider>());
            }
            bool seperationComplete = false;
            while (!seperationComplete)
            {
                seperationComplete = true;
                IComparer roomSorter = new RoomSorter();
                foreach (Collider col in colliders)
                {
                    Collider[] intersectingColliders = Physics.OverlapBox(col.transform.position, new Vector3(col.transform.localScale.x / 2f, 1f, col.transform.localScale.z / 2f));
                    Array.Sort(intersectingColliders, roomSorter);
                    if (intersectingColliders.Length > 1)
                    {
                        seperationComplete = false;
                        foreach (Collider c in intersectingColliders)
                        {
                            if (col != c)
                            {
                                Vector3 directionToMove = col.transform.position - c.transform.position;
                                directionToMove.Normalize();
                                col.transform.position = new Vector3(col.transform.position.x + (directionToMove.x), 0f, col.transform.position.z + (directionToMove.z));
                            }
                        }
                        col.GetComponent<Room>().isSnappedToGrid = true;
                        DungeonUtilities.SnapBoundsToGrid(col.transform);
                    }
                    else if(!col.GetComponent<Room>().isSnappedToGrid)
                    {
                        col.GetComponent<Room>().isSnappedToGrid = true;
                        DungeonUtilities.SnapBoundsToGrid(col.transform);
                    }
                }
            }
            SnapRoomsToPositiveXZ();
            roomGenerationComplete = true;
            yield return null;
        }

        private void SnapRoomsToPositiveXZ()
        {
            float xShiftValue = 0;
            float zShiftValue = 0;
            maxX = 0;
            maxY = 0;

            foreach (Room room in dungeonGenerator.dungeonRooms)
            {
                if (room.GetXIndexStart() < xShiftValue)
                {
                    xShiftValue = room.GetXIndexStart();
                }
                if (room.GetXIndexEnd() > maxX)
                {
                    maxX = room.GetXIndexEnd();
                }
                if (room.GetYIndexEnd() > maxY)
                {
                    maxY = room.GetYIndexEnd();
                }
                if (room.GetYIndexStart() < zShiftValue)
                {
                    zShiftValue = room.GetYIndexStart();
                }
            }
            dungeonParent.transform.position = new Vector3(dungeonParent.transform.position.x + Mathf.Abs(xShiftValue) + 5, 0f, dungeonParent.transform.position.z + Mathf.Abs(zShiftValue) + 5);
            maxX += Mathf.Abs(xShiftValue)+10;
            maxY += Mathf.Abs(zShiftValue)+10;
        }

        private int[] GetRoomSizeDistribution()
        {
            switch (dungeonGenerator.roomSize)
            {
                case DungeonGenerator.RoomSize.small:
                    return new int[]{3, 4, 5, 6};
                case DungeonGenerator.RoomSize.medium:
                    return new int[]{4, 5, 6, 7, 7, 8, 8, 9, 9, 10, 10};
                case DungeonGenerator.RoomSize.large:
                    return new int[]{5, 6, 7, 8, 9, 10, 11, 11, 12, 12, 13, 13, 14, 14};
                case DungeonGenerator.RoomSize.smallMedium:
                    return new int[]{3, 4, 5, 6, 7, 8, 9, 10};
                case DungeonGenerator.RoomSize.mediumLarge:
                    return new int[]{4, 5, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14};
                case DungeonGenerator.RoomSize.smallLarge:
                    return new int[]{3, 4, 5, 6, 11, 12, 13, 14};
                case DungeonGenerator.RoomSize.evenSizeDistribution:
                    return new int[]{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
                default:
                    return null;
            }
        }
    }
}

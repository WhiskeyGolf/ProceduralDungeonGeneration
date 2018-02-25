using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SacredTreeStudios.DungeonGeneration
{
    public class DungeonConstructor : MonoBehaviour
    {
        private RoomGenerator roomGenerator;
        private DungeonGenerator dungeonGenerator;

        private bool debugMode;

        private int[,] dungeon2DArray;
        private const int floor = 0;
        private const int empty = 1;
        private const int wall = 2;
        private const int door = 3;

        private Vector3 rot180 = new Vector3(0, 180f, 0);
        private Vector3 rot90 = new Vector3(0, 90f, 0);
        private Vector3 rot270 = new Vector3(0, 270f, 0);

        GameObject dungeonParent;

        private void Awake()
        {
            roomGenerator = GetComponent<RoomGenerator>();
            dungeonGenerator = GetComponent<DungeonGenerator>();
        }

        public void ConstructDungeon()
        {
            dungeonParent = GameObject.FindGameObjectWithTag("DungeonParent");
            debugMode = dungeonGenerator.debugMode;
            FixScale();
            CreateDungeonArray();
            CreateRooms();
            CreateConnections();
            CreateWalls();
            CombineMeshes();
            if (debugMode)
            {
                DungeonUtilities.WriteDungeonArrayToTextFile(dungeon2DArray, empty);
            }
        }

        private void FixScale()
        {
            foreach (Room room in dungeonGenerator.dungeonRooms)
            {
                Transform scaleObject = room.transform.GetChild(0);
                scaleObject.transform.parent = null;
                scaleObject.transform.localScale = Vector3.one;
                scaleObject.transform.parent = room.transform;
            }
        }

        private void CreateDungeonArray()
        {
            dungeon2DArray = new int[(int)roomGenerator.maxX, (int)roomGenerator.maxY];
            for (int x = 0; x < dungeon2DArray.GetLength(0); x++)
            {
                for (int y = 0; y < dungeon2DArray.GetLength(1); y++)
                {
                    dungeon2DArray[x, y] = empty;
                }
            }
        }
            
        private void CreateRooms()
        {
            foreach (Room room in dungeonGenerator.dungeonRooms)
            {
                int xStart = room.GetXIndexStart();
                int xEnd = room.GetXIndexEnd();
                int yStart = room.GetYIndexStart();
                int yEnd = room.GetYIndexEnd();
                Transform tileParent = room.floorParent;
                for (int x = xStart; x <= xEnd; x++)
                {
                    for (int y = yStart; y <= yEnd; y++)
                    {
                        dungeon2DArray[x, y] = floor;
                        InstantiateFloor(x, y, tileParent);
                    }
                }
            }
        }

        private void InstantiateFloor(int x, int y, Transform parent)
        {
            Instantiate(dungeonGenerator.tileSet.floor[0], new Vector3(x, 0, y), Quaternion.Euler(rot180), parent);
        }

        private void CreateConnections()
        {
            foreach (Room startRoom in dungeonGenerator.dungeonRooms)
            {
                foreach (Room endRoom in startRoom.connectedRooms)
                {
                    Transform tileParent = startRoom.floorParent;
                    int currentX = (int)startRoom.GetCenter().x;
                    int currentY = (int)startRoom.GetCenter().y;
                    int endX = (int)endRoom.GetCenter().x;
                    int endY = (int)endRoom.GetCenter().y;
                    int xDirection = 0;
                    int yDirection = 0;
                    if (currentX > endX)
                    {
                        xDirection = -1;
                    }
                    else if (currentX < endX)
                    {
                        xDirection = 1;
                    }
                    if (currentY > endY)
                    {
                        yDirection = -1;
                    }
                    else if (currentY < endY)
                    {
                        yDirection = 1;
                    }
                    while (currentX != endX)
                    {
                        if (dungeon2DArray[currentX, currentY - 2] == empty)
                        {
                            dungeon2DArray[currentX, currentY - 2] = wall;
                            startRoom.wallsLeavingRoom.Add(Tuple.Create(currentX, currentY - 2));
                        }
                        if (dungeon2DArray[currentX, currentY + 2] == empty)
                        {
                            dungeon2DArray[currentX, currentY + 2] = wall;
                            startRoom.wallsLeavingRoom.Add(Tuple.Create(currentX, currentY + 2));
                        }
                        if (dungeon2DArray[currentX, currentY] != floor)
                        {
                            dungeon2DArray[currentX, currentY] = floor;
                            InstantiateFloor(currentX, currentY, tileParent);
                        }
                        if (dungeon2DArray[currentX, currentY - 1] != floor)
                        {
                            dungeon2DArray[currentX, currentY - 1] = floor;
                            InstantiateFloor(currentX, currentY - 1, tileParent);
                        }
                        if (dungeon2DArray[currentX, currentY + 1] != floor)
                        {
                            dungeon2DArray[currentX, currentY + 1] = floor;
                            InstantiateFloor(currentX, currentY + 1, tileParent);
                        }
                        currentX += xDirection;
                    }
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (dungeon2DArray[currentX + x, currentY + y] == empty || dungeon2DArray[currentX + x, currentY + y] == wall)
                            {
                                dungeon2DArray[currentX + x, currentY + y] = floor;
                                InstantiateFloor(currentX + x, currentY + y, tileParent);
                            }
                        }
                    }
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            if (dungeon2DArray[currentX + x, currentY + y] == empty)
                            {
                                dungeon2DArray[currentX + x, currentY + y] = wall;
                                startRoom.wallsLeavingRoom.Add(Tuple.Create(currentX + x, currentY + y));
                            }
                        }
                    }
                    while (currentY != endY)
                    {
                        if (dungeon2DArray[currentX - 2, currentY] == empty)
                        {
                            dungeon2DArray[currentX - 2, currentY] = wall;
                            startRoom.wallsLeavingRoom.Add(Tuple.Create(currentX - 2, currentY));
                        }
                        if (dungeon2DArray[currentX + 2, currentY] == empty)
                        {
                            dungeon2DArray[currentX + 2, currentY] = wall;
                            startRoom.wallsLeavingRoom.Add(Tuple.Create(currentX + 2, currentY));
                        }
                        if (dungeon2DArray[currentX, currentY] != floor)
                        {
                            dungeon2DArray[currentX, currentY] = floor;
                            InstantiateFloor(currentX, currentY, tileParent);
                        }
                        if (dungeon2DArray[currentX + 1, currentY] != floor)
                        {
                            dungeon2DArray[currentX + 1, currentY] = floor;
                            InstantiateFloor(currentX + 1, currentY, tileParent);
                        }
                        if (dungeon2DArray[currentX - 1, currentY] != floor)
                        {
                            dungeon2DArray[currentX - 1, currentY] = floor;
                            InstantiateFloor(currentX - 1, currentY, tileParent);
                        }
                        currentY += yDirection;
                    }

                }
            }
        }
            
        private void CreateWalls()
        {
            System.Random pseudoRNG = new System.Random(dungeonGenerator.seed);
            foreach (Room room in dungeonGenerator.dungeonRooms)
            {
                Transform tileParent = room.wallParent;
                int xStart = room.GetXIndexStart();
                int xEnd = room.GetXIndexEnd();
                int yStart = room.GetYIndexStart();
                int yEnd = room.GetYIndexEnd();
                //Check corners
                if (dungeon2DArray[xStart - 1, yStart -1] == empty)
                {
                    InstantiateWall(xStart - 1, yStart - 1, tileParent);
                }
                if (dungeon2DArray[xStart - 1, yEnd + 1] == empty)
                {
                    InstantiateWall(xStart - 1, yEnd + 1, tileParent);
                }
                if (dungeon2DArray[xEnd + 1, yStart - 1] == empty)
                {
                    InstantiateWall(xEnd + 1, yStart - 1, tileParent);
                }
                if (dungeon2DArray[xEnd + 1, yEnd + 1] == empty)
                {
                    InstantiateWall(xEnd + 1, yEnd + 1, tileParent);
                }
                //Check edges
                for (int x = xStart; x <= xEnd; x++)
                {   
                    if (dungeon2DArray[x, yStart - 1] == empty)
                    {
                        InstantiateWall(x, yStart - 1, tileParent);
                    }
                    if (dungeon2DArray[x, yEnd + 1] == empty)
                    {
                        InstantiateWall(x, yEnd + 1, tileParent);
                    }
                    DoorCheck(x, yStart - 1, false, tileParent, pseudoRNG);
                    DoorCheck(x, yEnd + 1, false, tileParent, pseudoRNG);
                }
                for (int y = yStart; y <= yEnd; y++)
                {
                    if (dungeon2DArray[xStart - 1, y] == empty)
                    {
                        InstantiateWall(xStart - 1, y, tileParent);
                    }
                    if (dungeon2DArray[xEnd + 1, y] == empty)
                    {
                        InstantiateWall(xEnd + 1, y, tileParent);
                    }
                    DoorCheck(xStart - 1, y, true, tileParent, pseudoRNG);
                    DoorCheck(xEnd + 1, y, true, tileParent, pseudoRNG);
                }
                //Check for corridor walls leaving the room
                foreach (Tuple<int,int> tuple in room.wallsLeavingRoom)
                {
                    int x = tuple.Item1;
                    int y = tuple.Item2;
                    if (dungeon2DArray[x, y] == wall)
                    {
                        InstantiateWall(x, y, tileParent);
                    }
                }
            }
        }

        private void DoorCheck(int x, int y, bool yCheck, Transform parent, System.Random pseudoRNG)
        {
            if (yCheck)
            {
                if (dungeon2DArray[x, y] == wall && dungeon2DArray[x, y + 1] == floor && dungeon2DArray[x, y + 2] == floor && dungeon2DArray[x, y + 3] == floor && (dungeon2DArray[x, y + 4] == empty || dungeon2DArray[x, y + 4] == wall))
                {
                    if (dungeon2DArray[x + 1, y + 2] != door && dungeon2DArray[x - 1, y + 2] != door)
                    {
                        if (pseudoRNG.NextDouble() < dungeonGenerator.doorFrequency)
                        {
                            dungeon2DArray[x, y + 2] = door;
                            Instantiate(dungeonGenerator.tileSet.door[0], new Vector3(x, 0, y + 2), Quaternion.Euler(rot90), parent);
                        }
                    }
                }
            }
            else
            {
                if (dungeon2DArray[x, y] == wall && dungeon2DArray[x + 1, y] == floor && dungeon2DArray[x + 2, y] == floor && dungeon2DArray[x + 3, y] == floor && (dungeon2DArray[x + 4, y] == empty || dungeon2DArray[x + 4, y] == wall))
                {    
                    if (dungeon2DArray[x + 2, y + 1] != door && dungeon2DArray[x + 2, y - 1] != door)
                    {
                        if (pseudoRNG.NextDouble() < dungeonGenerator.doorFrequency)
                        {
                            dungeon2DArray[x + 2, y] = door;
                            Instantiate(dungeonGenerator.tileSet.door[0], new Vector3(x + 2, 0, y), Quaternion.identity, parent);
                        }

                    }
                }
            }
        }

        private void InstantiateWall(int x, int y, Transform parent){
            dungeon2DArray[x, y] = wall;
            int floorHashValue = CellHashValue(x, y, floor);
            switch (floorHashValue)
            {
                case 0:
                    Instantiate(dungeonGenerator.tileSet.ceilingTile[0], new Vector3(x, 0, y), Quaternion.identity, parent);
                    break;
                case 1:
                    Instantiate(dungeonGenerator.tileSet.wallOneSided[0], new Vector3(x, 0, y), Quaternion.identity, parent);
                    break;
                case 2:
                    Instantiate(dungeonGenerator.tileSet.wallOneSided[0], new Vector3(x, 0, y), Quaternion.Euler(rot270), parent);
                    break;
                case 3:
                    Instantiate(dungeonGenerator.tileSet.wallTwoSidedCorner[0], new Vector3(x, 0, y), Quaternion.identity, parent);
                    break;
                case 4:
                    Instantiate(dungeonGenerator.tileSet.wallOneSided[0], new Vector3(x, 0, y), Quaternion.Euler(rot90), parent);
                    break;
                case 5:
                    Instantiate(dungeonGenerator.tileSet.wallTwoSidedCorner[0], new Vector3(x, 0, y), Quaternion.Euler(rot90), parent);
                    break;
                case 6:
                    Instantiate(dungeonGenerator.tileSet.wallTwoSided[0], new Vector3(x, 0, y), Quaternion.Euler(rot90), parent);
                    break;
                case 7:
                    Instantiate(dungeonGenerator.tileSet.wallThreeSided[0], new Vector3(x, 0, y), Quaternion.identity, parent);
                    break;
                case 8:
                    Instantiate(dungeonGenerator.tileSet.wallOneSided[0], new Vector3(x, 0, y), Quaternion.Euler(rot180), parent);
                    break;
                case 9:
                    Instantiate(dungeonGenerator.tileSet.wallTwoSided[0], new Vector3(x, 0, y), Quaternion.identity, parent);
                    break;
                case 10:
                    Instantiate(dungeonGenerator.tileSet.wallTwoSidedCorner[0], new Vector3(x, 0, y), Quaternion.Euler(rot270), parent);
                    break;
                case 11:
                    Instantiate(dungeonGenerator.tileSet.wallThreeSided[0], new Vector3(x, 0, y), Quaternion.Euler(rot270), parent);
                    break;
                case 12:
                    Instantiate(dungeonGenerator.tileSet.wallTwoSidedCorner[0], new Vector3(x, 0, y), Quaternion.Euler(rot180), parent);
                    break;
                case 13:
                    Instantiate(dungeonGenerator.tileSet.wallThreeSided[0], new Vector3(x, 0, y), Quaternion.Euler(rot90), parent);
                    break;
                case 14:
                    Instantiate(dungeonGenerator.tileSet.wallThreeSided[0], new Vector3(x, 0, y), Quaternion.Euler(rot180), parent);
                    break;
                case 15:
                    Instantiate(dungeonGenerator.tileSet.wallFourSided[0], new Vector3(x, 0, y), Quaternion.identity, parent);
                    break;
            }
        }

        private int CellHashValue(int xIndex, int yIndex, int centerTile)
        {
            int hashValue = 0;
            if (dungeon2DArray[xIndex, yIndex + 1] == centerTile)
            {
                hashValue += 1;
            }
            if (dungeon2DArray[xIndex - 1, yIndex] == centerTile)
            {
                hashValue += 2;
            }
            if (dungeon2DArray[xIndex + 1, yIndex] == centerTile)
            {
                hashValue += 4;
            }
            if (dungeon2DArray[xIndex, yIndex - 1] == centerTile)
            {
                hashValue += 8;
            }
            return hashValue;
        }

        private void CombineMeshes()
        {

            foreach (Room room in dungeonGenerator.dungeonRooms)
            {
                room.gameObject.GetComponent<BoxCollider>().enabled = false;
                DungeonUtilities.CombineChildMeshes(room.floorParent);
                room.floorParent.gameObject.AddComponent<BoxCollider>();
                DungeonUtilities.CombineChildMeshes(room.wallParent);
                room.wallParent.gameObject.AddComponent<MeshCollider>();
            }
        }
    }
}
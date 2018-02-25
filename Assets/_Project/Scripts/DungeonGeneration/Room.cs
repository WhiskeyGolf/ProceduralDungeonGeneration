using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SacredTreeStudios.DungeonGeneration
{

    public class Room : MonoBehaviour {

        public bool isSnappedToGrid { get; set; }
        public bool isStartRoom { get; set; }
        public bool isExitRoom { get; set; }

        public int roomID;

        public Transform wallParent;
        public Transform floorParent;
        public Transform objectParent;

        public List<Room> connectedRooms = new List<Room>();
        public List<Tuple<int, int>> wallsLeavingRoom = new List<Tuple<int, int>>();

        void Awake()
        {
            isSnappedToGrid = false;
            isStartRoom = false;
            isExitRoom = false;
            connectedRooms = new List<Room>();
            wallsLeavingRoom =  new List<Tuple<int, int>>();
        }

        public Vector2 GetCenter()
        {
            return new Vector2(transform.position.x, transform.position.z);
        }

        public float GetWidth()
        {
            return transform.localScale.x;
        }

        public float GetHalfWidth()
        {
            return transform.localScale.x / 2f;
        }

        public float GetLength()
        {
            return transform.localScale.z;
        }

        public float GetHalfLength()
        {
            return transform.localScale.z / 2f;
        }

        public int GetXIndexStart()
        {
            return (int) (transform.position.x - transform.localScale.x / 2f);
        }

        public int GetXIndexEnd()
        {
            return (int) (transform.position.x + transform.localScale.x / 2f) - 1;
        }

        public int GetYIndexStart()
        {
            return (int) (transform.position.z - transform.localScale.z / 2f);
        }

        public int GetYIndexEnd()
        {
            return (int) (transform.position.z + transform.localScale.z / 2f) - 1;
        }

    }
}
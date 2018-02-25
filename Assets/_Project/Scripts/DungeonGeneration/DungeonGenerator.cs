using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SacredTreeStudios.DungeonGeneration
{
    public class DungeonGenerator : MonoBehaviour {

        #region Dungeon Generator Variables
        [Range(10, 600)]
        public int numberOfRooms;

        [Range(10f, 300f)]
        public float roomSpawnRadius;

        [Range(0.05f, 1f)]
        public float corridorDensity;

        [Range(0f, 1f)]
        public float doorFrequency;

        public enum RoomSize {small, medium, large, smallMedium, smallLarge, mediumLarge, evenSizeDistribution};
        public RoomSize roomSize;

        public int seed;
        #endregion

        public DungeonTileSet tileSet;
        public bool debugMode;

        public List<Room> dungeonRooms;

        private ConnectionGenerator connectionGenerator;
        private RoomGenerator roomGenerator;
        private DungeonConstructor dungeonConstructor;

        private void Awake()
        {
            List<Room> dungeonRooms = new List<Room>();
            connectionGenerator = gameObject.GetComponent<ConnectionGenerator>();
            roomGenerator = gameObject.GetComponent<RoomGenerator>();
            dungeonConstructor = gameObject.GetComponent <DungeonConstructor>();
        }

        private void Start()
        {
            StartCoroutine(GenerateDungeon());
        }

        private IEnumerator GenerateDungeon()
        {
            roomGenerator.GenerateRooms();
            yield return new WaitUntil(() => roomGenerator.roomGenerationComplete == true);
            connectionGenerator.ConnectRooms();
            dungeonConstructor.ConstructDungeon();
            yield return null;
        }

        public void GenerateNewDungeon()
        {
            #if UNITY_EDITOR
            StopAllCoroutines();
            Debug.Log("Generating new dungeon.");
            DestroyImmediate(GameObject.FindGameObjectWithTag("DungeonParent"));
            LineRenderer[] lineRenderers = GameObject.FindObjectsOfType<LineRenderer>();
            foreach(LineRenderer lr in lineRenderers)
            {
                DestroyImmediate(lr.gameObject);
            }
            dungeonRooms = new List<Room>();
            StartCoroutine(GenerateDungeon());
            #endif
        }
    }
}

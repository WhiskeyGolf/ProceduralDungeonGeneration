using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace SacredTreeStudios.DungeonGeneration
{

    static class DungeonUtilities
    {
        public static Vector3 CalculatePointInCircle(float radius, System.Random pseudoRNG)
        {
            float angle = (float)pseudoRNG.NextDouble() * Mathf.PI * 2f;
            float r = Mathf.Sqrt((float)pseudoRNG.NextDouble()) * radius;
            return new Vector3((int)r * Mathf.Cos(angle), 0f, (int)r * Mathf.Sin(angle)); 
        }
            
        public static void SnapBoundsToGrid(Transform t)
        {
            int x = (int)t.localScale.x;
            int z = (int)t.localScale.z;
            if (x % 2 != 0 && z % 2 != 0)
            {
                t.position = new Vector3(Mathf.Round(t.position.x) + .5f, 0f, Mathf.Round(t.position.z) + .5f);
            }
            else if (x % 2 != 0)
            {
                t.position = new Vector3(Mathf.Round(t.position.x) + .5f, 0f, Mathf.Round(t.position.z));
            }
            else if (z % 2 != 0)
            {
                t.position = new Vector3(Mathf.Round(t.position.x), 0f, Mathf.Round(t.position.z) + .5f);
            }
            else
            {
                t.position = new Vector3(Mathf.Round(t.position.x), 0f, Mathf.Round(t.position.z));
            }
        }

        public static void CombineChildMeshes(Transform parent)
        {
            Material m = parent.GetChild(0).GetComponent<MeshRenderer>().material;
            Matrix4x4 parentTransform = parent.worldToLocalMatrix;
            MeshFilter[] meshFilters = new MeshFilter[parent.childCount];
            for (int x = 0; x < parent.childCount; x++)
            {
                meshFilters[x] = parent.GetChild(x).GetComponent<MeshFilter>();
            }
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length) {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = parentTransform * meshFilters[i].transform.localToWorldMatrix;
                UnityEngine.Object.Destroy(meshFilters[i].gameObject);
                i++;
            }
            parent.GetComponent<MeshFilter>().mesh = new Mesh();
            parent.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            parent.GetComponent<MeshRenderer>().material = m;
            parent.gameObject.active = true;
        }

        public static void WriteDungeonArrayToTextFile(int[,] dungeonArray, int emptyCell)
        {
            string path = "Assets/Resources/Dungeon.txt";

            using (StreamWriter streamWriter = new StreamWriter(path, false)){ 
                string dungeonString = "";
                for (int y = dungeonArray.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < dungeonArray.GetLength(0); x++)
                    {
                        if (dungeonArray[x, y] == emptyCell)
                        {
                            dungeonString += " ";
                        }
                        else
                        {
                            dungeonString += dungeonArray[x, y];
                        }
                    }
                    if (y != 0)
                    {
                        dungeonString += "\n";
                    }
                }
                streamWriter.WriteLine(dungeonString);
            }
                
            AssetDatabase.ImportAsset(path); 
            TextAsset asset = (TextAsset)Resources.Load("Dungeon");

        }
    }
}
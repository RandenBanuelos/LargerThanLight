﻿using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Obstacle and its mesh/vertices
    public static GameObject[] allObstacles;
    public static Mesh[] allMeshes;
    public static List<List<Vector3>> meshVertices;

    //Pool of N game objects
    public int poolCount = 10;
    public static List<GameObject> gameObjectPool;
    public static List<EdgeCollider2D> edgeCollider2DPool; //Keep a reference to pooled game object's edge collider
    public static bool hasResettedColliderPool = false;

    GameObject gameObjectParent;
    public GameObject originWall;

    public LightController luxControl;
    public SimpleController skiaControl;

    // Start is called before the first frame update
    void Start()
    {
        //Get all shadow-casting obstacles in scene by their tag "Obstacle" as well their mesh
        allObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        allMeshes = new Mesh[allObstacles.Length];

        //Store mesh vertices
        meshVertices = new List<List<Vector3>>();
        for (int i = 0; i < allMeshes.Length; ++i)
        {
            allMeshes[i] = allObstacles[i].GetComponent<MeshFilter>().sharedMesh;
            int numVertices = allMeshes[i].vertexCount;
            List<Vector3> vertexPositions = new List<Vector3>();
            for (int j = 0; j < numVertices; j++)
            {
                Vector3 pos = allObstacles[i].transform.localToWorldMatrix * allMeshes[i].vertices[j];
                if (!vertexPositions.Contains(pos))
                    vertexPositions.Add(pos);
            }
            Debug.Log("Adding " + vertexPositions.Count + " vertices");
            meshVertices.Add(vertexPositions);
        }

        //Create parent for pooled objects
        gameObjectParent = new GameObject();
        gameObjectParent.name = "Object Pool";

        gameObjectPool = new List<GameObject>();
        edgeCollider2DPool = new List<EdgeCollider2D>();

        PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D();
        physicsMaterial.name = "Edge collider physics material";
        physicsMaterial.friction = 0.0f;
        physicsMaterial.bounciness = 0.0f;

        //Create pool of gameobjects and store edge colliders
        for (int i = 0; i < poolCount; ++i)
        {
            GameObject gameObject = new GameObject();
            gameObject.layer = 10;
            gameObject.transform.SetParent(gameObjectParent.transform);
            gameObject.transform.position = gameObjectParent.transform.position;
            EdgeCollider2D edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider2D.sharedMaterial = physicsMaterial;
            edgeCollider2DPool.Add(edgeCollider2D);
            gameObject.SetActive(false);
            gameObjectPool.Add(gameObject);
        }
    }

    private void Update()
    {
        // For testing
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        hasResettedColliderPool = false;
    }

    public void DisableCharacterControl()
    {
        luxControl.enabled = false;
        skiaControl.rb2D.velocity = Vector2.zero;
        skiaControl.enabled = false;
        Debug.Log("control disabled");
    }

    public void EnableCharacterControl()
    {
        luxControl.enabled = true;
        skiaControl.enabled = true;
        Debug.Log("control enabled");
    }
}

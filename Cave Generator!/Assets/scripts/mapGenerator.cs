using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class MapGenerator : MonoBehaviour
{
    public int width, height;
    [Range(0, 100)]
    public int FillPercent;
    int[,] map;
    public string seed;
    public bool randomSeed;
    public TMP_InputField seedInput;
    public Slider percentSlider;

    private void Start()
    {
        GenerateMap();
    }

    void Update()
    {
       
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
        int borderSize = 1;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];
        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        MeshGenerator mesh =GetComponent<MeshGenerator>();
        mesh.GenerateMesh(borderedMap, 1);
        

    }

    void RandomFillMap()
    {
        if (randomSeed||seed==null) 
        {
            seed = Time.time.ToString();
        }
        System.Random rand = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (rand.Next(0, 100) < FillPercent) ? 1 : 0;
                }
            }
        }
    }

   /* void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }*/

    int GetSurroundingWallCount(int x, int y)
    {
        int count = -1;
        for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
        {
            for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                   
                        count += map[neighborX, neighborY];
                    
                }
                else
                {
                    count++;
                }
            }
        }
        return count;
    }
    public void SetPercent()
    {
        FillPercent = (int)percentSlider.value;
        GenerateMap();
    }
    public void SetSeed()
    {
        seed = seedInput.text;
        GenerateMap();
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    
    //private float divisor = 3725f;
    private const float divisor = 1000f;
    private const int depth = 100;
    //3725f is max value that you want to measure
    //official max is 4500f but I dont think so


    public void UpdateTerrain(float[,] depthMap, int width, int height)
    {
        //updates as every third kinect frame comes in
        Terrain terrain = GetComponent<Terrain>();
        TerrainData tD = terrain.terrainData;
        tD.heightmapResolution = width + 1;
        tD.size = new Vector3(width, depth, height);
        Debug.Log("Actual Value: "+depthMap[211, 255]);

        //depthMap = Divide(depthMap, width, height);
        depthMap = Clean(depthMap, width, height);
        Debug.Log("Multiplier: "+depthMap[211, 255]);
        tD.SetHeights(0, 0, depthMap);
        //Debug.Log(depthMap[212, 212]);
    }

    /*
    //combine 3 frames into one, hopefully more accurate than just one
    public float[,] Combine(float[,] tempMap, float[,] depthMap, int width, int height)
    {
        if (tempMap == null)
        {
            return depthMap;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tempMap[x, y] = (tempMap[x, y] + depthMap[x, y]) / 2;
            }
        }
        return tempMap;
    }
    */

    //depthMapTxt += depthMap[x, y].ToString();
    //System.IO.File.WriteAllText(@"C: \Users\Theo Levison\Desktop\Desktop\Computing\ProjectSandbox\depthMap.txt", depthMapTxt);

    private float[,] Divide(float[,] depthMap, int width, int height)
    {
        //should height and width be switched around? I think so but it breaks when that is done?! And no error is returned
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                depthMap[x, y] = depthMap[x, y] / divisor;
            }
        }
        return depthMap;
    }

    private float[,] Clean(float[,] depthMap, int width, int height)
    {
        for (int x=0; x < width; x++)
        {
            for (int y=0; y < height; y++)
            {
                if (depthMap[x, y] > divisor || depthMap[x, y] == 0)
                {
                    depthMap[x, y] = 0f;
                    depthMap = Compare4x(depthMap, x, y);
                    //depthMap = Compare9x(depthMap, x, y);
                }
                else if (depthMap[x, y] < 500f && depthMap[x, y] != 0)
                {
                    depthMap[x, y] = 1f;
                    depthMap = Compare4x(depthMap, x, y);
                    //depthMap = Compare9x(depthMap, x, y);
                }
                else
                {
                    depthMap[x, y] = (divisor - (depthMap[x, y] - 500f)) / divisor;
                    depthMap = Compare4x(depthMap, x, y);
                    //depthMap = Compare9x(depthMap, x, y);
                }
            }
        }
        return depthMap;
    }

    //Averageing functions
    //Has to point up left so that we are not comparing to values that have not been processed yet (Any value right or down of pointer has not been processed yet)

    //Should try to assign lower weight to values further away from pointer, make new function to do this
    //can we do all of this with a for loop? Yes lol

    //Use terrainData.GetSteepness to get rid of stupidly steep values??
    
    private float[,] Compare4x(float[,] depthMap, int widthIndex, int heightIndex)
    {
        if (widthIndex > 0 && heightIndex > 0)
        {
            //compare block of four pointing top left from indexed cell
            // [1][2]
            // [3][x]
            //like this

            float one = depthMap[widthIndex - 1, heightIndex - 1];
            float two = depthMap[widthIndex, heightIndex - 1];
            float three = depthMap[widthIndex - 1, heightIndex];
            float x = depthMap[widthIndex, heightIndex];

            float ave = (one + two + three + x) / 4;

            depthMap[widthIndex - 1, heightIndex - 1] = ave;
            depthMap[widthIndex, heightIndex - 1] = ave;
            depthMap[widthIndex - 1, heightIndex] = ave;
            depthMap[widthIndex, heightIndex] = ave;

            return depthMap;
        }
        else
        {
            return depthMap;
        }
    }

    //Does not work, why is this???
    private float[,] Compare9x(float[,] depthMap, int widthIndex, int heightIndex)
    {
        if (widthIndex > 1 && heightIndex > 1)
        {
            //compare block of nine pointing top left from indexed cell
            // [1][2][3]
            // [4][5][6]
            // [7][8][x]
            //like this

            float one = depthMap[widthIndex - 2, heightIndex - 2];
            float two = depthMap[widthIndex - 1, heightIndex - 2];
            float three = depthMap[widthIndex, heightIndex - 2];
            float four = depthMap[widthIndex - 2, heightIndex + 1];
            float five = depthMap[widthIndex - 1, heightIndex + 1];
            float six = depthMap[widthIndex, heightIndex + 1];
            float seven = depthMap[widthIndex - 2, heightIndex];
            float eight = depthMap[widthIndex - 1, heightIndex];
            float x = depthMap[widthIndex, heightIndex];

            float ave = (one + two + three + four + five + six + seven + eight + x) / 9;


            depthMap[widthIndex - 2, heightIndex - 2] = ave;
            depthMap[widthIndex - 1, heightIndex - 2] = ave;
            depthMap[widthIndex, heightIndex - 2] = ave;
            depthMap[widthIndex - 2, heightIndex + 1] = ave;
            depthMap[widthIndex - 1, heightIndex + 1] = ave;
            depthMap[widthIndex, heightIndex + 1] = ave;
            depthMap[widthIndex - 2, heightIndex] = ave;
            depthMap[widthIndex - 1, heightIndex] = ave;
            depthMap[widthIndex, heightIndex] = ave;

            return depthMap;
        }
        else
        {
            return depthMap;
        }
    }

    /*
    private float[,] Clean(float[,] depthMap, int width, int height)
    {
        for (int x = 1; x < width; x+=2)
        {
            for (int y = 1; y < height; y+=2)
            {
                float averageValue = 0;
                for (int i = -1; i < 2; i++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        averageValue += depthMap[x + i, y + i];
                    }
                }
                averageValue = averageValue / 9 / divisor;
                Debug.Log(averageValue);
                for (int i = -1; i < 2; i++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        depthMap[x + i, y + i] = averageValue;
                    }
                }
            }
        }
        return depthMap;
    }
    */
}
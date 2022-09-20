using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityHandler : MonoBehaviour
{
    //The colored boxes located in the room
    public SpriteRenderer[] boxes;
    //The candles located in the room
    public GameObject[] candles;
    //How many candles there are left in the room
    int totalCandles;
    //The max amount of darkness allowed in a single room
    float maxOpacity = 0.6666667f;

    // Update is called once per frame
    void Update()
    {
        //Sets the total candles to 0
        totalCandles = 0;
        //Loops though all possible candles in the room
        foreach (GameObject g in candles) {
            //If the gameobject doesnt exist anymore continue the loop (skipping this index)
            if (g == null) continue;
            //If the object still exists add to total candles
            if (GameObject.Find(g.name))
            {
                totalCandles++;
            }
        }
        
        //Loops though all the colored boxes in the room and sets there opacity
        foreach (SpriteRenderer render in boxes)
        {
            //Gets the darkness of the room based
            //maxOpacity - ((maxOpacity / how many candles there are in the room) * how many candles are left)
            float darkness = maxOpacity - ((maxOpacity / candles.Length) * totalCandles);
            //Sets the color with the darkness included
            render.color = new Color(render.color.r, render.color.g, render.color.b, darkness);
        }
    }
}

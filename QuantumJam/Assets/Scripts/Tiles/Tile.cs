using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    public enum Type { Blank, Start, Empty, BlackHole, QuantumZone, RSS }
    public Type TileType;
    public string TileDescription;

    private void Start() {
        // if (TileType == Type.Blank) GetComponent<SpriteRenderer>().enabled = false;
    }
}

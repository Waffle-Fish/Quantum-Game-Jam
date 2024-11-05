using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Tile : MonoBehaviour
{
    public enum Type { Empty, RSS, BlackHole, QuantumZone }
    public Type TileType {get; private set;}
    public string TileDescription;
}

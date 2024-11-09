using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum Item { RepairKit, Fuel, Probe}
    public Item ItemType;
    public int val;
}

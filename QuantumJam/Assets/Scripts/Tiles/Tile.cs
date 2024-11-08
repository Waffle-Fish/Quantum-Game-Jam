using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    public enum Type { Blank, Start, Empty, BlackHole, QuantumZone, RSS }
    public Type TileType;
    public string TileDescription;
    public Pickup pickup;

    public bool EnableSpriteRenderOnAwake = true;

    SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        spriteRenderer.enabled = EnableSpriteRenderOnAwake;
        if(pickup) {
            pickup.gameObject.SetActive(false);
            StartCoroutine(PlacePickup());
        }
    }

    IEnumerator PlacePickup() {
        yield return null;
        pickup.transform.position = transform.position;
        pickup.gameObject.SetActive(true);
    }
}

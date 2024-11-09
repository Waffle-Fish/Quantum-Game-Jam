using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField]
    private GameObject hoverOverlay;

    private Vector3 mouseWorldPosition;
    private GameBoardManager gbm;


    private void Start() {
        gbm = GameBoardManager.Instance;
        hoverOverlay.SetActive(false);
        
    }

    private void Update() {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        HoverTile();
        DetectLeftClick();
        DetectRightClick();
    }

    private void DetectRightClick() {
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;
        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(clickRay);
        if (hit.collider && hit.collider.CompareTag("Tile")) {
            hoverOverlay.SetActive(false);
            gbm.SelectTile(hit.collider.gameObject);
            gbm.MovePlayer();
        };
    }

    private void DetectLeftClick() {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(clickRay);
        if (hit.collider && hit.collider.CompareTag("Tile")) {
            gbm.SelectTile(hit.collider.gameObject);
        };
    }

    private void HoverTile() {
        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(clickRay);
        if (hit.collider && hit.collider.CompareTag("Tile")) {
            hoverOverlay.transform.position = hit.collider.transform.position;
            // play sfx
            hoverOverlay.SetActive(true);
        } else {
            hoverOverlay.SetActive(false);
        }
    }

}
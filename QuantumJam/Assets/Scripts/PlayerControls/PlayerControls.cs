using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private Vector3 mouseWorldPosition;
    private GameBoardManager gbm;

    private void Start() {
        gbm = GameBoardManager.Instance;
    }

    private void Update() {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DetectLeftClick();
    }

    private void DetectLeftClick() {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(clickRay);
        if (hit.collider && hit.collider.CompareTag("Tile")) {
            gbm.ProcessMovePlayer(hit.collider.gameObject);
        };
    }

}
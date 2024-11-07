using System.Collections;
using System.Collections.Generic;
using QRG.QuantumForge.Runtime;
using UnityEngine;

[RequireComponent(typeof(QuantumProperty))]
public class QuantumZone : MonoBehaviour
{
    public QuantumProperty TileQP {get; private set;}
    public ProbabilityTracker ProbabilityTracker {get; private set;}

    [SerializeField]
    private QuantumZone zonePair;

    SpriteRenderer spriteRender;

    public bool IsMeasured = false;

    private void Awake() {
        TileQP = GetComponent<QuantumProperty>();
        ProbabilityTracker = GetComponent<ProbabilityTracker>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if (!zonePair) Debug.LogError("Missing Zone Pair");
        QuantumProperty.Hadamard(TileQP);
    }

    private void Update() {
        QuantumProperty.NCycle(TileQP,zonePair.TileQP);
    }
    
    public void Measure() {
        if (IsMeasured) return;
        // QuantumProperty.NCycle(TileQP,zonePair.TileQP);
        QuantumProperty.Measure(zonePair.TileQP);
        QuantumProperty.Measure(TileQP);
        // if (ProbabilityTracker.GetBasisProbabilities() == zonePair.ProbabilityTracker.GetBasisProbabilities()) QuantumProperty.Cycle(TileQP);
        UpdateColor();
        zonePair.UpdateColor();
        IsMeasured = true;
        zonePair.IsMeasured = true;
    }

    public void Cycle() {
        TileQP.Cycle();
        
    }

    public void UpdateColor() {
        if (IsSafe()) spriteRender.color = Color.blue;
        else spriteRender.color = Color.red;
    }

    public bool IsSafe() {
        return Mathf.Approximately(ProbabilityTracker.GetBasisProbabilities()[0].Probability,1f);
    }
}

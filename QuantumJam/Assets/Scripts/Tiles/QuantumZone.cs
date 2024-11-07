using System.Collections;
using System.Collections.Generic;
using QRG.QuantumForge.Runtime;
using UnityEngine;

[RequireComponent(typeof(QuantumProperty))]
public class QuantumZone : MonoBehaviour
{
    public QuantumProperty TileQP {get; private set;}
    public ProbabilityTracker ProbabilityTracker {get; private set;}
    public bool IsMeasured = false;

    [SerializeField]
    private QuantumZone zonePair;
    SpriteRenderer spriteRender;

    private void Awake() {
        TileQP = GetComponent<QuantumProperty>();
        ProbabilityTracker = GetComponent<ProbabilityTracker>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if (!zonePair) Debug.LogError("Missing Zone Pair");
    }

    private void Update() {
        UpdateColors();
    }
    
    public void Measure() {
        if (IsMeasured) return;
        QuantumProperty.Hadamard(zonePair.TileQP);
        NCycle21();
        MeasureSelf();
        if (UnityEngine.Random.Range(0, 1) == 0) MeasureSelf();
        else MeasurePair();
        IsMeasured = true;
        zonePair.IsMeasured = true;
    }

    private void UpdateColors() {
        if (!IsMeasured) return;
        UpdateColor();
        zonePair.UpdateColor();
    }

    public void UpdateColor() {
        if (IsSafe()) {
            spriteRender.color = Color.blue;
        }
        else {
            spriteRender.color = Color.red;
        }
    }

    public bool IsSafe() {
        // if (ProbabilityTracker.Probabilities.Length == 4) return Mathf.Approximately(ProbabilityTracker.Probabilities[2].Probability, 1f);
        // else return Mathf.Approximately(ProbabilityTracker.Probabilities[0].Probability, 1f);
        var probs = ProbabilityTracker.Probabilities;
        foreach (var basisProbability in probs)
        {
            foreach (var item in basisProbability.QuditValues)
            {
                Debug.Log(item);
            }
        }
        return true;
    }

    #region Qunatum Helper Functions
    public void Cycle() {
        TileQP.Cycle();
    }

    public void CyclePartner() {
        zonePair.Cycle();
    }

    public void NCycle12() {
        QuantumProperty.NCycle(TileQP,zonePair.TileQP);
    }
    public void NCycle21() {
        QuantumProperty.NCycle(zonePair.TileQP,TileQP);
    }

    public void MeasureSelf() {
        int[] arr = QuantumProperty.Measure(TileQP);
    }

    public void MeasurePair() {
        QuantumProperty.Measure(zonePair.TileQP);
    }

    public void HadamardSelf() {
        QuantumProperty.Hadamard(TileQP);
    }

    public void HadamardPair() {
        QuantumProperty.Hadamard(zonePair.TileQP);
    }
    #endregion
}

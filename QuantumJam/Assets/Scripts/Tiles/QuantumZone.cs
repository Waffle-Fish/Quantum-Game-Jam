using System.Collections;
using System.Collections.Generic;
using QRG.QuantumForge.Runtime;
using Unity.VisualScripting;
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
    QuantumProperty zonePairQP;

    public float DangerProbability {get; private set;}
    public float SafeProbability {get; private set;}

    private void Awake() {
        TileQP = GetComponent<QuantumProperty>();
        ProbabilityTracker = GetComponent<ProbabilityTracker>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if (!zonePair) Debug.LogError("Missing Zone Pair");
        zonePairQP = zonePair.GetComponent<QuantumProperty>();
        // if (ProbabilityTracker.Probabilities.Length != 2 || ProbabilityTracker.Probabilities.Length != 4) {
        //     Debug.LogError("Probability tracker doesnt have 1 or 2 Quantum Properties");
        // }
        // StartCoroutine(UpdateDangerProbability());
        // StartCoroutine(UpdateSafeProbability());
    }

    private void Update() {
        UpdateColors();
        UpdateProbabilities();
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
        StartCoroutine(UpdateColor());
        StartCoroutine(zonePair.UpdateColor());
    }

    IEnumerator UpdateColor() {
        yield return null;
        if (IsSafe()) {
            spriteRender.color = Color.blue;
        }
        else {
            spriteRender.color = Color.red;
        }
    }

    public bool IsSafe() {
        if (ProbabilityTracker.Probabilities.Length == 4) return Mathf.Approximately(ProbabilityTracker.Probabilities[2].Probability, 1f);
        else return Mathf.Approximately(ProbabilityTracker.Probabilities[0].Probability, 1f);
    }

    public void PhaseAll(float rotVal) {
        HadamardSelf();
        HadamardPair();
        PhaseRotateSelf(rotVal);
        HadamardSelf();
        HadamardPair();
        NCycle12();
    }

    private void UpdateProbabilities() {
        DangerProbability = ProbabilityTracker.Probabilities[1].Probability;
        if (ProbabilityTracker.Probabilities.Length == 4) {
            SafeProbability = ProbabilityTracker.Probabilities[2].Probability;
        } else {
            SafeProbability = ProbabilityTracker.Probabilities[0].Probability;
        }
    }

    IEnumerator UpdateSafeProbability() {
        yield return null;
        if (ProbabilityTracker.Probabilities.Length == 4) {
            Debug.Log("Chance of Safe" + ProbabilityTracker.Probabilities[2].Probability);
            SafeProbability = ProbabilityTracker.Probabilities[2].Probability;
        } else {
            Debug.Log("Chance of Safe" + ProbabilityTracker.Probabilities[0].Probability);
            SafeProbability = ProbabilityTracker.Probabilities[0].Probability;
        }
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

    public void PhaseRotateSelf(float val) {
        TileQP.PhaseRotate(val, TileQP.BasisValues.values[0]);
    }

    public void PhaseRotatePair(float val) {
        zonePairQP.PhaseRotate(val, zonePairQP.BasisValues.values.ToArray());
    }
    #endregion
}

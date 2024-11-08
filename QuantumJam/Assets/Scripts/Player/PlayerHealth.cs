using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set;}
    [SerializeField]
    private int maxHealth;
    public int CurrentHealth {get; private set;}

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void DamagePlayer() {
        CurrentHealth--;
        if (CurrentHealth <= 0) {
            ProcessDeath();
        }
    }

    public void HealPlayer(int val) {
        if (CurrentHealth < maxHealth) {
            CurrentHealth += val;
        }
    }

    private void ProcessDeath() {
        Debug.Log("Player has died!");
    }
}

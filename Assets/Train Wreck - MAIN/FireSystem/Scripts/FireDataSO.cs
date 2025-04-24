using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FireSO", menuName = "Scriptable Objects/Fire/FireData")]
public class FireDataSO : ScriptableObject, IHealthDataAdapter
{
    [field: Header("Fire Settings")]
    [field: SerializeField] public GameObject FirePrefab { get; set; }
    [field: SerializeField] public GameObject FireOnPlayerVFX { get; set; }
    [field: SerializeField] public float NearbyObjectRadius { get; private set; }  = 2f;
    [field: SerializeField] public float DamageAmount { get; private set; } = 10f;
    [field: SerializeField] public float FireSpawnDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float FireOnPlayerDuration { get; private set; } = 5f;
    [field: Header("Active Fires")]
    [ShowInInspector, NonSerialized] public List<GameObject> ActiveFires;
    [field: Header("Health Settings")]
    [field: Tooltip("This is the health of the fire")]
    [field: SerializeField] public float Health { get; private set; } = 100f;
    [field: Tooltip("This is the value the train gains back when each fire is destroyed")]
    [field: SerializeField] public float HealDamagedAmount { get; private set; } = 10f;
}
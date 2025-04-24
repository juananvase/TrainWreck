using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    [field: SerializeField] public bool IsTargetable { get; set; } = true;
    [field: SerializeField] public int Team { get; private set; } = 1;
}

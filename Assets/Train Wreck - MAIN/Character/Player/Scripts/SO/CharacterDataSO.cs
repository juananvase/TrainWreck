using UnityEngine;
using GameEvents;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/Characters/CharacterData")]
public class CharacterDataSO : ScriptableObject
{
    [field: Header("Movement")]
    [field: SerializeField] public float MoveSpeed { get; protected set; } = 5f;
    [field: SerializeField] public float Acceleration { get; protected set; } = 10f;
    [field: SerializeField] public float RotationSpeed { get; protected set; } = 10f;
    [field: SerializeField] public float RotationSpeedMultiplier { get; protected set; } = 1f;
    [field: SerializeField] public float Gravity { get; protected set; } = -20f;
    [field: Header("Knockback")]
    [field: SerializeField] public float KnockbackForceToOther { get; protected set; } = -20f;
    [field: SerializeField] public Vector3 UpwardDirection { get; protected set; }
    
    [field: Header("On Fire")]
    [field: SerializeField] public float RandomDirectionChangeInterval { get; private set; } = 0.5f;
    [field: SerializeField] public Vector3 MinBounds { get; private set; } = Vector3.one;
    [field: SerializeField] public Vector3 MaxBounds { get; private set; }  = Vector3.one;
    [field: SerializeField] public float MaxRespawnTime { get; protected set; } = 10f;

    [field: Header("On Fire")]
    [field: SerializeField] public float OnFireSpeed { get; set; } = 1f;
    [field: SerializeField] public float OnFireDestroyDuration { get; set; } = 3f;
    [field: Header("Stunned")]
    [field: SerializeField] public float StunDuration { get; set; } = 5f;
    [field: SerializeField] public float StunInVulnerabilityDuration { get; set; } = 5f;
    [field: Tooltip("The velocity accumulated when an object is thrown. The higher the value means objects throw from closer has higher chance to get you stunned.")]
    [field: SerializeField] public float VelocityThreshold { get; set; } = 5f;
    [field: Header("Throw")]
    [field: SerializeField] public float ThrowForce { get; set; } = 20f;
    [field: Header("Dash")]
    [field: Tooltip("This value changes based on if there are colliders infront")]
    [field: SerializeField] public float DashDistance { get; set; } = 2f;
    [field: Tooltip("This is the default dash distance when there are no colliders infront")]
    [field: SerializeField] public float DefaultDashDistance { get; protected set; } = 2f;
    [field: SerializeField] public float DashSpeed { get; protected set; } = 10f;
    [field: SerializeField] public float DashToWallDistance { get; protected set; } = 2f;
    [field: SerializeField] public float DashDuration { get; protected set; } = 1f;
    [field: SerializeField] public float DashCooldown { get; protected set; } = 2f;
    [field: SerializeField] public float DashTrailDuration { get; protected set; } = 2f;
    [field: SerializeField] public float DashHeight { get; protected set; } = 0.5f;
    [field: Header("Interaction")]
    [field: SerializeField] public Vector3 InteractionRange { get; protected set; }
    [field: SerializeField] public Vector3 InteractionOffset { get; protected set; } 
    [field: Header("Grounding")]
    [field: SerializeField] public float GroundCheckDistance { get; protected set; } = 0.1f;
    [field: SerializeField] public float GroundCheckOffset { get; protected set; } = 0.4f;
    [field: Header("Layer Masks")]
    [field: SerializeField] public LayerMask GroundMask { get; protected set; } = 1 << 0;
    [field: SerializeField] public LayerMask InteractableMask { get; protected set; } = 1 << 0;
    [field: SerializeField] public LayerMask TrainLayerMask { get; private set; }
    [field: SerializeField] public LayerMask PlayerLayerMask { get; private set; }

    [field: Header("Game Events")]
    [field: SerializeField] public PlayerInputEventAsset OnPassingPlayerInputComponent { get; private set; }
    [field: SerializeField] public PlayerInputEventAsset OnPausingGame { get; private set; }
    [field: SerializeField] public PlayerInputEventAsset OnUnpausingGame { get; private set; }
    [field: SerializeField] public GameObjectEventAsset OnBombExplodeInHand { get; private set; }
}
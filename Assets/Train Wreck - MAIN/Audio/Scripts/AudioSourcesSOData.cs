using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable Objects/AudioSourcesData")]
public class AudioSourcesSOData : ScriptableObject
{
    [field: Header("Character SFX")]
    [field: SerializeField] public EventReference HopSFX { get; private set; }
    [field: SerializeField] public EventReference ThrowSFX { get; private set; }
    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference EnemyGunSFX { get; private set; }
    [field: SerializeField] public EventReference BomberAppearanceSFX { get; private set; }
    [field: SerializeField] public EventReference EnemyEntranceSFX { get; private set; }
    [field: SerializeField] public EventReference EnemyDeathSFX { get; private set; }
    [field: SerializeField] public EventReference EnemyExplosionSFX { get; private set; }
    [field: Header("Gun SFX")]
    [field: SerializeField] public EventReference MachineGunSFX { get; private set; }
    [field: SerializeField] public EventReference ReloadingBulletsSFX { get; private set; }
    [field: Header("Wood SFX")]
    [field: SerializeField] public EventReference PlaceWoodMaterial { get; private set; }
    [field: SerializeField] public EventReference BrokenWindowSFX { get; private set; }
    [field: Header("Pipe SFX")]
    [field: SerializeField] public EventReference BrokenPipeSFX { get; private set; }
    [field: SerializeField] public EventReference PlaceMetalMaterial { get; private set; }
    [field: Header("Bomb SFX")]
    [field: SerializeField] public EventReference BombTickingSFX { get; private set; }
    [field: SerializeField] public EventReference BombExplosionSFX { get; private set; }
    [field: Header("Resource Boxes SFX")]
    [field: SerializeField] public EventReference UseResourceStationSFX { get; private set; }
    [field: SerializeField] public EventReference UseCoalPileSFX { get; private set; }
    [field: Header("Lever SFX")]
    [field: SerializeField] public EventReference ActiveLeverSFX { get; private set; }
    [field: SerializeField] public EventReference InActiveLeverSFX { get; private set; }
    [field: Header("Repair SFX")]
    [field: SerializeField] public EventReference RepairedSFX { get; private set; }
    [field: Header("Fixing SFX")]
    [field: SerializeField] public EventReference FixingSFX { get; private set; }
    [field: Header("Firebox SFX")]
    [field: SerializeField] public EventReference FireBoxBurn { get; private set; }
    [field: SerializeField] public EventReference FuelingFirebox { get; private set; }
    [field: Header("Music")]
    [field: SerializeField] public EventReference GameMusic { get; private set; }
    [field: Header("Extinguisher SFX")]
    [field: SerializeField] public EventReference ExtinguisherSFX { get; private set; }
    [field: Header("Fire SFX")]
    [field: SerializeField] public EventReference FireAlarmSFX { get; private set; }
    [field: SerializeField] public EventReference FireboxExplosionSFX { get; private set; }
    [field: Header("Steam SFX")]
    [field: SerializeField] public EventReference SteamSFX { get; private set; }
    [field: Header("Train SFX")]
    [field: SerializeField] public EventReference TrainMovingSFX { get; private set; }
    [field: SerializeField] public EventReference TrainIdleSFX { get; private set; }
    [field: SerializeField] public EventReference ChooChooSFX { get; private set; }
    [field: SerializeField] public EventReference TrainExplosionSFX { get; private set; }
    [field: Header("Pickup SFX")]
    [field: SerializeField] public EventReference PickupPipeSFX { get; private set; }
    [field: SerializeField] public EventReference PickupWoodSFX { get; private set; }
    [field: SerializeField] public EventReference PickupBucketSFX { get; private set; }
    [field: SerializeField] public EventReference AmmoPickupSFX { get; private set; }
    [field: SerializeField] public EventReference ExtinguisherPickupSFX { get; private set; }
    [field: SerializeField] public EventReference BombPickupSFX { get; private set; }
    [field: Header("Drop SFX")]
    [field: SerializeField] public EventReference DropPipeSFX { get; private set; }
    [field: SerializeField] public EventReference DropWoodSFX { get; private set; }
    [field: SerializeField] public EventReference DropBucketSFX { get; private set; }
    [field: SerializeField] public EventReference DropAmmoSFX { get; private set; }
    [field: SerializeField] public EventReference DropExtinguisherSFX { get; private set; }
    [field: SerializeField] public EventReference DropBombSFX { get; private set; }
    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference UISelectSFX { get; private set; }
    [field: SerializeField] public EventReference UIMoveSFX { get; private set; }
}

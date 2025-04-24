using UnityEngine;

public class TriggerIconMountedGun : TriggerIcon
{
    [SerializeField] private MountedGun _mountedGun;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_mountedGun == null)
            Debug.LogWarning("MountedGun component missing");
    }

    protected override void CheckDisplayButton(Collider other)
    {
        if (other.TryGetComponent(out Interactor interactor))
        {
            AmmoPickup ammo = interactor.ObjectInHand as AmmoPickup;

            if (ammo != null && !_mountedGun.FullyReloaded)
            {
                _icon.SetActive(true);
            }

        }
    }
}

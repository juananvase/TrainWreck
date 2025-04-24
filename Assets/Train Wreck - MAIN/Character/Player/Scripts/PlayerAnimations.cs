using System;
using GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimations : MonoBehaviour
{
    [field: Header("Animation States")]
    [field: SerializeField] public PlayerStateMachine player { get; private set; }
    [SerializeField] private BoolEventAsset _onLevelWin;
    private MountedGun _mountedGun;

    //Hashes for animator parameters
    private static readonly int OnWinHash = Animator.StringToHash("OnWin");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int HoldingItemHash = Animator.StringToHash("HoldingItem");
    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");
    private static readonly int IsStunnedHash = Animator.StringToHash("IsStunned");
    private static readonly int OnMountedGunHash = Animator.StringToHash("OnMountedGun");
    private static readonly int OnFireHash = Animator.StringToHash("OnFire");
    private static readonly int IsInteractingHash = Animator.StringToHash("IsInteracting");
    private static readonly int IsThrowingHash = Animator.StringToHash("IsThrowing");
    private static readonly int IsAimingHash = Animator.StringToHash("IsAiming");
    private static readonly int IsKillingFireHash = Animator.StringToHash("IsKillingFire");
    private static readonly int IsRefuelHash = Animator.StringToHash("IsRefuel");

    private void OnEnable()
    {
        _onLevelWin.AddListener(OnLevelWin);
    }
    private void OnDisable()
    {
        _onLevelWin.RemoveListener(OnLevelWin);

    }

    private void OnLevelWin(bool value)
    {
        player.Animator.SetBool(OnWinHash, true);
    }

    private void Update()
    {
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        if (player.Interactor.ObjectInHand != null && player.IsMounted) 
        {
            if (player.Interactor.ObjectInHand.TryGetComponent(out MountedGun mountedGun)) _mountedGun = mountedGun;
        }
        player.Animator.SetBool(IsMovingHash, player.IsMoving);
        player.Animator.SetBool(HoldingItemHash, player.Interactor.ObjectInHand != null);
        if (player.IsDashing)
        {
            player.Animator.SetTrigger(IsDashingHash);
        }
        else
        {
            player.Animator.ResetTrigger(IsDashingHash);
        }
        player.Animator.SetBool(IsStunnedHash, player.IsStunned);
        player.Animator.SetBool(OnMountedGunHash, player.IsMounted);
        player.Animator.SetBool(OnFireHash, player.IsOnFire);
        player.Animator.SetBool(IsInteractingHash, TriggerInteractionAnimation());
        player.Animator.SetBool(IsThrowingHash, TriggerThrowAnimation());
        player.Animator.SetBool(IsAimingHash, TriggerThrowAnimation());
        player.Animator.SetBool(IsKillingFireHash, TriggerExtinguisherAnimation());
        player.Animator.SetBool(OnMountedGunHash, player.IsMounted);
        player.Animator.SetBool(IsRefuelHash, player.IsReloadingGun);
    }

    private bool TriggerThrowAnimation() 
    {
        return player.ThrowInput >= 0.1f && player.Interactor.ObjectInHand;
    }

    private bool TriggerInteractionAnimation()
    {
        if (player.UsePressed) 
        {
            Extinguisher extinguisher = player.Interactor.ObjectInHand as Extinguisher;
            if (extinguisher != null) 
            {
                return true;
            } 
            if (player.Interactor.ObjectInHand == null)
            {
                return player.Interactor.IsFixing;
            }

        }
        return false;
    }

    private bool TriggerExtinguisherAnimation() 
    {
        if (player.Interactor.ObjectInHand != null && player.Interactor.ObjectInHand.TryGetComponent(out Extinguisher extinguisher))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

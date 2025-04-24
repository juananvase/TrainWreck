using System.Collections;
using UnityEngine;

public class GotHit : MonoBehaviour
{
    [field: SerializeField] private PlayerStateMachine Player;

    private bool _canStunAgain = true;

    private void OnValidate()
    {
        if (Player == null)
        {
            Player = GetComponent<PlayerStateMachine>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_canStunAgain == false)
        {
            return;
        }
        
        if (collision.collider.TryGetComponent(out Bullet bullet))
        {
            BulletDataSO bulletData = bullet.BulletData;
            if (bulletData != null && bulletData.Team != 0)
            {
                Player.SetState(new StunnedState(Player, Player.states));
                StartCoroutine(StunInVulnerabilityChecker());
            }
        }
    }

    private IEnumerator StunInVulnerabilityChecker()
    {
        _canStunAgain = false;
        yield return new WaitForSeconds(Player.CharacterDataSO.StunDuration + Player.CharacterDataSO.StunInVulnerabilityDuration);
        _canStunAgain = true;
    }
}

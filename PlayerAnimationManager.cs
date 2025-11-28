using UnityEngine;
using Unity.Netcode;

public class PlayerAnimationManager : NetworkBehaviour
{
    [SerializeField] private NetPlayerAnimator _animations;
    [SerializeField] private ClientPlayerAnimator _clientAnimator;

    private void Awake()
    {        
        _clientAnimator.enabled = false;
        _animations.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {            
            _clientAnimator.enabled = true;
        }        

        if (IsServer)
        {
            _animations.enabled = true;
        }
    } 


    [Rpc(target: SendTo.NotServer)]
    private void SendCurrentAnimStateRpc(float fade, AnimancerStateData[] states)
    {
        _clientAnimator.RecieveAnimationData(fade, states);
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
               
        _animations.GatherAnimData();
        AnimancerStateData[] states = _animations.animStates.ToArray();
        float fade = _animations.remainingFadeDuration;
        SendCurrentAnimStateRpc(fade, states);
    }
}





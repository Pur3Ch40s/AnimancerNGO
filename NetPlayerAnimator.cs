using Animancer;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetPlayerAnimator : MonoBehaviour
{    
    [SerializeField] private AnimancerComponent _animancer;
        
    public float remainingFadeDuration;    
    public List<AnimancerStateData> animStates = new();
    
    [SerializeField] private StringAsset idle;
    [SerializeField] private StringAsset move;

    public void PlayMovementAnim(bool isMoving)
    {
        if (isMoving)
        {
            _animancer.TryPlay(move);
        }
        else
        {
            _animancer.TryPlay(idle);
        }
    }

    public void GatherAnimData()
    {
        animStates.Clear();
        remainingFadeDuration = 0;        

        IReadOnlyIndexedList<AnimancerState> activeStates = _animancer.Layers[0].ActiveStates;
        for (int i = 0; i < activeStates.Count; i++)
        {
            AnimancerState state = activeStates[i];

            animStates.Add(new AnimancerStateData()
            {
                index = (byte)_animancer.Graph.Transitions.IndexOf(state.Key),
                time = state.Time,
                weight = state.Weight,
            });

            if (state.FadeGroup != null && state.TargetWeight == 1)
            {
                remainingFadeDuration = state.FadeGroup.RemainingFadeDuration;

                if (i > 0)
                {
                    (animStates[0], animStates[i]) = (animStates[i], animStates[0]);
                }
            }           
        }            
    }
}

public struct AnimancerStateData : INetworkSerializable
{
    public byte index;
    public float time;
    public float weight;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(index);
            serializer.GetFastBufferWriter().WriteValueSafe(time);
            serializer.GetFastBufferWriter().WriteValueSafe(weight);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out index);
            serializer.GetFastBufferReader().ReadValueSafe(out time);
            serializer.GetFastBufferReader().ReadValueSafe(out weight);
        }        
    }
}
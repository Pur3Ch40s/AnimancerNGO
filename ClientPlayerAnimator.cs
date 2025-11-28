using Animancer;
using UnityEngine;
using System.Collections.Generic;
using Animancer.TransitionLibraries;
using System.Linq;

public class ClientPlayerAnimator : MonoBehaviour
{
    [SerializeField] AnimancerComponent _animancerComponent;

    private float _fade;
    private List<AnimancerStateData> _states = new();

    public void RecieveAnimationData(float remainingFade, AnimancerStateData[] animSates)
    {
        _fade = remainingFade;
        _states = animSates.ToList();
        PlayAnimation();
    }


    public void PlayAnimation()
    {
        AnimancerLayer layer = _animancerComponent.Layers[0];
        layer.Stop();
        layer.Weight = 1;       

        AnimancerState firstState = null;

        for (int i = _states.Count - 1; i >= 0; i--)
        {
            AnimancerStateData stateData = _states[i];        

            if (!_animancerComponent.Graph.Transitions.TryGetTransition(stateData.index, out TransitionModifierGroup transition))
            {
                Debug.LogError(
                    $"Transition Library '{_animancerComponent.Transitions}'" +
                    $" doesn't contain transition index {stateData.index}.",
                    _animancerComponent);
                continue;
            }
                               

            AnimancerState state = layer.GetOrCreateState(transition.Transition);
            state.IsPlaying = true;
            state.Time = stateData.time;
            state.SetWeight(stateData.weight);

            if(i == 0)
            {
                firstState = state;
            }

            layer.TryPlay(firstState, _fade);
        }            
    }   
}

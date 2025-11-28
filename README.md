# AnimancerNGO
A very basic server-authoritative example combining [Animancer]([url](https://kybernetik.com.au/animancer/)) and Unity's [Netcode for GameObjects]([url](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/index.html)).

There are three scripts:
* PlayerAnimationManager : Gathers the current animation data from the net animator once per fixed update and sends it to each client.
* NetPlayerAnimator  : Runs only on the server, takes some request to play an animation and has a method to gather and pack the current animation data. Can be expanded to include additional parameters such as remaining fade duration.
* ClientPlayerAnimator : Runs only on the client, unpacks the animation data and uses the local transition library to play the animation locally.

This simple implementation means that all logic runs on the server and the animation itself is also played on the server instance, which means things like Animancer Events could be used to activate logic at specific server-ticks. All the animation clips, fades, transitions and parameters are all stored on each client's transition library so only a small amount of data is sent in each Rcp. The obvious downside is that the responsiveness for the player will depend on the latency. The delay between moving and seeing the walk animation may lead to a bad experience. This can be solved using the standard [client prediction and reconcillation pattern]([url](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/learn/dealing-with-latency.html)) and would require the animation to play locally first, then compare itself to the animation data sent from the server. 

Video: https://youtu.be/bcsbjvwo1xk 


namespace PlusVariants;

using System;

using Godot;

[GlobalClass]
public partial class AnimationPlayerPlus : AnimationPlayer
{
    private bool _restartingAnimation = false;

    private AnimationFinishedEventHandler _playQueue = null;
    
    public void PlayOrRestart(StringName name)
    {
        PlayOrRestartFull(name);
    }

    public void PlayOrRestartFull(StringName name, double customBlend = -1, float customSpeed = 1f, bool fromEnd = false)
    {
        if (CurrentAnimation == name && !_restartingAnimation)
        {
            _restartingAnimation = true;
            Play("RESET", customBlend, customSpeed, fromEnd);
            
            _playQueue = (StringName animationName) =>
            {
                if (animationName == "RESET")
                {
                    _restartingAnimation = false;
                    Play(name, customBlend, customSpeed, fromEnd);
                    AnimationFinished -= _playQueue;
                }
            };

            AnimationFinished += _playQueue;
        }
        else
        {
            Play(name, customBlend, customSpeed, fromEnd);
        }
    }
}

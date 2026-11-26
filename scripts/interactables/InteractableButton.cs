using CastInfo2D;
using Godot;
using Puppets.Bodies;
using System;

public partial class InteractableButton : Node2D, IInteractable, IPopup
{
    private PlayerBodyTop2D _player = null;

    public bool IsBeingInteractedWith { get => _player != null; }

    [Export]
    private bool _lockOnInteraction = false;

    [Export]
    private string[] _popupText = null;

    [Export]
    private float _range = 128f;

    [Export]
    private RayCastInfo2D _lineOfSightCast = null;

    [Signal]
    public delegate void InteractionStartedEventHandler(PlayerBodyTop2D player);    

    [Signal]
    public delegate void InteractionStoppedEventHandler(PlayerBodyTop2D player);

    private bool PlayerInRangeAndSight(PlayerBodyTop2D player)
    {
        float distance = GlobalPosition.DistanceTo(player.GlobalPosition);

        if (distance > _range)
        {
            return false;
        }

        PhysicsRayQueryParameters2D query = _lineOfSightCast.CreateQuery(GlobalPosition, player.GlobalPosition);
        RayCastResults2D results = RayCastInfo2D.IntersectRay(GetWorld2D().DirectSpaceState, query);
        return results.Collider == player;
    }

    public bool StartInteraction(PlayerBodyTop2D player)
    {
        if (player == null)
        {
            throw new ArgumentNullException("player");
        }

        if (!PlayerInRangeAndSight(player))
        {
            return false;
        }

        _player = player;
        EmitSignal(SignalName.InteractionStarted, player);
        return _lockOnInteraction;
    }

    public void StopInteraction(PlayerBodyTop2D player)
    {
        _player = null;
        EmitSignal(SignalName.InteractionStopped, player);
    }

    public string[] GetText(PlayerBodyTop2D player)
    {
        if (player == null)
        {
            throw new ArgumentNullException("player");
        }

        if (!PlayerInRangeAndSight(player))
        {
            return null;
        }

        return _popupText;
    }
}

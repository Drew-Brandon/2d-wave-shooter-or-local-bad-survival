using System;
using Godot;
using HealthSources;
using Puppets.Bodies;
using CastInfo2D;
using Godot.Collections;

public partial class AimInterface : UIFollow
{
    private bool _allowAuto = false;
    public bool AllowAuto { get => _allowAuto; set => _allowAuto = value;}

    private ISkeletalHealthSource _target = null;
    public ISkeletalHealthSource Target { get => _target; set => _target = value; }

    private PreciseUseableTool _tool = null;
    public PreciseUseableTool Tool { get => _tool; }

    private PlayerBodyTop2D _player = null;
    public PlayerBodyTop2D Player { get => _player; }
 
    [Export(PropertyHint.InputName)]
    private StringName _fireInput = "";

    [Export(PropertyHint.Range, "0,180,0.1,radians_as_degrees")]
    private float _maxMissPenalty = Mathf.DegToRad(10f);

    [Export]
    private RayCastInfo2D _occlusionCast = null;

    [Export]
    private AimInterfaceButton[] _buttons = null;

    private void OnToolChanged(UseableTool newTool)
    {
        PreciseUseableTool preciseTool = newTool as PreciseUseableTool;

        if (preciseTool == null)
        {
            QueueFree();
        }

        _tool = preciseTool;
    }

    public void AimFor(PlayerBodyTop2D player)
    {
        if (player == null)
        {
            throw new ArgumentNullException("player");
        }

        PreciseUseableTool preciseTool = player.CurrentTool as PreciseUseableTool;

        if (preciseTool == null)
        {
            throw new InvalidOperationException("Cannot aim for a player that does not currently have a precise tool equipped.");
        }

        _tool = preciseTool;
        _player = player;
        _player.ToolChanged += OnToolChanged;
        _player.InventoryOpened += QueueFree;
        player.FocusOnTarget(ToFollow);
        player.AddToWorldUI(this);
    }

    private StringName GetHitPart()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i].IsMouseOver)
            {
                return _buttons[i].TargetPart;
            }
        }

        return "";
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player != null)
        {
            if (!_player.IsFrozen && (Input.IsActionJustPressed(_fireInput) || (_tool.IsAuto && Input.IsActionPressed(_fireInput))))
            {
                StringName hitPart = GetHitPart();

                if (hitPart == "")
                {
                    float toolRotation = _tool.Rotation;
                    _tool.Rotation += RandUtils.RandRangef(-_maxMissPenalty, _maxMissPenalty);
                    _tool.PreciselyUse(ToFollow.GlobalPosition, hitPart);
                    _tool.Rotation = toolRotation;
                }
                else
                {
                    _tool.PreciselyUse(ToFollow.GlobalPosition, hitPart);
                }
            }

            PhysicsRayQueryParameters2D query = _occlusionCast.CreateQuery(_player.GlobalPosition, ToFollow.GlobalPosition);
            RayCastResults2D results = RayCastInfo2D.IntersectRay(GetWorld2D().DirectSpaceState, query);

            if (results.Collider != null)
            {
                QueueFree();
            }
        }
    }

    public override void _ExitTree()
    {
        if (_player != null)
        {
            _player.ToolChanged -= OnToolChanged;
            _player.InventoryOpened -= QueueFree;
            _player.StopFocusing();
        }
    }
}

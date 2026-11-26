namespace Puppets.Controllers;
using System;
using Godot;
using Bodies;

/// <summary>
/// The base class for all other puppet controllers to inherent from.
/// Controllers are meant to direct character actions in-game.
/// For instance, a player controller will utilize player inputs
/// to determine what actions the affiliated character should perform.
/// </summary>
[GlobalClass, Tool]
public abstract partial class PuppetControllerTop2D : Node
{
    [Export]
    private bool _isEnabled = true;

    /// <summary>
    /// Whether or not this controller is enabled and can exert control over its puppet.
    /// </summary>
    public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }

    /// <summary>
    /// The type of body that this controller can use.
    /// Note that other types that are assignable to this one will also be counted.
    /// For instance, the default for this property is the PuppetBodyTop2D type,
    /// which means that said type and any of its child classes can be used by this
    /// controller.
    /// </summary>
    public virtual Type BodyType { get => typeof(PuppetBodyTop2D); }

    private PuppetBodyTop2D _body = null;

    [Export]
    private PuppetBodyTop2D _bodyProperty
    {
        get => _body;

        set
        {
            if (value == null)
            {
                _body = null;
            }
            else if (BodyType.IsAssignableFrom(value.GetType()))
            {
                _body = value;
            }
        }
    }

    /// <summary>
    /// The body that this controller is currently using.
    /// </summary>
    public PuppetBodyTop2D Body { get => _body; }

    /// <summary>
    /// Called when the controller is processed by the scene tree.
    /// The reason for using this over "_Process" is that this will not run if the
    /// controller is disabled.
    /// </summary>
    /// <param name="delta">
    /// The delta time for this process.
    /// </param>
    protected virtual void OnControllerProcess(double delta)
    {
        
    }

    /// <summary>
    /// Called when the controller is physics processed by the scene tree.
    /// The reason for using this over "_PhysicsProcess" is that this will not run
    /// if the controller is disabled.
    /// </summary>
    /// <param name="delta">
    /// The delta time for this physics process.
    /// </param>
    protected virtual void OnControllerPhysicsProcess(double delta)
    {
        
    }

    /// <summary>
    /// Sets the controller to be enabled or disabled.
    /// If the controller is disabled, it won't be able to exert control over
    /// its puppet.
    /// </summary>
    /// <param name="enabledState">
    /// Whether or not this controller should be enabled.
    /// </param>
    public void SetEnabledState(bool enabledState)
    {
        _isEnabled = enabledState;
    }

    protected virtual void OnBodyChanged(PuppetBodyTop2D oldBody, PuppetBodyTop2D newBody)
    {
        
    }

    /// <summary>
    /// Sets the puppet for this controller to use.
    /// </summary>
    /// <param name="body">
    /// The body of the puppet to control.
    /// A null value is allowed and, if it has one, the controller will detach
    /// from its current puppet.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Will be thrown if the provided body does not match the type the controller
    /// is looking for. See "BodyType" for more info.
    /// </exception>
    public void SetBody(PuppetBodyTop2D body)
    {
        if (_body == body)
        {
            return;
        }

        if (body == null)
        {
            if (_body.Controller != null)
            {
                _body.SetController(null);
            }
            
            _body = null;
            return;
        }

        if (body.GetType() != BodyType)
        {
            throw new ArgumentException($"Invalid body type for controller. Body's type should be \"{BodyType}\" and not \"{body.GetType()}\"");
        }

        OnBodyChanged(_body, body);
        _body = body;

        /* 
         * The "SetController" method for the puppet also tries to call this one.
         * This occurs when the puppet is assigned to the controller first.
         * So, if the puppet's controller is already set to this one, don't call its method.
         */
        if (_body.Controller != this)
        {
            _body.SetController(this);
        }
    }

    public override void _Ready()
    {
        if (_body != null && _body.Controller != this)
        {
            OnBodyChanged(null, _body);
            _body.SetController(this);
        }
    }

    public override void _Process(double delta)
    {
        if (_isEnabled)
        {
            OnControllerProcess(delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isEnabled)
        {
            OnControllerPhysicsProcess(delta);
        }
    }
}

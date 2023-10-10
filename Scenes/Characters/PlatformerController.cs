using Godot;
using System;

public partial class PlatformerController : CharacterBody2D
{

    #region Initialize

    /// <summary>
    /// Called once on start
    /// </summary>
    public override void _Ready()
    {
        GatherRequirements();
        _sprite.TopLevel = true;
    }

    /// <summary>
    /// Gather any requirements for class functionality
    /// </summary>
    private void GatherRequirements()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    #endregion

    #region Process
    /// <summary>
    /// Called every frame
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
        GatherInput();
        FlipSprite();
        _sprite.GlobalPosition = _sprite.GlobalPosition.Lerp(GlobalPosition + Vector2.Up * 16, (float)Engine.GetPhysicsInterpolationFraction());
    }
    /// <summary>
    /// Called every physics; default: 60fps
    /// </summary>
    /// <param name="delta"></param>

    public override void _PhysicsProcess(double delta)
    {
        CalculateVelocity((float)delta);
        MoveAndSlide();
    }
    #endregion

    #region Input

    [Export] private bool _useRawInput = true;
    private Vector2 _input;
    private bool _didJump = false;

    /// <summary>
    /// Gathers input from the player
    /// </summary>
    private void GatherInput()
    {
         _input = Input.GetVector(InputNames.Left, InputNames.Right, InputNames.Up, InputNames.Down);
        if (_useRawInput) _input = _input.GetRaw();
        if (Input.IsActionJustPressed(InputNames.Jump))
        {
            _didJump = true;
        }
    }

    #endregion

    #region Gravity

    [Export] private float _gravity = 800f;
    [Export] private float _terminalVelocity = 600f;


    /// <summary>
    /// Apply gravity to the character body velocity
    /// </summary>
    /// <param name="vel"></param>
    /// <param name="delta"></param>
    private void ApplyGravity(ref Vector2 vel, float delta)
    {
        if (IsOnFloor()) return;
        _previousVelocityY = vel.Y;
        _newVelocityY = vel.Y + _gravity * delta;
        _newVelocityY = Mathf.Clamp(_newVelocityY, -Mathf.Inf, _terminalVelocity);
        vel.Y = (_previousVelocityY + _newVelocityY) * 0.5f;
    }

    #endregion


    #region Velocity

    private Vector2 _velocity;
    private float _targetVelocityX;
    private float _previousVelocityY, _newVelocityY;

    [Export] private float _moveSpeed = 100f;
    [Export] private float _acceleration = 7f;
    [Export] private float _deceleration = 10f;

    /// <summary>
    /// Calculate the bi-directional velocity and apply it to the character body
    /// </summary>
    /// <param name="delta"></param>
    private void CalculateVelocity(float delta)
    {
        _velocity = Velocity;
        CalculateVelocityY(ref _velocity, delta);
        CalculateVelocityX(ref _velocity);
        Velocity = _velocity;
    }

    /// <summary>
    /// Calculate vertical velocity of the character body
    /// </summary>
    /// <param name="vel"></param>
    /// <param name="delta"></param>
    private void CalculateVelocityY(ref Vector2 vel, float delta)
    {
        ApplyGravity(ref vel, delta);
        HandleJump(ref vel);
    }

    /// <summary>
    /// Calculate X velocity of the character body
    /// </summary>
    /// <param name="vel"></param>
    private void CalculateVelocityX(ref Vector2 vel)
    {
        _targetVelocityX = _input.X * _moveSpeed;
        vel.X = Mathf.MoveToward(vel.X, _targetVelocityX, Mathf.Sign(_input.X) == Mathf.Sign(_velocity.X)?_acceleration:_deceleration);
    }

    #endregion

    #region Jump

    [Export]private float _jumpForce = -300f;

    private void HandleJump(ref Vector2 vel)
    {
        if (!IsOnFloor() || !_didJump) return;
        _didJump = false;
        vel.Y = _jumpForce;
    }

    #endregion

    #region Animation

    private AnimatedSprite2D _sprite;

    /// <summary>
    /// Flip the character sprite based on input    
    /// </summary>
    private void FlipSprite()
    {
        _sprite.FlipH = _input.X switch
        {
            < 0 => true,
            > 0 => false,
            _ => _sprite.FlipH
        };
    }

    #endregion

}

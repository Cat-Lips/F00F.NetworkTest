using Godot;

namespace Game;

public partial class Player : CharacterBody2D
{
    public const float Speed = 100.0f;
    public const float JumpVelocity = -200.0f;

    public override void _PhysicsProcess(double _delta)
    {
        var delta = (float)_delta;
        var velocity = Velocity;
        var grounded = IsOnFloor();

        ApplyJump();
        ApplyGravity();
        ApplyMovement();

        Velocity = velocity;
        MoveAndSlide();

        void ApplyJump()
        {
            if (grounded && Input.IsActionJustPressed("Jump"))
                velocity.Y = JumpVelocity;
        }

        void ApplyGravity()
        {
            if (!grounded)
                velocity += GetGravity() * delta;
        }

        void ApplyMovement()
        {
            var direction = Input.GetAxis("Left", "Right");

            velocity.X = direction is not 0
                ? direction * Speed
                : Mathf.MoveToward(Velocity.X, 0, Speed);
        }
    }
}

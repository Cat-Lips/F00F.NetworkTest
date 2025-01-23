using F00F;
using Godot;
using static Godot.MultiplayerApi;

namespace Game;

[Tool]
public partial class Player : CharacterBody2D
{
    #region Private

    private Vector2 damage;

    private Node Game => field ??= GetParent();
    private Node2D Gun => field ??= GetNode<Node2D>("%Gun");
    private Sprite2D GunSprite => field ??= GetNode<Sprite2D>("%GunSprite");
    private Marker2D GunMuzzle => field ??= GetNode<Marker2D>("%GunMuzzle");

    #endregion

    #region Export

    [Export] public float Speed { get; set; } = 100.0f;
    [Export] public float JumpVelocity { get; set; } = -200.0f;

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        if (!this.IsLocal())
            SetPhysicsProcess(false);
    }

    public sealed override void _PhysicsProcess(double _delta)
    {
        var delta = (float)_delta;
        var velocity = Velocity;
        var grounded = IsOnFloor();

        ApplyJump();
        ApplyGravity();
        ApplyMovement();

        ApplyDamage();

        Velocity = velocity;
        MoveAndSlide();

        UpdateGun();

        #region Common

        void ApplyJump()
        {
            if (grounded && MyInput.IsActionJustPressed(MyInput.Jump))
                velocity.Y = JumpVelocity;
        }

        void ApplyGravity()
        {
            if (!grounded)
                velocity += GetGravity() * delta;
        }

        void ApplyMovement()
            => velocity.X = MyInput.GetAxis(MyInput.Left, MyInput.Right) * Speed;

        #endregion

        void ApplyDamage()
        {
            if (damage != Vector2.Zero)
            {
                velocity += damage * 100;
                damage = Vector2.Zero;
            }
        }

        void UpdateGun()
        {
            MoveGun();
            FireGun();

            void MoveGun()
            {
                var mouse = GetGlobalMousePosition();

                Gun.LookAt(mouse);
                GunSprite.FlipV = mouse.X < GlobalPosition.X;
            }

            void FireGun()
            {
                if (MyInput.IsActionPressed(MyInput.Shoot))
                    Rpc(MethodName.FireGun, this.PeerId());
            }
        }
    }

    #endregion

    #region RPC

    [Rpc(CallLocal = true)]
    private void FireGun(int shooter)
    {
        var bullet = Bullet.New(shooter);
        bullet.Transform = GunMuzzle.GlobalTransform;
        Game.AddChild(bullet, forceReadableName: true);
    }

    [Rpc(RpcMode.AnyPeer)]
    public void TakeDamage(/*in */Vector2 damage)
        => this.damage += damage;

    #endregion
}

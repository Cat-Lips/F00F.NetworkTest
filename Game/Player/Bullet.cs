using F00F;
using Godot;

namespace Game;

[Tool]
public partial class Bullet : Area2D
{
    private static readonly PackedScene Scene = Utils.LoadScene<Bullet>();
    public static Bullet New() => Scene.Instantiate<Bullet>();

    #region Export

    [Export] public float Speed { get; set; } = 250.0f;
    [Export(PropertyHint.Range, "0,1")] public float Power { get; set; } = .1f;

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        BodyEntered += OnBodyEntered;

        void OnBodyEntered(Node2D body)
        {
            if (this.IsAuth())
            {
                if (body is Player player)
                    player.RpcId(Player.MethodName.TakeDamage, velocity * Power);
                Rpc(MethodName.RemoveBullet);
            }
        }
    }

    private Vector2 velocity;
    public sealed override void _PhysicsProcess(double delta)
    {
        var oldPos = Position;
        var newPos = oldPos + this.Fwd() * Speed * (float)delta;
        velocity = newPos - oldPos;
        Position = newPos;
    }

    #endregion

    #region RPC

    [Rpc(CallLocal = true)]
    private void RemoveBullet()
        => QueueFree();

    #endregion
}

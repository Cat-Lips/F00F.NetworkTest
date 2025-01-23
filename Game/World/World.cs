using System.Linq;
using F00F;
using Godot;

namespace Game;

[Tool]
public partial class World : TileMapLayer
{
    private Camera2D Camera => field ??= GetNode<Camera2D>("Camera");
    public Vector2[] SpawnPoints => field ??= this.GetChildren<Marker2D>().Select(x => x.Position).ToArray();

    public sealed override void _Ready()
        => Camera.ZoomToView();
}

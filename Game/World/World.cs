using System.Linq;
using F00F;
using Godot;

namespace Game;

[Tool]
public partial class World : Game2D
{
    private int spawnCount;
    private Vector2[] spawnPoints;

    [Export] public int Level { get; set => this.Set(ref field, value.Clamp(0, Levels.Length()), OnLevelSet); }
    [Export] private PackedScene[] Levels { get; set; }

    public Vector2 NextSpawnPoint()
        => spawnPoints.PickNext(ref spawnCount);

    private void OnLevelSet()
    {
        Origin.RemoveChildren();
        if (Level is 0) return;

        LoadLevel(out var level);
        ResetSpawnPoints();

        void LoadLevel(out Node level)
            => Origin.AddChild(level = Levels[Level - 1].Instantiate());

        void ResetSpawnPoints()
        {
            spawnCount = 0;
            spawnPoints = level.GetChildren<Marker2D>().Select(x => x.Position).ToArray();
        }
    }
}

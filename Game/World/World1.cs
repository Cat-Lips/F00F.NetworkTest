using System.Linq;
using F00F;
using Godot;

namespace Game;

[Tool]
public partial class World1 : Game2D
{
    private int spawnCount;
    private Vector2[] spawnPoints;

    [Export] public int Level { get; set => this.Set(ref field, value.Clamp(0, Levels.Length()), OnLevelSet); }
    [Export] private PackedScene[] Levels { get; set; }

    public Vector2 NextSpawnPoint()
        => spawnPoints.PickNext(ref spawnCount);

    private Node level = null;
    private void OnLevelSet()
    {
        UnloadLevel();
        if (Level is not 0)
            LoadLevel();

        void LoadLevel()
        {
            level = Levels[Level - 1].Instantiate();
            spawnPoints = level.GetChildren<Marker2D>().Select(x => x.Position).ToArray();

            spawnCount = 0;
            AddChild(level);
        }

        void UnloadLevel()
        {
            if (level is null) return;
            this.RemoveChild(level, free: true);
            level = null;
        }
    }
}

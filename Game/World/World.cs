using System;
using System.Linq;
using F00F;
using Godot;
using static Godot.TileSet;

namespace Game;

[Tool]
public partial class World : Node
{
    private const string IsScoreTile = "IsScoreTile";

    #region Private

    private Camera2D Camera => field ??= GetNode<Camera2D>("Camera");

    #endregion

    #region Export

    [Export] public PackedScene[] Levels { get; set => this.Set(ref field, value ?? [], () => Level = Level); } = [];

    #endregion

    public event Action LevelChanged;
    public int Level { get; set => this.Set(ref field, value.Clamp(0, Levels.Length(1) - 1), SetLevel, LevelChanged); }

    public TileMapLayer CurrentLevel { get; private set; }
    public Vector2[] SpawnPoints { get; private set; }

    public Vector2 GetSpawnPoint()
        => SpawnPoints.PickRandom();

    public bool IsOnScoreTile(Node2D player)
    {
        var lvl = CurrentLevel;
        var pos = lvl.LocalToMap(player.Position);
        var floor = lvl.GetNeighborCell(pos, CellNeighbor.BottomSide);
        var data = lvl.GetCellTileData(floor);
        return data?.GetCustomData(IsScoreTile).AsBool() ?? false;
    }

    #region Godot

    public sealed override void _Ready()
    {
        Camera.ZoomToView();
        SetLevel();
    }

    #endregion

    #region Private

    private void SetLevel()
    {
        InitiateRpc();
        UnloadLevel();
        LoadLevel();

        void InitiateRpc()
        {
            if (this.IsServer())
                Rpc(MethodName.SetRemoteLevel, Level);
        }

        void UnloadLevel()
        {
            CurrentLevel?.DetachChild(free: true);
            CurrentLevel = null;
            SpawnPoints = null;
        }

        void LoadLevel()
        {
            if (Levels.Length() is 0) return;
            AddChild(CurrentLevel = Levels[Level].New<TileMapLayer>());
            SpawnPoints = CurrentLevel.GetChildren<Marker2D>().Select(x => x.Position).ToArray();
        }
    }

    #endregion

    #region RPC

    [Rpc]
    private void SetRemoteLevel(int level)
        => Level = level;

    #endregion
}

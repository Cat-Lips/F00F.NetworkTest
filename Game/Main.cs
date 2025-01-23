using F00F;
using Godot;

namespace Game;

[Tool]
public partial class Main : F00F.Game
{
    private World World => field ??= GetNode<World>("World");

    protected sealed override void OnReady()
    {
        World.Level = 1;
        InitGame<Player>(new GameConfig(), World, InitLocalPlayer);

        static void InitLocalPlayer(Player player, IPlayerData data)
        {
            data.DisplayColor = Colors.Red;
            player.Modulate = Colors.Red;
            player.ZIndex += 1;
        }
    }

    #region Config

    private new class GameConfig : F00F.Game.GameConfig
    {
        public sealed override bool EnablePlayerColor => false;
    }

    #endregion
}

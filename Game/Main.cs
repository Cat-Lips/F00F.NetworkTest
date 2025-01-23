using F00F;
using Godot;

namespace Game;

public partial class Main : Node
{
    private UI UI => field ??= GetNode<UI>(nameof(UI));

    private Network Network => field ??= GetNode<Network>(nameof(Network));
    private NetworkMenu NetworkMenu => UI.NetworkMenu;

    public sealed override void _Ready()
    {
        Network.Initialise<Player>(InitPlayer);
        NetworkMenu.Initialise(Network);

        static void InitPlayer(Player player)
            => player.Modulate = Colors.Red;
    }
}

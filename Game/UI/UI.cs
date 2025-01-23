using F00F;
using Godot;

namespace Game;

public partial class UI : CanvasLayer
{
    public NetworkMenu NetworkMenu => field ??= GetNode<NetworkMenu>($"%{nameof(NetworkMenu)}");
}

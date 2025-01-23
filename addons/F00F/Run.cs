using Godot;

namespace F00F.Init;

internal partial class Run : Node
{
    public sealed override void _Ready()
    {
#if TOOLS
        SyncSharp.Execute();
#endif
        GetTree().Quit();
    }
}

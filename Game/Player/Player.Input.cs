using System;
using Godot;

namespace Game;

public partial class Player
{
    static Player() => MyInput.Init();

    private class MyInput : F00F.MyInput
    {
        static MyInput() => Init<MyInput>();
        public static void Init() { }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Left;
        public static readonly StringName Right;
        public static readonly StringName Jump;
        public static readonly StringName Shoot;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Defaults
        {
            public static readonly Key[] Left = [Key.A, Key.Left];
            public static readonly Key[] Right = [Key.D, Key.Right];
            public static readonly Key[] Jump = [Key.Space, Key.W, Key.Up];
            public static readonly Enum[] Shoot = [Key.Ctrl, Key.Shift, MouseButton.Left];
        }

        private MyInput() { }
    }
}

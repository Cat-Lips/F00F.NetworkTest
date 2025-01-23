using System;
using System.Collections.Generic;
using System.Linq;
using F00F;
using Godot;

namespace Game;

[Tool]
public partial class Main : F00F.Game
{
    private Player myPlayer;
    private GameConfig myGame;

    private World World => field ??= GetNode<World>("World");

    protected sealed override void OnReady()
    {
        myGame = new(this);
        UI.PlayerList.DisplayPrecision = 2;
        InitGame<Player>(myGame, World, InitLocalPlayer);

        void InitLocalPlayer(Player player)
        {
            myPlayer = player;

            myPlayer.ZIndex += 1;
            myPlayer.Modulate = Colors.Red;
            myPlayer.Position = World.GetSpawnPoint();
            World.LevelChanged += () => myPlayer.Position = World.GetSpawnPoint();
            UI.PlayerList.SetDisplayColor(myPlayer, Colors.Red);
        }
    }

    public sealed override void _Process(double delta)
    {
        if (!IsInstanceValid(myPlayer)) return;
        var grounded = myPlayer.IsOnFloor();

        UpdateScore();

        void UpdateScore()
        {
            if (grounded && World.IsOnScoreTile(myPlayer))
                PlayerData.CurrentScore += (float)delta;
        }
    }

    #region Config

    private new class GameConfig(Main self) : F00F.Game.GameConfig
    {
        public PopupMenu LevelsMenu { get; } = new();
        public sealed override bool EnablePlayerColor => false;

        public override void AddGameOptions(PopupMenu root)
        {
            var sep = root.ItemCount;
            root.AddSeparator("Game");
            var items = root.AddRadioItems(Levels());

            SetCheckedItem(self.World.Level);
            self.World.LevelChanged += () => SetCheckedItem(self.World.Level);
            self.Network.StateChanged += () => DisableItems(self.Network.IsClient);

            IEnumerable<(string Label, Action Action)> Levels()
            {
                return self.World.Levels.Select((x, idx) => (GetLabel(x), GetAction(idx)));

                string GetLabel(PackedScene x)
                    => x.GetState().GetNodeName(0).Capitalise();

                Action GetAction(int idx)
                    => () => self.World.Level = idx;
            }

            void DisableItems(bool disabled)
            {
                root.SetItemDisabled(sep, disabled);
                items.ForEach(idx => root.SetItemDisabled(idx, disabled));
            }

            void SetCheckedItem(int lvl)
            {
                if (self.Network.IsServer) return;

                var active = sep + 1 + lvl;
                items.ForEach(idx => root.SetItemChecked(idx, idx == active));
            }
        }
    }

    #endregion
}

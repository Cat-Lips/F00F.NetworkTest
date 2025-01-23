using System;
using System.Collections.Generic;
using System.Data;
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
        UI.PlayerList.DisplayPrecision = 2;
        InitGame<Player>(myGame = new(this), World, InitLocalPlayer);

        void InitLocalPlayer(Player player)
        {
            InitPlayer();
            SetPlayerColor();
            SetStartPosition();

            myPlayer = player;

            void InitPlayer()
                => player.ZIndex += 1;

            void SetPlayerColor()
            {
                player.Modulate = Colors.Red;
                UI.PlayerList.SetDisplayColor(player, Colors.Red);
            }

            void SetStartPosition()
            {
                SetStartPosition();
                World.LevelChanged += SetStartPosition;

                void SetStartPosition()
                    => player.Position = World.GetSpawnPoint();
            }
        }
    }

    public sealed override void _Process(double delta)
    {
        if (IsInstanceValid(myPlayer))
            UpdatePlayer((float)delta);

        void UpdatePlayer(float delta)
        {
            var grounded = myPlayer.IsOnFloor();

            UpdateScore();

            void UpdateScore()
            {
                if (grounded && World.IsOnScoreTile(myPlayer))
                    PlayerData.CurrentScore += delta;
            }
        }
    }

    #region Config

    private new class GameConfig(Main self) : F00F.Game.GameConfig
    {
        private static readonly float[] TimeLimits = [1, 3, 5]; // Minutes
        private static readonly float[] PointLimits = [10, 15, 30];

        public sealed override bool EnablePlayerColor => false;

        public sealed override void AddGameOptions(PopupMenu root)
        {
            var sep = root.ItemCount;
            root.AddSeparator("Game");
            root.AddSubmenuNodeItem("Time Limit", TimeOptions());
            root.AddSubmenuNodeItem("Point Limit", PointOptions());
            root.AddSubmenuNodeItem("Level Select", LevelOptions());

            PopupMenu TimeOptions()
            {
                var menu = new PopupMenu();
                var items = menu.AddRadioItems(TimeOptions());
                self.World.TimeLimitChanged += () => menu.CheckRadioItem(items, OptionIndex());
                self.Network.StateChanged += () => menu.DisableItems(items, self.Network.IsClient);
                return menu;

                IEnumerable<(string Label, Action Action)> TimeOptions()
                {
                    yield return ("No time limit", () => self.World.TimeLimit = -1);
                    foreach (var value in TimeLimits)
                        yield return ($"Play for {value} minute".Pluralise(value is not 1), () => self.World.TimeLimit = value);
                }

                int OptionIndex()
                    => Array.IndexOf(TimeLimits, self.World.TimeLimit) + 1;
            }

            PopupMenu PointOptions()
            {
                var menu = new PopupMenu();
                var items = menu.AddRadioItems(PointOptions());
                self.World.PointLimitChanged += () => menu.CheckRadioItem(items, OptionIndex());
                self.Network.StateChanged += () => menu.DisableItems(items, self.Network.IsClient);
                return menu;

                IEnumerable<(string Label, Action Action)> PointOptions()
                {
                    yield return ("No point limit", () => self.World.PointLimit = -1);
                    foreach (var value in PointLimits)
                        yield return ($"Play to {value} points", () => self.World.PointLimit = value);
                }

                int OptionIndex()
                    => Array.IndexOf(PointLimits, self.World.PointLimit) + 1;
            }

            PopupMenu LevelOptions()
            {
                var menu = new PopupMenu();
                var items = menu.AddRadioItems(LevelOptions());
                self.World.LevelChanged += () => menu.CheckRadioItem(items, OptionIndex());
                self.Network.StateChanged += () => menu.DisableItems(items, self.Network.IsClient);
                return menu;

                IEnumerable<(string Label, Action Action)> LevelOptions()
                {
                    return self.World.Levels
                        .Select((x, idx) => (GetLabel(x), GetAction(idx)))
                        .Append(("Random", GetRandomAction()));

                    string GetLabel(PackedScene x)
                        => x.GetState().GetNodeName(0).Capitalise();

                    Action GetAction(int idx)
                        => () => self.World.Level = idx;

                    Action GetRandomAction(bool skipFirst = true)
                    {
                        var first = skipFirst ? 1 : 0;
                        var last = self.World.Levels.Length - 1;
                        return () => self.World.Level = Rng.Next(first, last);
                    }
                }

                int OptionIndex()
                    => Array.IndexOf(PointLimits, self.World.PointLimit) + 1;
            }
        }
    }

    #endregion
}

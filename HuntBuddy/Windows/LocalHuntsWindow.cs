﻿using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using HuntBuddy.Utils;
using ImGuiNET;

namespace HuntBuddy.Windows;

/// <summary>
/// Local hunts window.
/// </summary>
public class LocalHuntsWindow : Window
{
    public LocalHuntsWindow() : base(
        $"{Plugin.Instance.Name}",
        ImGuiWindowFlags.NoNavInputs | ImGuiWindowFlags.NoDocking,
        true)
    {
        this.Size = Vector2.Zero;
        this.SizeCondition = ImGuiCond.Always;

        this.IsOpen = true;
    }

    public override void PreOpenCheck()
    {
        if (Plugin.Instance.Configuration.HideLocalHuntBackground)
        {
            this.Flags |= ImGuiWindowFlags.NoBackground;
        }

        if (Plugin.Instance.Configuration.LockWindowPositions)
        {
            this.Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        }
    }

    public override unsafe bool DrawConditions() =>
        Plugin.Instance.Configuration.ShowLocalHunts &&
        !Plugin.Instance.CurrentAreaMobHuntEntries.IsEmpty &&
        Plugin.Instance.CurrentAreaMobHuntEntries.Count(
            x => Plugin.Instance.MobHuntStruct->CurrentKills[x.CurrentKillsOffset] == x.NeededKills) !=
        Plugin.Instance.CurrentAreaMobHuntEntries.Count;

    public override unsafe void Draw()
    {
        foreach (var mobHuntEntry in Plugin.Instance.CurrentAreaMobHuntEntries)
        {
            var currentKills = Plugin.Instance.MobHuntStruct->CurrentKills[mobHuntEntry.CurrentKillsOffset];

            if (Plugin.Instance.Configuration.HideCompletedHunts && currentKills == mobHuntEntry.NeededKills)
            {
                continue;
            }

            if (Location.Database.ContainsKey(mobHuntEntry.MobHuntId))
            {
                if (InterfaceUtil.IconButton(FontAwesomeIcon.MapMarkerAlt, $"pin##{mobHuntEntry.MobHuntId}"))
                {
                    Location.CreateMapMarker(
                        mobHuntEntry.TerritoryType,
                        mobHuntEntry.MapId,
                        mobHuntEntry.MobHuntId,
                        mobHuntEntry.Name,
                        Location.OpenType.None);
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Place marker on the map");
                    ImGui.EndTooltip();
                }

                ImGui.SameLine();

                if (InterfaceUtil.IconButton(FontAwesomeIcon.MapMarkedAlt, $"open##{mobHuntEntry.MobHuntId}"))
                {
                    var includeArea = Plugin.Instance.Configuration.IncludeAreaOnMap;
                    if (ImGui.IsKeyDown(ImGuiKey.ModShift))
                    {
                        includeArea = !includeArea;
                    }

                    Location.CreateMapMarker(
                        mobHuntEntry.TerritoryType,
                        mobHuntEntry.MapId,
                        mobHuntEntry.MobHuntId,
                        mobHuntEntry.Name,
                        includeArea ? Location.OpenType.ShowOpen : Location.OpenType.MarkerOpen);
                }

                if (ImGui.IsItemHovered())
                {
                    var color = ImGui.IsKeyDown(ImGuiKey.ModShift)
                        ? new Vector4(0f, 0.7f, 0f, 1f)
                        : new Vector4(0.7f, 0.7f, 0.7f, 1f);
                    ImGui.BeginTooltip();
                    if (Plugin.Instance.Configuration.IncludeAreaOnMap)
                    {
                        ImGui.Text("Show hunt area on the map");
                        ImGui.TextColored(
                            color,
                            "Hold [SHIFT] to show the location only");
                    }
                    else
                    {
                        ImGui.Text("Show hunt location on the map");
                        ImGui.TextColored(
                            color,
                            "Hold [SHIFT] to include the area");
                    }

                    ImGui.EndTooltip();
                }

                ImGui.SameLine();
            }

            ImGui.Text($"{mobHuntEntry.Name} ({currentKills}/{mobHuntEntry.NeededKills})");

            if (!Plugin.Instance.Configuration.ShowLocalHuntIcons)
            {
                continue;
            }

            InterfaceUtil.DrawHuntIcon(mobHuntEntry);
        }
    }
}
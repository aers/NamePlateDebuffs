using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Dalamud.Interface.Components;

namespace NamePlateDebuffs
{
    internal class NamePlateDebuffsPluginUI : IDisposable
    {
        private readonly NamePlateDebuffsPlugin _plugin;

#if DEBUG
        private bool ConfigOpen = true;
#else
        private bool ConfigOpen = false;
#endif
        public bool IsConfigOpen => ConfigOpen;

        public NamePlateDebuffsPluginUI(NamePlateDebuffsPlugin p)
        {
            _plugin = p;

            _plugin.Interface.UiBuilder.OnOpenConfigUi += UiBuilder_OnOpenConfigUi;
            _plugin.Interface.UiBuilder.OnBuildUi += UiBuilder_OnBuild;
        }

        public void Dispose()
        {
            _plugin.Interface.UiBuilder.OnOpenConfigUi -= UiBuilder_OnOpenConfigUi;
            _plugin.Interface.UiBuilder.OnBuildUi -= UiBuilder_OnBuild;
        }

        public void ToggleConfig()
        {
            ConfigOpen = !ConfigOpen;
        }

        public void UiBuilder_OnOpenConfigUi(object sender, EventArgs args) => ConfigOpen = true;

        public void UiBuilder_OnBuild()
        {
            if (!ConfigOpen)
                return;

            ImGui.SetNextWindowSize(new Vector2(420, 625), ImGuiCond.Always);

            if (!ImGui.Begin(_plugin.Name, ref ConfigOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.End();
                return;
            }

            if (ImGui.CollapsingHeader("General", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (ImGui.Checkbox("Enabled", ref _plugin.Config.Enabled))
                    _plugin.Config.Save();
                if (ImGui.InputInt("Update Interval (ms)", ref _plugin.Config.UpdateInterval, 10))
                    _plugin.Config.Save();
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Interval between status updates in milliseconds");
                if (ImGui.Button("Reset Config to Defaults"))
                {
                    _plugin.Config.SetDefaults();
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                ImGui.Text("While config is open, test nodes are displayed to help with configuration.");
            }

            if (ImGui.CollapsingHeader("Node Group", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (ImGui.InputInt("X Offset", ref _plugin.Config.GroupX))
                {
                    if (_plugin.Config.GroupX > 100)
                        _plugin.Config.GroupX = 100;
                    if (_plugin.Config.GroupX < -100)
                        _plugin.Config.GroupX = -100;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Y Offset", ref _plugin.Config.GroupY))
                {
                    if (_plugin.Config.GroupY > 100)
                        _plugin.Config.GroupY = 100;
                    if (_plugin.Config.GroupY < -100)
                        _plugin.Config.GroupY = -100;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Node Spacing", ref _plugin.Config.NodeSpacing))
                {
                    if (_plugin.Config.NodeSpacing > 20)
                        _plugin.Config.NodeSpacing = 20;
                    if (_plugin.Config.NodeSpacing < -5)
                        _plugin.Config.NodeSpacing = -5;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.DragFloat("Group Scale", ref _plugin.Config.Scale, 0.01F, 0.25F, 3.0F))
                {
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
            }

            if (ImGui.CollapsingHeader("Node", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Text("Try and maintain a 3:4 ratio of Icon Width:Icon Height for best results.");
                if (ImGui.InputInt("Icon X Offset", ref _plugin.Config.IconX))
                {
                    if (_plugin.Config.IconX > 100)
                        _plugin.Config.IconX = 100;
                    if (_plugin.Config.IconX < -100)
                        _plugin.Config.IconX = -100;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Icon Y Offset", ref _plugin.Config.IconY))
                {
                    if (_plugin.Config.IconY > 100)
                        _plugin.Config.IconY = 100;
                    if (_plugin.Config.IconY < -100)
                        _plugin.Config.IconY = -100;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Icon Width", ref _plugin.Config.IconWidth))
                {
                    if (_plugin.Config.IconWidth > 100)
                        _plugin.Config.IconWidth = 100;
                    if (_plugin.Config.IconWidth < 10)
                        _plugin.Config.IconWidth = 10;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Icon Height", ref _plugin.Config.IconHeight))
                {
                    if (_plugin.Config.IconHeight > 100)
                        _plugin.Config.IconHeight = 100;
                    if (_plugin.Config.IconHeight < 10)
                        _plugin.Config.IconHeight = 10;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Duration X Offset", ref _plugin.Config.DurationX))
                {
                    if (_plugin.Config.DurationX > 100)
                        _plugin.Config.DurationX = 100;
                    if (_plugin.Config.DurationX < -100)
                        _plugin.Config.DurationX = -100;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Duration Y Offset", ref _plugin.Config.DurationY))
                {
                    if (_plugin.Config.DurationY > 100)
                        _plugin.Config.DurationY = 100;
                    if (_plugin.Config.DurationY < -100)
                        _plugin.Config.DurationY = -100;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Duration Font Size", ref _plugin.Config.FontSize))
                {
                    if (_plugin.Config.FontSize > 60)
                        _plugin.Config.FontSize = 60;
                    if (_plugin.Config.FontSize < 1)
                        _plugin.Config.FontSize = 1;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.InputInt("Duration Padding", ref _plugin.Config.DurationPadding))
                {
                    if (_plugin.Config.DurationPadding > 100)
                        _plugin.Config.DurationPadding = 100;
                    if (_plugin.Config.DurationPadding < -100)
                        _plugin.Config.DurationPadding = -100;
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.ColorEdit4("Duration Text Color", ref _plugin.Config.DurationTextColor))
                {
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }
                if (ImGui.ColorEdit4("Duration Edge Color", ref _plugin.Config.DurationEdgeColor))
                {
                    _plugin.StatusNodeManager.LoadConfig();
                    _plugin.Config.Save();
                }

            }

            ImGui.End();
        }
    }
}

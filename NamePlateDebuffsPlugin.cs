using Dalamud.Game.Command;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using NamePlateDebuffs.StatusNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamePlateDebuffs
{
    public class NamePlateDebuffsPlugin : IDalamudPlugin
    {
        public string Name => "NamePlateDebuffs";

        internal DalamudPluginInterface Interface;
        internal PluginAddressResolver Address;
        internal StatusNodeManager StatusNodeManager;
        internal AddonNamePlateHooks Hooks;
        internal NamePlateDebuffsPluginUI UI;
        internal NamePlateDebuffsPluginConfig Config;

        internal bool InPvp;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            Interface = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface), "DalamudPluginInterface cannot be null");

            Config = pluginInterface.GetPluginConfig() as NamePlateDebuffsPluginConfig ?? new NamePlateDebuffsPluginConfig();
            Config.Initialize(pluginInterface);

            if (!FFXIVClientStructs.Resolver.Initialized) FFXIVClientStructs.Resolver.Initialize();

            Address = new PluginAddressResolver();
            Address.Setup(Interface.TargetModuleScanner);

            StatusNodeManager = new StatusNodeManager(this);

            Hooks = new AddonNamePlateHooks(this);
            Hooks.Initialize();

            UI = new NamePlateDebuffsPluginUI(this);

            Interface.ClientState.TerritoryChanged += OnTerritoryChange;

            Interface.CommandManager.AddHandler("/npdebuffs", new CommandInfo(this.ToggleConfig)
            {
                HelpMessage = "Toggles config window."
            });
        }
        public void Dispose()
        {
            Interface.ClientState.TerritoryChanged -= OnTerritoryChange;
            Interface.CommandManager.RemoveHandler("/npdebuffs");

            UI.Dispose();
            Hooks.Dispose();
            StatusNodeManager.Dispose();
        }

        private void OnTerritoryChange(object sender, ushort e)
        {
            try
            {
                var territory = this.Interface.Data.GetExcelSheet<TerritoryType>().GetRow(e);
                this.InPvp = territory.IsPvpZone;
            }
            catch (KeyNotFoundException)
            {
                PluginLog.Warning("Could not get territory for current zone");
            }
        }

        private void ToggleConfig(string command, string args)
        {
            UI.ToggleConfig();
        }
    }
}

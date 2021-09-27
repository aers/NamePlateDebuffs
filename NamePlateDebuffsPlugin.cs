using Dalamud.Game.Command;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using NamePlateDebuffs.StatusNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Logging;

namespace NamePlateDebuffs
{
    public class NamePlateDebuffsPlugin : IDalamudPlugin
    {
        public string Name => "NamePlateDebuffs";

        public ClientState ClientState { get; private set; } = null!;
        public static CommandManager CommandManager { get; private set; } = null!;
        public DalamudPluginInterface Interface { get; private set; } = null!;
        public DataManager DataManager { get; private set; } = null!;
        public Framework Framework { get; private set; } = null!;
        public PluginAddressResolver Address { get; private set; } = null!;
        public StatusNodeManager StatusNodeManager { get; private set; } = null!;
        public static SigScanner SigScanner { get; private set; } = null!;
        public static AddonNamePlateHooks Hooks { get; private set; } = null!;
        public NamePlateDebuffsPluginUI UI { get; private set; } = null!;
        public NamePlateDebuffsPluginConfig Config { get; private set; } = null!;

        internal bool InPvp;

        public NamePlateDebuffsPlugin(
            ClientState clientState,
            CommandManager commandManager, 
            DalamudPluginInterface pluginInterface, 
            DataManager dataManager,
            Framework framework, 
            SigScanner sigScanner
            )
        {
            ClientState = clientState;
            CommandManager = commandManager;
            DataManager = dataManager;
            Interface = pluginInterface;
            Framework = framework;
            SigScanner = sigScanner;

            Config = pluginInterface.GetPluginConfig() as NamePlateDebuffsPluginConfig ?? new NamePlateDebuffsPluginConfig();
            Config.Initialize(pluginInterface);

            if (!FFXIVClientStructs.Resolver.Initialized) FFXIVClientStructs.Resolver.Initialize(sigScanner.SearchBase);

            Address = new PluginAddressResolver();
            Address.Setup();

            StatusNodeManager = new StatusNodeManager(this);

            Hooks = new AddonNamePlateHooks(this);
            Hooks.Initialize();

            UI = new NamePlateDebuffsPluginUI(this);

            ClientState.TerritoryChanged += OnTerritoryChange;

            CommandManager.AddHandler("/npdebuffs", new CommandInfo(this.ToggleConfig)
            {
                HelpMessage = "Toggles config window."
            });
        }
        public void Dispose()
        {
            ClientState.TerritoryChanged -= OnTerritoryChange;
            CommandManager.RemoveHandler("/npdebuffs");

            UI.Dispose();
            Hooks.Dispose();
            StatusNodeManager.Dispose();
        }

        private void OnTerritoryChange(object sender, ushort e)
        {
            try
            {
                var territory = DataManager.GetExcelSheet<TerritoryType>()?.GetRow(e);
                if (territory != null) InPvp = territory.IsPvpZone;
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

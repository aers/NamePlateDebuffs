using Dalamud.Plugin;
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

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            Interface = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface), "DalamudPluginInterface cannot be null");

            if (!FFXIVClientStructs.Resolver.Initialized) FFXIVClientStructs.Resolver.Initialize();

            Address = new PluginAddressResolver();
            Address.Setup(Interface.TargetModuleScanner);

            StatusNodeManager = new StatusNodeManager(this);

            Hooks = new AddonNamePlateHooks(this);
            Hooks.Initialize();
        }
        public void Dispose()
        {
            Hooks.Dispose();
            StatusNodeManager.Dispose();
        }

    }
}

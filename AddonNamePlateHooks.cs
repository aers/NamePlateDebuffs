using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamePlateDebuffs
{
    internal unsafe class AddonNamePlateHooks : IDisposable
    {
        private NamePlateDebuffsPlugin _plugin;

        private delegate void AddonNamePlateFinalizePrototype(AddonNamePlate* thisPtr);
        private Hook<AddonNamePlateFinalizePrototype> hookAddonNamePlateFinalize;

        public AddonNamePlateHooks(NamePlateDebuffsPlugin p)
        {
            _plugin = p;
        }

        public void Initialize()
        {
            hookAddonNamePlateFinalize = new Hook<AddonNamePlateFinalizePrototype>(_plugin.Address.AddonNamePlateFinalizeAddress, AddonNamePlateFinalizeDetour);

            hookAddonNamePlateFinalize.Enable();
        }

        public void Dispose()
        {
            hookAddonNamePlateFinalize.Dispose();
        }

        public void AddonNamePlateFinalizeDetour(AddonNamePlate* thisPtr)
        {
            _plugin.StatusNodeManager.DestroyNodes();
            _plugin.StatusNodeManager.SetNamePlateAddonPointer(null);
            hookAddonNamePlateFinalize.Original(thisPtr);
        }
    }
}

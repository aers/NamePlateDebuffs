using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private delegate void AddonNamePlateDrawPrototype(AddonNamePlate* thisPtr);
        private Hook<AddonNamePlateDrawPrototype> hookAddonNamePlateDraw;

        private Stopwatch Timer;
        private long Elapsed;

        public AddonNamePlateHooks(NamePlateDebuffsPlugin p)
        {
            _plugin = p;

            Timer = new Stopwatch();
            Elapsed = 0;
        }

        public void Initialize()
        {
            hookAddonNamePlateFinalize = new Hook<AddonNamePlateFinalizePrototype>(_plugin.Address.AddonNamePlateFinalizeAddress, AddonNamePlateFinalizeDetour);
            hookAddonNamePlateDraw = new Hook<AddonNamePlateDrawPrototype>(_plugin.Address.AddonNamePlateDrawAddress, AddonNamePlateDrawDetour);

            hookAddonNamePlateFinalize.Enable();
            hookAddonNamePlateDraw.Enable();
        }

        public void Dispose()
        {
            hookAddonNamePlateFinalize.Dispose();
            hookAddonNamePlateDraw.Dispose();
        }

        public void AddonNamePlateDrawDetour(AddonNamePlate* thisPtr)
        {
            Elapsed += Timer.ElapsedMilliseconds;
            Timer.Restart();

            if (Elapsed >= 100)
            {
                if (!_plugin.StatusNodeManager.Built)
                {
                    _plugin.StatusNodeManager.SetNamePlateAddonPointer(thisPtr);
                    if (!_plugin.StatusNodeManager.BuildNodes())
                        return;
                }

                var framework = (Framework*)_plugin.Interface.Framework.Address.BaseAddress.ToPointer();
                var ui3DModule = framework->GetUiModule()->GetUI3DModule();

                for (int i = 0; i < ui3DModule->NamePlateObjectInfoCount; i++)
                {
                    var objectInfo = ((UI3DModule.ObjectInfo**)ui3DModule->NamePlateObjectInfoPointerArray)[i];
                    if (objectInfo->NamePlateObjectKind != 3 && objectInfo->NamePlateObjectKind != 4)
                    {
                        _plugin.StatusNodeManager.SetGroupVisibility(objectInfo->NamePlateIndex, false, true);
                        continue;
                    }

                    _plugin.StatusNodeManager.SetGroupVisibility(objectInfo->NamePlateIndex, true, false);

                    var localPlayerID = _plugin.Interface.ClientState.LocalPlayer.ActorId;
                    var targetStatus = ((BattleChara*)objectInfo->GameObject)->StatusManager;

                    var statusArray = (Status*)targetStatus.Status;

                    var count = 0;

                    for (int j = 0; j < 30; j++)
                    {
                        var status = statusArray[j];
                        if (status.StatusID == 0) continue;
                        if (status.SourceID != localPlayerID) continue;

                        _plugin.StatusNodeManager.SetStatus(objectInfo->NamePlateIndex, count, status.StatusID, (int)status.RemainingTime);
                        count++;

                        if (count == 4)
                            break;
                    }

                    _plugin.StatusNodeManager.HideUnusedStatus(objectInfo->NamePlateIndex, count);
                }

                Elapsed = 0;
            }

            hookAddonNamePlateDraw.Original(thisPtr);
        }

        public void AddonNamePlateFinalizeDetour(AddonNamePlate* thisPtr)
        {
            _plugin.StatusNodeManager.DestroyNodes();
            _plugin.StatusNodeManager.SetNamePlateAddonPointer(null);
            hookAddonNamePlateFinalize.Original(thisPtr);
        }
    }
}

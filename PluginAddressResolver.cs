using Dalamud.Game;
using Dalamud.Game.Internal;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamePlateDebuffs
{
    internal class PluginAddressResolver : BaseAddressResolver
    {
        public IntPtr AddonNamePlateFinalizeAddress { get; private set;  }

        private const string AddonNamePlateFinalizeSignature = "40 53 48 83 EC 20 48 8B D9 48 81 C1 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 48 8B CB";

        protected override void Setup64Bit(SigScanner scanner)
        {
            AddonNamePlateFinalizeAddress = scanner.ScanText(AddonNamePlateFinalizeSignature);

            PluginLog.Verbose("===== NamePlate Debuffs =====");
            PluginLog.Verbose($"{nameof(AddonNamePlateFinalizeAddress)} {AddonNamePlateFinalizeAddress.ToInt64():X}");
        }
    }
}

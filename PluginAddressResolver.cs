using Dalamud.Game;
using Dalamud.Game.Internal;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;

namespace NamePlateDebuffs
{
    public class PluginAddressResolver : BaseAddressResolver
    {
        public IntPtr AddonNamePlateFinalizeAddress { get; private set;  }

        private const string AddonNamePlateFinalizeSignature = "40 53 48 83 EC 20 48 8B D9 48 81 C1 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 48 8B CB";

        public IntPtr AddonNamePlateDrawAddress { get; private set; }

        private const string AddonNamePlateDrawSignature = "0F B7 81 ?? ?? ?? ?? 4C 8B C1 66 C1 E0 06";

        protected override void Setup64Bit(SigScanner scanner)
        {
            AddonNamePlateFinalizeAddress = scanner.ScanText(AddonNamePlateFinalizeSignature);
            AddonNamePlateDrawAddress = scanner.ScanText(AddonNamePlateDrawSignature);

            PluginLog.Verbose("===== NamePlate Debuffs =====");
            PluginLog.Verbose($"{nameof(AddonNamePlateFinalizeAddress)} {AddonNamePlateFinalizeAddress.ToInt64():X}");
            PluginLog.Verbose($"{nameof(AddonNamePlateDrawAddress)} {AddonNamePlateDrawAddress.ToInt64():X}");
        }
    }
}

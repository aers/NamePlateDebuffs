using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NamePlateDebuffs
{
    [Serializable]
    internal class NamePlateDebuffsPluginConfig : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        // General
        public bool Enabled = true;
        public int UpdateInterval = 100;

        // NodeGroup
        public int GroupX = 27;
        public int GroupY = 30;
        public int NodeSpacing = 3;
        public float Scale = 1;
        public bool FillFromRight = true;

        // Node
        public int IconX = 0;
        public int IconY = 0;
        public int IconWidth = 24;
        public int IconHeight = 32;
        public int DurationX = 0;
        public int DurationY = 23;
        public int FontSize = 14;
        public int DurationPadding = 2;
        public Vector4 DurationTextColor = new Vector4(1, 1, 1, 1);
        public Vector4 DurationEdgeColor = new Vector4(0, 0, 0, 1);

        public void SetDefaults()
        {
            // General
            Enabled = true;
            UpdateInterval = 100;

            // NodeGroup
            GroupX = 27;
            GroupY = 30;
            NodeSpacing = 3;
            Scale = 1;

            // Node
            IconX = 0;
            IconY = 0;
            IconWidth = 24;
            IconHeight = 32;
            DurationX = 0;
            DurationY = 23;
            FontSize = 14;
            DurationPadding = 2;
            DurationTextColor.X = 1;
            DurationTextColor.Y = 1;
            DurationTextColor.Z = 1;
            DurationTextColor.W = 1;
            DurationEdgeColor.X = 0;
            DurationEdgeColor.Y = 0;
            DurationEdgeColor.Z = 0;
            DurationEdgeColor.W = 1;
        }

        [NonSerialized] private DalamudPluginInterface _pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
        }

        public void Save()
        {
            _pluginInterface.SavePluginConfig(this);
        }
    }
}

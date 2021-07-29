using Dalamud.Hooking;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NamePlateDebuffs.StatusNode
{
    internal unsafe class StatusNodeManager : IDisposable
    {
        private NamePlateDebuffsPlugin _plugin;

        private AddonNamePlate* namePlateAddon;

        private Dictionary<ulong, StatusNodeGroup> NodeGroups = new();

        private static byte NamePlateCount = 50;
        private static uint StartingNodeId = 50000;

        internal StatusNodeManager(NamePlateDebuffsPlugin p)
        {
            _plugin = p; 
        }

        public void Dispose()
        {
            DestroyNodes();
        }

        public void SetNamePlateAddonPointer(AddonNamePlate* addon)
        {
            namePlateAddon = addon;
        }

        public bool BuildNodes(bool rebuild = false)
        {
            if (namePlateAddon == null) return false;
            if (NodeGroups.Any() && !rebuild) return true;
            if (rebuild) DestroyNodes();
 
            for(byte i = 0; i < NamePlateCount; i++)
            {
                var nodeGroup = new StatusNodeGroup();
                var npObj = &namePlateAddon->NamePlateObjectArray[i];
                if (!nodeGroup.BuildNodes(StartingNodeId))
                {
                    DestroyNodes();
                    return false;
                }
                var npComponent = npObj->RootNode->Component;

                var lastChild = npComponent->UldManager.RootNode;
                while (lastChild->PrevSiblingNode != null) lastChild = lastChild->PrevSiblingNode;

                lastChild->PrevSiblingNode = nodeGroup.RootNode;
                nodeGroup.RootNode->NextSiblingNode = lastChild;
                nodeGroup.RootNode->ParentNode = (AtkResNode*) npObj->RootNode;

                npComponent->UldManager.UpdateDrawNodeList();

                NodeGroups.Add((ulong)npObj, nodeGroup);
            }

            return true;
        }

        public void DestroyNodes()
        {
            if (namePlateAddon == null) return;

            for(byte i = 0; i < NamePlateCount; i++)
            {
                var npObj = &namePlateAddon->NamePlateObjectArray[i];
                var npComponent = npObj->RootNode->Component;

                StatusNodeGroup nodeGroup;
                if (NodeGroups.TryGetValue((ulong)npObj, out nodeGroup))
                {
                    var lastDefaultNode = nodeGroup.RootNode->NextSiblingNode;
                    lastDefaultNode->PrevSiblingNode = null;
                    nodeGroup.DestroyNodes();
                }
                NodeGroups.Remove((ulong)npObj);

                npComponent->UldManager.UpdateDrawNodeList();
            }

            if (NodeGroups.Any())
            {
                PluginLog.Debug("node group still has member after removing all - something went wrong");
            }
        }
    }
}

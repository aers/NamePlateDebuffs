using Dalamud.Hooking;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
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

        private StatusNodeGroup[] NodeGroups;

        private ExcelSheet<Status> StatusSheet;

        private static byte NamePlateCount = 50;
        private static uint StartingNodeId = 50000;

        public bool Built { get; private set; }

        internal StatusNodeManager(NamePlateDebuffsPlugin p)
        {
            _plugin = p; 

            NodeGroups = new StatusNodeGroup[NamePlateCount];

            StatusSheet = _plugin.Interface.Data.GetExcelSheet<Status>();
        }

        public void Dispose()
        {
            DestroyNodes();
        }

        public void SetNamePlateAddonPointer(AddonNamePlate* addon)
        {
            namePlateAddon = addon;
        }

        public void SetGroupVisibility(int index, bool enable, bool setChildren = false)
        {
            var group = NodeGroups[index];

            if (group == null)
                return;

            group.SetVisibility(enable, setChildren);
        }

        public void SetStatus(int groupIndex, int statusIndex, int id, int timer)
        {
            var group = NodeGroups[groupIndex];

            if (group == null)
                return;

            var row = StatusSheet.GetRow((uint) id);
            
            group.SetStatus(statusIndex, row.Icon, timer);
        }

        public void HideUnusedStatus(int groupIndex, int statusCount)
        {
            var group = NodeGroups[groupIndex];

            if (group == null)
                return;

            group.HideUnusedStatus(statusCount);
        }

        public bool BuildNodes(bool rebuild = false)
        {
            if (namePlateAddon == null) return false;
            if (Built && !rebuild) return true;
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

                NodeGroups[i] = nodeGroup;
            }

            Built = true;

            return true;
        }

        public void DestroyNodes()
        {
            if (namePlateAddon == null) return;

            for(byte i = 0; i < NamePlateCount; i++)
            {
                var npObj = &namePlateAddon->NamePlateObjectArray[i];
                var npComponent = npObj->RootNode->Component;

                if (NodeGroups[i] != null)
                {
                    var lastDefaultNode = NodeGroups[i].RootNode->NextSiblingNode;
                    lastDefaultNode->PrevSiblingNode = null;
                    NodeGroups[i].DestroyNodes();
                }
                NodeGroups[i] = null;

                npComponent->UldManager.UpdateDrawNodeList();
            }

            Built = false;
        }
    }
}

using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace NamePlateDebuffs.StatusNode
{
    internal unsafe class StatusNodeGroup
    {
        public AtkResNode* RootNode { get; private set; }
        public StatusNode[] StatusNodes { get; private set; }

        public static ushort NodePerGroupCount = 4;

        public StatusNodeGroup()
        {
            StatusNodes = new StatusNode[NodePerGroupCount];
            for (int i = 0; i < NodePerGroupCount; i++)
                StatusNodes[i] = new StatusNode();
        }

        public bool Built()
        {
            if (RootNode == null) return false;
            foreach (var node in StatusNodes)
                if (!node.Built()) return false;

            return true;
        }

        public bool BuildNodes(uint baseNodeId)
        {
            if (Built()) return true;

            var rootNode = CreateRootNode();
            if (rootNode == null) return false;
            RootNode = rootNode;
            RootNode->NodeID = baseNodeId;

            for (uint i = 0; i < NodePerGroupCount; i++)
            {
                if (!StatusNodes[i].BuildNodes(baseNodeId + 1 + i * 3))
                {
                    DestroyNodes();
                    return false;
                }
            }

            RootNode->ChildCount = (ushort) (NodePerGroupCount * 3);
            RootNode->ChildNode = StatusNodes[0].RootNode;
            StatusNodes[0].RootNode->ParentNode = RootNode;

            var lastNode = StatusNodes[0].RootNode;
            for (uint i = 1; i < NodePerGroupCount; i++)
            {
                var currNode = StatusNodes[i].RootNode;
                lastNode->PrevSiblingNode = currNode;
                currNode->NextSiblingNode = lastNode;
                currNode->ParentNode = RootNode;
                lastNode = currNode;
            }

            SetupLayout();
            SetupVisibility();

            return true;
        }

        public void DestroyNodes()
        {
            foreach(var node in StatusNodes)
            {
                node.DestroyNodes();
            }
            if (RootNode != null)
            {
                RootNode->Destroy(true);
                RootNode = null;
            }
        }

        public void SetupLayout()
        {
            for (uint i = 0; i < NodePerGroupCount; i++)
            {
                StatusNodes[i].RootNode->SetPositionShort((short)(i * (24 + 3)), 0);
            }
        }

        public void SetupVisibility()
        {
            for (uint i = 0; i < NodePerGroupCount; i++)
            {
                StatusNodes[i].IconNode->AtkResNode.ToggleVisibility(true);
                StatusNodes[i].DurationNode->AtkResNode.ToggleVisibility(true);
                StatusNodes[i].RootNode->ToggleVisibility(true);
            }

            RootNode->ToggleVisibility(true);
        }

        private AtkResNode* CreateRootNode()
        {
            var newResNode = (AtkResNode*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkResNode), 8);
            if (newResNode == null)
            {
                PluginLog.Debug("Failed to allocate memory for res node");
                return null;
            }
            IMemorySpace.Memset(newResNode, 0, (ulong)sizeof(AtkResNode));
            newResNode->Ctor();

            newResNode->Type = NodeType.Res;
            newResNode->SetHeight(41);
            newResNode->SetWidth(24 * 4 + 3 * 3);
            newResNode->Flags = (short)(NodeFlags.AnchorLeft | NodeFlags.AnchorTop);
            newResNode->DrawFlags = 0;

            return newResNode;
        }
    }
}

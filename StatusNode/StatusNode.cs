using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace NamePlateDebuffs.StatusNode
{
    internal unsafe class StatusNode
    {
        public AtkResNode* RootNode { get; private set; }
        public AtkImageNode* IconNode { get; private set; }
        public AtkTextNode* DurationNode { get; private set; }
        public bool Visible { get; private set; }

        private static int DefaultIconId = 10205;

        public bool Built() => RootNode != null && IconNode != null && DurationNode != null;

        public bool BuildNodes(uint baseNodeId)
        {
            if (Built()) return true;

            var rootNode = CreateRootNode();
            if (rootNode == null) return false;
            RootNode = rootNode;

            var iconNode = CreateIconNode();
            if (iconNode == null)
            {
                DestroyNodes();
                return false;
            }
            IconNode = iconNode;

            var durationNode = CreateDurationNode();
            if (durationNode == null)
            {
                DestroyNodes();
                return false;
            }
            DurationNode = durationNode;

            RootNode->NodeID = baseNodeId;
            RootNode->ChildCount = 2;
            RootNode->ChildNode = (AtkResNode*) IconNode;

            IconNode->AtkResNode.NodeID = baseNodeId + 1;
            IconNode->AtkResNode.ParentNode = RootNode;
            IconNode->AtkResNode.PrevSiblingNode = (AtkResNode*)DurationNode;

            DurationNode->AtkResNode.NodeID = baseNodeId + 2;
            DurationNode->AtkResNode.ParentNode = RootNode;
            DurationNode->AtkResNode.NextSiblingNode = (AtkResNode*)IconNode;

            return true;
        }

        public void DestroyNodes()
        {
            if (IconNode != null)
            {
                IconNode->UnloadTexture();
                IMemorySpace.Free(IconNode->PartsList->Parts->UldAsset, (ulong)sizeof(AtkUldAsset));
                IMemorySpace.Free(IconNode->PartsList->Parts, (ulong)sizeof(AtkUldPart));
                IMemorySpace.Free(IconNode->PartsList, (ulong)sizeof(AtkUldPartsList));
                IconNode->PartsList = null;
                IconNode->AtkResNode.Destroy(true);
                IconNode = null;
            }
            if (DurationNode != null)
            {
                DurationNode->AtkResNode.Destroy(true);
                DurationNode = null;
            }
            if (RootNode != null)
            {
                RootNode->Destroy(true);
                RootNode = null;
            }
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
            newResNode->Flags = (short)(NodeFlags.AnchorLeft | NodeFlags.AnchorTop);
            newResNode->DrawFlags = 0;

            newResNode->SetWidth(24);
            newResNode->SetHeight(41);

            return newResNode;
        }

        private AtkImageNode* CreateIconNode()
        {
            var newImageNode = (AtkImageNode*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkImageNode), 8);
            if (newImageNode == null)
            {
                PluginLog.Debug("Failed to allocate memory for image node");
                return null;
            }
            IMemorySpace.Memset(newImageNode, 0, (ulong)sizeof(AtkImageNode));
            newImageNode->Ctor();

            newImageNode->AtkResNode.Type = NodeType.Image;
            newImageNode->AtkResNode.SetWidth(24);
            newImageNode->AtkResNode.SetHeight(32);
            newImageNode->AtkResNode.Flags = (short)(NodeFlags.AnchorLeft | NodeFlags.AnchorTop);
            newImageNode->AtkResNode.DrawFlags = 0;

            newImageNode->WrapMode = 1;
            newImageNode->Flags |= (byte)ImageNodeFlags.AutoFit;

            var partsList = (AtkUldPartsList*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPartsList), 8);
            if (partsList == null)
            {
                PluginLog.Debug("Failed to allocate memory for parts list");
                newImageNode->AtkResNode.Destroy(true);
                return null;
            }

            partsList->Id = 0;
            partsList->PartCount = 1;

            var part = (AtkUldPart*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPart), 8);
            if (part == null)
            {
                PluginLog.Debug("Failed to allocate memory for part");
                IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                newImageNode->AtkResNode.Destroy(true);
            }

            part->U = 0;
            part->V = 0;
            part->Width = 24;
            part->Height = 32;

            partsList->Parts = part;

            var asset = (AtkUldAsset*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldAsset), 8);
            if (asset == null)
            {
                PluginLog.Log("Failed to allocate memory for asset");
                IMemorySpace.Free(part, (ulong)sizeof(AtkUldPart));
                IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                newImageNode->AtkResNode.Destroy(true);
            }

            asset->Id = 0;
            asset->AtkTexture.Ctor();

            part->UldAsset = asset;

            newImageNode->PartsList = partsList;

            newImageNode->LoadIconTexture(DefaultIconId, 0);

            return newImageNode;
        }

        private AtkTextNode* CreateDurationNode()
        {
            var newTextNode = (AtkTextNode*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkTextNode), 8);
            if (newTextNode == null)
            {
                PluginLog.Debug("Failed to allocate memory for text node");
                return null;
            }
            IMemorySpace.Memset(newTextNode, 0, (ulong)sizeof(AtkTextNode));
            newTextNode->Ctor();

            newTextNode->AtkResNode.Type = NodeType.Text;
            newTextNode->AtkResNode.SetPositionShort(0, 23);
            newTextNode->AtkResNode.SetWidth(24);
            newTextNode->AtkResNode.SetHeight(18);
            newTextNode->AtkResNode.Flags = (short)(NodeFlags.AnchorLeft | NodeFlags.AnchorTop);
            newTextNode->AtkResNode.DrawFlags = 12;

            newTextNode->LineSpacing = 12;
            newTextNode->AlignmentFontType = 4;
            newTextNode->FontSize = 12;
            newTextNode->TextFlags = 8;
            newTextNode->TextFlags2 = 0;

            newTextNode->TextColor.R = 0xFF;
            newTextNode->TextColor.G = 0xFF;
            newTextNode->TextColor.B = 0xFF;
            newTextNode->TextColor.A = 0xFF;

            newTextNode->EdgeColor.R = 0x00;
            newTextNode->EdgeColor.G = 0x00;
            newTextNode->EdgeColor.B = 0x00;
            newTextNode->EdgeColor.A = 0xFF;

            newTextNode->SetText("60");

            return newTextNode;
        }
    }
}

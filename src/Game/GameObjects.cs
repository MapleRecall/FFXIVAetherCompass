using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using static FFXIVClientStructs.FFXIV.Client.UI.UI3DModule;

namespace AetherCompass.Game
{
    internal unsafe static class GameObjects
    {
        private unsafe static readonly UI3DModule* UI3DModule 
            = ((UIModule*)Plugin.GameGui.GetUIModule())->GetUI3DModule();

        // Those that would be rendered on screen
        internal unsafe static ObjectInfo** SortedObjectInfoPointerArray
        => UI3DModule != null
            ? (ObjectInfo**)Unsafe.AsPointer(ref MemoryMarshal.GetReference(UI3DModule->SortedObjectInfoPointers)) //(ObjectInfo**)UI3DModule->SortedObjectInfoPointerArray 
            : null;
        internal unsafe static int SortedObjectInfoCount
            => UI3DModule != null ? UI3DModule->SortedObjectInfoCount : 0;

#if DEBUG
        private unsafe static readonly GameObjectManager* gameObjMgr 
            = GameObjectManager.Instance();
        internal unsafe static GameObject* ObjectListFiltered
            => (GameObject*)&gameObjMgr->Objects;
        internal unsafe static int ObjectListFilteredCount
            => gameObjMgr->Objects.GameObjectIdSortedCount;

        static GameObjects()
        {
            LogDebug($"UI3DModule @{(IntPtr)UI3DModule:X}");
            // LogDebug($"SortedObjectInfoPointerArray @{(IntPtr)MemoryMarshal.GetReference(UI3DModule->SortedObjectInfoPointers):X}");
            LogDebug($"SortedObjectInfoCount = {SortedObjectInfoCount}");
        }
#endif

    }
}

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using static FFXIVClientStructs.FFXIV.Client.UI.UI3DModule;

namespace AetherCompass.Game;

internal static unsafe class GameObjects
{
	private static readonly UI3DModule* UI3DModule = (
		(UIModule*)Plugin.GameGui.GetUIModule()
	)->GetUI3DModule();

	// Those that would be rendered on screen
	internal static ObjectInfo** SortedObjectInfoPointerArray =>
		UI3DModule != null
			? (ObjectInfo**)
				Unsafe.AsPointer(
					ref MemoryMarshal.GetReference(UI3DModule->SortedObjectInfoPointers)
				) //(ObjectInfo**)UI3DModule->SortedObjectInfoPointerArray
			: null;

	internal static int SortedObjectInfoCount =>
		UI3DModule != null ? UI3DModule->SortedObjectInfoCount : 0;

#if DEBUG

	private static readonly GameObjectManager* gameObjMgr = GameObjectManager.Instance();

	internal static GameObject* ObjectListFiltered => (GameObject*)&gameObjMgr->Objects;

	internal static int ObjectListFilteredCount => gameObjMgr->Objects.GameObjectIdSortedCount;

	static GameObjects()
	{
		LogDebug($"UI3DModule @{(IntPtr)UI3DModule:X}");
		// LogDebug($"SortedObjectInfoPointerArray @{(IntPtr)MemoryMarshal.GetReference(UI3DModule->SortedObjectInfoPointers):X}");
		LogDebug($"SortedObjectInfoCount = {SortedObjectInfoCount}");
	}

#endif
}

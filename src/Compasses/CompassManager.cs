﻿using AetherCompass.Common;
using AetherCompass.Game;
using AetherCompass.UI;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using System.Collections.Generic;
using System.Threading;
using ObjectInfo = FFXIVClientStructs.FFXIV.Client.UI.UI3DModule.ObjectInfo;


namespace AetherCompass.Compasses
{
    public unsafe sealed class CompassManager
    {
        private readonly HashSet<Compass> compasses = new();
        private readonly HashSet<Compass> workingCompasses = new();

        private CancellationTokenSource cancellationTokenSrc = new();


        private unsafe static UI3DModule* UI3DModule => ((UIModule*)Plugin.GameGui.GetUIModule())->GetUI3DModule();

        // Those that would be rendered on screen
        private unsafe static ObjectInfo** SortedObjectInfoPointerArray
            => UI3DModule != null ? (ObjectInfo**)UI3DModule->SortedObjectInfoPointerArray : null;
        private unsafe static int SortedObjectInfoCount => UI3DModule != null ? UI3DModule->SortedObjectInfoCount : 0;

#if DEBUG
        private unsafe static readonly GameObjectManager* gameObjMgr = GameObjectManager.Instance();
#endif


        private bool hasMapFlagToProcess;
        private System.Numerics.Vector2 mapFlagCoord;


        public bool AddCompass(Compass c)
        {
            if (!compasses.Add(c)) return false;
            if (!Plugin.DetailsWindow.RegisterCompass(c)) return false;
            if (c.IsEnabledInCurrentTerritory())
                workingCompasses.Add(c);
            return true;
        }

        public bool RemoveCompass(Compass c)
        {
            if (!compasses.Contains(c)) return false;
            Plugin.DetailsWindow.UnregisterCompass(c);
            workingCompasses.Remove(c);
            return compasses.Remove(c);
        }

        public void RegisterMapFlag(System.Numerics.Vector2 flagCoord)
        {
            hasMapFlagToProcess = true;
            mapFlagCoord = flagCoord;
        }

        public void OnTick()
        {
            var lastCancellationTokenSrc = cancellationTokenSrc;
            if (!cancellationTokenSrc.IsCancellationRequested)
                cancellationTokenSrc.Cancel();
            cancellationTokenSrc = new();

            Plugin.Overlay.Clear();
            Plugin.DetailsWindow.Clear();

            if (workingCompasses.Count > 0)
            {
                foreach (var compass in workingCompasses)
                    if (compass.CompassEnabled) compass.Reset();

#if DEBUG
                var debugTestAll = gameObjMgr != null && Plugin.Config.DebugTestAllGameObjects;
                void* array = debugTestAll ? gameObjMgr->ObjectListFiltered : SortedObjectInfoPointerArray;
                int count = debugTestAll ? gameObjMgr->ObjectListFilteredCount : SortedObjectInfoCount;
#else
                var array = SortedObjectInfoPointerArray;
                var count = SortedObjectInfoCount;
#endif

                if (array == null) return;

                foreach (var compass in workingCompasses)
                {
                    if (compass.CompassEnabled)
                    {
#if DEBUG
                        if (debugTestAll)
                            compass.ProcessLoopDebugAllObjects((GameObject**)array, count, cancellationTokenSrc.Token);
                        else
#endif
                            compass.ProcessLoop((ObjectInfo**)array, count, cancellationTokenSrc.Token);
                    }

                }
            }

            ProcessFlagOnTickEnd();

            lastCancellationTokenSrc.Dispose();
        }

        private void ProcessFlagOnTickEnd()
        {
            if (!hasMapFlagToProcess) return;

            // NOTE: Dirty fix
            // Currently Dalamud's MapLinkPayload internally does not take into account Map's X/Y-offset,
            // so in map with non-zero offsets (e.g., Mist subdivision) it's always incorrect.
            // Tweak it with a FixedMapLinkPayload that has the original raw X/Y
            // but our calcualted map coord to fix this issue.
            var terrId = Plugin.ClientState.TerritoryType;
            //var maplink = new MapLinkPayload(terrId, ZoneWatcher.CurrentMapId, 
            //    mapFlagCoord.X, mapFlagCoord.Y, fudgeFactor: 0.01f);
            var map = ZoneWatcher.CurrentMap;
            if (map != null)
            {
                var fixedMapLink = FixedMapLinkPayload.FromMapCoord(terrId, ZoneWatcher.CurrentMapId,
                    mapFlagCoord.X, mapFlagCoord.Y, map.SizeFactor, map.OffsetX, map.OffsetY);
#if DEBUG
                Plugin.LogDebug($"Create MapLinkPayload from {mapFlagCoord}: {fixedMapLink}");
#endif
                //if (Plugin.GameGui.OpenMapWithMapLink(maplinkFix))
                //{
                //    var msg = Chat.CreateMapLink(terrId, ZoneWatcher.CurrentMapId, maplink.XCoord, maplink.YCoord).PrependText("Flag set: ");
                //    Chat.PrintChat(msg);
                //    hasMapFlagToProcess = false;
                //}
                if (Plugin.GameGui.OpenMapWithMapLink(fixedMapLink))
                {
                    var msg = Chat.CreateMapLink(fixedMapLink).PrependText("Flag set: ");
                    Chat.PrintChat(msg);
                }
            }

            hasMapFlagToProcess = false;
        }

        public void OnZoneChange()
        {
            try
            {
                cancellationTokenSrc.Cancel();
                workingCompasses.Clear();
                foreach (var compass in compasses)
                {
                    compass.OnZoneChange();
                    if (compass.IsEnabledInCurrentTerritory())
                        workingCompasses.Add(compass);
                }
            } catch (System.ObjectDisposedException) { }
        }

        public void DrawCompassConfigUi()
        {
            foreach (var compass in compasses)
            {
                compass.DrawConfigUi();
                ImGuiNET.ImGui.NewLine();
            }
        }
    }
}

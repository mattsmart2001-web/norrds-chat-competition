// Streamer.bot Action: "Draw - Sync Entries"
// Triggers:
//   1. WebSocket Server > Client Connected — auto-syncs whenever an overlay connects
//   2. Hotkey — e.g. Ctrl+Shift+S for manual sync

using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        string json = CPH.GetGlobalVar<string>("draw_entries", true);
        var entries = string.IsNullOrEmpty(json)
            ? new List<Dictionary<string, string>>()
            : JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

        var payload = new
        {
            type         = "full_sync",
            entries      = entries,
            totalEntries = entries.Count
        };

        CPH.WebsocketBroadcastString(JsonConvert.SerializeObject(payload));
        CPH.LogInfo($"[Draw] Synced {entries.Count} entries to overlays.");
        return true;
    }
}

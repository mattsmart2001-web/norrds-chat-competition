// Streamer.bot Action: "Draw - Sync Entries"
// Trigger: Called automatically when an overlay connects (via DoAction from the overlay JS)
// Also wire this to a hotkey if you want to manually force a sync.

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

        CPH.WebsocketBroadcastString(
            JsonConvert.SerializeObject(new {
                @event = new { source = "General", type = "Custom" },
                data   = new { name = "DrawUpdate", data = JsonConvert.SerializeObject(payload) }
            })
        );

        CPH.LogInfo($"[Draw] Synced {entries.Count} entries to overlays.");
        return true;
    }
}

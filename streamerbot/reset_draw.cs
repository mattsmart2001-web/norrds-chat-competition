// Streamer.bot Action: "Draw - Reset"
// Use between streams (or if you need to clear entries).
// Wire to a hotkey — e.g. Ctrl+Shift+R — and confirm before pressing!

using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Clear stored entries
        CPH.SetGlobalVar("draw_entries",     "[]",  true);
        CPH.SetGlobalVar("draw_winner_name", "",    true);
        CPH.SetGlobalVar("draw_winner_id",   "",    true);

        // Notify overlays
        var payload = new { type = "reset" };
        CPH.WebsocketBroadcastString(
            JsonConvert.SerializeObject(new {
                @event = new { source = "General", type = "Custom" },
                data   = new { name = "DrawUpdate", data = JsonConvert.SerializeObject(payload) }
            })
        );

        CPH.LogInfo("[Draw] Draw has been reset.");
        return true;
    }
}

// Streamer.bot Action: "Draw - Reset"
// Trigger: YouTube > Chat > Message (same as Draw - Enter)
// Only fires on exactly "!reset" — won't loop on bot messages.

using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Guard: only fire on exactly !reset
        string msgText = "";
        if (args.ContainsKey("rawInput")) msgText = args["rawInput"].ToString().Trim().ToLower();
        else if (args.ContainsKey("message")) msgText = args["message"].ToString().Trim().ToLower();
        if (!string.IsNullOrEmpty(msgText) && msgText != "!reset")
            return false;

        CPH.SetGlobalVar("draw_entries",     "[]", true);
        CPH.SetGlobalVar("draw_winner_name", "",   true);
        CPH.SetGlobalVar("draw_winner_id",   "",   true);

        CPH.LogInfo("[Draw] Draw has been reset.");
        CPH.SendYouTubeMessage("🔄 The draw has been reset. Type !enter to enter!");
        return true;
    }
}

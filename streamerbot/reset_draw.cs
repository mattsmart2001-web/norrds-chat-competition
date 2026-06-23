// Streamer.bot Action: "Draw - Reset"
// Trigger: YouTube > Chat > Message (no criteria — guard handles filtering)

using System.IO;
using Newtonsoft.Json;

public class CPHInline
{
    const string OVERLAY_DIR = @"D:\chatcomp\norrds-chat-competition-claude-eric-shannon-8qkyob\overlay";

    public bool Execute()
    {
        string msgText = "";
        if (args.ContainsKey("rawInput")) msgText = args["rawInput"].ToString().Trim().ToLower();
        else if (args.ContainsKey("message")) msgText = args["message"].ToString().Trim().ToLower();
        if (!string.IsNullOrEmpty(msgText) && msgText != "!reset")
            return false;

        CPH.SetGlobalVar("draw_entries",     "[]", true);
        CPH.SetGlobalVar("draw_winner_name", "",   true);
        CPH.SetGlobalVar("draw_winner_id",   "",   true);

        try
        {
            var data = new { totalEntries = 0, entries = new object[0] };
            string path = Path.Combine(OVERLAY_DIR, "draw_data.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
        }
        catch { }

        CPH.LogInfo("[Draw] Draw has been reset.");
        CPH.SendYouTubeMessage("🔄 The draw has been reset. Type !enter to enter!");
        return true;
    }
}

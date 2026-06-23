// Streamer.bot Action: "Draw - Enter"
// Trigger: YouTube > Chat > Message (no criteria — guard handles filtering)

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    // !! SET THIS to the full path of your overlay folder (where index.html lives)
    const string OVERLAY_DIR = @"D:\chatcomp\norrds-chat-competition-claude-epic-shannon-8qkyob\overlay";

    public bool Execute()
    {
        // Guard: only process exact !enter messages
        string msgText = "";
        if (args.ContainsKey("rawInput")) msgText = args["rawInput"].ToString().Trim().ToLower();
        else if (args.ContainsKey("message")) msgText = args["message"].ToString().Trim().ToLower();
        if (!string.IsNullOrEmpty(msgText) && msgText != "!enter")
            return false;

        string userId   = args.ContainsKey("userId")   ? args["userId"].ToString()   : "";
        string userName = args.ContainsKey("userName") ? args["userName"].ToString() : "Unknown";
        string avatar   = args.ContainsKey("userProfileUrl") ? args["userProfileUrl"].ToString() : "";

        if (string.IsNullOrEmpty(userId))
        {
            CPH.LogWarn("[Draw] !enter fired with no userId — skipping.");
            return false;
        }

        string json = CPH.GetGlobalVar<string>("draw_entries", true);
        var entries = string.IsNullOrEmpty(json)
            ? new List<Dictionary<string, string>>()
            : JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

        bool alreadyEntered = entries.Exists(e =>
            e.ContainsKey("userId") && e["userId"] == userId);

        if (alreadyEntered)
        {
            CPH.SendYouTubeMessage($"@{userName} you're already in the draw! 🏁 Good luck!");
            return true;
        }

        var entry = new Dictionary<string, string>
        {
            { "userId",     userId },
            { "userName",   userName },
            { "userAvatar", avatar },
            { "timestamp",  DateTime.UtcNow.ToString("o") }
        };
        entries.Add(entry);
        CPH.SetGlobalVar("draw_entries", JsonConvert.SerializeObject(entries), true);

        WriteDataFile(entries);

        CPH.SendYouTubeMessage(
            $"@{userName} you're entered! 🎉 ({entries.Count} total entries) | Type !enter to join the 24H Race Draw!");

        CPH.LogInfo($"[Draw] {userName} entered. Total: {entries.Count}");
        return true;
    }

    void WriteDataFile(List<Dictionary<string, string>> entries)
    {
        try
        {
            var data = new { totalEntries = entries.Count, entries = entries };
            string path = Path.Combine(OVERLAY_DIR, "draw_data.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
            CPH.LogInfo($"[Draw] Wrote draw_data.json ({entries.Count} entries)");
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Draw] Failed to write draw_data.json: {ex.Message}");
        }
    }
}

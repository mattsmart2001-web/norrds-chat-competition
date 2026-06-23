// Streamer.bot Action: "Draw - Enter"
// Trigger: YouTube Chat Message Command — !enter
// Sub-action: Execute C# Code → paste this entire file

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Guard: only process exact !enter messages (prevents bot response loop)
        // Core>Commands uses "rawInput"; YouTube>Chat uses "message"
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

        // Load existing entries (persisted across stream restarts)
        string json = CPH.GetGlobalVar<string>("draw_entries", true);
        var entries = string.IsNullOrEmpty(json)
            ? new List<Dictionary<string, string>>()
            : JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

        // Deduplicate
        bool alreadyEntered = entries.Exists(e =>
            e.ContainsKey("userId") && e["userId"] == userId);

        if (alreadyEntered)
        {
            CPH.SendYouTubeMessage($"@{userName} you're already in the draw! 🏁 Good luck!");
            return true;
        }

        // Add entry
        var entry = new Dictionary<string, string>
        {
            { "userId",      userId },
            { "userName",    userName },
            { "userAvatar",  avatar },
            { "timestamp",   DateTime.UtcNow.ToString("o") }
        };
        entries.Add(entry);
        CPH.SetGlobalVar("draw_entries", JsonConvert.SerializeObject(entries), true);

        // Broadcast to overlays — send payload directly as a raw JSON string
        var payload = new
        {
            type         = "new_entry",
            entry        = entry,
            entries      = entries,
            totalEntries = entries.Count
        };
        string broadcastJson = JsonConvert.SerializeObject(payload);
        CPH.WebsocketCustomServerBroadcast(broadcastJson, null, "Draw Overlay");
        CPH.LogInfo($"[Draw] Broadcast: {broadcastJson}");

        CPH.SendYouTubeMessage(
            $"@{userName} you're entered! 🎉 ({entries.Count} total entries) | Type !enter to join the 24H Race Draw!");

        CPH.LogInfo($"[Draw] {userName} entered. Total: {entries.Count}");
        return true;
    }
}

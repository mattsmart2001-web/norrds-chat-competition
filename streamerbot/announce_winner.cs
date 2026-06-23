// Streamer.bot Action: "Draw - Announce Winner"
// Triggers:
//   1. WebSocket Server > Message — fires when winner overlay sends {"type":"announce_winner",...}
//      (Set trigger criteria: message contains "announce_winner")
//   2. Hotkey — optional, for manual announcement

using Newtonsoft.Json;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        // When triggered by WebSocket message, parse the winner from the message body
        string winnerName = "";
        string winnerId   = "";

        if (args.ContainsKey("message"))
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    args["message"].ToString());
                if (msg.ContainsKey("winnerName")) winnerName = msg["winnerName"];
                if (msg.ContainsKey("winnerId"))   winnerId   = msg["winnerId"];
            }
            catch { }
        }

        // Fallback: args passed directly (e.g. from hotkey with set args)
        if (string.IsNullOrEmpty(winnerName) && args.ContainsKey("winnerName"))
            winnerName = args["winnerName"].ToString();
        if (string.IsNullOrEmpty(winnerId) && args.ContainsKey("winnerId"))
            winnerId = args["winnerId"].ToString();

        if (string.IsNullOrEmpty(winnerName))
        {
            CPH.LogWarn("[Draw] announce_winner fired but no winnerName found in args.");
            return false;
        }

        CPH.SendYouTubeMessage(
            $"🏆 The winner of the 24H Race Draw is @{winnerName}! " +
            $"Congratulations! You've won a 10% SimLab Voucher & a SparksTheory Hoodie! " +
            $"Please DM the channel to claim your prizes! 🎉");

        CPH.SetGlobalVar("draw_winner_name", winnerName, true);
        CPH.SetGlobalVar("draw_winner_id",   winnerId,   true);

        CPH.LogInfo($"[Draw] Winner announced: {winnerName}");
        return true;
    }
}

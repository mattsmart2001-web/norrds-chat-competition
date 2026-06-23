// Streamer.bot Action: "Draw - Announce Winner"
// Triggered automatically by the winner overlay after the spin animation completes.
// Args passed in: winnerName, winnerId
// You can also wire a hotkey to this to announce in chat manually.

using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        string winnerName = args.ContainsKey("winnerName") ? args["winnerName"].ToString() : "Unknown";
        string winnerId   = args.ContainsKey("winnerId")   ? args["winnerId"].ToString()   : "";

        // Announce in YouTube chat
        CPH.SendYouTubeMessage(
            $"🏆 The winner of the 24H Race Draw is @{winnerName}! " +
            $"Congratulations! You've won a 10% SimLab Voucher & a SparksTheory Hoodie! " +
            $"Please DM the channel to claim your prizes! 🎉");

        // Store winner for reference
        CPH.SetGlobalVar("draw_winner_name", winnerName, true);
        CPH.SetGlobalVar("draw_winner_id",   winnerId,   true);

        CPH.LogInfo($"[Draw] Winner announced: {winnerName} ({winnerId})");
        return true;
    }
}

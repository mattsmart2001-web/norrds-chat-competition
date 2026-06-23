# Norrds Chat Competition — 24H Race Draw

A Streamer.bot + OBS overlay for a YouTube live draw across a 24-hour race spanning three streams.

## Prizes
- 10% SimLab Voucher
- SparksTheory Hoodie

---

## File Structure

```
overlay/
  index.html   → Live entry feed (add to OBS as Browser Source)
  winner.html  → Winner picker (separate OBS scene/source)
streamerbot/
  enter_command.cs   → Processes !enter from YouTube chat
  sync_entries.cs    → Sends full entry list to overlays on connect
  announce_winner.cs → Posts winner announcement to YouTube chat
  reset_draw.cs      → Clears all entries (use between streams)
```

---

## Streamer.bot Setup

### 1. Enable the WebSocket Server
`Settings → WebSocket Server → Enable` (default port 8080, leave as-is)

### 2. Create four Actions

| Action Name | C# File |
|---|---|
| `Draw - Enter` | `enter_command.cs` |
| `Draw - Sync Entries` | `sync_entries.cs` |
| `Draw - Announce Winner` | `announce_winner.cs` |
| `Draw - Reset` | `reset_draw.cs` |

For each action:
1. Right-click in the Actions panel → **Add Action** → name it exactly as above
2. Add sub-action: **Core → C# → Execute C# Code**
3. Paste the contents of the corresponding `.cs` file

### 3. Set up the !enter command trigger

1. Open the `Draw - Enter` action
2. Click **Add Trigger** → **YouTube → Chat Message Command**
3. Set command to `!enter`
4. Set **Command Location** to `Anywhere in message` or `Starts with`
5. Leave cooldown at 0 (deduplication is handled in code)

### 4. Wire up Sync (optional but recommended)

Wire `Draw - Sync Entries` to a hotkey (e.g. `Ctrl+Shift+S`) so you can push the full entry list to overlays any time after a reconnect.

### 5. Wire up Reset

Wire `Draw - Reset` to a hotkey (e.g. `Ctrl+Shift+R`). Only press between streams — this clears all entries permanently.

---

## OBS Setup

### Entry Feed Overlay
1. In your main streaming scene, add a **Browser Source**
2. Set it to **Local File** → browse to `overlay/index.html`
3. Width: `420`, Height: `520`
4. Tick **Refresh browser when scene becomes active**

### Winner Picker Overlay
1. Create a new scene called `Winner Reveal` (or add it to your end-of-race scene)
2. Add a **Browser Source** → `overlay/winner.html`
3. Width: `600`, Height: `380`

To use: switch to the Winner Reveal scene, click **Pick Winner** in the overlay. The animation runs, picks a random winner, and automatically posts the announcement to YouTube chat via Streamer.bot.

---

## Between Streams

Entries **persist across restarts** — they're stored in Streamer.bot's global variables with `persisted = true`. You do not need to do anything between streams. Chatters who already typed `!enter` in a previous stream will not be able to enter again (deduplication is by YouTube user ID).

To fully reset the draw (e.g. start fresh), trigger `Draw - Reset`.

---

## How It Works

1. Viewer types `!enter` in YouTube chat
2. Streamer.bot fires `Draw - Enter`, checks for duplicates, adds the viewer
3. Broadcasts a WebSocket message to all connected overlays
4. Entry overlay animates the new entry in and updates the count
5. At end of race, switch to Winner Reveal scene → click Pick Winner
6. Spin animation plays, selects a random winner
7. Winner is revealed with confetti + automatically announced in YouTube chat

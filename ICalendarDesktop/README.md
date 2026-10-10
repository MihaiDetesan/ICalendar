# ICalendarDesktop

Cross-platform (Windows/macOS/Linux) desktop app for a year-at-a-glance view of
Google Calendar, built with Avalonia UI. Replaces the old WinForms app's UI
while reusing the same Google Calendar import/sync approach.

- `ICalendar.Core` — shared library: Google OAuth, calendar/event sync (`GoogleCalendarImporter`).
- `ICalendar.Desktop` — the Avalonia UI (year grid, calendar list, add/edit/delete events).

## Running it

You need a Google OAuth client to talk to the Calendar API:

1. In [Google Cloud Console](https://console.cloud.google.com/), create (or reuse) a project,
   enable the **Google Calendar API**, and create an OAuth client ID of type **Desktop app**.
2. Download its JSON and save it as `ICalendar.Desktop/credentials.json`
   (see `credentials.example.json` for the expected shape). This file is gitignored —
   it's a secret tied to your Google Cloud project, not something to commit.
3. Build and run:
   ```
   dotnet run --project ICalendar.Desktop
   ```
   On first run it opens a browser for Google sign-in and caches the resulting token
   under your OS's local app-data directory.

Note: while your OAuth consent screen is in "Testing" mode (the Cloud Console default),
only Google accounts you've explicitly added as test users can sign in.

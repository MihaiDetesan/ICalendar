using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using ICalendar.Core.Model;

namespace ICalendar.Core
{
    public class GoogleCalendarImporter
    {
        private static readonly string[] Scopes =
        {
            CalendarService.Scope.CalendarEvents,
            CalendarService.Scope.CalendarReadonly,
            CalendarService.Scope.Calendar
        };

        private readonly GoogleOptions options;
        private CalendarService? calendarService;

        public GoogleCalendarImporter(GoogleOptions options)
        {
            this.options = options;
            Events = new List<EventDay>();
            Calendars = new List<CalendarListEntry>();
        }

        public IList<EventDay> Events { get; private set; }
        public IList<CalendarListEntry> Calendars { get; private set; }

        /// <summary>
        /// Runs the OAuth flow (opening a browser if needed) and prepares the Calendar API client.
        /// Must be called once before any other method.
        /// </summary>
        public async Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            var credentials = await CreateCredentialsAsync(options.CredentialsPath, options.TokenPath, cancellationToken);

            if (credentials == null)
            {
                throw new UnauthorizedAccessException("Could not authenticate to Google Calendar");
            }

            calendarService = GetCalendarService(credentials);
        }

        public async Task RefreshEventsAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();

            Calendars.Clear();
            Events.Clear();

            await GetCalendarsFromGoogleAsync(cancellationToken);
            Events = await GetEventsFromGoogleAsync(startTime, endTime, cancellationToken);
        }

        public async Task AddEventAsync(EventDay @event, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();

            var googleEvent = ToGoogleEvent(@event);
            await calendarService!.Events.Insert(googleEvent, @event.CalendarId).ExecuteAsync(cancellationToken);
        }

        public async Task DeleteEventAsync(string eventId, string calendarId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            await calendarService!.Events.Delete(calendarId, eventId).ExecuteAsync(cancellationToken);
        }

        public async Task EditEventAsync(EventDay @event, string eventId, string calendarId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();

            var googleEvent = ToGoogleEvent(@event);
            await calendarService!.Events.Update(googleEvent, calendarId, eventId).ExecuteAsync(cancellationToken);
        }

        public IEnumerable<string> GetEventNamesForDay(DateTime selectedDate) =>
            Events.Where(ev => ev.StartDate <= selectedDate.Date && selectedDate.Date <= ev.EndDate)
                  .Select(ev => ev.Description);

        public IEnumerable<EventDay> GetEventsForDay(DateTime selectedDate) =>
            Events.Where(ev => ev.StartDate <= selectedDate.Date && selectedDate.Date < ev.EndDate);

        public async Task SetCalendarSelectedAsync(string calendarSummary, bool isSelected, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();

            var calendarToChange = Calendars.FirstOrDefault(c => c.Summary == calendarSummary);
            if (calendarToChange == null)
            {
                return;
            }

            calendarToChange.Selected = isSelected;
            await calendarService!.CalendarList.Update(calendarToChange, calendarToChange.Id).ExecuteAsync(cancellationToken);
        }

        /// <summary>
        /// EventDay.EndDate follows Google's own convention for all-day events: it is
        /// exclusive (one day past the last day the event actually occurs on).
        /// </summary>
        private static Event ToGoogleEvent(EventDay @event) => new Event
        {
            Start = new EventDateTime { Date = @event.StartDate.ToString("yyyy-MM-dd") },
            End = new EventDateTime { Date = @event.EndDate.ToString("yyyy-MM-dd") },
            Summary = @event.Description,
            Description = @event.Description,
        };

        private async Task<Events> GetEventsFromCalendarAsync(string calendarId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            var request = calendarService!.Events.List(calendarId);
            request.TimeMin = startTime;
            request.TimeMax = endTime;
            request.SingleEvents = true;
            request.MaxResults = 1000;
            return await request.ExecuteAsync(cancellationToken);
        }

        private async Task GetCalendarsFromGoogleAsync(CancellationToken cancellationToken)
        {
            var result = await calendarService!.CalendarList.List().ExecuteAsync(cancellationToken);
            Calendars = result.Items;
        }

        private static async Task<UserCredential?> CreateCredentialsAsync(string credentialsPath, string tokenPath, CancellationToken cancellationToken)
        {
            using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);

            // token.json stores the user's access/refresh tokens; created automatically on first authorization.
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                cancellationToken,
                new FileDataStore(tokenPath, true));
        }

        private CalendarService? GetCalendarService(UserCredential credentials)
        {
            if (credentials == null)
            {
                return null;
            }

            return new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = options.ApplicationName,
            });
        }

        private async Task<IList<EventDay>> GetEventsFromGoogleAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            var events = new List<EventDay>();

            foreach (var cal in Calendars.Where(c => c.Selected == true))
            {
                var eventsInCalendar = (await GetEventsFromCalendarAsync(cal.Id, startTime, endTime, cancellationToken)).Items;

                foreach (var ev in eventsInCalendar)
                {
                    if (ev.Start?.Date == null || ev.End?.Date == null)
                    {
                        continue;
                    }

                    events.Add(new EventDay
                    {
                        Id = ev.Id,
                        Calendar = cal.Summary,
                        CalendarBkColor = cal.BackgroundColor,
                        CalendarId = cal.Id,
                        Description = ev.Summary ?? string.Empty,
                        StartDate = DateTime.Parse(ev.Start.Date),
                        EndDate = DateTime.Parse(ev.End.Date),
                    });
                }
            }

            return events;
        }

        private void EnsureAuthenticated()
        {
            if (calendarService == null)
            {
                throw new InvalidOperationException($"{nameof(AuthenticateAsync)} must be called before using the importer.");
            }
        }
    }
}

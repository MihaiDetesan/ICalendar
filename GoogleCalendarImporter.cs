using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using ICalendar.Model;

namespace ICalendar
{
    public class GoogleCalendarImporter
    {
        private string[] Scopes = {
        CalendarService.Scope.CalendarEvents,
        CalendarService.Scope.CalendarReadonly
        };

        private readonly GoogleOptions options;

        private CalendarService calendarService;

        public GoogleCalendarImporter(GoogleOptions options)
        {
            this.options = options;
            Events = new List<EventDay>();
            Calendars = new List<CalendarListEntry>();

            var credentials = CreateCredentials(options.CredentialsPath, options.TokenPath);

            if (credentials == null)
            {
                throw new UnauthorizedAccessException("Could not authenticate to Google Calendar");
            }

            calendarService = GetCalendarService(credentials);
        }

        public IList<EventDay> Events { get; private set; }
        public IList<CalendarListEntry> Calendars { get; private set; }


        public void RefreshEvents(int year)
        {
            RefreshCalendars(calendarService, year);
            Events = GetEventsFromGoogle(calendarService, year);
        }

        public void AddEvent(EventDay @event)
        {
            var googleEvent = new Event()
            {
                Start = new EventDateTime
                {
                    Date = @event.StartDate.ToString("yyyy-MM-dd")
                },
                End = new EventDateTime
                {
                    Date = @event.EndDate.ToString("yyyy-MM-dd")
                },
                Summary = @event.Description,
                Description = @event.Description,
            };

            calendarService.Events.Insert(googleEvent, @event.CalendarId).Execute();
        }

        public void DeleteEvent(string eventId, string calendarId)
        {
            calendarService.Events.Delete(calendarId, eventId).Execute();
        }

        public IEnumerable<string> GetEventNamesForDay(DateTime selectedDate) =>
            Events.Where(ev =>
            {
                return (ev.StartDate <= selectedDate.Date && selectedDate.Date <= ev.EndDate) ? true : false;
            }).Select(ev => ev.Description);

        public IEnumerable<EventDay> GetEventsForDay(DateTime selectedDate) =>
        Events.Where(ev =>
        {
            return (ev.StartDate <= selectedDate.Date && selectedDate.Date < ev.EndDate) ? true : false;
        });


        public void RefreshCalendars(CalendarService calendarService, int year)
        {
            Calendars = calendarService.CalendarList.List().Execute().Items;
        }

        internal UserCredential CreateCredentials(string credentialsPath, string tokenPath)
        {
            UserCredential credentials = null;
            using (var stream =
            new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true)).Result;
            }

            return credentials;
        }
        internal CalendarService GetCalendarService(UserCredential credentials)
        {
            if (credentials == null)
                return null;

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = options.ApplicationName,
            });

            return service;
        }

        /// <summary>
        /// Get all events from Google from all calendars.
        /// </summary>
        /// 
        internal IList<EventDay> GetEventsFromGoogle(CalendarService calendarService, int year)
        {
            var events = new List<EventDay>();

            foreach (var cal in Calendars)
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31);

                var eventsInCalendar = GetEventsFromCalendar(cal.Id, calendarService, startDate, endDate).Items;

                foreach (var ev in eventsInCalendar)
                {
                    events.Add(new EventDay()
                    {
                        Id = ev.Id,
                        Calendar = cal.Summary,
                        CalendarBkColor = cal.BackgroundColor,
                        CalendarId= cal.Id,
                        Description = ev.Summary,
                        StartDate = DateTime.Parse(ev.Start.Date),
                        EndDate = DateTime.Parse(ev.End.Date),                        
                    });
                }
            }

            return events;
        }

        internal static Events GetEventsFromCalendar(string calendarName, CalendarService service, DateTime startTime, DateTime endTime)
        {
            EventsResource.ListRequest request;

            request = service.Events.List(calendarName);
            request.TimeMin = startTime;
            request.TimeMax = endTime;
            request.SingleEvents = true;
            request.MaxResults = 1000;
            return request.Execute();
        }
    }
}


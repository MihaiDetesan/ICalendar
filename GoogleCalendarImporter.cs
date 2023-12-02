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
        CalendarService.Scope.CalendarReadonly,
        CalendarService.Scope.Calendar
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
        public Dictionary<int, int> CommittedHolidayDaysPerYear = new Dictionary<int, int>();

        public void RefreshEvents(DateTime startTime, DateTime endTime)
        {
            CommittedHolidayDaysPerYear.Clear();
            Calendars.Clear();
            Events.Clear();

            GetCalendarsFromGoogle(calendarService);
            Events = GetEventsFromGoogle(calendarService, startTime, endTime);
        }

        /// <summary>
        /// Add event to calndar.
        /// </summary>
        /// <param name="event"></param>
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
                    Date = @event.EndDate.AddMinutes(1).ToString("yyyy-MM-dd")
                },
                Summary = @event.Description,
                Description = @event.Description,
            };

            calendarService.Events.Insert(googleEvent, @event.CalendarId).Execute();
        }

        /// <summary>
        /// Delete event from calendar.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="calendarId"></param>
        public void DeleteEvent(string eventId, string calendarId)
        {
            calendarService.Events.Delete(calendarId, eventId).Execute();
        }

        /// <summary>
        /// Update event in calendar.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="eventId"></param>
        /// <param name="calendarId"></param>
        public void EditEvent(EventDay @event, string eventId, string calendarId)
        {
            var googleEvent = new Event()
            {
                Start = new EventDateTime
                {
                    Date = @event.StartDate.ToString("yyyy-MM-dd")
                },
                End = new EventDateTime
                {
                    Date = @event.EndDate.AddMinutes(1).ToString("yyyy-MM-dd")
                },
                Summary = @event.Description,
                Description = @event.Description,
            };

            calendarService.Events.Update(googleEvent, calendarId, eventId).Execute();
        }

        /// <summary>
        /// Get event names for a single day.
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        public IEnumerable<string> GetEventNamesForDay(DateTime selectedDate) =>
            Events.Where(ev =>
            {
                return (ev.StartDate <= selectedDate.Date && selectedDate.Date <= ev.EndDate) ? true : false;
            }).Select(ev => ev.Description);

        /// <summary>
        /// Get events for a day.
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        public IEnumerable<EventDay> GetEventsForDay(DateTime selectedDate) =>
        Events.Where(ev =>
        {
            return (ev.StartDate <= selectedDate.Date && selectedDate.Date < ev.EndDate) ? true : false;
        });

        /// <summary>
        /// Get events from Google within a start adn end date.
        /// </summary>
        /// <param name="calendarName"></param>
        /// <param name="service"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static Events GetEventsFromCalendar(string calendarName, CalendarService service, DateTime startTime, DateTime endTime)    
        {
            EventsResource.ListRequest request;

            request = service.Events.List(calendarName);
            request.TimeMin = startTime;
            request.TimeMax = endTime;
            request.SingleEvents = true;
            request.MaxResults = 1000;
            return request.Execute();
        }

        /// <summary>
        /// Gets calendars from Google.
        /// </summary>
        /// <param name="calendarService"></param>
        private void GetCalendarsFromGoogle(CalendarService calendarService)
        {
            Calendars = calendarService.CalendarList.List().Execute().Items;
        }

        /// <summary>
        /// Create credentials for goolge access.
        /// </summary>
        /// <param name="credentialsPath"></param>
        /// <param name="tokenPath"></param>
        /// <returns></returns>
        private UserCredential CreateCredentials(string credentialsPath, string tokenPath)
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

        /// <summary>
        /// Gets the calendar service.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        private CalendarService GetCalendarService(UserCredential credentials)
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
        private IList<EventDay> GetEventsFromGoogle(CalendarService calendarService, DateTime startTime, DateTime endTime)
        {
            var events = new List<EventDay>();

            foreach (var cal in Calendars.Where(x=>x.Selected==true))
            {
                var eventsInCalendar = GetEventsFromCalendar(cal.Id, calendarService, startTime, endTime).Items;

                foreach (var ev in eventsInCalendar)
                {
                    try
                    {
                        var eventDay = new EventDay()
                        {
                            Id = ev.Id,
                            Calendar = cal.Summary,
                            CalendarBkColor = cal.BackgroundColor,
                            CalendarId = cal.Id,
                            Description = ev.Summary,
                            StartDate = DateTime.Parse(ev.Start.Date),
                            EndDate = DateTime.Parse(ev.End.Date),
                        };

                        events.Add(eventDay);                        
                        
                        //if (vacationWords.Any(s=> eventDay.Description.IndexOf(s,StringComparison.InvariantCultureIgnoreCase) !=-1))
                        //{
                        //    if (CommittedHolidayDaysPerYear.ContainsKey(eventDay.StartDate.Year))
                        //    {
                        //        CommittedHolidayDaysPerYear[eventDay.StartDate.Year]+=(eventDay.EndDate - eventDay.StartDate).Days;
                        //    }
                        //    else
                        //    {
                        //        CommittedHolidayDaysPerYear[eventDay.StartDate.Year] = (eventDay.EndDate - eventDay.StartDate).Days;
                        //    }
                        //}
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Id:{ev.Id}");
                        Console.WriteLine($"StartDate:{ev.Start.Date}");
                        Console.WriteLine($"EndDate:{ev.End.Date}");
                        Console.WriteLine($"Description:{ev.Summary}");
                    }
                }
            }

            return events;
        }

        internal void ChageSelectStatus(string text, bool isChecked)
        {
            var calendarToChange = calendarService.CalendarList.List().Execute().Items.First(x => x.Summary.Equals(text));

            if (calendarToChange != null) 
            {

                calendarToChange.Selected = isChecked;
                calendarService.CalendarList.Update(calendarToChange, calendarToChange.Id).Execute();
            }
        }
    }
}


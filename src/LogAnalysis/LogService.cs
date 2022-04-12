using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace LTExercise.LogAnalysis
{
    public class LogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository repository)
        {
            _logRepository = repository;
        }
        public List<SharedSessionResponse> GetMaxSharedUserSessions(int from, int to)
        {
           var filteredLogs = _logRepository.GetLogs().Where(x => x.Time > from && x.Time < to);
           return GetMaxSharedUserSessions(filteredLogs.ToList());
        }
        public List<SharedSessionResponse> GetMaxSharedUserSessions(IEnumerable<LogRecord> list)
        {
            //sort list of users this.
            var sorted = list.OrderBy(l => l.Time);
            var loggedIn = new List<Session>();
            var groups = new Dictionary<string, List<List<Session>>>();
            var maxUsers = 0;
            var maxKey = "";
            foreach (var record in sorted)
            {
                //add login users
                if (record.EventType == "login")
                {
                    loggedIn.Add(new Session(record));
                }

                //logout user if they are logged in
                if (record.EventType == "logout" && loggedIn.Any(x => x.Name == record.Name && x.LogOutTime == 0))
                {
                    var index = loggedIn.FindIndex(x => x.Name == record.Name && x.LogOutTime == 0);
                    loggedIn[index].LogOutTime = record.Time;
                }
                // only loop over completed sessions
                var currentlyLoggedIn = loggedIn.Where(x=>x.LogOutTime==0);
                if (currentlyLoggedIn.Count() > 1)
                {
                    //find subsets e.g. 5 users logged in, get combo of all remaining sessions. 
                    var subsets = Utilities.GetSubsets(currentlyLoggedIn).Where(x => x.Count() > 1);
                    //store grouped sessions as dictionary
                    foreach (var set in subsets)
                    {
                        var key = String.Join(",", set.Select(x => x.Name).OrderBy(x => x).ToList());
                        if (groups.ContainsKey(key) && !groups[key].Any(x =>x.SequenceEqual(set)))
                        {
                            groups[key].Add(set);
                        }
                        else if(!groups.ContainsKey(key))
                        {
                            var logged = new List<List<Session>>();
                            logged.Add(set);
                            groups.Add(key, logged);
                        }
                    }
                }
                //at this point we have groups of sessions by key(names) and list of sessions. 
                foreach (var value in groups)
                {
                    if (value.Key.Split(",").Length > maxUsers && value.Value.Count() > 1)
                    {
                        maxUsers = value.Key.Split(",").Length;
                        maxKey = value.Key;
                    }
                }
            }
            //here we can just get the latest login time and earliest logout time which will show the overlap. 
            var times = new List<SharedSessionResponse>();
            foreach (var loggedSession in groups[maxKey])
            {
                var loginTime = loggedSession.Max(t => t.Time);
                var logOutTime = loggedSession.Min(t => t.LogOutTime);
                var record = new SharedSessionResponse(maxKey, loginTime, logOutTime);
                times.Add(record);
            }

            return times;
        }
    }

    public record LogRecord(string Name, int Time, string EventType);
    public record Session : LogRecord
    {
        public Session(LogRecord parent) : base(parent)
        {
        }
        public int LogOutTime { get; set; }
    }
    public record SharedSessionResponse(string Names, int StartTime, int EndTime);
}
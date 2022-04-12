using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace console
{
    public static class LogAnalysis
    {
        public static List<LogRecord> GetLogs()
        {
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Mode = CsvMode.NoEscape,
                TrimOptions = TrimOptions.InsideQuotes,
                MissingFieldFound = null
            };
            using (var reader = new StreamReader("test.csv"))
            using (var csv = new CsvReader(reader, cfg))
            {
                return csv.GetRecords<LogRecord>().ToList();
            }
        }


        public static void AnalyseLogs(List<LogRecord> list)
        {
            var sorted = list.OrderBy(l => l.time);
            var loggedIn = new List<Session>();
            var groups = new Dictionary<string, List<List<Session>>>();
            var maxUsers = 0;
            var maxKey = "";
            foreach (var record in sorted)
            {
                //add login users
                if (record.eventType == "login")
                {
                    loggedIn.Add(new Session(record));
                }

                //logout user if they are logged in
                if (record.eventType == "logout" && loggedIn.Any(x => x.name == record.name && x.LogOutTime == 0))
                {
                    var index = loggedIn.FindIndex(x => x.name == record.name && x.LogOutTime == 0);
                    loggedIn[index].LogOutTime = record.time;
                }

                var currentlyLoggedIn = loggedIn.Where(x=>x.LogOutTime==0);
                if (currentlyLoggedIn.Count() > 1)
                {
                    var subsets = Utilities.GetSubsets(currentlyLoggedIn).Where(x => x.Count() > 1);
                    foreach (var set in subsets)
                    {
                        var key = String.Join(",", set.Select(x => x.name).OrderBy(x => x).ToList());
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
                    //Console.WriteLine($"count of dict{groups.Count()}");
                    //var filteredDict = groups.Keys.Max(x => x.Split(",").Length);
                }

                foreach (var value in groups)
                {
                    if (value.Key.Split(",").Length > maxUsers && value.Value.Count() > 1)
                    {
                        maxUsers = value.Key.Split(",").Length;
                        maxKey = value.Key;
                    }
                }
            }

            Console.WriteLine($"max:{maxUsers}");
            Console.WriteLine($"max:{maxKey}");
            Console.WriteLine($"keys{maxUsers} count: {groups[maxKey].Count()}");
            var times = new List<(int, int)>();
            foreach (var loggedSession in groups[maxKey])
            {
                var loginTime = loggedSession.Max(t => t.time);
                var logOutTime = loggedSession.Min(t => t.LogOutTime);
                times.Add((loginTime, logOutTime));
            }

            foreach (var time in times)
            {
                Console.WriteLine($"login: {time.Item1}, logout: {time.Item2}");
            }
        }
    }

    public record LogRecord(string name, int time, string eventType);

    public record Session : LogRecord
    {
        public Session(LogRecord parent) : base(parent)
        {
        }
        public int LogOutTime { get; set; }
    }
}
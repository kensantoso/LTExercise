using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace LTExercise.LogAnalysis;

public class LogRepository :ILogRepository
{
    public List<LogRecord> GetLogs()
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
}

public interface ILogRepository
{
    public List<LogRecord> GetLogs();
}
using System.Collections.Immutable;
using console;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;
using CsvHelper.Expressions;

// See https://aka.ms/new-console-template for more information
var list = LogAnalysis.GetLogs();
LogAnalysis.AnalyseLogs(list);
//sort list by time

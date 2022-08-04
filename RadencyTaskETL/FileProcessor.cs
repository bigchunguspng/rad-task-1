using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RadencyTaskETL.OutputModel;

namespace RadencyTaskETL
{
    public class FileProcessor
    {
        private readonly string _path;
        private static readonly string PathDone = ConfigurationManager.AppSettings["path_a_done"];

        private static Logger _logger = new Logger();
        private static DateTime _today = DateTime.Today;

        public FileProcessor(string path)
        {
            _path = path;
        }

        public void Run()
        {
            using var reader = File.OpenText(_path);
            var lines = reader.ReadToEnd().Split('\n');
            var data = lines.Skip(_path.EndsWith(".csv") ? 1 : 0).Select(ProcessLine).Where(x => x != null);
            var cities = Transform(data);
            var saver = new FileOutput<List<CityData>>(GetOutputPath());
            
            saver.SaveData(cities);
            _logger.ParsedFiles++;
        }

        public void MoveFile()
        {
            var directory = $@"{PathDone}\{_today:yyyy-MM-dd}";
            Directory.CreateDirectory(directory);
            var file = new FileInfo(_path);
            file.MoveTo($@"{directory}\{file.Name}");
        }

        private InputData ProcessLine(string line)
        {
            try
            {
                var kek = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(x => x.Trim()).ToArray();
                var obj = new InputData
                {
                    FirstName = kek[0],
                    LastName = kek[1],
                    City = kek[2].Trim('\"').Split(',')[0],
                    Payment = decimal.Parse(kek[3], CultureInfo.InvariantCulture),
                    Date = DateOnly.ParseExact(kek[4], "yyyy-dd-MM"),
                    AccountNumber = long.Parse(kek[5]),
                    Service = kek[6]
                };
                if (!obj.IsValid()) throw new ValidationException();
                _logger.ParsedLines++;
                return obj;
            }
            catch
            {
                _logger.FoundErrors++;
                _logger.InvalidFiles.Add(_path);
                _logger.InvalidLines.Add(line.Trim('\r'));
                return null;
            }
        }
        
        private List<CityData> Transform(IEnumerable<InputData> data)
        {
            var xd = data.Select(x => (
                city: x.City,
                service: x.Service,
                payer: new PayerData()
                {
                    Name = x.GetFullName(),
                    Payment = x.Payment,
                    Date = x.Date,
                    AccountNumber = x.AccountNumber
                })).ToArray();
            
            var result = xd.GroupBy(x => x.city).Select(x => new CityData(){City = x.Key}).ToList();
            foreach (var city in result)
            {
                city.Services = xd.Where(x => x.city == city.City).GroupBy(x => x.service).Select(x => new ServiceData(){Name = x.Key}).ToList();
                foreach (var service in city.Services)
                {
                    service.Payers = xd.Where(x => x.city == city.City && x.service == service.Name).Select(x => x.payer).ToList();
                    service.Total = service.GetTotal();
                }
                city.Total = city.GetTotal();
            }
            
            return result;
        }

        private string GetOutputPath()
        {
            var path = $@"{GetWorkingDirectory()}";
            Directory.CreateDirectory(path);

            return $@"{path}\output{(_logger.ParsedFiles > 0 ? _logger.ParsedFiles.ToString() : "")}.json";
        }

        private static string GetMetalogPath() => $@"{GetWorkingDirectory()}\meta.log";
        private static string GetWorkingDirectory() => $@"{ConfigurationManager.AppSettings["path_b"]!}\{_today:yyyy-MM-dd}";

        public static void RenderMetaLog()
        {
            _logger.LogData(GetMetalogPath());
            
            _today = DateTime.Today;
        }

        public static void LoadMetaData()
        {
            string path = GetMetalogPath();
            _logger = File.Exists(path) ? new Logger(GetMetalogPath()) : new Logger();
        }
        
    }
}
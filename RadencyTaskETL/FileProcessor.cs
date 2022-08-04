using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RadencyTaskETL
{
    public class FileProcessor
    {
        private readonly string _path;

        public static int ParsedFiles, ParsedLines, FoundErrors;
        public static HashSet<string> InvalidFiles = new HashSet<string>();

        public FileProcessor(string path)
        {
            _path = path;
        }

        public void Run()
        {
            var lines = File.ReadAllLines(_path);
            var data = lines.Select(ProcessLine).Where(x => x != null);
            var cities = Transform(data);
            var saver = new FileOutput<List<CityData>>(GetOutputPath());
            
            saver.SaveData(cities);
            ParsedFiles++;
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
                ParsedLines++;
                return obj;
            }
            catch
            {
                FoundErrors++;
                InvalidFiles.Add(_path);
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
            var path = $@"{ConfigurationManager.AppSettings["path_b"]!}\{DateTime.Today:yyyy-MM-dd}";
            Directory.CreateDirectory(path);

            return $@"{path}\output{(ParsedFiles > 0 ? ParsedFiles.ToString() : "")}.json";
        }
    }
}
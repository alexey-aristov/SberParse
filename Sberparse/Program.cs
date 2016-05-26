using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sberparse
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = new DateTime(2015, 04, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var dir = @"D:\SberbankData";
            var cookieContainer = new CookieContainer();
            string cookies =
                "RAW COOKIE HERE";
            var ss = cookies.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(a =>
               {
                   var cook = a.Split('=');
                   return new Cookie(cook[0], a.Substring(cook[0].Length + 1), "/", "front.node1.online.sberbank.ru");
               }).ToList();
            foreach (var cookie in ss)
            {
                cookie.Expires = DateTime.MaxValue;
                cookieContainer.Add(cookie);
            }
            while (start < DateTime.Now)
            {
                Console.WriteLine("start: {0}, end {1}", start.ToString("dd/MM/yy"), end.ToString("dd/MM/yy"));
                var handler = new HttpClientHandler()
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true,

                };



                HttpClient r = new HttpClient(handler)
                {

                };
                r.DefaultRequestHeaders.Clear();
                r.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36");
                r.DefaultRequestHeaders.Add("RSA-Token", "RSA TOKEN HERE");
                r.DefaultRequestHeaders.Add("Referer", "https://front.node1.online.sberbank.ru/sbtsbol/private/alf?freshPage=true");
                r.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                r.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
                r.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                r.DefaultRequestHeaders.Add("X-Compress", "null");
                r.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                r.DefaultRequestHeaders.Add("Connection", "keep-alive");



                var result1 = r.GetAsync(
                    $"https://front.node1.online.sberbank.ru/sbtsbol/api/alf/operations/history?paginationSize=20&paginationOffset=0&categoryId=209&showOtherAccounts=false&from={start.Date.Day.ToString("00")}%2F{start.Date.Month.ToString("00")}%2F{start.Date.Year}&to={end.Date.Day.ToString("00")}%2F{end.Date.Month.ToString("00")}%2F{end.Date.Year}&showCash=true&showCashPayments=true"
                    ).Result;

                var result = result1.Content.ReadAsStringAsync().Result;
                File.WriteAllText(Path.Combine(dir, $"{start.Month}.{start.Year}.json"), result);

                start = end.AddDays(1);
                end = start.AddMonths(1).AddDays(-1);
            }
        }
    }
}

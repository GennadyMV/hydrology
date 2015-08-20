using HydrologyBorshchForecastCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HydrologyBorshchForecastConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {

                    Console.WriteLine("Start Parser");
                    DateTime currDate = DateTime.Now;
                    Send(currDate);
                    Request(currDate.AddDays(-5));
                    Request(currDate.AddDays(-4));
                    Request(currDate.AddDays(-3));
                    Request(currDate.AddDays(-2));
                    Request(currDate.AddDays(-1));
                    Request(currDate);
                    Request(currDate.AddDays(1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(1000 * 60 * 60);
                break;
            }
        }

        static void Send(DateTime dateCurr)
        {
            MailMessage mail = new MailMessage();
            string host = "192.168.72.65";
            int port = 25;
            string msg_to = "gerasimov@dvugms.khv.ru";
            string msg_from = "ГИС Портал ФГБУ Дальневосточное УГМС <dvmeteo@dvugms.khv.ru>";
            SmtpClient SmtpServer = new SmtpClient(host);
            mail.From = new MailAddress(msg_from);
            mail.To.Add(msg_to);
            mail.To.Add("voda@dvugms.khv.ru");
            mail.Subject = "Гидропрогноз ФГБУ Гидрометцентр России от " + dateCurr.ToString("dd.MM.yyyy");
            mail.IsBodyHtml = true;
            mail.Body = "<h2>Прогноз уровня рек и притока воды к Зейскому водохранилищу от " + dateCurr.ToString("dd.MM.yyyy") + "</h2>";
            mail.Body += "<p>";
            mail.Body += "Прогноз доступен по ссылке: <a href=\"http://10.8.3.180/hydrology/forecast/index?YYYY="+dateCurr.ToString("yyyy")+"&MM="+dateCurr.ToString("MM")+"&DD="+dateCurr.ToString("dd")+"\">открыть </a>";
            mail.Body += "</p>";
            mail.Body += "<p>";
            mail.Body += "Данное письмо отправлено автоматически при обновлении прогноза. Уточнения по тел. 11-20 РВЦ ФГБУ Дальневосточное УГМС";
            mail.Body += "</p>";
            SmtpServer.Port = port;


            SmtpServer.EnableSsl = false;

            SmtpServer.Send(mail);



        }
        static void Request(DateTime dateCurr)
        {
            ILogger logger = new LoggerConsole();

            IBot theBotRiver = new BotRiver(logger);
            theBotRiver.Parser(dateCurr);
            Thread.Sleep(1000 * 60);

            IBot theBotReservoir = new BotReservoir(logger);
            theBotReservoir.Parser(dateCurr);
            Thread.Sleep(1000 * 60);

            if (theBotRiver.isNew() == true)
            {
                Send(dateCurr);
            }
        }
    }
}

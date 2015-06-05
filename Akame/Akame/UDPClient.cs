using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NATUPNPLib;

namespace Akame
{
    class UDPClient
    {

        static private IPEndPoint ep;
        static private UdpClient client;
        private string IP = "217.121.7.227";
        private int port = 5151;

        public UDPClient(String IP)
        {
            this.IP = IP;
            Thread client = new Thread(new ThreadStart(udpClient));
            client.Start();
            
        }

        private void udpClient()
        {
            Console.WriteLine("Local IP: " + GetLocalIPv4());

            UPnPNAT upnpnat = new UPnPNAT();
            IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            mappings.Add(5151, "UDP", 5151, GetLocalIPv4(), true, "test");

            byte[] packetData = System.Text.UnicodeEncoding.ASCII.GetBytes("\r\n UDP CLIENT SPEAKING HERE :)");
            ep = new IPEndPoint(IPAddress.Parse(IP), port);

            client = new UdpClient();
            client.Connect(ep);

            UDPListener();

            // then receive data

        }

        public void sendMessage(String send)
        {

            byte[] packetData = System.Text.UnicodeEncoding.ASCII.GetBytes(send);
            client.Send(packetData, packetData.Length);

        }

        private static void UDPListener()
        {
                    while (true)
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var receivedResults = client.Receive(ref ep);
                        String serverResponse = Encoding.ASCII.GetString(receivedResults);
                        Console.WriteLine("serverResponse");
                    }
        }

        private string byteToString(byte[] input, int recv)
        {
            return Encoding.ASCII.GetString(input, 0, recv);
        }

        public string GetLocalIPv4()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
       
    }
}

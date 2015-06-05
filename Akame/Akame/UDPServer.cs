using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.PeerToPeer;
using NATUPNPLib;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;

namespace Akame
{
    class UDPServer
    {

        private byte[] response;

        public UDPServer()
        {

            Console.WriteLine("local ip: " + GetLocalIPv4());
            UPnPNAT upnpnat = new UPnPNAT();
            IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
            mappings.Add(5151, "UDP", 5151, GetLocalIPv4(), true, "udp-test");

            Thread server = new Thread(new ThreadStart(udpServer));
            server.Start();
        }

        private void udpServer()
        {
            int recv;
            byte[] data = new byte[1024];

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 5151);
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            newSocket.Bind(endpoint);

            Console.WriteLine("Waiting for a client");

            while (true)
            {
                IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
                EndPoint tmpRemote = (EndPoint)client;

                recv = newSocket.ReceiveFrom(data, ref tmpRemote);

                Console.WriteLine("Message received from {0}", tmpRemote.ToString());
                Console.WriteLine(byteToString(data, recv));

                string welcome = "HELLO, YOU CONNECTED SUCCESFULLY";

                response = Encoding.ASCII.GetBytes(welcome);
                if (newSocket.Connected)
                {
                    newSocket.Send(response);
                }

                while (true)
                {
                    if (!newSocket.Connected)
                    {
                        break;
                    }
                    else
                    {
                        data = new byte[1024];
                        if ((recv = newSocket.ReceiveFrom(data, ref tmpRemote)) != 0)
                        {
                            Console.WriteLine(byteToString(data, recv));
                            string responseMessage = "Message received by server! Message received: " + byteToString(data, recv);

                            response = Encoding.ASCII.GetBytes(responseMessage);
                            newSocket.SendTo(response, client); 
                        }
                    }
                }

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

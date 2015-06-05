using System;
using System.Net;

namespace Akame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IP FOR OUTSIDE CONNECTION(Share only with known people):");
            Console.WriteLine(new WebClient().DownloadString("http://icanhazip.com"));

            while (true)
            {
                setup();
            }
            
            Console.ReadLine();
        }

        static void setup()
        {
            
                    Console.WriteLine("IP to connect to: ");
                    String ip = Console.ReadLine();
                    if (ip != "")
                    {
                        try
                        {
                            Console.WriteLine("STARTING CLIENT AND SERVER, CONNECTING TO " + ip);
                            UDPServer server = new UDPServer();
                            UDPClient client = new UDPClient(ip);
                            while (true)
                            {
                                Console.WriteLine("SEND MESSAGE: ");
                                string message = Console.ReadLine();
                                if (message != "")
                                {
                                    client.sendMessage(message);
                                    message = "";
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Input was not a valid IP, example ip: 127.0.0.1");
                            Console.WriteLine("Other possible problems: " + e.ToString());
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("Input was not a valid IP, example ip: 127.0.0.1");
                    }
                
        }
    }
}

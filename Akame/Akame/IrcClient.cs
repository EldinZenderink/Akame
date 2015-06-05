﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Akame
{
    class IrcClient
    {
        //default construct
        public IrcClient()
        {
            newIP = "";
            newPort = 0;
            newUsername = "";
            newPassword = "";
            newChannel = "";
        }

        //overload construct
        public IrcClient(string IP, int Port, string Username, string Password, string Channel, string presetMessage, bool autoQuit)
        {
            newIP = IP;
            newPort = Port;
            newUsername = Username;
            newPassword = Password;
            newChannel = Channel;
            newPresetMessage = presetMessage;
            newAutoQuit = autoQuit;
            dccReady = false;
        }

        //Accessor Functions
        public string getIP()
        {
            return newIP;
        }

        public int getPort()
        {
            return newPort;
        }

        public string getUsername()
        {
            return newUsername;
        }

        public string getPassword()
        {
            return newPassword;
        }

        public string getChannel()
        {
            return newChannel;
        }

        //Mutator Functions
        public void setIP(string IP)
        {
            newIP = IP;
        }

        public void setPort(int Port)
        {
            newPort = Port;
        }

        public void setUsername(string Username)
        {
            newUsername = Username;
        }

        public void setPassword(string Password)
        {
            newPassword = Password;
        }

        public void setChannel(string Channel)
        {
            newChannel = Channel;
        }

        //program function

        //its the heart, here it outputs the server responses, here you can type to the server, here is where it responses to the server etc
        public void IrcClientRun()
        {
            IrcConnect con = new IrcConnect(newIP, newPort, newUsername, newPassword, newChannel);
            conCheck = con.Connect();

            Console.WriteLine("IP: " + con.getIP());
            Console.WriteLine("Port: " + con.getPort());
            Console.WriteLine("Username: " + con.getUsername());
            Console.WriteLine("Preset message: " + newPresetMessage);
            bool send = false;
            if (conCheck)
            {
                Console.WriteLine("succesful connected to the irc server!");
                string ircData = "";
                var worker = new Thread(() =>
                {
                    while ((ircData = IrcConnect.reader.ReadLine()) != null)
                    {
                        if (ircData.Contains("PONG"))
                        {
                            IrcSend.sendInput("PING :" + newIP);
                        }
                        else
                        {
                            Console.WriteLine(ircData);
                        }

                        if (ircData.Contains("PING"))
                        {
                            IrcSend.sendInput("PONG :" + newIP);
                        }

                        if (ircData.Contains("366") && !send)
                        {
                            IrcSend.sendMsg(newPresetMessage);
                            send = true;
                        }
                        if (ircData.Contains("366"))
                        {
                            dccReady = true;
                        }

                        if (ircData.Contains("DCC SEND") && ircData.Contains(newUsername) && !ircData.Contains("DENIED"))
                        {
                            DCCClient dcc = new DCCClient(ircData);
                            dcc.getDccParts();
                            dcc.setDccValues();

                            Console.WriteLine("FILENAME: " + dcc.getFileName());
                            Console.WriteLine("FILESIZE: " + dcc.getFileSize());
                            Console.WriteLine("IP: " + dcc.getIp());
                            Console.WriteLine("PORT: " + dcc.getPortNum());

                            dcc.Downloader();
                        }

                       
                    }
                });

                worker.Start();


                var dccReader = new Thread(() =>
                {
                    String oldLine = "";
                    while (true)
                    {
                        if (File.Exists("dcc.txt"))
                        {
                            string text = System.IO.File.ReadAllText("dcc.txt");
                            if (text != oldLine)
                            {
                                if (dccReady)
                                {
                                    IrcSend.sendMsg(text);
                                }
                               
                            }
                            if (!dccReady)
                            {
                                oldLine = "";
                            }
                            else
                            {
                                oldLine = text;
                            }
                            
                        }
                        Thread.Sleep(500);
                    }
                   
                    
                });

                dccReader.Start();

                do
                {
                    string Input = Console.ReadLine();
                    if (Input != null || Input != "" || Input != String.Empty)
                    {
                        IrcSend.sendMsg(Input);
                    }
                } while (true);              
            }
            else
            {
                Console.WriteLine("Something went wrong, try again!");
            }
        }

        //Member vars
        private string newIP;
        private int newPort;
        private string newUsername;
        private string newPassword;
        private string newChannel;
        private string newPresetMessage;
        private bool newAutoQuit;
        private bool conCheck;
        private bool dccReady;
    }
}

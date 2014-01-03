using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using SecretLabs.NETMF.Hardware.Netduino;

namespace GarageDoor
{
    public class WebServer : IDisposable
    {
        private Socket socket = null;
        //open connection to onbaord led so we can blink it with every request
        GarageSensor sensor;
        GarageActivator activator;

        public WebServer()
        {
            sensor = new GarageSensor();
            activator = new GarageActivator();
            //Initialize Socket class
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Request and bind to an IP from DHCP server
            socket.Bind(new IPEndPoint(IPAddress.Any, 80));
            //Debug print our IP address
            Debug.Print(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);
            //Start listen for web requests
            socket.Listen(10);
            ListenForRequest();
        }

        public void ListenForRequest()
        {
            while (true)
            {
                using (Socket clientSocket = socket.Accept())
                {
                    //Get clients IP
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    EndPoint clientEndPoint = clientSocket.RemoteEndPoint;
                    //int byteCount = cSocket.Available;
                    int bytesReceived = clientSocket.Available;
                    if (bytesReceived > 0)
                    {
                        //Get request
                        byte[] buffer = new byte[bytesReceived];
                        int byteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);
                        string request = new string(Encoding.UTF8.GetChars(buffer));
                        string firstLine = request.Substring(0, request.IndexOf('\n')); //Example "GET /activatedoor HTTP/1.1"
                        string[] words = firstLine.Split(' ');  //Split line into words
                        string command = string.Empty;
                        if (words.Length > 2)
                        {
                            string method = words[0]; //First word should be GET
                            command = words[1].TrimStart('/'); //Second word is our command - remove the forward slash
                        }
                        string response = string.Empty;
                        string header = string.Empty;
                        switch (command.ToLower())
                        {
                            case "activatedoor":
                                activator.Activate();
                                //Compose a response
                                response = "I just activated the garage!";
                                header = "HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
                                break;
                            case "status":
                            case "":
                                response = sensor.Status == GarageDoorStatus.Open ? "0" : "1";
                                header = "HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
                                break;
                            default:
                                //Did not recognize command
                                response = "Bad command " + request;
                                header = "HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
                                break;
                        }
                        clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                        clientSocket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
                        
                    }
                }
            }
        }
        
        #region IDisposable Members
        ~WebServer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (socket != null)
                socket.Close();
            sensor.Dispose();
            activator.Dispose();
        }
        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp.Net;

namespace Server
{
    class Server : WebSocketBehavior
    {
        static Dictionary<string, Chat> ChatList = new Dictionary<string, Chat>();
        static Dictionary<WebSocket, Client> Users = new Dictionary<WebSocket, Client>();
        static WebSocketServer webSocketServer;
        
        static public void Start(int Port, string Service)
        {
            webSocketServer = new WebSocketServer( Port);
           
            webSocketServer.AddWebSocketService<Server>("/" + Service);
            webSocketServer.Start();
            Console.WriteLine("Started");
        }
        static public void Start(string Service)
        {
            webSocketServer = new WebSocketServer();
            webSocketServer.AddWebSocketService<Server>(Service);
            webSocketServer.Start();
            Console.WriteLine("Started");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                JObject json = JObject.Parse(e.Data);
                if (json.ContainsKey("Type"))
                {
                    if ((string)json["Type"] == "Join" && json.ContainsKey("Chat") && json.ContainsKey("Name"))
                    {
                        string chat = (string)json["Chat"];
                        if (Users.ContainsKey(Context.WebSocket))
                        {
                            Close(Context.WebSocket);
                        }
                        Client C = new Client(Context.WebSocket, (string)json["Name"], chat);
                        Users.Add(Context.WebSocket, C);
                        if (!ChatList.ContainsKey(chat))
                        {
                            ChatList.Add(chat, new Chat());
                            Console.WriteLine("Chat Created -> " + "[" + chat + "]");
                        }
                        ChatList[chat].Add(C);
                        Console.WriteLine("User Joined -> " + C.Name + "[" + chat + "]");
                    }
                    else if ((string)json["Type"] == "Out")
                    {
                        Close(Context.WebSocket);
                    }
                }
                else
                {
                    Client Cl = GetUser(Context.WebSocket);
                    Chat Ch = GetChat(Cl);
                    if (Ch != null)
                    {
                        Ch.Message(Cl, e.Data);
                    }
                }
            }
            catch
            {
                Client Cl = GetUser(Context.WebSocket);
                Chat Ch = GetChat(Cl);
                if (Ch != null)
                {
                    Ch.Message(Cl, e.Data);
                }
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            Close(Context.WebSocket);
        }

        static Client GetUser(WebSocket WebSocket)
        {
            if(WebSocket != null && Users.ContainsKey(WebSocket))
            {
                return Users[WebSocket];
            }
            return null;
        }
        static Chat GetChat(WebSocket WebSocket) => GetChat(GetUser(WebSocket));
        static Chat GetChat(Client Client)
        {
            if(Client != null)
            {
                return GetChat(Client.Chat);
            }    
            return null;
        }
        static Chat GetChat(string chat)
        {
            if(chat != null && ChatList.ContainsKey(chat))
            {
                return ChatList[chat];
            }
            return null;
        }


        static void Close(WebSocket WebSocket)
        {
            Client Cl = GetUser(WebSocket);
            if(Cl != null)
            {
                Users.Remove(WebSocket);
                Chat Ch = GetChat(Cl.Chat);
                if(Ch != null)
                {
                    Ch.Remove(Cl);
                    Console.WriteLine("User Outed -> " + Cl.Name + "[" + Cl.Chat + "]");
                    if (!Ch.isAlive)
                    {
                        ChatList.Remove(Cl.Chat);
                        Console.WriteLine("Chat Removed -> " + "[" + Cl.Chat + "]");
                    }
                }
            }
        }
    }
    class Client
    {
        public string Chat;
        public string Name;
        WebSocket WebSocket;
        public Client(WebSocket WebSocket, string Name, string Chat)
        {
            this.Chat = Chat;
            this.Name = Name;
            this.WebSocket = WebSocket;
        }
        public void Message(string msg)
        {
            WebSocket.Send(msg);
        }
    }
    class Chat
    {
        List<Client> Clients= new List<Client>();
        int MessageId = 0;
        public Chat() { }
        public void Message(Client client, string msg)
        {
            if(Clients.Contains(client))
            {
                int id = ++MessageId;
                string Msg = (new JObject() { { "Type", "Message" }, { "Name", client.Name }, { "Message", msg }, { "Id", id } }).ToString();

                SendMessage(Msg);
            }
        }
        public void Add(Client client)
        {
            string Msg = (new JObject() { { "Type", "Join" }, { "Name", client.Name } }).ToString();
            SendMessage(Msg);

            JObject json = new JObject { { "Type", "List" } };
            JArray a = new JArray();

            for (int i = 0; i < Clients.Count; i++)
            {
                a.Add(Clients[i].Name);
            }
            json.Add("List", a);
            client.Message(json.ToString());

            Clients.Add(client);
        }
        public void Remove(Client client)
        {
            if (Clients.Contains(client))
            {
                Clients.Remove(client);
                string Msg = (new JObject() { { "Type", "Out" }, { "Name", client.Name } }).ToString();
                SendMessage(Msg);
            }  
        }


        private void SendMessage(string msg)
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Message(msg);
            }
        }


        public bool isAlive => Clients.Count > 0;
    }
}

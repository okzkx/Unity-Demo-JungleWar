using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using GameServer.Controller;
using Common;
namespace GameServer.Servers {
    public class Server {
        private IPEndPoint ipEndPoint;
        private Socket socket;
        public List<Client> clientList = new List<Client>();
        public ControllerManager controllerManager;

        public Server() { }
        public Server(string ipStr, int port) {
            controllerManager = new ControllerManager(this);
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }

        public void Start() {
            Console.WriteLine("服务器开启socket。。");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEndPoint);
            socket.Listen(0);

            socket.BeginAccept(AcceptCallBack, null);
        }
        private void AcceptCallBack(IAsyncResult ar) {
            Console.WriteLine("一个客户端连接上服务器。。");
            Socket clientSocket = socket.EndAccept(ar);
            Client client = new Client(clientSocket, this);

            clientList.Add(client);
            socket.BeginAccept(AcceptCallBack, null);
        }
        public void RemoveClient(Client client) {
            lock (clientList) {
                clientList.Remove(client);
                Console.WriteLine("一个客户端移出服务器。。");
            }
        }
        public void SendResponse(Client client, ActionCode actionCode, string data) {
            client.Send(actionCode, data);
        }
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client) {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);
        }


        public void Broadcast(ActionCode actionCode, string data) {
            foreach (var client in clientList) {
                client.Send(actionCode, data);
            }
           
        }
    }
}

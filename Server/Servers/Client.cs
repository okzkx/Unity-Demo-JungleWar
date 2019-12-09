using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Common;
using MySql.Data.MySqlClient;
using GameServer.Tool;
using GameServer.Model;
using GameServer.DAO;
namespace GameServer.Servers {
    public class Client {
        //处理网络连接的属性
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mysqlConn;

        //当前玩家属性
        public Room room;
        private User user;
        private Result result;
        public Player player;

        //当前角色属性
        
        public MySqlConnection MySQLConn {
            get { return mysqlConn; }
        }
        public void SetUserData(User user, Result result) {
            this.user = user;
            this.result = result;
        }
        public string GetUserData() {
            return user.Id + "," + user.Username + "," + result.TotalCount + "," + result.WinCount;
        }
        public int GetUserId() {
            return user.Id;
        }

        public Client(Socket clientSocket, Server server) {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
            Start();
        }
        /// <summary>
        /// 每个Client处理一个连接，
        /// 这个方法接收数据
        /// </summary>
        public void Start() {
            if (clientSocket != null && clientSocket.Connected)
                clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize,
                    SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar) {
            try {
                int count = clientSocket.EndReceive(ar);
                if (count == 0) {
                    Console.WriteLine("客户端断开连接。。");
                    Close();
                }
                msg.ReadMessage(count, OnProcessMessage);
                clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize,
                    SocketFlags.None, ReceiveCallback, null);
            } catch (Exception) {
                Console.WriteLine("接收客户端信息失败，断开其连接。。");
                Close();
            }
        }

        /// <summary>
        /// 接收客户端消息，交给controllerManager处理客户端请求
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        private void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data) {
            Console.WriteLine(requestCode.ToString()+ ";" + actionCode.ToString());
            server.HandleRequest(requestCode, actionCode, data, this);
        }

        private void Close() {
            ConnHelper.CloseConnection(mysqlConn);
            if (clientSocket != null)
                clientSocket.Close();
            if (room != null) {
                room.QuitRoom(this);
            }
            server.RemoveClient(this);
        }

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        public void Send(ActionCode actionCode, string data) {
            try {
                byte[] bytes = Message.PackData(actionCode, data);
                clientSocket.Send(bytes);
            } catch (Exception e) {
                Console.WriteLine("无法发送消息:" + e);
            }
        }
        public bool IsHouseOwner() {
            return room.IsHouseOwner(this);
        }
        public void UpdateResult(bool isVictory) {
            UpdateResultToDB(isVictory);
            UpdateResultToClient();
        }
        private void UpdateResultToDB(bool isVictory) {
            result.TotalCount++;
            if (isVictory) {
                result.WinCount++;
            }
            new ResultDAO().UpdateOrAddResult(mysqlConn, result);
        }
        private void UpdateResultToClient() {
            Send(ActionCode.UpdateResult, string.Format("{0},{1}", result.TotalCount, result.WinCount));
        }
    }
}

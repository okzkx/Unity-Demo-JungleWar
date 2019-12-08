using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
namespace GameServer.Controller {
    public class RoomController : BaseController {
        public Server server;
        public List<Room> roomList = new List<Room>();

        public RoomController(ControllerManager cm):base(RequestCode.Room,cm) {
            this.server = cm.server;
        }

        //处理请求
        public string CreateRoom(string data, Client client) {
            Room room = new Room(this);
            room.AddClient(client);
            roomList.Add(room);
            return ((int)ReturnCode.Success).ToString() + "," + ((int)RoleType.Blue).ToString();
        }

        public string ListRoom(string data, Client client) {
            return GetRoomList();
        }

        public string JoinRoom(string data, Client client) {
            int id = int.Parse(data);
            Room room = GetRoomById(id);
            if (room == null) {
                return ((int)ReturnCode.NotFound).ToString();
            } else if (room.IsWaitingJoin() == false) {
                return ((int)ReturnCode.Fail).ToString();
            } else {
                room.AddClient(client);
                string roomData = room.GetRoomData();//"returncode,roletype-id,username,tc,wc|id,username,tc,wc"
                room.BroadcastMessage( ActionCode.UpdateRoom, client, roomData);
                return ((int)ReturnCode.Success).ToString() + "," + ((int)RoleType.Red).ToString() + "-" + roomData;
            }
        }
        public string QuitRoom(string data, Client client) {
            bool isHouseOwner = client.IsHouseOwner();
            Room room = client.room;
            if (isHouseOwner) {
                room.BroadcastMessage( ActionCode.QuitRoom, client, ((int)ReturnCode.Success).ToString());
                room.Close();
            } else {
                client.room.RemoveClient(client);
                room.BroadcastMessage( ActionCode.UpdateRoom, client, room.GetRoomData());
            }
            return null;
        }

        //管理房间
        public string GetRoomList() {
            StringBuilder sb = new StringBuilder();
            foreach (Room room in roomList) {
                if (room.IsWaitingJoin()) {
                    sb.Append(room.GetHouseOwnerData() + "|");
                }
            }
            if (sb.Length == 0) {
                sb.Append("0");
            } else {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public Room GetRoomById(int id) {
            foreach (Room room in roomList) {
                if (room.GetId() == id) return room;
            }
            return null;
        }
    }
}

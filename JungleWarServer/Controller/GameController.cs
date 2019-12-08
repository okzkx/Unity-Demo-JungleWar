using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
namespace GameServer.Controller {
    public class GameController : BaseController {
        public int maxHp = 100;

        public GameController(ControllerManager cm):base(RequestCode.User,cm) { }

        /// <summary>
        /// 玩家发起开始游戏，
        /// 不是房主返回失败，
        /// 是房主就不返回，
        /// 然后对所有玩家广播成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public string StartGame(string data, Client client) {
            if (client.IsHouseOwner()) {
                client.room.StartGame();
                return null;
            } else {
                return ((int)ReturnCode.Fail).ToString();
            }
        }

        public string Move(string data, Client client) {
            Room room = client.room;
            if (room != null)room.BroadcastMessage(ActionCode.Move, client, data);
            return null;
        }
        public string Shoot(string data, Client client) {
            Room room = client.room;
            if (room != null)room.BroadcastMessage( ActionCode.Shoot, client, data);
            return null;
        }
        //RoleType,damage
        public string Attack(string data, Client client) {
            string[] strs = data.Split(',');
            RoleType rt = (RoleType)Enum.Parse(typeof(RoleType),strs[0]);
            int damage = int.Parse(strs[1]);
            Room room = client.room;
            if (room != null)room.TakeDamage(rt,damage, client);
            return null;
        }
        public string QuitBattle(string data, Client client) {
            Room room = client.room;
            if (room != null) {
                room.BroadcastMessage(ActionCode.QuitBattle);
                room.Close();
            }
            return null;
        }
    }
}

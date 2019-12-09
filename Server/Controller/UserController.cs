using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using GameServer.DAO;
using GameServer.Model;

namespace GameServer.Controller
{
    public class UserController:BaseController
    {
        private UserDAO userDAO = new UserDAO();
        private ResultDAO resultDAO = new ResultDAO();

        public UserController(ControllerManager cm):base(RequestCode.User,cm) {}

        //username,password
        public string Login(string data, Client client) {
            string[] strs = data.Split(',');
            User user = userDAO.VerifyUser(client.MySQLConn, strs[0], strs[1]);
            if (user == null) {
                return ((int)ReturnCode.Fail).ToString();
            } else {
                Result res = resultDAO.GetResultByUserid(client.MySQLConn, user.Id);
                client.SetUserData(user, res);
                Console.WriteLine("UserController:用户"+ user.Username + "登录成功");
                return string.Format("{0},{1},{2},{3}", ((int)ReturnCode.Success).ToString(), user.Username, res.TotalCount, res.WinCount);
            }
        }

        //username,password
        public string Register(string data, Client client) {
            string[] strs = data.Split(',');
            string username = strs[0]; string password = strs[1];
            if (!userDAO.GetUserByUsername(client.MySQLConn, username)) {
                userDAO.AddUser(client.MySQLConn, username, password);
                return ((int)ReturnCode.Success).ToString();
            } else {
                return ((int)ReturnCode.Fail).ToString();
            }
        }
    }
}

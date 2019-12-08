using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;
using GameServer.Servers;
namespace GameServer.Controller {
    public class ControllerManager {
        public Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        public Server server;

        public ControllerManager(Server server) {
            this.server = server;
            InitController();
        }
        
        void InitController() {
            foreach (var item in Enum.GetValues(typeof(RequestCode))) {
                if ((RequestCode)item!=RequestCode.None) {
                    controllerDict.Add((RequestCode)item,
                        (BaseController)Activator.CreateInstance(
                            Type.GetType("GameServer.Controller." + item.ToString() + "Controller"),
                            new object[] { this }
                        )
                    );
                }
            }
        }

        public C GetController<C>()where C : BaseController {
            try {
                string typeName = typeof(C).ToString();
                string valueStr = typeName.Substring(0, typeName.LastIndexOf("Controller"));
                valueStr = valueStr.Substring(valueStr.LastIndexOf('.')+1);
                return (C)controllerDict[(RequestCode)Enum.Parse(typeof(RequestCode), valueStr)];
            } catch {
                return null;
            }
        }


        /// <summary>
        /// 服务端处理请求
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        /// <param name="client"></param>
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client) {
            //由RequestCode 从 controller 字典里得到对应的 Controller
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            if (isGet == false) {
                Console.WriteLine("无法得到[" + requestCode + "]所对应的Controller,无法处理请求"); return;
            }

            //由 ActionCode 得到 与其同名的方法
            string methodName = Enum.GetName(typeof(ActionCode), actionCode);
            MethodInfo mi = controller.GetType().GetMethod(methodName);
            if (mi == null) {
                Console.WriteLine("[警告]在Controller[" + controller.GetType() + "]中没有对应的处理方法:[" + methodName + "]"); return;
            }

            //由已得到的 Controller执行这个方法
            object[] parameters = new object[] { data, client };
            object o = mi.Invoke(controller, parameters);
            if (o == null || string.IsNullOrEmpty(o as string)) {
                return;
            }

            //结果返回 client，再向客户端返回结果
            server.SendResponse(client, actionCode, o as string);
        }

    }
}

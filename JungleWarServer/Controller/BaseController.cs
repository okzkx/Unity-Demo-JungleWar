using Common;
using GameServer.Controller;

public abstract class BaseController {
    public RequestCode requestCode;
    public ControllerManager controllerManager;

    public BaseController(RequestCode requestCode, ControllerManager controllerManager) {
        this.requestCode = requestCode;
        this.controllerManager = controllerManager;
    }
}

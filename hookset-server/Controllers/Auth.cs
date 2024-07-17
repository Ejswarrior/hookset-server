using Microsoft.AspNetCore.Mvc;

namespace hookset_server.Controllers
{
    public class Auth : ControllerBase
    {

        private readonly JWTManager jWTManager;

        public Auth(JWTManager jWTManager)
        {
            this.jWTManager = jWTManager;
        }
    }
}

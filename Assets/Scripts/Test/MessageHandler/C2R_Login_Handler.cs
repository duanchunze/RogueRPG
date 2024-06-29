using Hsenl.Network;
using UnityEngine;

namespace Hsenl {
    public class C2R_Login_Handler : AMessageHandlerAsync<C2R_Login, R2C_Login> {
        protected override async HTask<R2C_Login> Handle(C2R_Login message, long l) {
            await HTask.Completed;
            Debug.LogWarning($"用户登陆, {message.account} {message.password}");
            var response = new R2C_Login() {
                RpcId = message.RpcId,
                ip = "1.1.1.1", 
                token = 31415926
            };

            return response;
        }
    }
}
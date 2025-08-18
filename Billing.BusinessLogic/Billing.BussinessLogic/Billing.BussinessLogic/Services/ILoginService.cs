using Biling.DataModels.LoginModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.BussinessLogic
{
    public interface ILoginService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshRequest request);
        Task LogoutAsync(RefreshRequest request);
    }
}

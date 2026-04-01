using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services
{
    public interface IAuthService
    {
        Employee Register(RegisterDTO dto);
        string Login(LoginDTO dto);
    }
}
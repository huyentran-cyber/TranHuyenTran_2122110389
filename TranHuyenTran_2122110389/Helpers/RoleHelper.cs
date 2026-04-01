using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Helpers
{
    public static class RoleHelper
    {
        public static Role ParseRole(string? role)
        {
            return Enum.TryParse(role, true, out Role result)
                ? result
                : Role.Staff;
        }
    }
}
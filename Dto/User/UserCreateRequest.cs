using QLCHNT.Const;
using System;

namespace QLCHNT.Dto.User
{
    public class UserCreateRequest : UserRegister
    {   
        public Enums.Role Role { get; set; }
    }
}

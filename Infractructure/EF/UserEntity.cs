using ApplicationCore;
using Microsoft.AspNetCore.Identity;

namespace Infractructure.EF;

public class UserEntity :IdentityUser
{
    public UserDetails Details { get; set; }
}
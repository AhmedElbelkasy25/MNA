
using AutoMapper;
using Models;
using Models.ViewModels;
namespace MNA.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, AddToRoleVM>();
        }
    }
}

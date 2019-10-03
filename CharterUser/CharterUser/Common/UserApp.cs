using CharterUser.Common.Model;

namespace CharterUser.Common
{
    public class UserApp
    {
        public static UserApp SharedInstance { get; } = new UserApp();
        
        public IUserStore UserStore { get; set; }

        private UserApp()
        {
        }
    }
}
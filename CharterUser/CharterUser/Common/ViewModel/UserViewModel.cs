using CharterUser.Common.Model;

namespace CharterUser.Common.ViewModel
{
    public class UserViewModel
    {
        public IUserStore Users { get; private set; }

        public UserViewModel(IUserStore userStore)
        {
            Users = userStore;
        }
    }
}
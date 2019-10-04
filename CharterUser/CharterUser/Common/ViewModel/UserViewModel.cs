using CharterUser.Common.Model;

namespace CharterUser.Common.ViewModel
{
    public interface IUserView
    {
        IUserStore Users { get; }
        
        bool EmptyStateVisible { get; set; }
    }
    
    public class UserViewModel: IUserView
    {
        public IUserStore Users { get; private set; }
        
        public bool EmptyStateVisible { get; set; }

        public UserViewModel(IUserStore userStore)
        {
            Users = userStore;
        }
    }
}
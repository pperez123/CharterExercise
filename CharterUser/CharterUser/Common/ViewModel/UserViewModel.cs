using CharterUser.Common.Model;

namespace CharterUser.Common.ViewModel
{
    public interface IUserView
    {
        /// <summary>
        /// Platform specific storage object that manages the
        /// user list and saving to the local device.
        /// </summary>
        IUserStore Users { get; }
        
        /// <summary>
        /// Flag to denote if the empty state is shown in the users
        /// list view.
        /// </summary>
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
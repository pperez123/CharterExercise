using CharterUser.Common.Model;

namespace CharterUser.Common.ViewModel
{
    public interface ICreateUser
    {
        string Username { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        bool IsAdmin { get; set; }

        void Save();
    }
    
    public class CreateUserViewModel: ICreateUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        private readonly IUserStore userStore;

        public CreateUserViewModel(IUserStore store)
        {
            userStore = store;
        }
        
        public void Save()
        {
            // TODO: implement saving
        }
    }
}
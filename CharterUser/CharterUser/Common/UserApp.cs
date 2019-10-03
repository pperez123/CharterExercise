namespace CharterUser.Common
{
    public class UserApp
    {
        private static UserApp SharedInstance { get; } = new UserApp();

        private UserApp()
        {
            
        }
    }
}
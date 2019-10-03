using System.Collections.ObjectModel;
using System.Linq;

namespace CharterUser.Common.Model
{
    public interface IUserStore
    {
        ObservableCollection<User> Storage { get; }
        int Count { get; }
        void Add(User user);
        void Remove(User user);
        User Fetch(string id);
        bool Exists(string id);
    }
    
    public class UserStore: IUserStore
    {
        public ObservableCollection<User> Storage { get; private set; } = new ObservableCollection<User>();

        public void Add(User user)
        {
            Storage.Add(user);
        }

        public void Remove(User user)
        {
            Storage.Remove(user);
        }

        public User Fetch(string id)
        {
            return Storage.FirstOrDefault(user => user.UserId == id);
        }

        public bool Exists(string id)
        {
            return Storage.Any(user => user.UserId == id);
        }

        public int Count => Storage.Count;
    }
}
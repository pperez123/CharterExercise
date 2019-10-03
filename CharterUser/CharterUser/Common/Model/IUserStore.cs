using System.Collections.ObjectModel;

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
        void Persist();
        void LoadUsers();
    }
}
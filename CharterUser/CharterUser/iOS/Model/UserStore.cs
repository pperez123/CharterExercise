using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CharterUser.Common.Model;
using Foundation;
using Newtonsoft.Json;

namespace CharterUser.iOS.Model
{
    public class UserStore: IUserStore
    {
        static readonly string kPersistentStoreKey = "UserStoreKey";

        public ObservableCollection<User> Storage { get; private set; } 

        public UserStore()
        {
            LoadUsers();
        }

        public void Add(User user)
        {
            if (!Exists(user.UserId))
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

        public void Persist()
        {
            var json = JsonConvert.SerializeObject(Storage);
            NSUserDefaults.StandardUserDefaults.SetString(json, kPersistentStoreKey);
        }

        public void LoadUsers()
        {
            Storage = new ObservableCollection<User>();
            var json = NSUserDefaults.StandardUserDefaults.StringForKey(kPersistentStoreKey);

            if (!string.IsNullOrEmpty(json))
            {
                var users = JsonConvert.DeserializeObject<IEnumerable<User>>(json);
                
                if (users != null)
                {
                    var enumerable = users as User[] ?? users.ToArray();
                    
                    if (enumerable.Any())
                        Storage = new ObservableCollection<User>(enumerable);
                }
            }
        }
    }
}
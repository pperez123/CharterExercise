using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CharterUser.Common.Model;
using Foundation;
using Newtonsoft.Json;

namespace CharterUser.iOS.Model
{
    public class UserStore: IUserStore
    {
        const string PersistentStoreKey = "UserStoreKey";
        static readonly object LockObject = new object();

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
        
        /// <summary>
        /// Background operation to serialize collection object to a json string.
        /// The string is then stored in the device's standard user defaults storage.
        /// </summary>
        public void Persist()
        {
            Task.Factory.StartNew(() =>
            {
                lock (LockObject)
                {
                    var json = JsonConvert.SerializeObject(Storage);
                    NSUserDefaults.StandardUserDefaults.SetString(json, PersistentStoreKey);
                }
            });
        }
        
        /// <summary>
        /// This method retrieves the json representation of the user collection from the device's
        /// standard user defaults storage. The json is deserialized into an IEnumerable instance and used
        /// to initialize an ObservableCollection object.
        /// </summary>
        public void LoadUsers()
        {
            Storage = new ObservableCollection<User>();
            var json = NSUserDefaults.StandardUserDefaults.StringForKey(PersistentStoreKey);

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
        
        /// <summary>
        /// Removes the local device storage of the user list.
        /// </summary>
        public static void ClearLocalStorage()
        {
            NSUserDefaults.StandardUserDefaults.RemoveObject(PersistentStoreKey);
        }
    }
}
using System.Collections.ObjectModel;

namespace CharterUser.Common.Model
{
    public interface IUserStore
    {
        /// <summary>
        /// In-memory storage of user list primarily used as the backing store
        /// for the user list view. Can generate collection changed events that should
        /// be subscribed to and used to update the table/list view.
        /// </summary>
        ObservableCollection<User> Storage { get; }
        
        /// <summary>
        /// Current count of users in list
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Will add a new user if one with the same id does not already exist.
        /// </summary>
        /// <param name="user"></param>
        void Add(User user);
        
        /// <summary>
        /// Removes a user from the list.
        /// </summary>
        /// <param name="user"></param>
        void Remove(User user);
        
        /// <summary>
        /// Fetches user data with userId as search criterion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User Fetch(string id);
        
        /// <summary>
        /// Checks to see if user with given id already exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(string id);
        
        /// <summary>
        /// Persist the user list to local device storage.
        /// </summary>
        void Persist();
        
        /// <summary>
        /// Load user list from local device storage.
        /// Overwrites any current data in the collection object.
        /// </summary>
        void LoadUsers();
    }
}
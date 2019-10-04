
using System;
using System.Linq;
using CharterUser.Common.Model;
using CharterUser.Common.ViewModel;
using CharterUser.iOS.Model;
using Foundation;
using NUnit.Framework;

namespace CharterUserTest
{
    [TestFixture]
    public class MyTest
    {
        [Test]
        public void TestUserPersistence()
        {
            NSUserDefaults.StandardUserDefaults.RemoveObject(UserStore.PersistentStoreKey);
            
            var userStorage = new UserStore();
            
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = "pperez",
                Email = "pperez@slipsleeve.com",
                IsAdmin = true,
                Password = "123456"
            };
            
            userStorage.Add(user);
            userStorage.Persist();
            
            var userStorageVerify = new UserStore();
            Assert.True(userStorageVerify.Count == 1);

            var savedUser = userStorageVerify.Fetch(user.UserId);
            Assert.True(savedUser != null);

            if (savedUser != null)
            {
                Assert.True(savedUser.Username == user.Username);
                Assert.True(savedUser.Email == user.Email);
                Assert.True(savedUser.IsAdmin == user.IsAdmin);
                Assert.True(savedUser.Password == user.Password);
            }
        }

        [Test]
        public void TestPasswordValidation()
        {
            var viewModel = new CreateUserViewModel(null);
            viewModel.Password = "1234";
            var errors = viewModel.ValidatePassword().ToArray();
            Assert.True(errors.Any(x => x == CreateUserViewModel.PasswordLengthError));
            Assert.True(errors.Any(x => x == CreateUserViewModel.PasswordAtLeastOneLetterDigitError));

            viewModel.Password = "$!@#$%^&*()!@";
            var errors2 = viewModel.ValidatePassword().ToArray();
            Assert.True(errors2.Any(x => x == CreateUserViewModel.PasswordLengthError));
            Assert.True(errors2.Any(x => x == CreateUserViewModel.PasswordAlphanumericError));
            Assert.True(errors2.Any(x => x == CreateUserViewModel.PasswordAtLeastOneLetterDigitError));

            viewModel.Password = "abcabc123";
            var errors3 = viewModel.ValidatePassword().ToArray();
            Assert.True(errors3.Any(x => x == CreateUserViewModel.PasswordRepeatedSequenceError));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CharterUser.Common.Model;

namespace CharterUser.Common.ViewModel
{
    public interface ICreateUser
    {
        string Username { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        bool IsAdmin { get; set; }
        
        bool CanSave { get; }

        void Save();
        IEnumerable<string> ValidateRequiredFields();
        IEnumerable<string> ValidatePassword();
    }
    
    public class CreateUserViewModel: ICreateUser
    {
        public string Username { get; set; }
        public string Password { get; set; } = "";
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        
        public bool CanSave => !ValidateRequiredFields().Any() && !ValidatePassword().Any();

        private readonly IUserStore userStore;
        
        readonly Regex onlyAlphaNumeric = new Regex("^[A-Za-z0-9]+$");
        readonly Regex atLeastOneAlphaAndDigit = new Regex("[A-Za-z]+\\d+|\\d+[A-Za-z]+");
        readonly Regex sequenceRepetition = new Regex("(\\w{2,})\\1");

        public CreateUserViewModel(IUserStore store)
        {
            userStore = store;
        }

        public IEnumerable<string> ValidateRequiredFields()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrEmpty(Username))
            {
                errors.Add("Please provide a username.");
            }

            if (string.IsNullOrEmpty(Password))
            {
                errors.Add("Please provide a password.");
            }

            if (string.IsNullOrEmpty(Password))
            {
                errors.Add("Please provide an email.");
            }

            return errors.ToArray();
        }

        public IEnumerable<string> ValidatePassword()
        {
            var errors = new List<string>();

            if (Password.Length >= 5 && Password.Length <= 12)
            {
                errors.Add("Password must be between 5 and 12 characters in length.");
            }

            if (!onlyAlphaNumeric.Matches(Password).Any())
            {
                errors.Add("Password must contain letters and numbers only.");
            }

            if (!atLeastOneAlphaAndDigit.Matches(Password).Any())
            {
                errors.Add("Password must have at least one letter and one digit.");
            }

            if (sequenceRepetition.Matches(Password).Any())
            {
                errors.Add("Password must not contain any repeated sequences of characters.");
            }

            return errors;
        }

        public void Save()
        {
            if (CanSave)
            {
                var user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = Username,
                    Email = Email,
                    IsAdmin = IsAdmin,
                    Password = Password
                };
                
                userStore.Add(user);
            }
        }
    }
}
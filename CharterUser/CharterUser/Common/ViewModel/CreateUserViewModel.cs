using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
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

        void Save(bool persist);
        IEnumerable<string> ValidateRequiredFields();
        IEnumerable<string> ValidatePassword();

        IObservable<IEnumerable<string>> OnError { get; }
        IObservable<Unit> OnSave { get; }
    }
    
    public class CreateUserViewModel: ICreateUser
    {
        public const string PasswordLengthError = "Password must be between 5 and 12 characters in length.";
        public const string PasswordAlphanumericError = "Password must contain letters and numbers only.";
        public const string PasswordAtLeastOneLetterDigitError = "Password must have at least one letter and one digit.";
        public const string PasswordRepeatedSequenceError = "Password must not contain any consecutively repeated sequences of characters.";
        public string Username { get; set; }
        public string Password { get; set; } = "";
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        
        public bool CanSave => !ValidateRequiredFields().Any() && !ValidatePassword().Any();

        private readonly IUserStore userStore;
        
        readonly Regex onlyAlphaNumeric = new Regex("^[A-Za-z0-9]+$");
        readonly Regex atLeastOneAlphaAndDigit = new Regex("[A-Za-z]+\\d+|\\d+[A-Za-z]+");
        readonly Regex sequenceRepetition = new Regex("(\\w{2,})\\1");
        
        readonly Subject<IEnumerable<string>> onErrorSubject = new Subject<IEnumerable<string>>();
        readonly Subject<Unit> onSaveSubject = new Subject<Unit>();

        public IObservable<IEnumerable<string>> OnError => onErrorSubject;
        public IObservable<Unit> OnSave => onSaveSubject;

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

            if (string.IsNullOrEmpty(Email))
            {
                errors.Add("Please provide an email.");
            }

            return errors.ToArray();
        }

        public IEnumerable<string> ValidatePassword()
        {
            var errors = new List<string>();

            if (Password.Length < 5 || Password.Length > 12)
            {
                errors.Add(PasswordLengthError);
            }

            if (!onlyAlphaNumeric.Matches(Password).Any())
            {
                errors.Add(PasswordAlphanumericError);
            }

            if (!atLeastOneAlphaAndDigit.Matches(Password).Any())
            {
                errors.Add(PasswordAtLeastOneLetterDigitError);
            }

            if (sequenceRepetition.Matches(Password).Any())
            {
                errors.Add(PasswordRepeatedSequenceError);
            }

            return errors;
        }

        void CheckRequiredFields()
        {
            var errors = ValidateRequiredFields().ToArray();

            if (errors.Any())
            {
                onErrorSubject.OnNext(errors);
            }
        }

        void CheckPassword()
        {
            var errors = ValidatePassword().ToArray();

            if (errors.Any())
            {
                onErrorSubject.OnNext(errors);
            }
        }

        public void Save(bool persist)
        {
            CheckRequiredFields();
            CheckPassword();
            
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
                
                if (persist)
                    userStore.Persist();
                
                onSaveSubject.OnNext(Unit.Default);
            }
        }
    }
}
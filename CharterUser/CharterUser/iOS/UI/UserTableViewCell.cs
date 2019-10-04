using System;
using CharterUser.Common.Model;
using CharterUser.iOS.Extensions;
using Foundation;
using UIKit;

namespace CharterUser.iOS.UI
{
    public class UserTableViewCell: UITableViewCell
    {
        readonly UILabel userTypeLabel = new UILabel();
        readonly UILabel usernameLabel = new UILabel();
        readonly UILabel emailLabel = new UILabel();
        readonly UITextField passwordField = new UITextField();
        readonly UIView separatorView = new UIView();
        readonly UIImageView iconView = new UIImageView();
        readonly UIButton eyeButton = new UIButton(UIButtonType.Custom);
        
        public UserTableViewCell(IntPtr handle) : base (handle)
        {
            CommonInit();
        }
		
        public UserTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier)
        {
            CommonInit();
        }

        void CommonInit()
        {
            iconView.Image = UIImage.FromBundle("iconUser");
            eyeButton.SetImage(UIImage.FromBundle("iconEye"), UIControlState.Normal);
            userTypeLabel.Font = UIFont.SystemFontOfSize(11, UIFontWeight.Semibold);
            usernameLabel.Font = UIFont.BoldSystemFontOfSize(17);
            
            emailLabel.Font = UIFont.SystemFontOfSize(15);
            emailLabel.TextColor = UIColor.DarkGray;
            
            passwordField.Font = UIFont.SystemFontOfSize(15);
            passwordField.TextColor = UIColor.DarkGray;
            passwordField.SecureTextEntry = true;
            passwordField.Enabled = false;

            usernameLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
            emailLabel.LineBreakMode = UILineBreakMode.CharacterWrap;

            usernameLabel.Lines = 0;
            emailLabel.Lines = 0;

            userTypeLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            usernameLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            emailLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            passwordField.TranslatesAutoresizingMaskIntoConstraints = false;
            separatorView.TranslatesAutoresizingMaskIntoConstraints = false;
            iconView.TranslatesAutoresizingMaskIntoConstraints = false;
            eyeButton.TranslatesAutoresizingMaskIntoConstraints = false;
            
            ContentView.AddSubview(iconView);
            ContentView.AddSubview(userTypeLabel);
            ContentView.AddSubview(usernameLabel);
            ContentView.AddSubview(emailLabel);
            ContentView.AddSubview(passwordField);
            ContentView.AddSubview(separatorView);
            ContentView.AddSubview(eyeButton);

            iconView.AlignEdgesWithSuperview(RectEdge.Leading | RectEdge.Top, 
                new UIEdgeInsets(16, 16, 0, 0));
            iconView.PinSize(50, 50);

            userTypeLabel.PinHorizontalSpacing(iconView, 12);
            userTypeLabel.AlignEdgesWithSuperview(RectEdge.Top, new UIEdgeInsets(16, 0, 0, 0));

            usernameLabel.PinHorizontalSpacing(iconView, 12);
            usernameLabel.PinVerticalSpacing(userTypeLabel, 4);
            usernameLabel.AlignEdgeWithSuperview(RectEdge.Trailing, -16);

            emailLabel.PinHorizontalSpacing(iconView, 12);
            emailLabel.PinVerticalSpacing(usernameLabel, 7);
            emailLabel.AlignEdgeWithSuperview(RectEdge.Trailing, -16);
            
            passwordField.PinHorizontalSpacing(iconView, 12);
            passwordField.PinVerticalSpacing(emailLabel, 7);
            passwordField.PinWidth(120);

            eyeButton.PinHorizontalSpacing(passwordField, 8);
            eyeButton.AlignVerticalCenter(passwordField);
            eyeButton.PinSize(22, 22);

            separatorView.PinVerticalSpacing(passwordField, 16);
            separatorView.AlignEdgesWithSuperview(RectEdge.Leading | RectEdge.Bottom | RectEdge.Trailing,
                new UIEdgeInsets(0, 16, 0, 0));
            separatorView.PinHeight(1);
            separatorView.BackgroundColor = "#C8C8C8".ColorFromHexString();

            SelectionStyle = UITableViewCellSelectionStyle.None;

            eyeButton.TouchUpInside += (sender, args) =>
            { 
                passwordField.SecureTextEntry = !passwordField.SecureTextEntry;
                eyeButton.SetImage(passwordField.SecureTextEntry ? UIImage.FromBundle("iconEye") : UIImage.FromBundle("iconInvisible"), 
                    UIControlState.Normal);
            };
        }

        public void Configure(User user)
        {
            userTypeLabel.TextColor = user.IsAdmin ? "#E42346".ColorFromHexString() : "#007AFF".ColorFromHexString();
            userTypeLabel.Text = user.IsAdmin ? "Administrator".ToUpper() : "User".ToUpper();
            usernameLabel.Text = user.Username;
            emailLabel.Text = user.Email;
            passwordField.Text = user.Password;
        }
    }
}
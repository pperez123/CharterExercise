// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using CharterUser.Common.Extensions;
using CharterUser.Common.ViewModel;
using CharterUser.iOS.Extensions;
using Foundation;
using UIKit;

namespace CharterUser.iOS.UI
{
	public partial class CreateUserViewController : UIViewController, IUITableViewDataSource
	{
		UIButton saveButton = new UIButton(UIButtonType.Custom);
        
        public ICreateUser ViewModel { get; set; }
        
        readonly CompositeDisposable disposable = new CompositeDisposable();
        
		public CreateUserViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			TableView.DataSource = this;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			saveButton.TranslatesAutoresizingMaskIntoConstraints = false;
			saveButton.BackgroundColor = "#007AFF".ColorFromHexString();
			saveButton.Layer.CornerRadius = 10;

			saveButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			saveButton.SetTitle("Save", UIControlState.Normal);
			saveButton.PinHeight(48);

			saveButton.TouchUpInside += (sender, args) =>
			{
				ViewModel?.Save(true);
			};

			CancelButton.TouchUpInside += (sender, args) => { DismissModalViewController(true); };
		}

		public override void ViewWillAppear(bool animated)
		{
			ViewModel?.OnError
				.ObserveOn(SynchronizationContext.Current)
				.Subscribe(ShowErrorAlert)
				.AddToDisposable(disposable);

			ViewModel?.OnSave
				.ObserveOn(SynchronizationContext.Current)
				.Subscribe(_ =>
				{
					ShowSaveMessage("User successfully saved.");
				})
				.AddToDisposable(disposable);
		}
		
		public override void ViewWillDisappear(bool animated)
		{
			disposable.Clear();
		}

		void ShowErrorAlert(IEnumerable<string> errors)
		{
			var message = errors.Aggregate("", (s, s1) => $"{s}\n{s1}");
			var alertController = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
			var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => { });
			alertController.AddAction(okAction);
			
			PresentModalViewController(alertController, true);
		}

		void ShowSaveMessage(string message)
		{
			var alertController = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);

			var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default,
				action =>
				{
					DismissModalViewController(true);
				});
			
			alertController.AddAction(okAction);
			
			PresentModalViewController(alertController, true);
		}

		[Export("tableView:numberOfRowsInSection:")]
		public nint RowsInSection(UITableView tableview, nint section)
		{
			return 5;
		}
		
		[Export("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Item == 4)
			{
				var cell = new UITableViewCell(UITableViewCellStyle.Default, null);
				cell.ContentView.AddSubview(saveButton);
				saveButton.AlignEdgesWithSuperview(RectEdge.All, new UIEdgeInsets(16, 16, 16, 16));
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				return cell;
			}
			else
			{
				var cell = new CreateUserTableViewCell(UITableViewCellStyle.Default, null);
				
				switch (indexPath.Item)
				{
					case 0:
						cell.TitleLabel.Text = "Username";
						cell.InputField.Placeholder = "Please enter your username.";
						cell.InputField.EditingChanged += (sender, args) =>
						{
							if (sender is UITextField field)
							{
								ViewModel.Username = field.Text;
							}
						};
						break;
					case 1:
						cell.TitleLabel.Text = "Password";
						cell.InputField.Placeholder = "Please enter your password.";
						cell.InputField.SecureTextEntry = true;
						cell.InputField.EditingChanged += (sender, args) =>
						{
							if (sender is UITextField field)
							{
								ViewModel.Password = field.Text;
							}
						};
						break;
					case 2:
						cell.TitleLabel.Text = "Email Address";
						cell.InputField.Placeholder = "Please enter your email address.";
						cell.InputField.EditingChanged += (sender, args) =>
						{
							if (sender is UITextField field)
							{
								ViewModel.Email = field.Text;
							}
						};
						break;
					case 3:
						cell.TitleLabel.Text = "Administrator";
						cell.ToggleSwitchVisibility();
						cell.Switch.ValueChanged += (sender, args) =>
						{
							if (sender is UISwitch uiSwitch)
							{
								ViewModel.IsAdmin = uiSwitch.On;
							}
						};
						break;
					default:
						break;
				}
				
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				return cell;
			}
		}
	}
}

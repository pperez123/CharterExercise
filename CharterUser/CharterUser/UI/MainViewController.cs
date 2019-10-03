using System;
using System.Threading;
using CharterUser.Common.ViewModel;
using Foundation;
using UIKit;

namespace CharterUser.UI
{
    public partial class MainViewController : UIViewController
    {
        readonly UserViewModel viewModel = new UserViewModel();
        readonly MainTableViewSource tableViewSource = new MainTableViewSource();

        public MainViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            NavigationController.NavigationBar.BackIndicatorImage = new UIImage();

            tableViewSource.ViewModel = viewModel;
            
            TableView.Source = tableViewSource;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.DestinationViewController is CreateUserViewController vc)
            {
                vc.ViewModel = new CreateUserViewModel(viewModel.Users);
            }
        }
    }

    class MainTableViewSource : UITableViewSource
    {
        public UserViewModel ViewModel { get; set; }
        
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var count = ViewModel?.Users.Count ?? 0;

            if (count == 0)
            {
                var cell = new UITableViewCell(UITableViewCellStyle.Default, null);
                cell.TextLabel.Font = UIFont.SystemFontOfSize(17);
                cell.TextLabel.Text = "No users found. Please tap \"+\" to add a user.";

                return cell;
            }
            
            return new UITableViewCell();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var count = ViewModel?.Users.Count ?? 0;
            return count == 0 ? 1 : count;
        }
    }
}
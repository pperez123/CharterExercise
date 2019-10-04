using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CharterUser.Common;
using CharterUser.Common.ViewModel;
using Foundation;
using UIKit;

namespace CharterUser.iOS.UI
{
    public partial class MainViewController : UIViewController
    {
        public const string CellId = "userCell";
        readonly IUserView viewModel = new UserViewModel(UserApp.SharedInstance.UserStore);
        readonly MainTableViewSource tableViewSource = new MainTableViewSource();

        private bool appeared;

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
            TableView.RegisterClassForCellReuse(typeof(UserTableViewCell), CellId);
            
            viewModel.Users.Storage.CollectionChanged += StorageOnCollectionChanged;
        }

        public override void ViewWillAppear(bool animated)
        {
            if (!appeared)
                TableView.ReloadData();
        }

        public override void ViewDidAppear(bool animated)
        {
            appeared = true;
        }

        void StorageOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            BeginInvokeOnMainThread(() =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    {
                        var addList = new List<NSIndexPath>();
                        var endIdx = args.NewStartingIndex + args.NewItems.Count;
                        for (var i = args.NewStartingIndex; i < endIdx; i++)
                        {
                            addList.Add(NSIndexPath.FromRowSection(i, 0));
                        }

                        try
                        {
                            TableView.BeginUpdates();
                            
                            // Check if empty state row still visible and remove that row before adding first user
                            if (viewModel.EmptyStateVisible)
                            {
                                TableView.DeleteRows(new[] {NSIndexPath.FromRowSection(0, 0)},
                                    UITableViewRowAnimation.Automatic);
                                viewModel.EmptyStateVisible = false;
                            }
                            
                            TableView.InsertRows(addList.ToArray(), UITableViewRowAnimation.Automatic);
                            TableView.EndUpdates();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            TableView.ReloadData();
                        }
                    }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                    {
                        var removeList = new List<NSIndexPath>();
                        var endIdx = args.OldStartingIndex + args.OldItems.Count;
                        for (var i = args.OldStartingIndex; i < endIdx; i++)
                            removeList.Add(NSIndexPath.FromRowSection(i, 0));

                        try
                        {
                            TableView.DeleteRows(removeList.ToArray(), UITableViewRowAnimation.Automatic);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            TableView.ReloadData();
                        }
                    }
                        break;
                    default:
                        TableView.ReloadData();
                        break;
                }
            });
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
        public IUserView ViewModel { get; set; }
        
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var count = ViewModel?.Users.Count ?? 0;

            if (count == 0)
            {
                var cell = new UITableViewCell(UITableViewCellStyle.Default, null);
                cell.TextLabel.Font = UIFont.SystemFontOfSize(17);
                cell.TextLabel.Text = "No users found. Please tap \"+\" to add a user.";
                
                if (ViewModel != null)
                    ViewModel.EmptyStateVisible = true;
                
                return cell;
            }
            else
            {
                if (indexPath.Row < ViewModel?.Users.Count && 
                    tableView.DequeueReusableCell(MainViewController.CellId) is UserTableViewCell cell)
                {
                    var user = ViewModel.Users.Storage.ElementAt(indexPath.Row);
                    cell.Configure(user);

                    return cell;
                }
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
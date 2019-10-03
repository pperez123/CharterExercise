using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        readonly SemaphoreSlim tableLockObject = new SemaphoreSlim(1, 1);

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
            
            viewModel.Users.Storage.CollectionChanged += StorageOnCollectionChanged;
        }

        void StorageOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            tableLockObject.Wait();

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

                    BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            TableView.InsertRows(addList.ToArray(), UITableViewRowAnimation.Automatic);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            TableView.ReloadData();
                        }
                        finally
                        {
                            tableLockObject.Release();
                        }
                    });
                }
                    break;
                case NotifyCollectionChangedAction.Remove:
                {
                    var removeList = new List<NSIndexPath>();
                    var endIdx = args.OldStartingIndex + args.OldItems.Count;
                    for (var i = args.OldStartingIndex; i < endIdx; i++)
                        removeList.Add(NSIndexPath.FromRowSection(i, 0));

                    BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            TableView.DeleteRows(removeList.ToArray(), UITableViewRowAnimation.Automatic);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            TableView.ReloadData();
                        }
                        finally
                        {
                            tableLockObject.Release();
                        }
                    });
                }
                    break;
                default:
                    BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            TableView.ReloadData();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        finally
                        {
                            tableLockObject.Release();
                        }
                    });
                    
                    break;
            }
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
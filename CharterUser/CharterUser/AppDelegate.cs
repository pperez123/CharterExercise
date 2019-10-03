using System;
using CharterUser.Common;
using CharterUser.iOS.Model;
using Foundation;
using UIKit;

namespace CharterUser
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate
    {

        [Export("window")]
        public UIWindow Window { get; set; }

        [Export("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UserApp.SharedInstance.UserStore = new UserStore();
            return true;
        }

        [ Export("applicationDidEnterBackground:") ]
        public void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("App entering background state.");
            UserApp.SharedInstance.UserStore.Persist();
        }

        [ Export("applicationWillTerminate:") ]
        public void WillTerminate(UIApplication application)
        {
            Console.WriteLine("App is terminating.");
            UserApp.SharedInstance.UserStore.Persist();
        }

        // UISceneSession Lifecycle

        [Export("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create("Default Configuration", connectingSceneSession.Role);
        }

        [Export("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions(UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
    }
}


using System;
using Foundation;
using UIKit;

namespace PhonesTrisIV
{
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
    {
        private static PhonestrisIV phonestrisiv;

        internal static void RunGame()
        {
            phonestrisiv = new PhonestrisIV();
            phonestrisiv.Run();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override void FinishedLaunching(UIApplication app)
        {
            RunGame();
        }
    }
}

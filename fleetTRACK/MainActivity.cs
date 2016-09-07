using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using fleetTRACK.Model;

namespace fleetTRACK
{
    [Activity(Label = "fleetTRACK", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Journey _currentJourney = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.btnLoggingToggle);
            Single[] test = new Single[1];
            Android.Locations.Location.DistanceBetween(-31.7362024, 115.7659186, -31.7524007, 115.7710995, test);
            button.Click += delegate { button.Text = String.Format("Ditance between: {0}", test[0] / 1000); };
        }
    }
}


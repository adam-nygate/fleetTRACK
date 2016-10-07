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
            bool toggled = false;
            Button button = FindViewById<Button>(Resource.Id.btnLoggingToggle);
            // note: toggled is temporary until GUI is implemented
            button.Click +=  delegate {
                if (toggled == false)
                {
                    button.Text = "Starting...";
                    _currentJourney = new Journey(this, "Toyota", "WM01", "001", "A1", "1023", "1", "12", "ECU", "4", "Adam", "");
                    button.Text = "Starting to log...";
                    _currentJourney.Start();
                    button.Text = "Stop";
                    toggled = true;
                }
                else
                {
                    button.Text = "Stopping...";
                    _currentJourney.Stop();
                    button.Text = "Writing to storage...";
                    _currentJourney.WriteLogFiles();
                    button.Text = "Start";
                    toggled = false;
                }
            };
        }
    }
}


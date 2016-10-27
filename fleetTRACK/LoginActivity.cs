using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace fleetTRACK
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Admin);

            Button Login = FindViewById<Button>(Resource.Id.btnLogin);
            EditText AdminCode = FindViewById<EditText>(Resource.Id.entryAdminCode);

            Login.Click += delegate
            {
                // Get shared preferences
                ISharedPreferences Settings = GetSharedPreferences("settings", 0);

                // If entered admin code matches one from shared preferences
                if (Settings.GetString("AdminCode", "") == AdminCode.Text)
                {
                    // Go to the settings menu in the app
                    StartActivity(typeof(SettingsActivity));
                }
                else
                {
                    // Go to the main menu of the app
                    StartActivity(typeof(MainActivity));
                }
            };
        }
    }
}
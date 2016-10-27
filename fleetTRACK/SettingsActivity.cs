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
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view to the "settings" layout resource
            SetContentView(Resource.Layout.Settings);
            // Get shared preferences
            ISharedPreferences settings = GetSharedPreferences("settings", 0);
            // Get entry box for each field
            EditText AdminCode = FindViewById<EditText>(Resource.Id.entryAdminCode);
            EditText Vehicle = FindViewById<EditText>(Resource.Id.entryVehicle);
            EditText Rego = FindViewById<EditText>(Resource.Id.entryRego);
            EditText Account = FindViewById<EditText>(Resource.Id.entryAccount);
            EditText Activity = FindViewById<EditText>(Resource.Id.entryActivity);
            EditText Location = FindViewById<EditText>(Resource.Id.entryLocation);
            EditText Company = FindViewById<EditText>(Resource.Id.entryCompany);
            // Get save, cancel and email buttons
            Button Save = FindViewById<Button>(Resource.Id.btnSaveChanges);
            Button Cancel = FindViewById<Button>(Resource.Id.btnCancelChanges);
            Button Email = FindViewById<Button>(Resource.Id.btnEmailMenu);
            // Set entry box values to ones from shared preferences
            AdminCode.Text = settings.GetString("AdminCode", "");
            Vehicle.Text = settings.GetString("Vehicle", "");
            Rego.Text = settings.GetString("Rego", "");
            Account.Text = settings.GetString("Account", "");
            Activity.Text = settings.GetString("Activity", "");
            Location.Text = settings.GetString("Location", "");
            Company.Text = settings.GetString("Company", "");

            Save.Click += delegate
            {
                // Get shared preferences
                ISharedPreferencesEditor SettingsEditor = settings.Edit();
                // Save new values to shared preferences
                SettingsEditor.PutString("AdminCode", AdminCode.Text);
                SettingsEditor.PutString("Vehicle", Vehicle.Text);
                SettingsEditor.PutString("Rego", Rego.Text);
                SettingsEditor.PutString("Account", Account.Text);
                SettingsEditor.PutString("Activity", Activity.Text);
                SettingsEditor.PutString("Location", Location.Text);
                SettingsEditor.PutString("Company", Company.Text);
                // Commit changes
                SettingsEditor.Commit();
                // Go to the main menu of the app
                StartActivity(typeof(MainActivity));
            };

            Cancel.Click += delegate
            {
                // Go to the main menu of the app
                StartActivity(typeof(MainActivity));
            };

            Email.Click += delegate
            {
                StartActivity(typeof(EmailActivity));
            };
        }
    }
}
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
            // Go to the main menu of the app
            MainMenu();
        }

        private void MainMenu()
        {
            // Set our view to the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.btnStartLogging);
            Button Admin = FindViewById<Button>(Resource.Id.btnAdmin);
            // Gets the entry box for each field
            EditText entryDriverName = FindViewById<EditText>(Resource.Id.entryDriverName);
            EditText entryProject = FindViewById<EditText>(Resource.Id.entryProject);
            EditText entryCostCentre = FindViewById<EditText>(Resource.Id.entryCostCentre);
            EditText entrySchoolArea = FindViewById<EditText>(Resource.Id.entrySchoolArea);

            // When button is clicked
            button.Click += delegate {
                // Get shared preferences
                ISharedPreferences settings = GetSharedPreferences("settings", 0);
                // Stores the values for each key
                string Vehicle = settings.GetString("Vehicle", "");
                string Rego = settings.GetString("Rego", "");
                string AccountNumber = settings.GetString("Account", "");
                string Activity = settings.GetString("Activity", "");
                string Location = settings.GetString("Location", "");
                string Company = settings.GetString("Company", "");

                // Start the journey
                _currentJourney = new Journey(this, Vehicle, Rego, entryProject.Text, entryCostCentre.Text, AccountNumber, Activity, Location, Company, entrySchoolArea.Text, entryDriverName.Text, "");
                _currentJourney.Start();
                // Go to the tracking page in the app
                TrackJourney();
            };

            Admin.Click += delegate
            {
                // Go to the admin menu in the app
                AdminMenu();
            };
        }

        private void TrackJourney()
        {
            // Set our view to the "activity" layout resource
            SetContentView(Resource.Layout.Activity);
            // Get our button which stops the logging
            Button button = FindViewById<Button>(Resource.Id.btnStopLogging);

            // When button is clicked
            button.Click += delegate
            {
                // Stops the logging and writes to csv in external storage
                _currentJourney.Stop();
                _currentJourney.WriteLogFiles();
                // Go back to the main menu in the app
                MainMenu();
            };
        }

        private void AdminMenu()
        {
            // Set our view to the "admin" layout resource
            SetContentView(Resource.Layout.Admin);
            // Gets button for logging in
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
                    SettingsMenu();
                }
                else
                {
                    // Go to the main menu of the app
                    MainMenu();
                }
            };
        }

        private void SettingsMenu()
        {
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
                MainMenu();
            };

            Cancel.Click += delegate
            {
                // Go to the main menu of the app
                MainMenu();
            };

            Email.Click += delegate
            {
                EmailMenu();
            };
        }

        private void EmailMenu()
        {
            // Set our view to the "Email" layout
            SetContentView(Resource.Layout.Email);
            // Get shared preferences
            ISharedPreferences settings = GetSharedPreferences("settings", 0);
            // Get entry box for each field
            EditText SendAddress = FindViewById<EditText>(Resource.Id.entrySendAddress);
            EditText SourceAddress = FindViewById<EditText>(Resource.Id.entrySourceAddress);
            EditText SmtpServer = FindViewById<EditText>(Resource.Id.entrySmtpServer);
            EditText SmtpUsername = FindViewById<EditText>(Resource.Id.entrySmtpUsername);
            EditText SmtpPassword = FindViewById<EditText>(Resource.Id.entrySmtpPassword);
            // Get save, cancel and send buttons
            Button Save = FindViewById<Button>(Resource.Id.btnSaveChanges);
            Button Cancel = FindViewById<Button>(Resource.Id.btnCancelChanges);
            Button Send = FindViewById<Button>(Resource.Id.btnSendEmail);
            // Set entry box values to ones from shared preferences
            SendAddress.Text = settings.GetString("SendAddress", "");
            SourceAddress.Text = settings.GetString("SourceAddress", "");
            SmtpServer.Text = settings.GetString("SmtpServer", "");
            SmtpUsername.Text = settings.GetString("SmtpUsername", "");
            SmtpPassword.Text = settings.GetString("SmtpPassword", "");

            Save.Click += delegate
            {
                // Get shared preferences
                ISharedPreferencesEditor SettingsEditor = settings.Edit();
                // Save new values to shared preferences
                SettingsEditor.PutString("SendAddress", SendAddress.Text);
                SettingsEditor.PutString("SourceAddress", SourceAddress.Text);
                SettingsEditor.PutString("SmtpServer", SmtpServer.Text);
                SettingsEditor.PutString("SmtpUsername", SmtpUsername.Text);
                SettingsEditor.PutString("SmtpPassword", SmtpPassword.Text);
                // Commit changes
                SettingsEditor.Commit();
            };

            Cancel.Click += delegate
            {
                // Go to the settings menu in the app
                SettingsMenu();
            };

            Send.Click += delegate
            {
                // Process the log files for emailing
                LogProcessor lp = new LogProcessor(this);
                lp.ProcessLogFiles();
                
            };
        }
    }
}


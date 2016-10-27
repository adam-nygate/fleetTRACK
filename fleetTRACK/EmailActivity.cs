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

using fleetTRACK.Model;

namespace fleetTRACK
{
    [Activity(Label = "Email")]
    public class EmailActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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
            EditText SmtpPort = FindViewById<EditText>(Resource.Id.entrySmtpPort);
            // Get save and send buttons
            Button Save = FindViewById<Button>(Resource.Id.btnSaveChanges);
            Button Send = FindViewById<Button>(Resource.Id.btnSendEmail);
            // Set entry box values to ones from shared preferences
            SendAddress.Text = settings.GetString("SendAddress", "");
            SourceAddress.Text = settings.GetString("SourceAddress", "");
            SmtpServer.Text = settings.GetString("SmtpServer", "");
            SmtpUsername.Text = settings.GetString("SmtpUsername", "");
            SmtpPassword.Text = settings.GetString("SmtpPassword", "");
            SmtpPort.Text = settings.GetInt("SmtpPort", 25).ToString();

            Save.Click += delegate
            {
                // Calls the save changes method with current EditText values
                SaveChanges(SendAddress.Text, SourceAddress.Text, SmtpServer.Text, SmtpUsername.Text, SmtpPassword.Text, SmtpPort.Text);
            };

            Send.Click += delegate
            {
                // Uncomment code to save changes before sending email
                //SaveChanges(SendAddress.Text, SourceAddress.Text, SmtpServer.Text, SmtpUsername.Text, SmtpPassword.Text, SmtpPort.Text);

                // To check if error has occured
                Boolean caught = false;

                // Process the log files for emailing
                LogProcessor lp = new LogProcessor(this);
                try
                {
                    lp.ProcessLogFiles();
                }
                catch
                {
                    // Show log processing failure dialog
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Error");
                    alert.SetMessage("Processing of logs failed");
                    alert.SetPositiveButton("OK", delegate { });
                    alert.Show();
                    caught = true;
                }
                if (!caught)
                {
                    // Show success dialog
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Success");
                    alert.SetMessage("Logs processed successfully");
                    alert.SetPositiveButton("OK", delegate { });
                    alert.Show();
                }
            };
        }

        public void SaveChanges(string SendAddress, string SourceAddress, string SmtpServer, string SmtpUsername, string SmtpPassword, string SmtpPort)
        {
            // To check if error has occured
            Boolean caught = false;

            ISharedPreferences settings = GetSharedPreferences("settings", 0);
            try
            {
                Convert.ToInt32(SmtpPort);
            }
            catch
            {
                // Show failure dialog
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage("SMTP Port must be a number");
                alert.SetPositiveButton("OK", delegate { });
                alert.Show();
                caught = true;
            }
            if (!caught)
            {
                int SmtpPortInt = Convert.ToInt32(SmtpPort);
                // Get shared preferences
                ISharedPreferencesEditor SettingsEditor = settings.Edit();
                // Save new values to shared preferences
                SettingsEditor.PutString("SendAddress", SendAddress);
                SettingsEditor.PutString("SourceAddress", SourceAddress);
                SettingsEditor.PutString("SmtpServer", SmtpServer);
                SettingsEditor.PutString("SmtpUsername", SmtpUsername);
                SettingsEditor.PutString("SmtpPassword", SmtpPassword);
                SettingsEditor.PutInt("SmtpPort", SmtpPortInt);

                // Commit changes
                SettingsEditor.Commit();

                // Show success dialog
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Success");
                alert.SetMessage("New settings saved");
                alert.SetPositiveButton("OK", delegate { });
                alert.Show();
            }
        }
    }
}
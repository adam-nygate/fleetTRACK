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
    [Activity(Label = "EmailActivity")]
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
            SmtpPort.Text = settings.GetInt("SmtpPort", 25).ToString();

            Save.Click += delegate
            {
                int SmtpPortInt = Convert.ToInt32(SmtpPort.Text);
                // Get shared preferences
                ISharedPreferencesEditor SettingsEditor = settings.Edit();
                // Save new values to shared preferences
                SettingsEditor.PutString("SendAddress", SendAddress.Text);
                SettingsEditor.PutString("SourceAddress", SourceAddress.Text);
                SettingsEditor.PutString("SmtpServer", SmtpServer.Text);
                SettingsEditor.PutString("SmtpUsername", SmtpUsername.Text);
                SettingsEditor.PutString("SmtpPassword", SmtpPassword.Text);
                SettingsEditor.PutInt("SmtpPort", SmtpPortInt);
                // Commit changes
                SettingsEditor.Commit();
            };

            Cancel.Click += delegate
            {
                // Go to the settings menu in the app
                StartActivity(typeof(SettingsActivity));
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
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using fleetTRACK.Model;
using System.Threading.Tasks;

namespace fleetTRACK
{
    [Activity(Label = "fleetTRACK", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Journey _currentJourney = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view to the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            // Get our button from the layout resource,
            // and attach an event to it
            Button StartButton = FindViewById<Button>(Resource.Id.btnStartLogging);
            Button Admin = FindViewById<Button>(Resource.Id.btnAdmin);
            // Gets the entry box for each field
            EditText entryDriverName = FindViewById<EditText>(Resource.Id.entryDriverName);
            EditText entryProject = FindViewById<EditText>(Resource.Id.entryProject);
            EditText entryCostCentre = FindViewById<EditText>(Resource.Id.entryCostCentre);
            EditText entrySchoolArea = FindViewById<EditText>(Resource.Id.entrySchoolArea);
            CheckBox tosAgreement = FindViewById<CheckBox>(Resource.Id.tosAgreement);

            Admin.Click += delegate
            {
                // Go to the admin menu in the app
                StartActivity(typeof(LoginActivity));
            };
            // When button is clicked
            StartButton.Click += delegate
            {
                if (tosAgreement.Checked)
                {
                    // Get shared preferences
                    ISharedPreferences settings = GetSharedPreferences("settings", 0);
                    // Stores the values for each key
                    string Vehicle = settings.GetString("Vehicle", "");
                    string Rego = settings.GetString("Rego", "");
                    string AccountNumber = settings.GetString("Account", "");
                    string Activity = settings.GetString("Activity", "");
                    string Location = settings.GetString("Location", "");
                    string Company = settings.GetString("Company", "");

                    // For displaying the driver's name
                    string DriverName = entryDriverName.Text;

                    // Start the journey
                    _currentJourney = new Journey(this, Vehicle, Rego, entryProject.Text, entryCostCentre.Text, AccountNumber, Activity, Location, Company, entrySchoolArea.Text, entryDriverName.Text, "");
                    _currentJourney.Start();

                    // Set our view to the "activity" layout resource
                    SetContentView(Resource.Layout.Activity);

                    // Get our button which stops the logging
                    Button StopButton = FindViewById<Button>(Resource.Id.btnStopLogging);

                    // Get our textbox which is for entering important notes
                    EditText editImportantNotes = FindViewById<EditText>(Resource.Id.editImportantNotes);

                    // Displays the driver name and vehicle rego
                    TextView viewDriverName = FindViewById<TextView>(Resource.Id.viewDriverName);
                    TextView viewCarRego = FindViewById<TextView>(Resource.Id.viewCarRego);
                    viewDriverName.Text = DriverName;
                    viewCarRego.Text = Rego;

                    // When button is clicked
                    StopButton.Click += delegate
                    {
                        // Sets the important notes for the journey with the text from our textbox
                        _currentJourney.SetImportantNotes(editImportantNotes.Text);

                        // Stops the logging and writes to csv in external storage
                        _currentJourney.Stop();
                        _currentJourney.WriteLogFiles();

                        // Go back to the main menu in the app
                        StartActivity(typeof(MainActivity));
                    };
                }
                else
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Warning");
                    alert.SetMessage("Must accept ToS agreement");
                    alert.SetPositiveButton("OK", delegate { });
                    alert.Show();
                }
            };
        }
    }
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;


namespace fleetTRACK.Model
{
    class Journey : Activity, ILocationListener
    {
        #region Public methods
        /// <summary>
        /// Constructor for the journey class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="carType"></param>
        /// <param name="carRegistration"></param>
        /// <param name="projectNumber"></param>
        /// <param name="costCentre"></param>
        /// <param name="accountNumber"></param>
        /// <param name="activityId"></param>
        /// <param name="locationNumber"></param>
        /// <param name="companyNumber"></param>
        /// <param name="schoolArea"></param>
        /// <param name="driverName"></param>
        /// <param name="importantNotes"></param>
        public Journey(Context context, string carType, string carRegistration, string projectNumber,
            string costCentre, string accountNumber, string activityId, string locationNumber,
            string companyNumber, string schoolArea, string driverName, string importantNotes)
        {
            // Define journey details
            this._carType = carType;
            this._carRegistration = carRegistration;
            this._projectNumber = projectNumber;
            this._costCentre = costCentre;
            this._accoutNumber = accountNumber;
            this._activityId = activityId;
            this._locationNumber = locationNumber;
            this._companyNumber = companyNumber;
            this._schoolArea = schoolArea;
            this._driverName = driverName;
            this._importantNotes = importantNotes;

            this._context = context;

            // Instantiate the location manager and define our criteria for GPS updates
            _locationManager = (LocationManager)this._context.GetSystemService(Context.LocationService);
            Criteria criteriaForLocationService = new Criteria { Accuracy = Accuracy.Fine };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            // Store the acceptable provider
            if (acceptableLocationProviders.Any())
                _locationProvider = acceptableLocationProviders.First();
            else
                throw new ApplicationException("No acceptable gps location providers found.");

            // Create the timer and register for callbacks
            _gpsTimer = new Timer(5000);
            _gpsTimer.AutoReset = true;
            _gpsTimer.Elapsed += RecordGpsLocation;
        }

        /// <summary>
        /// Starts the journey logging process
        /// </summary>
        public void Start()
        {
            // Subscribe to GPS updates and start the timer that records them
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            _startDateTime = DateTime.Now;
            _gpsTimer.Enabled = true;
        }

        /// <summary>
        /// Stops the journey logging process
        /// </summary>
        public void Stop()
        {
            // Unsubscribe from the updates and pause the timer
            _locationManager.RemoveUpdates(this);
            _gpsTimer.Enabled = false;
            _endDateTime = DateTime.Now;
        }

        /// <summary>
        /// Resumes the journey logging process (is an alias of Start())
        /// </summary>
        public void Resume()
        {
            // Alias for Start()
            this.Start();
        }

        /// <summary>
        /// Creates new csv files for the current journey.
        /// One file containing simple journey details and one file containing the extended detail set for auditing purposes.
        /// </summary>
        public void WriteLogFiles()
        {
            string rootLogDirectory = String.Format(
                "{0}{1}{2}{1}",
                Android.OS.Environment.ExternalStorageDirectory,
                Java.IO.File.Separator,
                _context.Resources.GetString(Resource.String.logDirectory));

            if (!Directory.Exists(rootLogDirectory))
            {
                try
                {
                    Directory.CreateDirectory(rootLogDirectory);
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException("Unable to create log directory", ex);
                }
            }

            string simpleJourneyDetailsFilename = String.Format("Trip_{0}_{1:yy-MM-dd_H-mm}_simple.csv", _carRegistration, _startDateTime);
            string extendedJourneyDetailsFilename = String.Format("Trip_{0}_{1:yy-MM-dd_H-mm}_extended.csv", _carRegistration, _startDateTime);

            // Write to new CSV file with cartype, car rego, project number, distance travelled etc.
            string simpleJourneyDetailsFilePath = String.Format(
                "{0}{1}",
                rootLogDirectory,
                simpleJourneyDetailsFilename);

            using (var sw = new StreamWriter(simpleJourneyDetailsFilePath))
            {
                // Write to the CSV
                sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},", _carType, _carRegistration, _startDateTime, _endDateTime, CalculateTotalDistanceInKilometres(), CalculateTotalAveragedAccuracy(), _projectNumber, _costCentre, _accoutNumber, _activityId, _locationNumber, _companyNumber, _schoolArea, _driverName, _importantNotes));
            }

            // Create a CSV file and write all locations to it (This is for auditing if average accuracy is too high).
            string extendedJourneyDetailsFilePath = String.Format(
                "{0}{1}",
                rootLogDirectory,
                extendedJourneyDetailsFilename);

            using (var sw = new StreamWriter(extendedJourneyDetailsFilePath))
            {
                sw.WriteLine("Longitude,Latitude,Accuracy,");
                foreach (Location location in _journeyLocations)
                {
                    // Write to the CSV
                    sw.WriteLine(string.Format("{0},{1},{2},", location.Longitude, location.Latitude, ConvertMetresToKilometres(location.Accuracy)));
                }
            }
        }

        /// <summary>
        /// Callback method that is invoked by the GPS provider when it has detected a change of location
        /// </summary>
        /// <param name="location"></param>
        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
        }

        // Yeah... let's not talk about these...
        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }
        #endregion

        #region Private properties
        // Misc properties
        Context _context;

        // GPS-specific properties
        private Location _currentLocation;
        private LocationManager _locationManager;
        private string _locationProvider;
        private static Timer _gpsTimer;

        // fleetTrack-specific properties
        private List<Location> _journeyLocations = new List<Location>();
        private string _carType, _carRegistration, _projectNumber, _costCentre, _accoutNumber, _activityId, 
            _locationNumber, _companyNumber, _schoolArea, _driverName, _importantNotes;
        private DateTime _startDateTime, _endDateTime;
        #endregion

        #region Private methods
        /// <summary>
        /// Calculates the total distance between all journey locations collected.
        /// </summary>
        /// <returns>Returns a double representing the total distance travelled in kilometres.</returns>
        private Double CalculateTotalDistanceInKilometres()
        {
            Single totalDistanceInMetres = 0;

            // Iterate through all journey locations collected
            for (int i = 0; i < _journeyLocations.Count; i++)
            {
                // Ensure that we are not trying to calulate the difference between the first element
                // and the one before (which would be null)
                if (i != 0)
                {
                    Single[] tempDistanceBetween = new Single[1];

                    // Calculate the distance between the current location and the one before
                    Location.DistanceBetween(
                        _journeyLocations[i].Latitude,
                        _journeyLocations[i].Longitude,
                        _journeyLocations[i - 1].Latitude,
                        _journeyLocations[i - 1].Longitude,
                        tempDistanceBetween);

                    // Add the distance to a total distance variable
                    totalDistanceInMetres += tempDistanceBetween[0];
                }
            }

            // Convert the total distance into kilometres and then round to two decimal places
            return ConvertMetresToKilometres(totalDistanceInMetres);
        }

        /// <summary>
        /// Calculates the total averaged accuracy of all journey locations collected.
        /// </summary>
        /// <returns>Returns a double representing the total averaged accuracy in kilometres.</returns>
        private Double CalculateTotalAveragedAccuracy()
        {
            Single totalAccuracyInMetres = 0;

            // Iterate through all journey locations collected
            foreach (Location location in _journeyLocations)
            {
                // Add the accuracy to total accuary variable
                totalAccuracyInMetres += location.Accuracy;
            }

            return ConvertMetresToKilometres(totalAccuracyInMetres / _journeyLocations.Count);
        }

        /// <summary>
        /// Converts the input from metres to kilometres and rounds to two decimal places.
        /// </summary>
        /// <param name="metres">Metres as a Single.</param>
        /// <returns>Returns a double representing the input converted to kilometres and rounded to two decimal places.</returns>
        private Double ConvertMetresToKilometres(Single metres)
        {
            return Math.Round(metres / 1000, 2);
        }

        /// <summary>
        /// A callback function for a timer to record the current gps location information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordGpsLocation(object sender, ElapsedEventArgs e)
        {
            _journeyLocations.Add(_currentLocation);
        }
        #endregion 
    }
}
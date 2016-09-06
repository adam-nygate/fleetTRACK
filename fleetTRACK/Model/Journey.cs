using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace fleetTRACK.Model
{
    class Journey
    {
        List<Location> _journeyLocations = new List<Location>();

        private string _carType, _carRegistration, _projectNumber, _costCentre, _accoutNumber, _activityId, 
            _locationNumber, _companyNumber, _schoolArea, _driverName, _importantNotes;
        private DateTime _startDateTime, _endDateTime;
        private Boolean _tosAgreement;

        public Journey()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Resume()
        {
            this.Start();
        }

        public void WriteToLog()
        {

        }

        /// <summary>
        /// Calculates the total distance between all journey locations collected.
        /// </summary>
        /// <returns>Returns a double representing the total distance travelled in kilometres.</returns>
        private Double CalculateTotalDistanceInKilometres()
        {
            Single totalDistanceInMetres = 0;
            
            // Iterate through all journey locations collected
            for(int i  = 0; i < _journeyLocations.Count; i++)
            {
                // Ensure that we are not trying to calulate the difference between the first element
                // and the one before (which would be null)
                if(i != 0)
                {
                    Single[] tempDistanceBetween = new Single[1];

                    // Calculate the distance between the current location and the one before
                    Location.DistanceBetween(
                        _journeyLocations[i].Latitude, 
                        _journeyLocations[i].Longitude, 
                        _journeyLocations[i-1].Latitude, 
                        _journeyLocations[i-1].Longitude,
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
            foreach(Location location in _journeyLocations)
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
    }
}
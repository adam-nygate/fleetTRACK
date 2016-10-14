using System;
using System.IO;
using MailKit.Net.Smtp;
using MimeKit;

using Android.App;
using Android.Content;
using Android.Net;

namespace fleetTRACK.Model
{
    class LogProcessor : Activity
    {
        #region Public methods
        /// <summary>
        /// Constructor for the LogProcessor class
        /// </summary>
        /// <param name="context"></param>
        public LogProcessor(Context context)
        {
            this._context = context;
        }

        /// <summary>
        /// Processes all fleetTRACK log files that have not been archived.
        /// </summary>
        public void ProcessLogFiles()
        {
            if (!IsNetworkConnectionValid())
                throw new ApplicationException("No valid network connection detected.");

            string logPath = String.Format(
                "{0}{1}{2}",
                Android.OS.Environment.ExternalStorageDirectory,
                Java.IO.File.Separator,
                _context.Resources.GetString(Resource.String.logDirectory));

            foreach (string filePath in Directory.GetFiles(logPath, "*_simple.csv"))
            {
                string rawJourneyName = filePath.Split(new string[] { "_simple.csv" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string simpleJourneyDetailsFilePath = filePath;
                string extendedJourneyDetailsFilePath = String.Format(
                    "{0}_extended.csv",
                    rawJourneyName);

                //Send email
                SendEmail(rawJourneyName, simpleJourneyDetailsFilePath, extendedJourneyDetailsFilePath);

                //Archive files to archive directory
                ArchiveFile(simpleJourneyDetailsFilePath);
                ArchiveFile(extendedJourneyDetailsFilePath);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Detects whether connected to a wifi network and can pass data.
        /// </summary>
        /// <returns>true, if network was detected, false otherwise.</returns>
        private Boolean IsNetworkConnectionValid()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)_context.GetSystemService(ConnectivityService);

            NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);

            if (wifiInfo.IsConnected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sends an email containing the journey details to the nominated email address.
        /// </summary>
        /// <param name="journeyName">The unique journey name.</param>
        /// <param name="simpleJourneyDetailsFilePath">The file path to the simple journey details.</param>
        /// <param name="extendedJourneyDetailsFilePath">The file path to the extended journey details.</param>
        private void SendEmail(string journeyName, string simpleJourneyDetailsFilePath, string extendedJourneyDetailsFilePath)
        {
            // Get shared preferences
            ISharedPreferences settings = GetSharedPreferences("settings", 0);

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("fleetTRACK", "fleetTRACK@ecu.edu.au"));
            message.To.Add(new MailboxAddress(settings.GetString("EmailAddress", ""), settings.GetString("EmailAddress", "")));
            message.Subject = journeyName;

            TextPart body = new TextPart("plain")
            {
                Text = "To whom it may concern,\n\tPlease see the attached trip details for billing purposes.\nKind regards,\nfleetTRACK"
            };

            // create an attachment for the file located at path
            MimePart simpleJourneyDetailsAttachment = new MimePart("text/csv")
            {
                ContentObject = new ContentObject(File.OpenRead(simpleJourneyDetailsFilePath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(simpleJourneyDetailsFilePath)
            };

            // create an attachment for the file located at path
            MimePart extendedJourneyDetailsAttachment = new MimePart("text/csv")
            {
                ContentObject = new ContentObject(File.OpenRead(extendedJourneyDetailsFilePath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(extendedJourneyDetailsFilePath)
            };

            // now create the multipart/mixed container to hold the message text and the attachments
            Multipart multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(simpleJourneyDetailsAttachment);
            multipart.Add(extendedJourneyDetailsAttachment);

            // now set the multipart/mixed as the message body
            message.Body = multipart;

            using (SmtpClient client = new SmtpClient())
            {
                client.Connect(settings.GetString("SmtpServer", ""), 587, false);

                // Since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                
                client.Authenticate(settings.GetString("SmtpUsername", ""), settings.GetString("SmtpPassword", ""));

                client.Send(message);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Archives the file
        /// </summary>
        /// <param name="originalFilePath">Path to the original file.</param>
		private void ArchiveFile(string originalFilePath)
        {
            String logArchivePath = String.Format(
                "{0}{1}{2}{1}",
                Android.OS.Environment.ExternalStorageDirectory,
                Java.IO.File.Separator,
                _context.Resources.GetString(Resource.String.logArchiveDirectory));

            if (!Directory.Exists(logArchivePath))
            {
                try
                {
                    Directory.CreateDirectory(logArchivePath);
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException("Unable to create log archive directory", ex);
                }
            }

            if (File.Exists(originalFilePath))
            {
                try
                {
                    File.Move(originalFilePath, logArchivePath + Path.GetFileName(originalFilePath));
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException("Unable to move log file to archive directory", ex);
                }
            }
        }
        #endregion

        #region Private Properties
        Context _context;
        #endregion
    }
}
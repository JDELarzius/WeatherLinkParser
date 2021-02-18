using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Weatherlink_Parser
{
    internal class WebManager
    {
        private const string CURRENT_OBS_FILE_NAME = "currentObservation.json";
        private const string TODAY_OBS_FILE_NAME = "todaysObservations.json";
        private const string YESTERDAY_OBS_FILE_NAME = "yesterdayObservations.json";
        private const string HIGHS_LOWS_FILE_NAME = "highsLows.json";
        private const string WEBCAM_IMAGE_FILE_NAME = "webcam.jpg";
        private static NetworkCredential Credentials;
        private static string FTPPath;

        internal static string GenerateJSONCurrentConditions(Observation observation)
        {
            string json = new JavaScriptSerializer().Serialize(observation.getSerializableObservationFull());
            return json;
        }

        internal static string GenerateJSONMultiple(List<Observation> observations)
        {
            string fullJSON = "[";

            foreach(Observation observation in observations)
            {
                string json = new JavaScriptSerializer().Serialize(observation.getSerializableObservationHistorical());
                fullJSON += json + ",";
            }

            //Remove the comma and replace it with a ]
            fullJSON = fullJSON.Remove(fullJSON.Length - 1, 1) + "]";
            
            return fullJSON;
        }

        private static string GenerateJSONHighsLows(List<HighLowObservation> observations)
        {
            string fullJSON = "[";

            foreach (HighLowObservation observation in observations)
            {
                string json = new JavaScriptSerializer().Serialize(observation.getSerializableHighLowSet());
                fullJSON += json + ",";
            }

            //Remove the comma and replace it with a ]
            fullJSON = fullJSON.Remove(fullJSON.Length - 1, 1) + "]";

            return fullJSON;
        }

        internal static void SetupFTP(string username, string password, string path)
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(path))
            {
                throw new Exception("Invalid FTP server information or credentials");
            }

            Credentials = new NetworkCredential(username, password);
            FTPPath = path;

            if (FTPPath.Length < 6 || FTPPath.Substring(0, 6) != "ftp://")
            {
                FTPPath = "ftp://" + FTPPath;
            }
        }

        internal static string SendCurrentObservationToServer(Observation observation)
        {
            string json = GenerateJSONCurrentConditions(observation);

            // Copy the contents of the file to the request stream.
            return uploadStringFile(json, CURRENT_OBS_FILE_NAME);
        }

        internal static string UploadTodayObservations()
        {
            DateTime today = DateTime.Now;
            List<Observation> observations = DatabaseManager.GetObservationsOnDate(today);

            string json = GenerateJSONMultiple(observations);

            json = "{" +
                "\"UploadTime\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\", " +
                "\"Data\":" +
                json + "}";

            return uploadStringFile(json, TODAY_OBS_FILE_NAME);
        }

        internal static string UploadYesterdayObservations()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            List<Observation> observations = DatabaseManager.GetObservationsOnDate(yesterday);

            string json = GenerateJSONMultiple(observations);

            json = "{" +
                "\"UploadTime\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\", " +
                "\"Data\":" +
                json + "}";

            return uploadStringFile(json, YESTERDAY_OBS_FILE_NAME);
        }

        internal static string UploadHighsAndLows()
        {
            DateTime today = DateTime.Now.AddDays(-1);
            DateTime oneYearAgo = DateTime.Now.AddYears(-1);
            List<HighLowObservation> observations = DatabaseManager.GetHighsAndLows(oneYearAgo, today);

            string json = GenerateJSONHighsLows(observations);

            json = "{" +
                "\"UploadTime\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\", " +
                "\"Data\":" +
                json + "}";

            return uploadStringFile(json, HIGHS_LOWS_FILE_NAME);
        }

        internal static string uploadWebcamImage(string sourceFilePath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPPath + WEBCAM_IMAGE_FILE_NAME);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = Credentials;
            request.UseBinary = true;

            byte[] fileContents = File.ReadAllBytes(sourceFilePath);
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            string results = string.Format("Upload File Complete, status {0}", response.StatusDescription.Substring(0, response.StatusDescription.IndexOf("\n") - 1));
            response.Close();
            return results;
        }

        private static string uploadStringFile(string contents, string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPPath + fileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = Credentials;
            request.UseBinary = false; 

            byte[] fileContents = Encoding.UTF8.GetBytes(contents);
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            string results = string.Format("Upload File Complete, status {0}", response.StatusDescription.Substring(0, response.StatusDescription.IndexOf("\n") - 1));
            response.Close();
            return results;
        }
    }
}
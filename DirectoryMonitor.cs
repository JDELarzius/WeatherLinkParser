using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Weatherlink_Parser
{
    internal class DirectoryMonitor
    {
        private static DateTime LastDailyUpload;
        private static FileSystemWatcher XMLFileWatcher;
        private static FileSystemWatcher JPEGFileWatcher;
        private static MainWindow MainWindow;
        internal static void Start(string XMLfileToWatch, string JPEGDirectoryToWatch, MainWindow window)
        {
            MainWindow = window;

            if (XMLFileWatcher != null)
            {
                XMLFileWatcher.Dispose();
            }

            XMLFileWatcher = new FileSystemWatcher();
            XMLFileWatcher.Filter = Path.GetFileName(XMLfileToWatch);
            XMLFileWatcher.Path = Path.GetDirectoryName(XMLfileToWatch);

            XMLFileWatcher.Created += new FileSystemEventHandler(HandleXMLFileAdded);
            XMLFileWatcher.Changed += new FileSystemEventHandler(HandleXMLFileAdded);
            XMLFileWatcher.EnableRaisingEvents = true;
            MainWindow.LogMessage("Directory monitor started on " + XMLFileWatcher.Path);

            if (JPEGFileWatcher != null)
            {
                JPEGFileWatcher.Dispose();
            }

            if(!string.IsNullOrWhiteSpace(JPEGDirectoryToWatch))
            {
                JPEGFileWatcher = new FileSystemWatcher();
                JPEGFileWatcher.Filter = "*.jpg";
                JPEGFileWatcher.Path = JPEGDirectoryToWatch;
                JPEGFileWatcher.IncludeSubdirectories = true;

                JPEGFileWatcher.Created += new FileSystemEventHandler(HandleJPEGFileAdded);
                JPEGFileWatcher.EnableRaisingEvents = true;
                MainWindow.LogMessage("Directory monitor started on " + JPEGFileWatcher.Path);
            }
        }

        internal static void Stop()
        {
            if (XMLFileWatcher != null)
            {
                XMLFileWatcher.Dispose();
            }
            if (JPEGFileWatcher != null)
            {
                JPEGFileWatcher.Dispose();
            }
            MainWindow.LogMessage("Directory monitor stopped");
        }

        private static void HandleJPEGFileAdded(object source, FileSystemEventArgs e)
        {
            MainWindow.LogMessage("New JPEG file added at " + e.FullPath);

            //It takes a bit after creation before the file is sendable, sleep a few seconds
            XMLFileWatcher.EnableRaisingEvents = false;
            Thread.Sleep(5000);
            XMLFileWatcher.EnableRaisingEvents = true;
            try
            {
                MainWindow.LogMessage("Uploading JPEG image");
                string results = WebManager.uploadWebcamImage(e.FullPath);
                MainWindow.LogMessage(results);
            }
            catch (Exception ex)
            {
                MainWindow.LogError("Unable to upload JPEG image", ex);
            }

            try
            {
                File.Delete(e.FullPath);
            }
            catch (Exception ex)
            {
                MainWindow.LogError("Unable to delete the file after upload", ex);
            }
        }

        private static void HandleXMLFileAdded(object source, FileSystemEventArgs e)
        {
            MainWindow.LogMessage("New XML file added at " + e.FullPath);

            //File watcher will trigger multiple events. Sleep for a bit.
            XMLFileWatcher.EnableRaisingEvents = false;
            Thread.Sleep(1000);
            XMLFileWatcher.EnableRaisingEvents = true;

            Observation observation = new Observation();
            bool save = true;
            try
            {
                observation = XMLParser.getObservationFromXML(e.FullPath);
            }
            catch(Exception ex)
            {
                MainWindow.LogError("Unable to parse observation.", ex);

                //Copy the XML file with a timestamp so we can look at it later
                try
                {
                    string directory = Path.GetDirectoryName(e.FullPath) + @"\";
                    File.Copy(e.FullPath, directory + "Parse Error - " + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".xml");
                }
                catch(Exception ex2)
                {
                    MainWindow.LogError("Unable to copy error XML file for review", ex2);
                }
                save = false;
            }
            if(save)
            {
                try
                {
                    MainWindow.LogMessage("Sending current observation to server");
                    string results = WebManager.SendCurrentObservationToServer(observation);
                    MainWindow.LogMessage(results);
                }
                catch (Exception ex)
                {
                    MainWindow.LogError("Unable to send the observation to the server", ex);
                }

                if (observation.SaveToDatabase)
                {
                    try
                    {
                        MainWindow.LogMessage("Saving observation to the database");
                        DatabaseManager.LogObservation(observation);
                        MainWindow.LogMessage("Observation saved to the database");
                    }
                    catch (Exception ex)
                    {
                        MainWindow.LogError("Unable to save the observation to the database", ex, false, false);
                    }

                    try
                    {
                        MainWindow.LogMessage("Uploading today's data to the server");
                        string results = WebManager.UploadTodayObservations();
                        MainWindow.LogMessage(results);
                    }
                    catch (Exception ex)
                    {
                        MainWindow.LogError("Unable to save the observation to the database", ex, false, false);
                    }
                }

                try
                {
                    CheckHighsAndLows(observation);
                }
                catch(Exception ex)
                {
                    MainWindow.LogError("Unable to update highs and lows", ex);
                }

                CheckPeriodicUploads(observation.ObservationDate);
            }

        }

        private static void CheckHighsAndLows(Observation observation)
        {
            MainWindow.LogMessage("Checking Highs and Lows");
            HighLowObservation highsAndLows = new HighLowObservation();
            bool exists = DatabaseManager.GetHighsAndLows(observation.ObservationDate, ref highsAndLows);
            if(exists && highsAndLows.CompareToObservation(observation))
            {
                MainWindow.LogMessage("New high/low values found, updating");
                DatabaseManager.UpdateHighsAndLows(highsAndLows);
                MainWindow.LogMessage("New high/low update complete");
            }
            else if( !exists )
            {
                MainWindow.LogMessage("No high/low entry found, creating");
                highsAndLows.Initialize(observation);
                DatabaseManager.CreateHighsAndLows(highsAndLows);
                MainWindow.LogMessage("New high/low entry created");
            }
            else
            {
                MainWindow.LogMessage("No changes to highs and lows detected");
            }
        }

        private static void CheckPeriodicUploads(DateTime observationDate)
        {
            DateTime thisObservationDate = DateTime.Parse(observationDate.ToString("yyyy-MM-dd"));
            if(thisObservationDate > LastDailyUpload)
            {
                bool uploadSuccess = true;
                try
                {
                    MainWindow.LogMessage("Sending yesterday's observations to server");
                    string results = WebManager.UploadYesterdayObservations();
                    MainWindow.LogMessage(results);
                }
                catch (Exception ex)
                {
                    MainWindow.LogError("Unable to upload yesterday's observations", ex);
                    uploadSuccess = false;
                }

                try
                {
                    MainWindow.LogMessage("Sending highs and lows to server");
                    string results = WebManager.UploadHighsAndLows();
                    MainWindow.LogMessage(results);
                }
                catch (Exception ex)
                {
                    MainWindow.LogError("Unable to upload highs and lows", ex);
                    uploadSuccess = false;
                }

                if(uploadSuccess)
                {
                    LastDailyUpload = thisObservationDate;
                }
            }
        }
    }
}
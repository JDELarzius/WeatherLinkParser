using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Weatherlink_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public const int MAX_OUTPUT_CHARACTERS = 20000;
        public const int MAX_LOG_SIZE = 1000000;
        public const string SETTINGS_FILE_NAME = "wlparsersettings.xml";
        private string Log;
        public MainWindow()
        {
            InitializeComponent();
            bool start = RestoreSettings();
            if(start)
            {
                StartButtonClick(null, null);
            }
        }

        private void TargetXMLFileButtonClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML File (*.xml)|*.xml";
            
            if(openFileDialog.ShowDialog() == true)
            {
                XMLLabel.Text = openFileDialog.FileName;
                LogMessage("Selected " + XMLLabel.Text);
            }
        }

        private void TargetJPEGDirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openDirectoryDialog = new FolderBrowserDialog();

            if (openDirectoryDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                JPEGLabel.Text = openDirectoryDialog.SelectedPath;
                LogMessage("Selected " + JPEGLabel.Text);
            }
        }

        private void TargetDatabaseButtonClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "DB File (*.db)|*.db";

            if (openFileDialog.ShowDialog() == true)
            {
                DatabaseLabel.Text = openFileDialog.FileName;
                LogMessage("Selected " + DatabaseLabel.Text);
                DatabaseManager.SetDatabasePath(DatabaseLabel.Text);
            }
        }
        private void CreateDatabaseButtonClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "WeatherData.db";
            saveFileDialog.Filter = "DB File (*.db)|*.db";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    DatabaseManager.CreateDatabase(saveFileDialog.FileName);
                    DatabaseManager.SetDatabasePath(saveFileDialog.FileName);
                    DatabaseLabel.Text = saveFileDialog.FileName;
                    LogMessage("Database successfully created");
                }
                catch(Exception ex)
                {
                    LogError("Unable to create database", ex);
                }
            }
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            LogMessage("Setting up FTP Parameters");
            bool ready = false;
            try
            {
                int uploadFrequency = int.Parse(UploadFrequency.Text);
                XMLParser.SetUploadFrequency(uploadFrequency);
                WebManager.SetupFTP(Username.Text, Password.Password, FTPServer.Text + "/" + RemotePath.Text + "/");
                ready = true;
            }
            catch (Exception ex)
            {
                LogError("Unable to set up the FTP parameters", ex);
            }

            if(ready)
            {
                LogMessage("Starting directory monitor");
                try
                {
                    string jpegDirectory = "";
                    if(Directory.Exists(JPEGLabel.Text))
                    {
                        jpegDirectory = JPEGLabel.Text;
                    }
                    DirectoryMonitor.Start(XMLLabel.Text, jpegDirectory, this);
                }
                catch (Exception ex)
                {
                    LogError("Unable to start directory monitor", ex);
                }
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            DirectoryMonitor.Stop();
        }
        private void TestParseXMLClick(object sender, RoutedEventArgs e)
        {
            if(null != GetObservation())
            {
                LogMessage("Successfully parsed the loaded XML file.");
            }
        }
        private void TestDBWriteClick(object sender, RoutedEventArgs e)
        {
            Observation observation = GetObservation();
            if (null != observation)
            {
                try
                {
                    DatabaseManager.LogObservation(observation);
                    LogMessage("Successfully wrote the DB entry.");
                }
                catch(Exception ex)
                {
                    LogError("Failed to log the observation.", ex);
                }
            }
        }
        private void TestCreateJSONClick(object sender, RoutedEventArgs e)
        {
            Observation observation = GetObservation();
            if(null != observation)
            {
                try
                {
                    string json = WebManager.GenerateJSONCurrentConditions(observation);
                    LogMessage("JSON generated successfully");
                    System.Windows.MessageBox.Show(json, "Generated JSON");
                    System.Windows.Clipboard.SetText(json);
                }
                catch(Exception ex)
                {
                    LogError("Failed to generate JSON.", ex);
                }
            }
        }
        private void ForceUploadTodayClick(object sender, RoutedEventArgs e)
        {
            LogMessage("Force uploading today's observations");
            try
            {
                LogMessage(WebManager.UploadTodayObservations());
            }
            catch(Exception ex)
            {
                LogError("Unable to upload today's observations", ex);
            }
            
        }

        private void ForceUploadDailyFiles(object sender, RoutedEventArgs e)
        {
            try
            {
                LogMessage("Sending current observation to server");
                string results = WebManager.UploadYesterdayObservations();
                LogMessage(results);
            }
            catch (Exception ex)
            {
                LogError("Unable to upload yesterday's observations", ex);
            }

            try
            {
                LogMessage("Sending current observation to server");
                string results = WebManager.UploadHighsAndLows();
                LogMessage(results);
            }
            catch (Exception ex)
            {
                LogError("Unable to upload highs and lows", ex);
            }
        }

        private Observation GetObservation()
        {
            string file = XMLLabel.Text;
            if (File.Exists(file))
            {
                try
                {
                    return XMLParser.getObservationFromXML(file); ;
                }
                catch (Exception ex)
                {
                    LogError("Unable to parse XML file.", ex);
                    return null;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("File not found:" + System.Environment.NewLine + file, "Invalid File");
                return null;
            }
        }
        public void LogError(string message, Exception e, bool aExit = false, bool includeStackTrace = true)
        {
            Console.WriteLine(message);
            Console.WriteLine(e.Message);
            if(includeStackTrace)
            {
                Console.WriteLine(e.StackTrace);
            }

            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                string innerExceptionText = "";
                if(e.InnerException != null)
                {
                    innerExceptionText = Environment.NewLine + "INNER EXCEPTION: " + e.InnerException.Message;
                }
                string newText = DateTime.Now.ToString("HH:mm:ss:fff") + ": " + message +
                Environment.NewLine + "\t" + "EXCEPTION: " + e.Message + innerExceptionText + Environment.NewLine;
                if (includeStackTrace)
                {
                    newText += e.StackTrace + Environment.NewLine;
                }
                AddToLog(newText);
                newText += Output.Text.Substring(0, Math.Min(Output.Text.Length, MAX_OUTPUT_CHARACTERS));

                Output.Text = newText;
            }));
        }

        public void LogMessage(string message)
        {
            Console.WriteLine(message);
            AddToLog(message);

            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                Output.Text = DateTime.Now.ToString("HH:mm:ss:fff") + ": " + message +
                Environment.NewLine + Output.Text.Substring(0, Math.Min(Output.Text.Length, MAX_OUTPUT_CHARACTERS));
            }));
        }

        private void AddToLog(string text)
        {
            Log += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + ": " + text + Environment.NewLine;
            if(Log.Length > MAX_LOG_SIZE)
            {
                //Truncate the first half of the log
                int halfway = Log.Substring(MAX_LOG_SIZE / 2).IndexOf(Environment.NewLine) + MAX_LOG_SIZE / 2;
                Log = Log.Substring(halfway + 1);
            }
        }

        private void ExportLog(object sender, RoutedEventArgs e)
        {
            LogMessage("Exporting log");
            try
            {
                File.WriteAllText("Weatherlink Parser Log.txt", Log);
                LogMessage("Log Exported");
            }
            catch(Exception ex)
            {
                LogError("Unable to export log", ex);
            }
            
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            string errors = ValidateSettings();
            if(string.IsNullOrWhiteSpace(errors))
            {
                try
                {
                    Settings settings;
                    settings.targetXML = XMLLabel.Text;
                    settings.targetJPEGDirectory = JPEGLabel.Text;
                    settings.targetDB = DatabaseLabel.Text;
                    settings.ftpServer = FTPServer.Text;
                    settings.username = Username.Text;
                    settings.password = EncryptPassword(Password.Password);
                    settings.remotePath = RemotePath.Text;
                    settings.uploadFrequency = int.Parse(UploadFrequency.Text);
                    XMLParser.WriteSettingsXML(settings, SETTINGS_FILE_NAME);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Unable to save settings!" + Environment.NewLine + ex.Message);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Cannot save settings because " + Environment.NewLine + errors);
            }
        }

        private bool RestoreSettings()
        {
            bool readyToStart = false;
            if (File.Exists(SETTINGS_FILE_NAME))
            {
                LogMessage("Attempting to load settings from file");
                
                try
                {
                    Settings settings = XMLParser.GetSettings(SETTINGS_FILE_NAME);
                    string errors = ValidateSettings(settings);
                    if (string.IsNullOrWhiteSpace(errors))
                    {
                        XMLLabel.Text = settings.targetXML;
                        JPEGLabel.Text = settings.targetJPEGDirectory;
                        DatabaseLabel.Text = settings.targetDB;
                        DatabaseManager.SetDatabasePath(DatabaseLabel.Text);
                        FTPServer.Text = settings.ftpServer;
                        Username.Text = settings.username;
                        Password.Password = DecryptPassword(settings.password);
                        RemotePath.Text = settings.remotePath;
                        UploadFrequency.Text = settings.uploadFrequency.ToString();
                        LogMessage("Settings loaded successfully");

                        readyToStart = true;
                    }
                    else
                    {
                        LogMessage("Unable to restore settings:" + Environment.NewLine + errors);
                    }
                }
                catch (Exception e)
                {
                    LogError("Unable to load settings XML.", e);
                }
            }
            else
            {
                LogMessage("Settings file not found");
            }
            
            return readyToStart;
        }

        private string ValidateSettings(Settings settings)
        {
            string errors = null;

            errors += string.IsNullOrWhiteSpace(settings.targetXML) ? "XML File Name Invalid" + Environment.NewLine : "";
            errors += (string.IsNullOrWhiteSpace(settings.targetDB) || !File.Exists(settings.targetDB)) ? "Database File Name Invalid" + Environment.NewLine : "";
            errors += string.IsNullOrWhiteSpace(settings.ftpServer) ? "FTP Server Name Invalid" + Environment.NewLine : "";
            errors += string.IsNullOrWhiteSpace(settings.username) ? "Username Invalid" + Environment.NewLine : "";
            errors += string.IsNullOrWhiteSpace(settings.password) ? "Password Invalid" + Environment.NewLine : "";
            errors += string.IsNullOrWhiteSpace(settings.remotePath) ? "Remote Path Invalid" + Environment.NewLine : "";
            errors += settings.uploadFrequency < 5 ? "Upload frequency must be > 5 minutes" + Environment.NewLine : "";

            return errors;
        }

        private string ValidateSettings()
        {
            Settings settings;
            settings.targetXML = XMLLabel.Text;
            settings.targetJPEGDirectory = JPEGLabel.Text;
            settings.targetDB = DatabaseLabel.Text;
            settings.ftpServer = FTPServer.Text;
            settings.username = Username.Text;
            settings.password = Password.Password;
            settings.remotePath = RemotePath.Text;
            try
            {
                settings.uploadFrequency = int.Parse(UploadFrequency.Text);
            }
            catch
            {
                UploadFrequency.Text = "15";
                System.Windows.MessageBox.Show("The upload frequency is invalid and has been set to 15 minutes", "Invalid Upload Frequency");
                settings.uploadFrequency = 15;
            }
            return ValidateSettings(settings);
        }

        private string EncryptPassword(string password)
        {
            //"encrypt" the password
            String pw = Password.Password;
            char[] array = pw.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (char)((i + 1) * 100 + array[i]);
            }

            return new string(array);
        }

        private string DecryptPassword(string encryptedPassword)
        {
            char[] array = encryptedPassword.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (char)(array[i] - (i + 1) * 100);
            }

            return new String(array); ;
        }

        
    }
}

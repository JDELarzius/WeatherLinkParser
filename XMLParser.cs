using System;
using System.IO;
using System.Xml;

namespace Weatherlink_Parser
{
    internal class XMLParser
    {
        private static int UploadFrequency;
        
        internal static Observation getObservationFromXML(string file)
        {
            Observation observation = new Observation();
            XmlTextReader reader = new XmlTextReader(file);

            string date = "";
            string time = "";

            try
            {
                while (reader.Read())
                {
                    switch (reader.Name)
                    {
                        case "uploadDate":
                            date = reader.ReadElementContentAsString();
                            break;

                        case "uploadTime":
                            time = reader.ReadElementContentAsString();
                            break;

                        case "temp":
                            observation.Temperature = reader.ReadElementContentAsFloat();
                            break;

                        case "hiTemp":
                            observation.HiTemperature = reader.ReadElementContentAsFloat();
                            break;

                        case "hiTempTime":
                            observation.HiTemperatureTime = ParseWeatherLinkDate(date, reader.ReadElementContentAsString());
                            break;

                        case "lowTemp":
                            observation.LowTemperature = reader.ReadElementContentAsFloat();
                            break;

                        case "lowTempTime":
                            observation.LowTemperatureTime = ParseWeatherLinkDate(date, reader.ReadElementContentAsString());
                            break;

                        case "windChill":
                            observation.WindChill = reader.ReadElementContentAsFloat();
                            break;

                        case "lowWindChill":
                            observation.LowWindChill = reader.ReadElementContentAsFloat();
                            break;

                        case "lowWindChillTime":
                            observation.LowWindChillTime = ParseWeatherLinkDate(date, reader.ReadElementContentAsString());
                            break;

                        case "heatIndex":
                            observation.HeatIndex = reader.ReadElementContentAsFloat();
                            break;

                        case "hiHeatIndex":
                            observation.HiHeatIndex = reader.ReadElementContentAsFloat();
                            break;

                        case "hiHeatIndexTime":
                            observation.HiHeatIndexTime = ParseWeatherLinkDate(date, reader.ReadElementContentAsString());
                            break;

                        case "windSpeed":
                            observation.WindSpeed = reader.ReadElementContentAsFloat();
                            break;

                        case "wind10MinAvg":
                            //for some odd reason, weatherlink decided to use ---- for a full 10 minutes of zero wind...
                            string avgwind = reader.ReadElementContentAsString();
                            try { observation.TenMinAvgWindSpeed = float.Parse(avgwind);  }
                            catch { observation.TenMinAvgWindSpeed = 0; }
                            break;

                        case "hiWindSpeed":
                            observation.HiWindSpeed = reader.ReadElementContentAsFloat();
                            break;

                        case "hiWindSpeedTime":
                            string hiWindSpeedTime = reader.ReadElementContentAsString();
                            if (hiWindSpeedTime != "----")
                            {
                                observation.HiWindSpeedTime = ParseWeatherLinkDate(date, hiWindSpeedTime);
                            }
                            break;

                        case "windDirectionDeg":
                            observation.WindDirection = reader.ReadElementContentAsInt();
                            break;

                        case "windDirectionSector":
                            observation.WindDirectionLetter = reader.ReadElementContentAsString();
                            break;

                        case "humidity":
                            observation.Humidity = reader.ReadElementContentAsFloat();
                            break;

                        case "dewPoint":
                            observation.DewPoint = reader.ReadElementContentAsFloat();
                            break;

                        case "pressure":
                            observation.Pressure = reader.ReadElementContentAsFloat();
                            break;

                        case "todayRain":
                            observation.TodayRain = reader.ReadElementContentAsFloat();
                            break;

                        case "stormRain":
                            observation.StormRain = reader.ReadElementContentAsFloat();
                            break;

                        case "monthRain":
                            observation.MonthRain = reader.ReadElementContentAsFloat();
                            break;

                        case "yearRain":
                            observation.YearRain = reader.ReadElementContentAsFloat();
                            break;

                        case "rainRate":
                            observation.RainRate = reader.ReadElementContentAsFloat();
                            break;

                        case "hiRainRate":
                            observation.HiRainRate = reader.ReadElementContentAsFloat();
                            break;

                        case "hiRainRateTime":
                            string rainRateTime = reader.ReadElementContentAsString();
                            if (rainRateTime != "----")
                            {
                                observation.HiRainRateTime = ParseWeatherLinkDate(date, rainRateTime);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                reader.Close();
                throw new Exception("XML Parse Failure", e);
            }
            
            reader.Close();

            if (!string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(time))
            {
                observation.ObservationDate = ParseWeatherLinkDate(date, time);
                observation.SaveToDatabase = GetShouldSaveToDB(observation.ObservationDate);
            }

            return observation;
        }

        internal static void WriteSettingsXML(Settings settings, string filePath)
        {
            if(string.IsNullOrWhiteSpace(settings.targetDB) || !File.Exists(settings.targetDB) || string.IsNullOrWhiteSpace(settings.targetXML) || !File.Exists(settings.targetXML))
            {
                throw new Exception("Invalid settings");
            }

            XmlWriter writer = XmlWriter.Create(filePath);
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");
            writer.WriteStartElement("XMLFile");
            writer.WriteString(settings.targetXML);
            writer.WriteEndElement();
            writer.WriteStartElement("JPEGFolder");
            writer.WriteString(settings.targetJPEGDirectory);
            writer.WriteEndElement();
            writer.WriteStartElement("DBFile");
            writer.WriteString(settings.targetDB);
            writer.WriteEndElement();
            writer.WriteStartElement("FTPServer");
            writer.WriteString(settings.ftpServer);
            writer.WriteEndElement();
            writer.WriteStartElement("Username");
            writer.WriteString(settings.username);
            writer.WriteEndElement();
            writer.WriteStartElement("Password");
            writer.WriteString(settings.password);
            writer.WriteEndElement();
            writer.WriteStartElement("RemotePath");
            writer.WriteString(settings.remotePath);
            writer.WriteEndElement();
            writer.WriteStartElement("UploadFrequency");
            writer.WriteValue(settings.uploadFrequency);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
        internal static Settings GetSettings(string filePath)
        {
            Settings settings;
            settings.targetDB = "";
            settings.targetXML = "";
            settings.targetJPEGDirectory = "";
            settings.ftpServer = "";
            settings.username = "";
            settings.password = "";
            settings.remotePath = "";
            settings.uploadFrequency = 15;

            XmlTextReader reader = new XmlTextReader(filePath);
            try
            {
                while (reader.Read())
                {
                    if(reader.NodeType == XmlNodeType.Element && reader.Name != "Settings")
                    {
                        string elementName = reader.Name;
                        reader.Read();
                        if(reader.NodeType != XmlNodeType.Text)
                        {
                            if(reader.NodeType == XmlNodeType.EndElement)
                            {
                                //In this case, we didn't have any setting saved for this particular element, so skip it
                                continue;
                            }
                            throw new Exception("XML is not formed correctly.");
                        }

                        switch (elementName)
                        {
                            case "XMLFile":
                                settings.targetXML = reader.Value;
                                break;

                            case "JPEGFolder":
                                settings.targetJPEGDirectory = reader.Value;
                                break;

                            case "DBFile":
                                settings.targetDB = reader.Value;
                                break;

                            case "FTPServer":
                                settings.ftpServer = reader.Value;
                                break;

                            case "Username":
                                settings.username = reader.Value;
                                break;

                            case "Password":
                                settings.password = reader.Value;
                                break;

                            case "RemotePath":
                                settings.remotePath = reader.Value;
                                break;

                            case "UploadFrequency:":
                                settings.uploadFrequency = int.Parse(reader.Value);
                                break;

                            default:
                                break;

                        }
                    }
                }
            }
            catch (Exception e)
            {
                reader.Close();
                throw new Exception("Failed to load settings", e);
            }
            reader.Close();

            return settings;
        }

        internal static void SetUploadFrequency(int uploadFrequency)
        {
            UploadFrequency = uploadFrequency;
        }

        private static DateTime ParseWeatherLinkDate(string stringDate, string stringTime)
        {
            if(string.IsNullOrWhiteSpace(stringDate))
            {
                throw new Exception("Invalid date string");
            }
            if (string.IsNullOrWhiteSpace(stringTime))
            {
                throw new Exception("Invalid time string");
            }

            string yearString = "20" + stringDate.Substring(6, 2);
            string monthString = stringDate.Substring(0, 2);
            string dayString = stringDate.Substring(3, 2);

            int hours = int.Parse(stringTime.Substring(0, 2).Trim());
            if(stringTime.Substring(5,1) == "p" &&  hours != 12)
            {
                hours = hours == 12 ? 0 : hours + 12;
            }
            else if(stringTime.Substring(5, 1) == "a" && hours == 12)
            {
                hours = 0;
            }
            string hoursString = hours.ToString().Length == 1 ? "0" + hours.ToString() : hours.ToString();
            string minutesString = stringTime.Substring(3, 2);

            string conversionString = yearString + "-" + monthString + "-" + dayString + "T" + hoursString + ":" + minutesString + ":00";
            DateTime date = DateTime.Parse(conversionString);
            return date;
        }

        private static bool GetShouldSaveToDB(DateTime observationTime)
        {
            bool save = false;

            if(observationTime.Minute % UploadFrequency == 0)
            {
                save = true;
            }

            return save;
        }
    }
}
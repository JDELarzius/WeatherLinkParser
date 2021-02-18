using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Weatherlink_Parser
{
    internal static class DatabaseManager
    {
        private static string databasePath = "";
        public static void CreateDatabase(string filePath)
        {
            //Delete the file if it exists
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            SqliteConnection connection = new SqliteConnection("Data Source=" + filePath);
            connection.Open();

            SqliteCommand createTable = connection.CreateCommand();
            createTable.CommandText = "CREATE TABLE Observations (" +
                "ObservationId integer primary key," +
                "ObservationDate string," +
                "Temperature real," +
                "WindChill real," +
                "HeatIndex real," +
                "WindSpeed real," +
                "TenMinAvgWindSpeed real," +
                "WindDirection integer," +
                "WindDirectionLetter string," + 
                "Humidity real," +
                "DewPoint real," +
                "Pressure real," +
                "TodayRain real," +
                "StormRain real," +
                "MonthRain real," +
                "YearRain real," +
                "RainRate real" +
                ")";

            createTable.ExecuteNonQuery();

            SqliteCommand addIndex = connection.CreateCommand();
            addIndex.CommandText = "CREATE UNIQUE INDEX idx_Observation_date ON Observations(ObservationDate)";
            addIndex.ExecuteNonQuery();

            SqliteCommand createHighsLowsTable = connection.CreateCommand();
            createHighsLowsTable.CommandText = "CREATE TABLE HighsAndLows (" +
                "EntryID integer primary key," +
                "ObservationDate string," +
                "HighTemp real," +
                "LowTemp real," +
                "HighHeatIndex real," +
                "LowHeatIndex real," +
                "HighWindChill real," +
                "LowWindChill real," +
                "HighWindSpeed real," +
                "HighHumidity real," +
                "LowHumidity real," +
                "HighDewPoint real," +
                "LowDewPoint real," +
                "HighPressure real," +
                "LowPressure real," +
                "DayRain real," +
                "HighRainRate real" +
                ")";

            createHighsLowsTable.ExecuteNonQuery();

            SqliteCommand addIndex2 = connection.CreateCommand();
            addIndex2.CommandText = "CREATE UNIQUE INDEX idx_Observation_Date_Highs_Lows ON HighsAndLows(ObservationDate)";
            addIndex2.ExecuteNonQuery();

            connection.Close();
        }

        public static void LogObservation(Observation observation)
        {
            SqliteConnection connection = createConnection();
            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "INSERT INTO Observations (" +
                "ObservationDate," +
                "Temperature," +
                "WindChill," +
                "HeatIndex," +
                "WindSpeed," +
                "TenMinAvgWindSpeed," +
                "WindDirection," +
                "WindDirectionLetter," +
                "Humidity," +
                "DewPoint," +
                "Pressure," +
                "TodayRain," +
                "StormRain," +
                "MonthRain," +
                "YearRain," +
                "RainRate" +
                ") VALUES(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16)";

            command.Prepare();

            command.Parameters.AddWithValue("@1", observation.ObservationDate.ToString("yyyy-MM-dd HH:mm"));
            command.Parameters.AddWithValue("@2",observation.Temperature.ToString());
            command.Parameters.AddWithValue("@3",observation.WindChill.ToString());
            command.Parameters.AddWithValue("@4",observation.HeatIndex.ToString());
            command.Parameters.AddWithValue("@5",observation.WindSpeed.ToString());
            command.Parameters.AddWithValue("@6",observation.TenMinAvgWindSpeed.ToString());
            command.Parameters.AddWithValue("@7",observation.WindDirection.ToString());
            command.Parameters.AddWithValue("@8",observation.WindDirectionLetter);
            command.Parameters.AddWithValue("@9",observation.Humidity.ToString());
            command.Parameters.AddWithValue("@10",observation.DewPoint.ToString());
            command.Parameters.AddWithValue("@11",observation.Pressure.ToString());
            command.Parameters.AddWithValue("@12",observation.TodayRain.ToString());
            command.Parameters.AddWithValue("@13",observation.StormRain.ToString());
            command.Parameters.AddWithValue("@14",observation.MonthRain.ToString());
            command.Parameters.AddWithValue("@15",observation.YearRain.ToString());
            command.Parameters.AddWithValue("@16",observation.RainRate.ToString());

            command.ExecuteNonQuery();
            connection.Close();
        }

        internal static void CreateHighsAndLows(HighLowObservation highsAndLows)
        {
            SqliteConnection connection = createConnection();
            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "INSERT INTO HighsAndLows (" +
                "ObservationDate," +
                "HighTemp," +
                "LowTemp," +
                "HighHeatIndex," +
                "LowHeatIndex," +
                "HighWindChill," +
                "LowWindChill," +
                "HighWindSpeed," +
                "HighHumidity," +
                "LowHumidity," +
                "HighDewPoint," +
                "LowDewPoint," +
                "HighPressure," +
                "LowPressure," +
                "DayRain," +
                "HighRainRate " +
                ") VALUES(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16)";

            command.Prepare();

            command.Parameters.AddWithValue("@1", highsAndLows.ObservationDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@2", highsAndLows.HighTemp.ToString());
            command.Parameters.AddWithValue("@3", highsAndLows.LowTemp.ToString());
            command.Parameters.AddWithValue("@4", highsAndLows.HighHeatIndex.ToString());
            command.Parameters.AddWithValue("@5", highsAndLows.LowHeatIndex.ToString());
            command.Parameters.AddWithValue("@6", highsAndLows.HighWindChill.ToString());
            command.Parameters.AddWithValue("@7", highsAndLows.LowWindChill.ToString());
            command.Parameters.AddWithValue("@8", highsAndLows.HighWindSpeed.ToString());
            command.Parameters.AddWithValue("@9", highsAndLows.HighHumidity.ToString());
            command.Parameters.AddWithValue("@10", highsAndLows.LowHumidity.ToString());
            command.Parameters.AddWithValue("@11", highsAndLows.HighDewPoint.ToString());
            command.Parameters.AddWithValue("@12", highsAndLows.LowDewPoint.ToString());
            command.Parameters.AddWithValue("@13", highsAndLows.HighPressure.ToString());
            command.Parameters.AddWithValue("@14", highsAndLows.LowPressure.ToString());
            command.Parameters.AddWithValue("@15", highsAndLows.DayRain.ToString());
            command.Parameters.AddWithValue("@16", highsAndLows.HighRainRate.ToString());

            command.ExecuteNonQuery();
            connection.Close();
        }

        internal static void UpdateHighsAndLows(HighLowObservation highsAndLows)
        {
            SqliteConnection connection = createConnection();
            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE HighsAndLows SET " +
                "HighTemp = @1," +
                "LowTemp = @2," +
                "HighHeatIndex = @3," +
                "LowHeatIndex = @4," +
                "HighWindChill = @5," +
                "LowWindChill = @6," +
                "HighWindSpeed = @7," +
                "HighHumidity = @8," +
                "LowHumidity = @9," +
                "HighDewPoint = @10," +
                "LowDewPoint = @11," +
                "HighPressure = @12," +
                "LowPressure = @13," +
                "DayRain = @14," +
                "HighRainRate = @15 " +
                "WHERE Date(ObservationDate) = Date('" + highsAndLows.ObservationDate.ToString("yyyy-MM-dd") + "')";

            command.Prepare();

            command.Parameters.AddWithValue("@1", highsAndLows.HighTemp.ToString());
            command.Parameters.AddWithValue("@2", highsAndLows.LowTemp.ToString());
            command.Parameters.AddWithValue("@3", highsAndLows.HighHeatIndex.ToString());
            command.Parameters.AddWithValue("@4", highsAndLows.LowHeatIndex.ToString());
            command.Parameters.AddWithValue("@5", highsAndLows.HighWindChill.ToString());
            command.Parameters.AddWithValue("@6", highsAndLows.LowWindChill.ToString());
            command.Parameters.AddWithValue("@7", highsAndLows.HighWindSpeed.ToString());
            command.Parameters.AddWithValue("@8", highsAndLows.HighHumidity.ToString());
            command.Parameters.AddWithValue("@9", highsAndLows.LowHumidity.ToString());
            command.Parameters.AddWithValue("@10", highsAndLows.HighDewPoint.ToString());
            command.Parameters.AddWithValue("@11", highsAndLows.LowDewPoint.ToString());
            command.Parameters.AddWithValue("@12", highsAndLows.HighPressure.ToString());
            command.Parameters.AddWithValue("@13", highsAndLows.LowPressure.ToString());
            command.Parameters.AddWithValue("@14", highsAndLows.DayRain.ToString());
            command.Parameters.AddWithValue("@15", highsAndLows.HighRainRate.ToString());

            command.ExecuteNonQuery();
            connection.Close();
        }

        internal static bool GetHighsAndLows(DateTime observationDate, ref HighLowObservation highsAndLows)
        {
            SqliteConnection connection = createConnection();
            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT EntryID," +
                "ObservationDate," +
                "HighTemp," +
                "LowTemp," +
                "HighHeatIndex," +
                "LowHeatIndex," +
                "HighWindChill," +
                "LowWindChill," +
                "HighWindSpeed," +
                "HighHumidity," +
                "LowHumidity," +
                "HighDewPoint," +
                "LowDewPoint," +
                "HighPressure," +
                "LowPressure," +
                "DayRain," +
                "HighRainRate " +
                "FROM HighsAndLows WHERE Date(ObservationDate) = Date('" + observationDate.ToString("yyyy-MM-dd") + "')";
            SqliteDataReader reader = command.ExecuteReader();

            bool resultFound;
            if (reader.Read())
            {
                highsAndLows.ObservationDate = DateTime.Parse(reader.GetString(1));
                highsAndLows.HighTemp = reader.GetFloat(2);
                highsAndLows.LowTemp = reader.GetFloat(3);
                highsAndLows.HighHeatIndex = reader.GetFloat(4);
                highsAndLows.LowHeatIndex = reader.GetFloat(5);
                highsAndLows.HighWindChill = reader.GetFloat(6);
                highsAndLows.LowWindChill = reader.GetFloat(7);
                highsAndLows.HighWindSpeed = reader.GetFloat(8);
                highsAndLows.HighHumidity = reader.GetFloat(9);
                highsAndLows.LowHumidity = reader.GetFloat(10);
                highsAndLows.HighDewPoint = reader.GetFloat(11);
                highsAndLows.LowDewPoint = reader.GetFloat(12);
                highsAndLows.HighPressure = reader.GetFloat(13);
                highsAndLows.LowPressure = reader.GetFloat(14);
                highsAndLows.DayRain = reader.GetFloat(15);
                highsAndLows.HighRainRate = reader.GetFloat(16);

                resultFound = true;
            }
            else
            {
                resultFound = false;
            }

            connection.Close();

            return resultFound;
        }

        internal static List<HighLowObservation> GetHighsAndLows(DateTime observationStartDate, DateTime observationEndDate)
        {
            SqliteConnection connection = createConnection();
            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT EntryID," +
                "ObservationDate," +
                "HighTemp," +
                "LowTemp," +
                "HighHeatIndex," +
                "LowHeatIndex," +
                "HighWindChill," +
                "LowWindChill," +
                "HighWindSpeed," +
                "HighHumidity," +
                "LowHumidity," +
                "HighDewPoint," +
                "LowDewPoint," +
                "HighPressure," +
                "LowPressure," +
                "DayRain," +
                "HighRainRate " +
                "FROM HighsAndLows WHERE Date(ObservationDate) BETWEEN " +
                "Date('" + observationStartDate.ToString("yyyy-MM-dd") + "') AND Date('" + observationEndDate.ToString("yyyy-MM-dd") + "')";
            SqliteDataReader reader = command.ExecuteReader();

            List<HighLowObservation> results = new List<HighLowObservation>();
            while (reader.Read())
            {
                HighLowObservation highsAndLows = new HighLowObservation();
                highsAndLows.ObservationDate = DateTime.Parse(reader.GetString(1));
                highsAndLows.HighTemp = reader.GetFloat(2);
                highsAndLows.LowTemp = reader.GetFloat(3);
                highsAndLows.HighHeatIndex = reader.GetFloat(4);
                highsAndLows.LowHeatIndex = reader.GetFloat(5);
                highsAndLows.HighWindChill = reader.GetFloat(6);
                highsAndLows.LowWindChill = reader.GetFloat(7);
                highsAndLows.HighWindSpeed = reader.GetFloat(8);
                highsAndLows.HighHumidity = reader.GetFloat(9);
                highsAndLows.LowHumidity = reader.GetFloat(10);
                highsAndLows.HighDewPoint = reader.GetFloat(11);
                highsAndLows.LowDewPoint = reader.GetFloat(12);
                highsAndLows.HighPressure = reader.GetFloat(13);
                highsAndLows.LowPressure = reader.GetFloat(14);
                highsAndLows.DayRain = reader.GetFloat(15);
                highsAndLows.HighRainRate = reader.GetFloat(16);
                results.Add(highsAndLows);
            }

            connection.Close();

            return results;
        }

        public static Observation GetObservationAtTime(DateTime time)
        {
            Observation observation = new Observation();
            SqliteConnection connection = createConnection();
            SqliteCommand command = connection.CreateCommand();
            
            command.CommandText = "SELECT ObservationID," +
                "ObservationDate," +
                "Temperature," +
                "WindChill," +
                "HeatIndex," +
                "WindSpeed," +
                "TenMinAvgWindSpeed," +
                "WindDirection," +
                "WindDirectionLetter," +
                "Humidity," +
                "DewPoint," +
                "Pressure," +
                "TodayRain," +
                "StormRain," +
                "MonthRain," +
                "YearRain," +
                "RainRate " +
                "FROM Observations WHERE Date(ObservationDate) = Date('" + time.ToString("yyyy-MM-dd HH:mm") + "')";
            SqliteDataReader reader = command.ExecuteReader();

            if(reader.Read())
            {
                observation.ObservationDate = DateTime.Parse(reader.GetString(1));
                observation.Temperature = reader.GetFloat(2);
                observation.WindChill = reader.GetFloat(3);
                observation.HeatIndex = reader.GetFloat(4);
                observation.WindSpeed = reader.GetFloat(5);
                observation.TenMinAvgWindSpeed = reader.GetFloat(6);
                observation.WindDirection = reader.GetInt32(7);
                observation.WindDirectionLetter = reader.GetString(8);
                observation.Humidity = reader.GetFloat(9);
                observation.DewPoint = reader.GetFloat(10);
                observation.Pressure = reader.GetFloat(11);
                observation.TodayRain = reader.GetFloat(12);
                observation.StormRain = reader.GetFloat(13);
                observation.MonthRain = reader.GetFloat(14);
                observation.YearRain = reader.GetFloat(15);
                observation.RainRate = reader.GetFloat(16);
            }
            else
            {
                throw new Exception("No observation at the specified time");
            }

            connection.Close();

            return observation;
        }

        public static void SetDatabasePath(string path)
        {
            if(File.Exists(path) && path.Substring(path.Length - 2).ToLower() == "db")
            {
                databasePath = path;
            }
            else
            {
                databasePath = "";
                throw new Exception("No file exists at the database path");
            }
        }

        public static List<Observation> GetObservationsOnDate(DateTime date)
        {
            List<Observation> observations = new List<Observation>();
            SqliteConnection connection = createConnection();
            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT ObservationID," +
                "ObservationDate," +
                "Temperature," +
                "WindChill," +
                "HeatIndex," +
                "WindSpeed," +
                "TenMinAvgWindSpeed," +
                "WindDirection," +
                "WindDirectionLetter," +
                "Humidity," +
                "DewPoint," +
                "Pressure," +
                "TodayRain," +
                "StormRain," +
                "MonthRain," +
                "YearRain," +
                "RainRate " +
                "FROM Observations WHERE ObservationDate LIKE ('" + date.ToString("yyyy-MM-dd") + "%')";
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Observation observation = new Observation();
                observation.ObservationDate = DateTime.Parse(reader.GetString(1));
                observation.Temperature = reader.GetFloat(2);
                observation.WindChill = reader.GetFloat(3);
                observation.HeatIndex = reader.GetFloat(4);
                observation.WindSpeed = reader.GetFloat(5);
                observation.TenMinAvgWindSpeed = reader.GetFloat(6);
                observation.WindDirection = reader.GetInt32(7);
                observation.WindDirectionLetter = reader.GetString(8);
                observation.Humidity = reader.GetFloat(9);
                observation.DewPoint = reader.GetFloat(10);
                observation.Pressure = reader.GetFloat(11);
                observation.TodayRain = reader.GetFloat(12);
                observation.StormRain = reader.GetFloat(13);
                observation.MonthRain = reader.GetFloat(14);
                observation.YearRain = reader.GetFloat(15);
                observation.RainRate = reader.GetFloat(16);
                observations.Add(observation);
            }

            connection.Close();

            return observations;
        }

        private static SqliteConnection createConnection()
        {
            if(string.IsNullOrWhiteSpace(databasePath))
            {
                throw new Exception("Database path has not been set");
            }

            SqliteConnection connection = new SqliteConnection("Data Source=" + databasePath);
            connection.Open();
            return connection;
        }
    }
}
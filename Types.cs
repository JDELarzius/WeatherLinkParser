using System;

namespace Weatherlink_Parser
{
    public struct Settings
    {
        public string targetXML;
        public string targetJPEGDirectory;
        public string targetDB;
        public string ftpServer;
        public string username;
        public string password;
        public string remotePath;
        public int uploadFrequency;
    }

    public class Observation
    {
        public Observation() { SaveToDatabase = false; }

        public DateTime ObservationDate { get; set; }
        public float Temperature { get; set; }
        public float HiTemperature { get; set; }
        public DateTime HiTemperatureTime { get; set; }
        public float LowTemperature { get; set; }
        public DateTime LowTemperatureTime { get; set; }
        public float WindChill { get; set; }
        public float LowWindChill { get; set; }
        public DateTime LowWindChillTime { get; set; }
        public float HeatIndex { get; set; }
        public float HiHeatIndex { get; set; }
        public DateTime HiHeatIndexTime { get; set; }
        public float WindSpeed { get; set; }
        public float TenMinAvgWindSpeed { get; set; }
        public float HiWindSpeed { get; set; }
        public DateTime HiWindSpeedTime { get; set; }
        public int WindDirection { get; set; }
        public string WindDirectionLetter { get; set; }
        public float Humidity { get; set; }
        public float DewPoint { get; set; }
        public float Pressure { get; set; }
        public float TodayRain { get; set; }
        public float StormRain { get; set; }
        public float MonthRain { get; set; }
        public float YearRain { get; set; }
        public float RainRate { get; set; }
        public float HiRainRate { get; set; }
        public DateTime HiRainRateTime { get; set; }
        public bool SaveToDatabase { get; set; }

        public struct SerializableObservationHistorical
        {
            public string ObservationDate;
            public float Temperature;
            public float WindChill;
            public float HeatIndex;
            public float WindSpeed;
            public float TenMinAvgWindSpeed;
            public int WindDirection;
            public string WindDirectionLetter;
            public float Humidity;
            public float DewPoint;
            public float Pressure;
            public float TodayRain;
            public float StormRain;
            public float MonthRain;
            public float YearRain;
            public float RainRate;
        }

        public SerializableObservationHistorical getSerializableObservationHistorical()
        {
            SerializableObservationHistorical serializableObject = new SerializableObservationHistorical();

            serializableObject.ObservationDate = ObservationDate.ToString("yyyy-MM-dd HH:mm");
            serializableObject.Temperature = Temperature;
            serializableObject.WindChill = WindChill;
            serializableObject.HeatIndex = HeatIndex;
            serializableObject.WindSpeed = WindSpeed;
            serializableObject.TenMinAvgWindSpeed = TenMinAvgWindSpeed;
            serializableObject.WindDirection = WindDirection;
            serializableObject.WindDirectionLetter = WindDirectionLetter;
            serializableObject.Humidity = Humidity;
            serializableObject.DewPoint = DewPoint;
            serializableObject.Pressure = Pressure;
            serializableObject.TodayRain = TodayRain;
            serializableObject.StormRain = StormRain;
            serializableObject.MonthRain = MonthRain;
            serializableObject.YearRain = YearRain;
            serializableObject.RainRate = RainRate;

            return serializableObject;
        }

        public struct SerializableObservationFull
        {
            public string ObservationDate;
            public string UTCObservationDate;
            public float Temperature;
            public float HiTemperature;
            public string HiTemperatureTime;
            public float LowTemperature;
            public string LowTemperatureTime;
            public float WindChill;
            public float LowWindChill;
            public string LowWindChillTime;
            public float HeatIndex;
            public float HiHeatIndex;
            public string HiHeatIndexTime;
            public float WindSpeed;
            public float TenMinAvgWindSpeed;
            public float HiWindSpeed;
            public string HiWindSpeedTime;
            public int WindDirection;
            public string WindDirectionLetter;
            public float Humidity;
            public float DewPoint;
            public float Pressure;
            public float TodayRain;
            public float StormRain;
            public float MonthRain;
            public float YearRain;
            public float RainRate;
            public float HiRainRate;
            public string HiRainRateTime;
        }

        public SerializableObservationFull getSerializableObservationFull()
        {
            SerializableObservationFull serializableObject = new SerializableObservationFull();

            serializableObject.ObservationDate = ObservationDate.ToString("yyyy-MM-dd HH:mm");
            serializableObject.UTCObservationDate = ObservationDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm");
            serializableObject.Temperature = Temperature;
            serializableObject.HiTemperature = HiTemperature;
            serializableObject.HiTemperatureTime = HiTemperatureTime.ToString("yyyy-MM-dd HH:mm");
            serializableObject.LowTemperature = LowTemperature;
            serializableObject.LowTemperatureTime = LowTemperatureTime.ToString("yyyy-MM-dd HH:mm");
            serializableObject.WindChill = WindChill;
            serializableObject.LowWindChill = LowWindChill;
            serializableObject.LowWindChillTime = LowWindChillTime.ToString("yyyy-MM-dd HH:mm");
            serializableObject.HeatIndex = HeatIndex;
            serializableObject.HiHeatIndex = HiHeatIndex;
            serializableObject.HiHeatIndexTime = HiHeatIndexTime.ToString("yyyy-MM-dd HH:mm");
            serializableObject.WindSpeed = WindSpeed;
            serializableObject.TenMinAvgWindSpeed = TenMinAvgWindSpeed;
            serializableObject.HiWindSpeed = HiWindSpeed;
            serializableObject.HiWindSpeedTime = HiWindSpeedTime.ToString("yyyy-MM-dd HH:mm");
            serializableObject.WindDirection = WindDirection;
            serializableObject.WindDirectionLetter = WindDirectionLetter;
            serializableObject.Humidity = Humidity;
            serializableObject.DewPoint = DewPoint;
            serializableObject.Pressure = Pressure;
            serializableObject.TodayRain = TodayRain;
            serializableObject.StormRain = StormRain;
            serializableObject.MonthRain = MonthRain;
            serializableObject.YearRain = YearRain;
            serializableObject.RainRate = RainRate;
            serializableObject.HiRainRate = HiRainRate;
            if(HiRainRate != 0)
            {
                serializableObject.HiRainRateTime = HiRainRateTime.ToString("yyyy-MM-dd HH:mm");
            }

            return serializableObject;
        }

    }

    public class HighLowObservation
    {
        public HighLowObservation() { }

        public DateTime ObservationDate { get; set; }
        public float HighTemp { get; set; }
        public float LowTemp { get; set; }
        public float HighHeatIndex { get; set; }
        public float LowHeatIndex { get; set; }
        public float HighWindChill { get; set; }
        public float LowWindChill { get; set; }
        public float HighWindSpeed { get; set; }
        public float HighHumidity { get; set; }
        public float LowHumidity { get; set; }
        public float HighDewPoint { get; set; }
        public float LowDewPoint { get; set; }
        public float HighPressure { get; set; }
        public float LowPressure { get; set; }
        public float DayRain { get; set; }
        public float HighRainRate { get; set; }

        public struct SerializableHighLowSet
        {
            public string ObservationDate;
            public float HighTemp;
            public float LowTemp;
            public float HighHeatIndex;
            public float LowHeatIndex;
            public float HighWindChill;
            public float LowWindChill;
            public float HighWindSpeed;
            public float HighHumidity;
            public float LowHumidity;
            public float HighDewPoint;
            public float LowDewPoint;
            public float HighPressure;
            public float LowPressure;
            public float DayRain;
            public float HighRainRate;
        }

        public SerializableHighLowSet getSerializableHighLowSet()
        {
            SerializableHighLowSet serializableObject = new SerializableHighLowSet();
            serializableObject.ObservationDate = ObservationDate.ToString("yyyy-MM-dd");
            serializableObject.HighTemp = HighTemp;
            serializableObject.LowTemp = LowTemp;
            serializableObject.HighHeatIndex = HighHeatIndex;
            serializableObject.LowHeatIndex = LowHeatIndex;
            serializableObject.HighWindChill = HighWindChill;
            serializableObject.LowWindChill = LowWindChill;
            serializableObject.HighWindSpeed = HighWindSpeed;
            serializableObject.HighHumidity = HighHumidity;
            serializableObject.LowHumidity = LowHumidity;
            serializableObject.HighDewPoint = HighDewPoint;
            serializableObject.LowDewPoint = LowDewPoint;
            serializableObject.HighPressure = HighPressure;
            serializableObject.LowPressure = LowPressure;
            serializableObject.DayRain = DayRain;
            serializableObject.HighRainRate = HighRainRate;
            return serializableObject;
        }

        public bool CompareToObservation(Observation observation)
        {
            bool changes = false;
            
            if(observation.Temperature > HighTemp)
            {
                HighTemp = observation.Temperature;
                changes = true;
            }

            if (observation.Temperature < LowTemp)
            {
                LowTemp = observation.Temperature;
                changes = true;
            }

            if (observation.HeatIndex > HighHeatIndex)
            {
                HighHeatIndex = observation.Temperature;
                changes = true;
            }

            if (observation.HeatIndex < LowHeatIndex)
            {
                LowHeatIndex = observation.Temperature;
                changes = true;
            }

            if (observation.WindChill > HighWindChill)
            {
                HighWindChill = observation.Temperature;
                changes = true;
            }

            if (observation.WindChill < LowWindChill)
            {
                LowWindChill = observation.Temperature;
                changes = true;
            }

            if (observation.WindSpeed > HighWindSpeed)
            {
                HighWindSpeed = observation.Temperature;
                changes = true;
            }

            if (observation.Humidity > HighHumidity)
            {
                HighHumidity = observation.Temperature;
                changes = true;
            }

            if (observation.Humidity < LowHumidity)
            {
                LowHumidity = observation.Temperature;
                changes = true;
            }

            if (observation.DewPoint > HighDewPoint)
            {
                HighDewPoint = observation.Temperature;
                changes = true;
            }

            if (observation.DewPoint < LowDewPoint)
            {
                LowDewPoint = observation.Temperature;
                changes = true;
            }

            if (observation.Pressure > HighPressure)
            {
                HighPressure = observation.Temperature;
                changes = true;
            }

            if (observation.Pressure < LowPressure)
            {
                LowPressure = observation.Temperature;
                changes = true;
            }

            if (observation.TodayRain > DayRain)
            {
                DayRain = observation.Temperature;
                changes = true;
            }

            if (observation.RainRate > HighRainRate)
            {
                HighRainRate = observation.Temperature;
                changes = true;
            }

            return changes;
        }

        internal void Initialize(Observation observation)
        {
            ObservationDate = DateTime.Parse(observation.ObservationDate.ToString("yyyy-MM-dd"));
            HighTemp = observation.Temperature;
            LowTemp = observation.Temperature;
            HighHeatIndex = observation.HeatIndex;
            LowHeatIndex = observation.HeatIndex;
            HighWindChill = observation.WindChill;
            LowWindChill = observation.WindChill;
            HighWindSpeed = observation.WindSpeed;
            HighHumidity = observation.Humidity;
            LowHumidity = observation.Humidity;
            HighDewPoint = observation.DewPoint;
            LowDewPoint = observation.DewPoint;
            HighPressure = observation.Pressure;
            LowPressure = observation.Pressure;
            DayRain = observation.TodayRain;
            HighRainRate = observation.RainRate;
        }
    }
}
 
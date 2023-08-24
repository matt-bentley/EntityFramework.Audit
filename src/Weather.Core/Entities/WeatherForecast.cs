using System;
using Weather.Core.Entities.Abstract;

namespace Weather.Core.Entities
{
    public sealed class WeatherForecast : Entity
    {
        public WeatherForecast(Guid id, DateTime date, int temperatureC, string summary, DateTime modifiedDate) : base(id)
        {
            Date = date;
            TemperatureC = temperatureC;
            Summary = summary;
            ModifiedDate = modifiedDate;
        }

        public static WeatherForecast Create(int temperatureC, string summary)
        {
            var now = DateTime.UtcNow;
            return new WeatherForecast(Guid.NewGuid(), now, temperatureC, summary, now);
        }

        public DateTime Date { get; private set; }
        public int TemperatureC { get; private set; }
        public string Summary { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        public void Update(int temperatureC, string summary)
        {
            TemperatureC = temperatureC;
            Summary = summary;
            ModifiedDate = DateTime.UtcNow;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.Filters.Models
{
    public class Stopwatch
    {
        public string Action { get; set;}
        public string Route { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}

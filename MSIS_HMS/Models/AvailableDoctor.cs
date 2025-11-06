using System;
namespace MSIS_HMS.Models
{
    public class AvailableDoctor
    {
        public AvailableDoctor()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int PatientInQueue { get; set; }
        public string EstWaitingTime { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
    }
}

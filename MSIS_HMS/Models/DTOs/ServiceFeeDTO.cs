using System;
namespace MSIS_HMS.Models.DTOs
{
    public class ServiceFeeDTO
    {
        public ServiceFeeDTO()
        {
        }

        public int LabResultId { get; set; }
        public int LabPersonId { get; set; }
        public int Type { get; set; }
        public string LabResultNo { get; set; }
        public int LabTestId { get; set; }
        public string LabTestName { get; set; }
    }
}

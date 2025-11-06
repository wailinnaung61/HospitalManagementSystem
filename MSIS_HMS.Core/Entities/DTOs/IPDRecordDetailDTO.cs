using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class IPDRecordDetailDTO
    {
        public List<RoomChargesDTO> roomChargesDTOs { get; set; }
        public List<MedicationsDTO> medicationsDTOs { get; set; }
        public List<IPDOrderServiceDTO> iPDOrderServiceDTOs { get; set; }
        public List<FeesDTO> feesDTOs { get; set; }
        public List<FoodDTO> foodDTOs { get; set; }
    }
}

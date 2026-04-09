using System.ComponentModel.DataAnnotations;

namespace TranHuyenTran_2122110389.DTOs
{
    public class WorkScheduleDTO
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int ShiftId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime WorkDate { get; set; }
    }
}
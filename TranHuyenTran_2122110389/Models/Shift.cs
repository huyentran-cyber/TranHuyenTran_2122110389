using System.ComponentModel.DataAnnotations;

namespace TranHuyenTran_2122110389.Models
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public string DeptType { get; set; }

        public ICollection<WorkSchedule>? WorkSchedules { get; set; }
    }
}
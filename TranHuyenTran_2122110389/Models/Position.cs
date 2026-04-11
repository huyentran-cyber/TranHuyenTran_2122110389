using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TranHuyenTran_2122110389.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        public int MinStaff { get; set; }

        public int MaxShiftPerDay { get; set; }

        [JsonIgnore]
        public ICollection<Employee>? Employees { get; set; }
        [JsonIgnore]
        public ICollection<Shift> Shifts { get; set; }
    }
}
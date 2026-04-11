using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TranHuyenTran_2122110389.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public Role Role { get; set; } = Role.Staff;

        public int PositionId { get; set; }
        [ForeignKey("PositionId")]
        public Position? Position { get; set; }

        [JsonIgnore]
        public ICollection<WorkSchedule>? WorkSchedules { get; set; }
        [JsonIgnore]
        public ICollection<Attendance>? Attendances { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? ResignationDate { get; set; }
    }
}   
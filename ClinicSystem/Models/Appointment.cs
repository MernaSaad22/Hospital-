using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicSystem.Models
{
    public class Appointment
    {

        public int Id { get; set; }

        [Required]
        public string PatientName { get; set; } = null!;

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }


        
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        
        public Doctor Doctor { get; set; } = null!;
    }
}

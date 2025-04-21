namespace ClinicSystem.Models
{
    public class DoctorswithfilterVM
    {
        public FilteritemsVM filteritemsVM { get; set; } = null!;
        public List<Doctor> DoctorsVM { get; set; } = null!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

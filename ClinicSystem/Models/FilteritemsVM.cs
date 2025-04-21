namespace ClinicSystem.Models
{
    public class FilteritemsVM
    {
        public string doctorName { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public List<string> SpecializationList { get; set; } = new();
    }
}

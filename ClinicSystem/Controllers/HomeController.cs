using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ClinicSystem.Data;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context=new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        //public IActionResult Bookanappointment(FilteritemsVM filteritemsVM)
        //{
        //    IQueryable<Doctor> doctor = _context.Doctors;
        //    //var doctor=new SampleDataDoctor().doctors
        //    //IQueryable<Doctor> doctors= _context.Doctors;
        //    if (filteritemsVM.doctorName is not null)
        //    {
        //        doctor=doctor.Where(e=>e.Name.Contains(filteritemsVM.doctorName)&&e.Specialization.Contains(filteritemsVM.Specialization));
        //    }
        //    DoctorswithfilterVM doctorswithfilterVM = new()
        //    {
        //        DoctorsVM = doctor.ToList(),
        //        filteritemsVM = filteritemsVM
        //    };
        //    return View(doctorswithfilterVM);
        //}




        public IActionResult Bookanappointment(FilteritemsVM filteritemsVM, int page = 1)
        {
            //i want to view 3 items in the page 
            int pageSize = 3;

            IQueryable<Doctor> doctorQuery = _context.Doctors;

            if (!string.IsNullOrEmpty(filteritemsVM.doctorName))
            {
                doctorQuery = doctorQuery.Where(e => e.Name.Contains(filteritemsVM.doctorName));
            }

            if (!string.IsNullOrEmpty(filteritemsVM.Specialization))
            {
                doctorQuery = doctorQuery.Where(e => e.Specialization == filteritemsVM.Specialization);
            }

            var totalDoctors = doctorQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalDoctors / pageSize);

            var filteredDoctors = doctorQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

           
            var specializations = _context.Doctors
                .Select(d => d.Specialization)
                .Distinct()
                .ToList();

            filteritemsVM.SpecializationList = specializations;

            var viewModel = new DoctorswithfilterVM
            {
                DoctorsVM = filteredDoctors,
                filteritemsVM = filteritemsVM,
                CurrentPage = page,
                TotalPages = totalPages
            };

            if (!filteredDoctors.Any() && (!string.IsNullOrEmpty(filteritemsVM.doctorName) || !string.IsNullOrEmpty(filteritemsVM.Specialization)))
            {
                ViewBag.Message = "No doctors found matching the given name or specialization.";
            }

            return View(viewModel);
        }







        public IActionResult BookAppointment(int doctorId)
        {
            var doctor = _context.Doctors.Find( doctorId);
            if (doctor == null)
            {
                return NotFound();
            }


            return View(doctor);


        }

        //without validation of date and time
        //[HttpPost]
        //public IActionResult BookAppointment(int doctorId, string PatientName,
        //    DateTime AppointmentDate, TimeSpan AppointmentTime)
        //{

        //    var doctor = _context.Doctors.Find(doctorId);
        //    if (doctor == null)
        //    {
        //        return NotFound();
        //    }
        //    var appointment = new Appointment
        //    {
        //        PatientName = PatientName,
        //        AppointmentDate = AppointmentDate,
        //        AppointmentTime = AppointmentTime,
        //        DoctorId = doctorId
        //    };

        //    _context.Appointments.Add(appointment);
        //    _context.SaveChanges();

        //    return RedirectToAction("AllAppointments");


        //}







        [HttpPost]
        public IActionResult BookAppointment(int doctorId, string PatientName,
            DateTime AppointmentDate, TimeSpan AppointmentTime)
        {
            var doctor = _context.Doctors.Find(doctorId);
            if (doctor == null)
                return NotFound();

            // Rule 1: Not Friday or Saturday
            if (AppointmentDate.DayOfWeek == DayOfWeek.Friday || AppointmentDate.DayOfWeek == DayOfWeek.Saturday)
            {
                ModelState.AddModelError("AppointmentDate", "Appointments cannot be booked on Fridays or Saturdays.");
            }

            // Rule 2: Time between 9 AM - 9 PM
            if (AppointmentTime < TimeSpan.FromHours(9) || AppointmentTime > TimeSpan.FromHours(21))
            {
                ModelState.AddModelError("AppointmentTime", "Appointment time must be between 9:00 AM and 9:00 PM.");
            }

            // Rule 3: At least 30 minutes gap
            var existingAppointments = _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == AppointmentDate.Date)
                .ToList();

            foreach (var existing in existingAppointments)
            {
                var difference = Math.Abs((existing.AppointmentTime - AppointmentTime).TotalMinutes);
                if (difference < 30)
                {
                    ModelState.AddModelError("AppointmentTime", "There must be at least a 30-minute gap between appointments.");
                    break;
                }
            }

            if (!ModelState.IsValid)
            {
                return View(doctor); 
            }

            // Save appointment
            var appointment = new Appointment
            {
                PatientName = PatientName,
                AppointmentDate = AppointmentDate,
                AppointmentTime = AppointmentTime,
                DoctorId = doctorId
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("AllAppointments");
        }
        public IActionResult EditAppointment(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Doctor)  
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.Doctors = _context.Doctors.ToList(); 
            return View(appointment);
        }

        [HttpPost]
        public IActionResult EditAppointment(Appointment appointment)
        {
            var existingAppointment = _context.Appointments
                .FirstOrDefault(a => a.Id == appointment.Id);

            if (existingAppointment == null)
            {
                return NotFound();
            }

            var doctorExists = _context.Doctors.Any(d => d.Id == appointment.DoctorId);
            if (!doctorExists)
            {
                ViewBag.Doctors = _context.Doctors.ToList();
                ModelState.AddModelError("DoctorId", "Selected doctor does not exist.");
                return View(appointment);  
            }

            existingAppointment.PatientName = appointment.PatientName;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.AppointmentTime = appointment.AppointmentTime;
            existingAppointment.DoctorId = appointment.DoctorId;

            _context.SaveChanges();

            return RedirectToAction(nameof(AllAppointments));
        }

        public IActionResult Deleteappointment(int id)
        {
            var deletedappointment = _context.Appointments.Find(id);
            if (deletedappointment == null)
            {
                return NotFound(); 
            }
            _context.Remove(deletedappointment);
            _context.SaveChanges();
            return RedirectToAction(nameof(AllAppointments));
           
        }





        public IActionResult AllAppointments()
        {
            // as join between doctor and Appointments
            var appointments = _context.Appointments
                .Include(a => a.Doctor) 
                .ToList();

            return View(appointments);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

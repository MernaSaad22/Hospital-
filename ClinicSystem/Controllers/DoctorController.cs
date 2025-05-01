using ClinicSystem.Data;
using ClinicSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context=new();
        public IActionResult Index()
        {
            var doctors = _context.Doctors;
            return View(doctors.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Doctor doctor,IFormFile Img) {
            if (Img is not null&& Img.Length>0)
            {
                //prepare the name of path
                var fileName=Guid.NewGuid().ToString()+Path.GetExtension(Img.FileName);
                var path=Path.Combine(Directory.GetCurrentDirectory(), "", "wwwroot\\images",fileName);
                //then i want to copy this path bit by bit
                //after copy file i want to destroy object so i use key word using
                using(var stream = System.IO.File.Create(path))
                {
                    Img.CopyTo(stream);
                }
                doctor.Img = fileName;
                _context.Doctors.Add(doctor);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            return NotFound();

        }
        public IActionResult Edit([FromRoute]int id)
        {

            var doctor= _context.Doctors.Find(id);
            if (doctor is not null)
            {

                return View(doctor);
            }

            return NotFound(); 
        
        }
        [HttpPost]
        public IActionResult Edit(Doctor doctor, IFormFile Img)
        {
            var doctorInDb = _context.Doctors.AsNoTracking().FirstOrDefault(d => d.Id == doctor.Id);


            if (doctorInDb == null)
            {
                return NotFound();
            }

            if (Img is not null && Img.Length > 0)
            {
                //no tracking

                //prepare the name of path
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "", "wwwroot\\images", fileName);

                //then i want to copy this path bit by bit
                //after copy file i want to destroy object so i use key word using
                using (var stream = System.IO.File.Create(path))
                {
                    Img.CopyTo(stream);
                }

                //Delete old image from DB
                var oldpath = Path.Combine(Directory.GetCurrentDirectory(), "", "wwwroot\\images", doctorInDb.Img);

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }

                doctor.Img = fileName;
            }
            else
            {
                doctor.Img = doctorInDb.Img;
            }

            _context.Doctors.Update(doctor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor is not null)
            {
                var oldpath = Path.Combine(Directory.GetCurrentDirectory(), "", "wwwroot\\images", doctor.Img);

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }
                _context.Remove(doctor);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Models;
using testproject.Models;

public class WorkScheduleController : Controller
{
    private readonly ApplicationDbContext _context;

    public WorkScheduleController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
       
        var schedule = _context.WorkSchedules
            .Include(ws => ws.Employee)
            .ToList();
        return View(schedule);
    }

    [HttpPost]
    public IActionResult AddShift(int employeeId, DateTime shiftDate, TimeSpan startTime, TimeSpan endTime, string notes)
    {
        var shift = new WorkSchedule
        {
            EmployeeId = employeeId,
            ShiftDate = shiftDate,
            StartTime = startTime,
            EndTime = endTime,
            Notes = notes
        };

        _context.WorkSchedules.Add(shift);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}

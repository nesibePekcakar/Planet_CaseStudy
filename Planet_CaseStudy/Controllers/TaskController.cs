using Planet_CaseStudy.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System;
using System.Linq;  
using System.Data.Entity;  

public class TaskController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: Task
    public ActionResult Index(string statusFilter)
    {
        try
        {
            // Get all tasks grouped by status
            var tasks = db.Tasks.AsQueryable();

            // Apply status filter if specified
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "Tümü" &&
                Enum.TryParse(statusFilter, out TaskStatus status))
            {
                tasks = tasks.Where(t => t.Status == status);
            }

            // Group tasks by status while maintaining the filtered results
            var groupedTasks = tasks
                .OrderBy(t => t.DueDate)
                .GroupBy(t => t.Status)
                .OrderBy(g => g.Key) // Optional: order by status enum value
                .ToList();

            // Prepare status filter dropdown
            ViewBag.statusFilter = Enum.GetNames(typeof(TaskStatus))
                .Select(x => new SelectListItem { Text = x, Value = x })
                .ToList();
            ViewBag.statusFilter.Insert(0, new SelectListItem { Text = "Tümü", Value = "" });

            return View(groupedTasks);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading tasks: {ex.Message}");
            return View(new List<IGrouping<TaskStatus, Task>>());
        }
    }

    // GET: Task/Create
    public ActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Task task)
    {
        if (ModelState.IsValid)
        {
            task.DueDate = task.DueDate?.Date;
            db.Tasks.Add(task);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Görev başarıyla oluşturuldu!";
            return RedirectToAction("Index");
        }
        return View(task);
    }

    // GET: Task/Edit/5
    public ActionResult Edit(int? id) => GetTaskView(id);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Task task)
    {
        if (ModelState.IsValid)
        {
            task.DueDate = task.DueDate?.Date;
            db.Entry(task).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Görev başarıyla güncellendi!";
            return RedirectToAction("Index");
        }
        return View(task);
    }

    // GET: Task/Delete/5
    public ActionResult Delete(int? id) => GetTaskView(id);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        var task = db.Tasks.Find(id);
        if (task == null) return HttpNotFound();

        db.Tasks.Remove(task);
        db.SaveChanges();
        TempData["SuccessMessage"] = "Görev başarıyla silindi!";
        return RedirectToAction("Index");
    }

    // Helper method for GET actions
    private ActionResult GetTaskView(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var task = db.Tasks.Find(id);
        return task == null ? HttpNotFound() : (ActionResult)View(task);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) db.Dispose();
        base.Dispose(disposing);
    }
}
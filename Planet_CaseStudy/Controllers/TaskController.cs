using Planet_CaseStudy.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Data.Entity;

[RoutePrefix("Task")]
public class TaskController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: Task
    public ActionResult Index(string statusFilter, bool showArchived = false)
    {
        try
        {
            var tasks = db.Tasks.AsQueryable();

            if (!showArchived)
                tasks = tasks.Where(t => !t.IsArchived);

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "Tümü" &&
                Enum.TryParse(statusFilter, out TaskStatus status))
            {
                tasks = tasks.Where(t => t.Status == status);
            }

            var groupedTasks = tasks
                .OrderBy(t => t.DueDate)
                .GroupBy(t => t.Status)
                .OrderBy(g => g.Key)
                .ToList();

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

    // AJAX: Load Create Form
    [HttpGet]
    public ActionResult Create()
    {
        var task = new Task(); // Empty model
        if (Request.IsAjaxRequest())
            return PartialView("_TaskFormPartial", task); // Modal version
        return View(task); // Fallback (if someone visits the page directly)
    }



    // AJAX: Load Edit Form
    [HttpGet]
    public ActionResult Edit(int? id)
    {
        if (id == null)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        var task = db.Tasks.Find(id); // DO NOT filter IsArchived here!
        if (task == null)
            return HttpNotFound();

        if (Request.IsAjaxRequest())
            return PartialView("_TaskFormPartial", task);
        return View(task);
    }


    // AJAX: Create task
    [HttpPost]
    [Route("AjaxCreate")]
    public JsonResult AjaxCreate(Task task)
    {
        if (ModelState.IsValid)
        {
            task.DueDate = task.DueDate?.Date;
            db.Tasks.Add(task);
            db.SaveChanges();

            return Json(new { success = true, message = "Görev başarıyla oluşturuldu!" });
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return Json(new { success = false, errors });
    }

    // AJAX: Edit task
    [Route("AjaxEdit")]
    [HttpPost]
    public JsonResult AjaxEdit(Task updatedTask)
    {
        if (!ModelState.IsValid)
        {
            var validationErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, errors = validationErrors });
        }

        try
        {
            var originalTask = db.Tasks.Find(updatedTask.Id);
            if (originalTask == null)
            {
                return Json(new { success = false, message = "Görev bulunamadı." });
            }

            // Update manually to avoid concurrency issues
            originalTask.Name = updatedTask.Name;
            originalTask.Description = updatedTask.Description;
            originalTask.Status = updatedTask.Status;
            originalTask.DueDate = updatedTask.DueDate?.Date;

            db.SaveChanges();

            return Json(new { success = true, message = "Görev başarıyla güncellendi!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }


    // AJAX: Archive task
    [HttpPost]
    [Route("AjaxArchive")]
    public JsonResult AjaxArchive(int id)
    {
        try
        {
            var task = db.Tasks.Find(id);
            if (task == null)
                return Json(new { success = false, message = "Görev bulunamadı." });

            task.IsArchived = true;
            db.Entry(task).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // Utility for fallback views
    private ActionResult GetTaskView(int? id)
    {
        if (id == null)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        var task = db.Tasks.Find(id);
        return task == null ? HttpNotFound() : (ActionResult)View(task);
    }
    [HttpGet]
    public ActionResult PartialTaskList(string statusFilter, bool showArchived = false)
    {
        var tasks = db.Tasks.AsQueryable();

        if (!showArchived)
            tasks = tasks.Where(t => !t.IsArchived);

        if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "Tümü" &&
            Enum.TryParse(statusFilter, out TaskStatus status))
        {
            tasks = tasks.Where(t => t.Status == status);
        }

        var groupedTasks = tasks
            .OrderBy(t => t.DueDate)
            .GroupBy(t => t.Status)
            .OrderBy(g => g.Key)
            .ToList();

        return PartialView("_TaskListPartial", groupedTasks);
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing) db.Dispose();
        base.Dispose(disposing);
    }
}
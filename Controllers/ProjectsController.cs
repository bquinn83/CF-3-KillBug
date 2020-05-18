using KillBug.Classes;
using KillBug.Models;
using KillBug.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KillBug.Controllers
{
    [Authorize(Roles = "Admin, Project Manager")]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();

        public ActionResult AssignProjectUsers()
        {
            var users = rolesHelper.UsersInRole("Developer");
            var submitters = rolesHelper.UsersInRole("Submitter");
            
            //users.Concat(submitters);
            foreach(var sub in submitters)
            {
                users.Add(sub);
            }
            
            var viewData = new List<DeveloperAssignmentsViewModel>();

            foreach (var user in users)
            {
                viewData.Add(new DeveloperAssignmentsViewModel
                {
                    Name = user.FullName,
                    Email = user.Email,
                    Projects = projHelper.ListUserProjects(user.Id).Select(p => p.Name).ToList()
                });
            }

            ViewBag.UserIds = new MultiSelectList(users, "Id", "FullNamePosition");
            ViewBag.ProjectIds = new MultiSelectList(db.Projects, "Id", "Name");
            return View(viewData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignProjectUsers(List<string> userIds, List<int> projectIds)
        {
            if (userIds != null && projectIds != null)
            {
                foreach (var userId in userIds)
                {
                    foreach (var projectId in projectIds)
                    {
                        projHelper.AddUserToProject(userId, projectId);
                    }
                }
            }
            return RedirectToAction("AssignProjectUsers");
        }

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.ProjectManagerId = new SelectList(rolesHelper.UsersInRole("Project Manager"), "Id", "FullName");
            }
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ProjectManagerId,")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                project.Updated = null;
                project.IsArchived = false;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ProjectManagerId,Created,Updated,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

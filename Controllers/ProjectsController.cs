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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private NotificationHelper notificationHelper = new NotificationHelper();

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignProjectUsers()
        {
            var users = rolesHelper.UsersInRole("Developer");
            var submitters = rolesHelper.UsersInRole("Submitter");

            //users.Concat(submitters);
            foreach (var sub in submitters)
            {
                users.Add(sub);
            }

            var viewData = new List<DeveloperAssignmentsViewModel>();

            foreach (var user in users)
            {
                viewData.Add(new DeveloperAssignmentsViewModel
                {
                    MenuDrop = $"{user.LastName}-{user.FirstName}",
                    Name = user.FullNamePosition,
                    AvatarPath = user.AvatarPath,
                    Email = user.Email,
                    Projects = projHelper.ListUserProjects(user.Id).Select(p => p.Name).ToList()
                });
            }

            ViewBag.UserIds = new MultiSelectList(users, "Id", "FullNamePosition");
            ViewBag.ProjectIds = new MultiSelectList(db.Projects, "Id", "Name");
            return View(viewData);
        }

        [Authorize(Roles = "Admin, Project Manager")]
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
                        notificationHelper.ProjectAssignmentNotification(projectId, userId);
                        projHelper.AddUserToProject(userId, projectId);
                    }
                }
            }
            return RedirectToAction("AssignProjectUsers");
        }
        
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveProjectUsers(List<string> userIds, List<int> projectIds)
        {
            if (userIds != null && projectIds != null)
            {
                foreach (var userId in userIds)
                {
                    foreach (var projectId in projectIds)
                    {
                        notificationHelper.RemovedProjectNotification(projectId, userId);
                        projHelper.RemoveUserFromProject(userId, projectId);
                    }
                }
            }
            return RedirectToAction("AssignProjectUsers");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ReAssignManager()
        {
            ViewBag.Projects = new MultiSelectList(db.Projects.ToList(), "Id", "Name");
            ViewBag.ProjectManagers = new SelectList(rolesHelper.UsersInRole("Project Manager"), "Id", "FullName");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReAssignManager(List<int> projects, string projectManagers)
        {
            if (projects != null)
            {
                foreach (var proj in projects)
                {
                    notificationHelper.NewProjectManagerNotification(proj, projectManagers);
                    projHelper.AssignProjectManager(proj, projectManagers);
                }
            }
            ViewBag.ProjectManagers = new SelectList(rolesHelper.UsersInRole("Project Manager"), "Id", "FullName");
            ViewBag.Projects = new MultiSelectList(db.Projects.ToList(), "Id", "Name");
            return View();
        }

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AllProjects()
        {
            return View(db.Projects.ToList());
        }

        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        // GET: Projects
        public ActionResult MyProjects()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Project Manager"))
            {
                return View(db.Projects.Where(p => p.ProjectManagerId == userId).ToList());
            }
            else
            {
                return View(db.Users.Find(userId).Projects.ToList());
            }
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
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
        [Authorize(Roles = "Admin, Project Manager")]
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
        [Authorize(Roles = "Admin, Project Manager")]
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

                if (User.IsInRole("Admin"))
                {
                    notificationHelper.NewProjectNotification(project);
                }

                return RedirectToAction("MyProjects", "Projects");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
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
            ViewBag.ProjectManagerId = new SelectList(rolesHelper.UsersInRole("Project Manager"), "Id", "FullName", project.ProjectManagerId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created,ProjectManagerId,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Updated = DateTime.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AllProjects");
                }
                else
                {
                    return RedirectToAction("MyProjects");
                }
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Project Manager")]
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
        [Authorize(Roles = "Admin, Project Manager")]
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

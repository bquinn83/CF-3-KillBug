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

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin, Project Manager")]
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

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin, Project Manager")]
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

        // POST: Re-Assign Manager
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
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

        // GET: ViewProjects
        [Authorize]
        public ActionResult ViewProjects(bool? AllProjects)
        {
            var userId = User.Identity.GetUserId();
            var userRole = rolesHelper.ListUserRoles(userId).FirstOrDefault();

            switch (userRole)
            {
                case "Admin":
                    ViewBag.Title = "All Projects";
                    return View(db.Projects.ToList());
                case "Project Manager":
                    if (AllProjects == true)
                    {
                        ViewBag.Title = "All Projects";
                        return View(db.Projects.ToList());
                    } else
                    {
                        ViewBag.Title = "My Projects";
                        return View(db.Projects.Where(p => p.ProjectManagerId == userId));
                    }
                case "Developer":
                case "Submitter":
                    ViewBag.Title = "My Projects";
                    return View(db.Users.Find(userId).Projects.ToList());
            }
            return Error(ProjectError.CannotRetrieveProjects);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult Details(int? id)
        {
            var userId = db.Users.Find(User.Identity.GetUserId()).Id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return RedirectToAction("Error", new { message = ProjectError.NullProject });
            }
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager") || project.Users.Any(u => u.Id == userId))
            {
                ViewBag.ProjectManagerId = new SelectList(rolesHelper.UsersInRole("Project Manager"), "Id", "FullName", project.ProjectManagerId);
                var devIds = projHelper.UsersNotOnProject(project.Id).Where(u => u.UserRole() == "Developer");
                ViewBag.DeveloperIds = new MultiSelectList(devIds.OrderBy(u => u.LastName), "Id", "FullName");
                var subIds = projHelper.UsersNotOnProject(project.Id).Where(u => u.UserRole() == "Submitter");
                ViewBag.SubmitterIds = new MultiSelectList(subIds.OrderBy(u => u.LastName), "Id", "FullName");
                ViewBag.ProjectTeam = new MultiSelectList(project.Users.OrderBy(u => u.LastName), "Id", "FullNamePosition");
                return View(project);
            } 
            else
            {
                return RedirectToAction("Error", new { message = ProjectError.NotAuthorizedToView });
            }
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
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin, Project Manager")]
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

                return RedirectToAction("Details", "Projects", new { id = project.Id });
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created,ProjectManagerId,IsArchived")] Project project)
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin") || userId == project.ProjectManagerId)
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
            else
            {
                return RedirectToAction("Error", "Projects", new { message = ProjectError.NotAuthorizedToEdit });
            }
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
                return RedirectToAction("Error", new { message = ProjectError.NullProject });
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin, Project Manager")]
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
        protected ActionResult Error(ProjectError? message)
        {
            ViewBag.ErrorMessage = message == ProjectError.NotAuthorizedToEdit ? "You are not authorized to edit this Project."
                : message == ProjectError.NotAuthorizedToView ? "You are not authorized to view this Project."
                : message == ProjectError.NullProject ? "There has been an error retrieving the project. Please alert your superior."
                : message == ProjectError.CannotRetrieveProjects ? "There has been an error retrieving your projects."
                : "There has been an error. Please alert your superior if it persists.";
            return View();
        }

        protected enum ProjectError
        {
            NotAuthorizedToView,
            NotAuthorizedToEdit,
            NullProject,
            CannotRetrieveProjects
        }
    }
}

using School.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace School.Controllers
{
    [RoutePrefix("teacher")]
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Teacher/List
        [Route("list")]
        public ActionResult List(string SearchKey = null)
        {
            TeacherDataController controller = new TeacherDataController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey);
            return View(Teachers);
        }

        //Get: /Teacher/New
        public ActionResult New()
        {
            return View();
        }

        //Post: /Teacher/Create
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal Salary)
        {
            // Initialize a new Teacher object
            Teacher newTeacher = new Teacher();

            // Assign the form parameter values to the corresponding properties of the Teacher object
            newTeacher.TeacherFname = TeacherFname;
            newTeacher.TeacherLname = TeacherLname;
            newTeacher.EmployeeNumber = EmployeeNumber;
            newTeacher.HireDate = HireDate;
            newTeacher.Salary = Salary;

            TeacherDataController controller = new TeacherDataController();
            controller.AddTeacher(newTeacher);
            return RedirectToAction("List");
        }

        //Delete: /Teacher/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            TeacherCourses NewTeacher = controller.FindTeacher(id);
            return View(NewTeacher);
        }

        //Post: /Teacher/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            controller.DeleteTeacher(id);
            return RedirectToAction("List");
        }

        //GET : /Teacher/Show/{id}
        public ActionResult Show(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            TeacherCourses NewTeacherCourses = controller.FindTeacher(id);


            return View(NewTeacherCourses);
        }

        //GET : /Teacher/ShowCourses/{id}
        public ActionResult ShowCourses(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            IEnumerable<Course> Classes = controller.FindListClasses(id);
            return View(Classes);
        }
    }
}
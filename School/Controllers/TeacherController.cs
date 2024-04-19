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

        /// <summary>
        /// Routes to a dynamically generated "Teacher Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">ID of the teacher</param>
        /// <returns>A dynamic "Update Teacher" webpage which provides the current information of the teacher and asks the user for new information as part of a form.</returns>
        /// <example>GET : /Teacher/Update/5</example>
        public ActionResult Update(int id)
        {
            // Instantiate the data controller for handling teacher data
            TeacherDataController controller = new TeacherDataController();

            // Retrieve the information of the selected teacher from the database
            TeacherCourses selectedTeacher = controller.FindTeacher(id);

            // Return the view with the information of the selected teacher
            return View(selectedTeacher);
        }

        /// <summary>
        /// Routes to a dynamically generated "Teacher Update" Page using AJAX. Gathers information from the database.
        /// </summary>
        /// <param name="id">ID of the teacher</param>
        /// <returns>A dynamic "Update Teacher" webpage which provides the current information of the teacher and asks the user for new information as part of a form.</returns>
        /// <example>GET : /Teacher/Ajax_Update/5</example>
        public ActionResult Ajax_Update(int id)
        {
            // Instantiate the data controller for handling teacher data
            TeacherDataController controller = new TeacherDataController();

            // Retrieve the information of the selected teacher from the database
            TeacherCourses selectedTeacher = controller.FindTeacher(id);

            // Return the view with the information of the selected teacher
            return View(selectedTeacher);
        }


        /// <summary>
        /// Receives a POST request containing information about an existing teacher in the system, with new values. Conveys this information to the API, and redirects to the "Teacher Show" page of the updated teacher.
        /// </summary>
        /// <param name="id">ID of the teacher to update</param>
        /// <param name="TeacherFname">The updated first name of the teacher</param>
        /// <param name="TeacherLname">The updated last name of the teacher</param>
        /// <param name="EmployeeNumber">The updated employee number of the teacher</param>
        /// <param name="HireDate">The updated hire date of the teacher</param>
        /// <param name="Salary">The updated salary of the teacher</param>
        /// <returns>A dynamic webpage which provides the current information of the teacher.</returns>
        /// <example>
        /// POST : /Teacher/Update/10
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///    "TeacherFname":"John",
        ///    "TeacherLname":"Doe",
        ///    "EmployeeNumber":"E12345",
        ///    "HireDate":"2024-04-18",
        ///    "Salary":"50000"
        /// }
        /// </example>
        [HttpPost]
        public ActionResult Update(int id, string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal Salary)
        {
            // Create a new instance of the Teacher class and populate it with the updated information
            Teacher teacherInfo = new Teacher();
            teacherInfo.TeacherFname = TeacherFname;
            teacherInfo.TeacherLname = TeacherLname;
            teacherInfo.EmployeeNumber = EmployeeNumber;
            teacherInfo.HireDate = HireDate;
            teacherInfo.Salary = Salary;

            // Instantiate the data controller for handling teacher data
            TeacherDataController controller = new TeacherDataController();

            // Call the method to update the teacher information
            controller.UpdateTeacher(id, teacherInfo);

            // Redirect to the "Show" page of the updated teacher
            return RedirectToAction("Show/" + id);
        }

    }
}
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
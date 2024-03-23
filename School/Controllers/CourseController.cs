using School.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace School.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Course/List
        public ActionResult List(string SearchKey = null)
        {
            CourseDataController controller = new CourseDataController();
            IEnumerable<Course> Courses = controller.ListCourses(SearchKey);
            return View(Courses);
        }

        //GET : /Course/Show/{id}
        public ActionResult Show(int id)
        {
            CourseDataController controller = new CourseDataController();
            Course NewCourses = controller.FindCourse(id);


            return View(NewCourses);
        }
    }
}
using MySql.Data.MySqlClient;
using School.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace School.Controllers
{
    public class CourseDataController : Controller
    {
        private SchoolDbContext School = new SchoolDbContext();
        // GET: CourseData

        //This Controller Will access the class table of our school database.
        /// <summary>
        /// Returns a list of Courses in the system
        /// </summary>
        /// <example>GET api/CourseData/ListCourses/2
        /// <returns>
        /// A list of course objects.
        /// </returns>
        [HttpGet]
        [Route("api/CourseData/ListCourses/{SearchKey?}")]
        public IEnumerable<Course> ListCourses(string SearchKey = null)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM classes WHERE " +
                   "LOWER(classname) LIKE LOWER(@key) OR " +
                   "LOWER(classcode) LIKE LOWER(@key)";
            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();
            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Courses
            List<Course> Courses = new List<Course> { };

            //Loop Through Each Row the Result Set
            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                int ClassId = Convert.ToInt32(ResultSet["classid"]);
                string ClassName = ResultSet["classname"].ToString();
                string ClassCode = ResultSet["classcode"].ToString();
                DateTime StartDate = (DateTime)ResultSet["startdate"];
                DateTime FinishDate = (DateTime)ResultSet["finishdate"];


                Course NewCourse = new Course();
                NewCourse.ClassId = ClassId;
                NewCourse.ClassName = ClassName;
                NewCourse.ClassCode = ClassCode;
                NewCourse.StartDate = StartDate;
                NewCourse.FinishDate = FinishDate;

                //Add the Course Name to the List
                Courses.Add(NewCourse);
            }

            ResultSet.Close();
            Conn.Close();

            //Return the final list of Course names
            return Courses;

        }
        /// <summary>
        /// Returns an individual Course from the database by specifying the primary key courseid
        /// </summary>
        /// <param name="id">the course's ID in the database</param>
        /// <returns>An coursee object</returns>
        [HttpGet]
        public Course FindCourse(int id)
        {
            Course NewCourse = new Course();

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "Select * from Classes where classid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                int CourseId = Convert.ToInt32(ResultSet["classid"]);
                string CoursName = ResultSet["classname"].ToString();
                string CourseCode = ResultSet["classcode"].ToString();
                DateTime StartDate = (DateTime)ResultSet["startdate"];
                DateTime FinishDate = (DateTime)ResultSet["finishdate"];

                NewCourse.ClassId = CourseId;
                NewCourse.ClassName = CoursName;
                NewCourse.ClassCode = CourseCode;
                NewCourse.StartDate = StartDate;
                NewCourse.FinishDate = FinishDate;
            }


            return NewCourse;
        }
        }
    }
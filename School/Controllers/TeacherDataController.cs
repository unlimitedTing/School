using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using School.Models;

namespace School.Controllers
{
    public class TeacherDataController : ApiController
    {
        // The database context class which allows us to access our MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        //This Controller Will access the teachers table of our school database.
        /// <summary>
        /// Returns a list of Teachers in the system
        /// </summary>
        /// <example>GET api/TeacherData/ListTeachers</example>
        /// <returns>
        /// A list of teacher objects.
        /// </returns>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string searchKey = null)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "SELECT * FROM Teachers WHERE " +
                   "(LOWER(teacherfname) LIKE LOWER(@key) OR " +
                   "LOWER(teacherlname) LIKE LOWER(@key) OR " +
                   "LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@key)) OR " +
                   "(DATE(hiredate) = @keyDate) OR " +
                   "(salary = @keySalary)";

            if (DateTime.TryParse(searchKey, out DateTime keyDate))
            {
                cmd.Parameters.AddWithValue("@keyDate", keyDate);
                cmd.Parameters.AddWithValue("@keySalary", DBNull.Value);
            }
            else if (decimal.TryParse(searchKey, out decimal keySalary))
            {
                cmd.Parameters.AddWithValue("@keyDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@keySalary", keySalary);
            }
            else
            {
                cmd.Parameters.AddWithValue("@keyDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@keySalary", DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@key", "%" + searchKey + "%");
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher> { };

            //Loop Through Each Row the Result Set
            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string TeacherFname = ResultSet["teacherfname"].ToString();
                string TeacherLname = ResultSet["teacherlname"].ToString();
                DateTime TeacherHireDate = (DateTime)ResultSet["hiredate"];
                decimal TeacherSalary = (decimal)ResultSet["salary"];


                Teacher NewTeacher = new Teacher();
                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.HireDate = TeacherHireDate;
                NewTeacher.Salary = TeacherSalary;

                //Add the Teacher to the List
                Teachers.Add(NewTeacher);
            }

            //Close the connection between the MySQL Database and the WebServer
            ResultSet.Close();
            Conn.Close();

            //Return the final list of Teacher
            return Teachers;
        }

        /// <summary>
        /// Returns an individual Teacher from the database by specifying the primary key teacherid
        /// </summary>
        /// <param name="id">the teacher's ID in the database</param>
        /// <returns>An teacher object</returns>
        [HttpGet]
        public TeacherCourses FindTeacher(int id)
        {
            Teacher NewTeacher = new Teacher();
            TeacherCourses NewTeacherCourses = new TeacherCourses();

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "Select * from Teachers where teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string TeacherFname = ResultSet["teacherfname"].ToString();
                string TeacherLname = ResultSet["teacherlname"].ToString();
                DateTime TeacherHireDate = (DateTime)ResultSet["hiredate"];
                decimal TeacherSalary = (decimal)ResultSet["salary"];

                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.HireDate = TeacherHireDate;
                NewTeacher.Salary = TeacherSalary;

                NewTeacherCourses.Teacher = NewTeacher;
            }

            ResultSet.Close();

            cmd.CommandText = "SELECT * FROM classes WHERE teacherid = @id";
            cmd.Parameters["@id"].Value = id;

            //Execute query to get courses taught by the teacher
            MySqlDataReader coursesResultSet = cmd.ExecuteReader();

            //Initialize list of courses taught by the teacher
            List<Course> coursesTaught = new List<Course>();
            while (coursesResultSet.Read())
            {
                Course course = new Course
                {
                    ClassId = Convert.ToInt32(coursesResultSet["classid"]),
                    ClassName = coursesResultSet["classname"].ToString(),
                    ClassCode = coursesResultSet["classcode"].ToString(),
                    StartDate = (DateTime)coursesResultSet["startdate"],
                    FinishDate = (DateTime)coursesResultSet["finishdate"],
                };

                coursesTaught.Add(course);
            }

            coursesResultSet.Close();
            Conn.Close();
            NewTeacherCourses.CoursesTaught = coursesTaught;

            return NewTeacherCourses;
        }

        [HttpGet]
        public IEnumerable<Course> FindListClasses(int id)
        {
            // Create a new instance of Class modal
            List<Course> Classes = new List<Course> { };

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            // Fetch courses taught by the teacher
            cmd.CommandText = "SELECT * FROM Classes WHERE teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                Course NewClass = new Course();
                NewClass.ClassId = Convert.ToInt32(ResultSet["classid"]);
                NewClass.ClassCode = ResultSet["classcode"].ToString();
                NewClass.ClassName = ResultSet["classname"].ToString();
                DateTime StartDate = (DateTime)ResultSet["startdate"];
                DateTime FinishDate = (DateTime)ResultSet["finishdate"];
                NewClass.FinishDate = FinishDate;
                NewClass.StartDate = StartDate;

                Classes.Add(NewClass);
            }

            ResultSet.Close();
            Conn.Close();

            return Classes;
        }

    }
}

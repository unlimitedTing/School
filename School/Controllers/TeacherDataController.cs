using MySql.Data.MySqlClient;
using School.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

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
        [EnableCors(origins: "*", methods: "*", headers: "*")]
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
        [EnableCors(origins: "*", methods: "*", headers: "*")]
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
                string EmployeeNumber = ResultSet["employeenumber"].ToString();
                DateTime TeacherHireDate = (DateTime)ResultSet["hiredate"];
                decimal TeacherSalary = (decimal)ResultSet["salary"];

                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.HireDate = TeacherHireDate;
                NewTeacher.EmployeeNumber = EmployeeNumber;
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

        /// <summary>
        /// Adds a new teacher record to the database.
        /// </summary>
        /// <param name="teacher">The teacher object containing details of the teacher to be added.</param>
        /// <remarks>
        /// This method inserts a new teacher record into the "Teachers" table in the database
        /// with the specified details provided in the teacher object parameter.
        /// </remarks>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        public void AddTeacher(Teacher teacher)
        {
            // Check if any required property is null or empty
            if (string.IsNullOrWhiteSpace(teacher.TeacherFname) ||
                string.IsNullOrWhiteSpace(teacher.TeacherLname) ||
                string.IsNullOrWhiteSpace(teacher.EmployeeNumber) ||
                teacher.HireDate == default ||
                teacher.Salary == default)
            {
                // If any required property is missing, throw an exception
                throw new ArgumentException("All fields are required.");
            }
            // Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server and database
            Conn.Open();

            // Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL QUERY to insert new teacher record
            cmd.CommandText = "INSERT INTO Teachers (teacherfname, teacherlname,employeenumber, hiredate, salary) " +
                                "VALUES (@teacherFname, @teacherLname,@employeeNumber, @hireDate, @salary)";
            cmd.Parameters.AddWithValue("@teacherFname", teacher.TeacherFname);
            cmd.Parameters.AddWithValue("@teacherLname", teacher.TeacherLname);
            cmd.Parameters.AddWithValue("@employeeNumber", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@hireDate", teacher.HireDate);
            cmd.Parameters.AddWithValue("@salary", teacher.Salary);

            // Execute the command
            cmd.ExecuteNonQuery();
            
            // Close the connection between the MySQL Database and the WebServer
            Conn.Close();

        }

        /// <summary>
        /// Deletes a teacher record from the database based on the specified teacher ID.
        /// </summary>
        /// <param name="id">The ID of the teacher to be deleted.</param>
        /// <remarks>
        /// This method deletes the teacher record from the "Teachers" table in the database
        /// where the teacher ID matches the specified ID parameter.
        /// </remarks>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        public void DeleteTeacher(int id)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server and database
            Conn.Open();

            // Begin a database transaction to ensure atomicity
            MySqlTransaction transaction = Conn.BeginTransaction();

            // Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            // First, set all records from Classes table where teacherid matches the id
            cmd.CommandText = "UPDATE courses SET teacherid = NULL WHERE teacherid = @teacherId";
            cmd.Parameters.AddWithValue("@teacherId", id);
            cmd.ExecuteNonQuery();

            // Next, delete the teacher record from Teachers table
            cmd.CommandText = "DELETE FROM Teachers WHERE teacherid = @teacherId";
            cmd.ExecuteNonQuery();

            // Commit the transaction if all operations succeed
            transaction.Commit();


            Conn.Close();
        }

        /// <summary>
        /// Updates an Teacher on the MySQL Database. 
        /// </summary>
        /// <param name="Teacher">An object with fields that map to the columns of the teacher's table.</param>
        /// <example>
        /// POST api/TeacherData/UpdateTeacher/208 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"TeacberFname":"Alexander",
        ///	"TeacberLname":"Bennett",
        ///	"EmployeeNumber":"T378",
        ///	"HireDate":"2016-08-05 00:00:00",
        ///	"Salary":"55.30'
        /// }
        /// </example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        public void UpdateTeacher(int id, [FromBody] Teacher teacher)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Debug.WriteLine(AuthorInfo.AuthorFname);

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "UPDATE Teachers SET teacherfname = @TeacherFname, teacherlname = @TeacherLname, employeenumber = @EmployeeNumber, hiredate = @HireDate, salary = @Salary WHERE teacherid = @TeacherId";
            cmd.Parameters.AddWithValue("@TeacherFname", teacher.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", teacher.TeacherLname);
            cmd.Parameters.AddWithValue("@EmployeeNumber", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", teacher.HireDate);
            cmd.Parameters.AddWithValue("@Salary", teacher.Salary);
            cmd.Parameters.AddWithValue("@TeacherId", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();


        }

        /// <summary>
        /// Return a list of courses teached by the teacher provided by the teacher id
        /// </summary>
        /// <param name="id">the teacher's id</param>
        /// <returns>A list of courses object</returns>
        [HttpGet]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
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

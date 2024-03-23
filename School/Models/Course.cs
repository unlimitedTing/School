using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace School.Models
{
    public class Course
    {
        public int ClassId;
        public string ClassCode;
        public int TeacherId;
        public DateTime StartDate;
        public DateTime FinishDate;
        public string ClassName;
    }
}
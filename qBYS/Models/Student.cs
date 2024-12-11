using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using qBYS.Models;

namespace qBYS.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? AdvisorID { get; set; }

        // Navigation Property for the Advisor
        [ForeignKey("AdvisorID")]
        public virtual Advisors? Advisors { get; set; }
        public DateTime EnrollmentDate
        {
            get;
            set;
        }
        

        public virtual ICollection<NonConfirmedSelections> NonConfirmedSelections { get; set; }
        public virtual ICollection<StudentCourseSelections> StudentCourseSelections { get; set; }


    }
}
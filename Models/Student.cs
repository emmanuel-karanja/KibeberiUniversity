using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KibeberiUniversity.Models
{
    public class Student : IEntity
    {
        public int Id{get;set;}

        [Required]
        [StringLength(50,MinimumLength=2)]
        public string LastName {get;set;}

        [Required]
        [StringLength(50,ErrorMessage="First name cannot be longer than 50 characters")]
        [Column("FirstName")]
        public string FirstMidName{get;set;}

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString="{) : dd-MM-yyyy}",ApplyFormatInEditMode=true)]
        public DateTime EnrollmentDate {get;set;}

        public string FullName =>  LastName+", "+FirstMidName;

       //each student is enrolled for a number of courses.
        public ICollection<Enrollment> Enrollments {get;set;}=new List<Enrollment>();
        
    }
}
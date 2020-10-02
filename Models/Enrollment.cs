using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KibeberiUniversity.Models
{
    public enum Grade
    {
        A,B,C,D,E,F
    }

    public class Enrollment : IEntity
    {
        [Column("EnrollmentId")]
        public int Id {get;set;}

        public int CourseId{get;set;}
        public int StudentID{get;set;}
        
        [DisplayFormat(NullDisplayText="No Grade")]
        public Grade? Grade {get;set;}
        
        public Course Course{get;set;}
        public Student Student {get;set;}
        
    }
}
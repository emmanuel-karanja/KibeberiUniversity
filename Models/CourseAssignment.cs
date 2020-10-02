namespace KibeberiUniversity.Models
{
    //junction table for courses and the instructor who teaches it mant-to-many btn
    //course and instructor
    public class CourseAssignment
    {
        public int InstructorId{get;set;}
        public int CourseId{get;set;}
        public Instructor Instructor{get;set;}
        public Course Course{get;set;}
    }
}
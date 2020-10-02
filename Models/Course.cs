using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KibeberiUniversity.Models
{
    public class Course : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name="Number")]
        [Column("CourseId")]
        public int Id{get;set;}

        [StringLength(50,MinimumLength=3)]
        public string Title {get;set;}
        
        [Range(1,5)]
        public int Credits{get;set;}

        public int DepartmentId{get;set;}
        public ICollection<Enrollment> Enrollments {get;set;}
        public ICollection<CourseAssignment> CourseAssignments{get;set;}
    }
}
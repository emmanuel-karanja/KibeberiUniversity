using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KibeberiUniversity.Models
{
    public class Department : IEntity
    {
        [Column("DepartmentId")]
        public int Id{get;set;}

        [StringLength(50,MinimumLength = 3)]
        public string Name {get;set;}

        [DataType(DataType.Currency)]
        public decimal Budget{get;set;}

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString="{0:dd-MM-yyyy", ApplyFormatInEditMode=true)]
        [Display(Name="Start Date")]
        public DateTime StartDate{get;set;}

        public int? InstructorId{get;set;}

        [Timestamp]
        public byte[] RowVersion {get;set;}
        
        public Instructor Administrator{get;set;}
        public ICollection<Course> Courses{get;set;}
    }
}
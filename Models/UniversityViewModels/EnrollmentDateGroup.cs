using System;
using System.ComponentModel.DataAnnotations;


namespace KibeberiUniversity.Models.UniversityViewModels
{
    public class EnrollmentDateGroup
    {
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate {get;set;}
        public int StudentCount{get;set;}
    }
}
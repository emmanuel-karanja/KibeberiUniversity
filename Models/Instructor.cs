using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using KibeberiUniversity.Pages.Instructors;

namespace KibeberiUniversity.Models
{
    public class Instructor : IEntity
    {
        public int Id{get;set;}

        [Required]
        [Column("FirstName")]
        [StringLength(50, MinimumLength=2)]
        public string FirstMidName {get;set;}

        [Required]
        [StringLength(50,MinimumLength=2)]
        public string LastName{get;set;}

        [DataType(DataType.Date)]
        public DateTime HireDate{get;set;}

        public string FullName => LastName +", "+FirstMidName;

        public ICollection<CourseAssignment> CourseAssignments {get;private set;}=new List<CourseAssignment>();
        public OfficeAssignment OfficeAssignment {get;private set;}

        public void Handle(CreateEditModel.CreateEditInstructorCommand request, IEnumerable<Course> courses)
        {
            UpdateDetails(request);
            UpdateInstructorCourses(request.SelectedCourses,courses);
        }

        public void Handle(DeleteModel.DeleteInstructorCommand request) => OfficeAssignment = null;

        private void UpdateDetails(CreateEditModel.CreateEditInstructorCommand request)
        {
            FirstMidName=request.FirstMidName;
            LastName=request.LastName;
            HireDate=request.HireDate.GetValueOrDefault();

            if(string.IsNullOrWhiteSpace(request.OfficeAssignmentLocation))
            {
                OfficeAssignment=null;
            }
            else if (OfficeAssignment==null)
            {
                OfficeAssignment=new OfficeAssignment { Location=request.OfficeAssignmentLocation};
            }
            else
            {
                OfficeAssignment.Location=request.OfficeAssignmentLocation;
            }
        }

        private void UpdateInstructorCourses(string[] selectedCourses, IEnumerable<Course> courses)
        {
            if(selectedCourses==null)
            {
                CourseAssignments= new List<CourseAssignment>();
                return;
            }

            var selectedCoursesHs=new HashSet<string>(selectedCourses);
            var instructorCourses=new HashSet<int>(CourseAssignments.Select(c=>c.CourseId));


            foreach(var course in courses)
            {
                if(selectedCoursesHs.Contains(course.Id.ToString()))
                {
                    if(!instructorCourses.Contains(course.Id))
                    {
                        CourseAssignments.Add(new CourseAssignment{Course =course, Instructor= this});
                    }
                }
                else
                {
                    if(instructorCourses.Contains(course.Id))
                    {
                        var toRemove=CourseAssignments.Single(x=> x.CourseId == course.Id);
                        CourseAssignments.Remove(toRemove);
                    }
                }         
            }  
        }
    }
}
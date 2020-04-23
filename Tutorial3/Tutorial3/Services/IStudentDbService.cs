using System;
using Tutorial3.Models.Enrollment;
using Tutorial3.Models.Promotion;

namespace Tutorial3.Services
{
    public interface IStudentDbService
    {
        AddEnrollment EnrollStudent(AddEnrollment student);
        PromoteStudents Promote(PromoteStudents study);
    }
}
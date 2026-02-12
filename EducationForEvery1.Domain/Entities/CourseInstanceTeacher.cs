using System;
using System.Collections.Generic;
using System.Text;

namespace EducationForEvery1.Domain.Entities;

public class CourseInstanceTeacher
{
    public int CourseInstanceId { get; private set; }
    public CourseInstance CourseInstance { get; private set; } = null!;

    public int TeacherId { get; private set; }
    public Teacher Teacher { get; private set; } = null!;

    private CourseInstanceTeacher() { }

    public CourseInstanceTeacher(int courseInstanceId, int teacherId)
    {
        CourseInstanceId = courseInstanceId;
        TeacherId = teacherId;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EducationForEvery1.Domain.Entities;

public enum EnrollmentStatus
{
    Active = 1,
    Cancelled = 2
}

public class Enrollment
{
    public int Id { get; private set; }

    public int StudentId { get; private set; }
    public Student Student { get; private set; } = null!;

    public int CourseInstanceId { get; private set; }
    public CourseInstance CourseInstance { get; private set; } = null!;

    public DateTime EnrolledAtUtc { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    private Enrollment() { }

    public Enrollment(int studentId, int courseInstanceId)
    {
        StudentId = studentId;
        CourseInstanceId = courseInstanceId;
        EnrolledAtUtc = DateTime.UtcNow;
        Status = EnrollmentStatus.Active;
    }

    private readonly List<Enrollment> _enrollments = new();

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;

    private readonly List<CourseInstanceTeacher> _teachers = new();

    public IReadOnlyCollection<CourseInstanceTeacher> Teachers => _teachers;

    public void Cancel() => Status = EnrollmentStatus.Cancelled;
}

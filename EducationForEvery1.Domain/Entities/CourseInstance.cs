using System;
using System.Collections.Generic;
using System.Text;

namespace EducationForEvery1.Domain.Entities
{
    public class CourseInstance
    {
        public int Id { get; private set; }

        public int CourseId { get; private set; }
        public Course Course { get; private set; } = null!;

        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }

        public string Location { get; private set; } = string.Empty;
        public int Capacity { get; private set; }

        private CourseInstance() { }

        public CourseInstance(int courseId, DateOnly startDate, DateOnly endDate, string location, int capacity)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be empty.");
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero."); 

            CourseId = courseId;
            StartDate = startDate;
            EndDate = endDate;
            Location = location.Trim();
            Capacity = capacity;
        }

        public void Update(DateOnly startDate, DateOnly endDate, string location, int capacity)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be empty.");
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            StartDate = startDate;
            EndDate = endDate;
            Location = location.Trim();
            Capacity = capacity;
        }

    }
}

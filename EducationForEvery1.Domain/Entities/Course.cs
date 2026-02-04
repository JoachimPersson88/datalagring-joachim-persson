using System;
using System.Collections.Generic;
using System.Text;

namespace EducationForEvery1.Domain.Entities
{
    public class Course
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        private Course() { }

        public Course(string title, string? description)
        {
            SetTitle(title);
            Description = description;
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");

            Title = title.Trim();
        }

        public void SetDescription(string? description) => Description = description;

    }
}

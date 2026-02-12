using System;
using System.Collections.Generic;
using System.Text;

namespace EducationForEvery1.Domain.Entities;
public class Teacher
{
    public int Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private Teacher() { } // EF Core

    public Teacher(string firstName, string lastName, string email)
    {
        SetName(firstName, lastName);
        SetEmail(email);
    }

    public void SetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("FirstName is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("LastName is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
        Email = email.Trim().ToLowerInvariant();
    }
}

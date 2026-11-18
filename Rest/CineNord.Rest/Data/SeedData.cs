using CineNord.Rest.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNord.Rest.Data;

public static class SeedData
{
    public static void Apply(ModelBuilder mb)
    {
        var cinemas = new[]
        {
            new Cinema { Id = 1, Name = "CineNord Palads",   City = "København", Address = "Axeltorv 9" },
            new Cinema { Id = 2, Name = "CineNord Bruuns",   City = "Aarhus",    Address = "M.P. Bruuns Gade 25" },
            new Cinema { Id = 3, Name = "CineNord Rosengård",City = "Odense",    Address = "Ørbækvej 75" },
            new Cinema { Id = 4, Name = "CineNord Kennedy",  City = "Aalborg",   Address = "John F. Kennedys Plads 1" },
            new Cinema { Id = 5, Name = "CineNord Vest",     City = "Esbjerg",   Address = "Kongensgade 12" }
        };
        mb.Entity<Cinema>().HasData(cinemas);

        // 2 sale pr. biograf = 10 sale
        var auditoriums = new List<Auditorium>();
        var audId = 1;
        foreach (var c in cinemas)
        {
            auditoriums.Add(new Auditorium { Id = audId++, CinemaId = c.Id, Name = "Sal 1", Rows = 12, SeatsPerRow = 16 });
            auditoriums.Add(new Auditorium { Id = audId++, CinemaId = c.Id, Name = "Sal 2", Rows = 8,  SeatsPerRow = 12 });
        }
        mb.Entity<Auditorium>().HasData(auditoriums);

        var movies = new[]
        {
            new Movie { Id = 1,  Title = "Dune: Part Two",        Genre = "sci-fi",    DurationMinutes = 166, AgeRating = "11", ReleaseYear = 2024 },
            new Movie { Id = 2,  Title = "Oppenheimer",           Genre = "drama",     DurationMinutes = 180, AgeRating = "15", ReleaseYear = 2023 },
            new Movie { Id = 3,  Title = "Inside Out 2",          Genre = "animation", DurationMinutes = 96,  AgeRating = "A",  ReleaseYear = 2024 },
            new Movie { Id = 4,  Title = "Gladiator II",          Genre = "action",    DurationMinutes = 148, AgeRating = "15", ReleaseYear = 2024 },
            new Movie { Id = 5,  Title = "Wicked",                Genre = "musical",   DurationMinutes = 160, AgeRating = "7",  ReleaseYear = 2024 },
            new Movie { Id = 6,  Title = "Interstellar",          Genre = "sci-fi",    DurationMinutes = 169, AgeRating = "11", ReleaseYear = 2014 },
            new Movie { Id = 7,  Title = "The Batman",            Genre = "action",    DurationMinutes = 176, AgeRating = "15", ReleaseYear = 2022 },
            new Movie { Id = 8,  Title = "Barbie",                Genre = "comedy",    DurationMinutes = 114, AgeRating = "7",  ReleaseYear = 2023 },
            new Movie { Id = 9,  Title = "Druk",                  Genre = "drama",     DurationMinutes = 117, AgeRating = "11", ReleaseYear = 2020 },
            new Movie { Id = 10, Title = "Flow",                  Genre = "animation", DurationMinutes = 85,  AgeRating = "A",  ReleaseYear = 2024 },
            new Movie { Id = 11, Title = "Top Gun: Maverick",     Genre = "action",    DurationMinutes = 131, AgeRating = "11", ReleaseYear = 2022 },
            new Movie { Id = 12, Title = "Arrival",               Genre = "sci-fi",    DurationMinutes = 116, AgeRating = "11", ReleaseYear = 2016 },
            new Movie { Id = 13, Title = "Jagten",                Genre = "drama",     DurationMinutes = 115, AgeRating = "15", ReleaseYear = 2012 },
            new Movie { Id = 14, Title = "Spider-Man: Across the Spider-Verse", Genre = "animation", DurationMinutes = 140, AgeRating = "7", ReleaseYear = 2023 },
            new Movie { Id = 15, Title = "John Wick: Chapter 4",  Genre = "action",    DurationMinutes = 169, AgeRating = "15", ReleaseYear = 2023 },
            new Movie { Id = 16, Title = "Poor Things",           Genre = "comedy",    DurationMinutes = 141, AgeRating = "15", ReleaseYear = 2023 },
            new Movie { Id = 17, Title = "Blade Runner 2049",     Genre = "sci-fi",    DurationMinutes = 164, AgeRating = "15", ReleaseYear = 2017 },
            new Movie { Id = 18, Title = "Retfærdighedens ryttere", Genre = "comedy",  DurationMinutes = 116, AgeRating = "15", ReleaseYear = 2020 },
            new Movie { Id = 19, Title = "Mission: Impossible – Dead Reckoning", Genre = "action", DurationMinutes = 163, AgeRating = "11", ReleaseYear = 2023 },
            new Movie { Id = 20, Title = "The Wild Robot",        Genre = "animation", DurationMinutes = 102, AgeRating = "A",  ReleaseYear = 2024 }
        };
        mb.Entity<Movie>().HasData(movies);

        // 60 forestillinger fordelt over 10 sale og 6 dage
        var screenings = new List<Screening>();
        var start = new DateTime(2026, 12, 1, 0, 0, 0, DateTimeKind.Utc);
        int[] hours = [14, 17, 20];
        for (var i = 0; i < 60; i++)
        {
            screenings.Add(new Screening
            {
                Id = i + 1,
                MovieId = i % movies.Length + 1,
                AuditoriumId = i % auditoriums.Count + 1,
                StartsAt = start.AddDays(i / 10).AddHours(hours[i % hours.Length]),
                Price = i % 3 == 0 ? 95m : 125m
            });
        }
        mb.Entity<Screening>().HasData(screenings);
    }
}
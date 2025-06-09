using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Infractructure.EF;

namespace Infractructure;

public class KickstarterSeeder
{
    private readonly AppDbContext _context;

    public KickstarterSeeder(AppDbContext context)
    {
        _context = context;
    }

    public void Seed(string csvFilePath)
    {
        if (_context.Kickstarters.Any())
            return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using var reader = new StreamReader(csvFilePath);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<KickstarterEntity>();

        foreach (var record in records)
        {
            var entity = new KickstarterEntity
            {
                ID = record.ID,
                Name = record.Name,
                Category = record.Category,
                Subcategory = record.Subcategory,
                Country = record.Country,
                Launched = record.Launched,
                Deadline = record.Deadline,
                Goal = record.Goal,
                Pledged = record.Pledged,
                Backers = record.Backers,
                State = record.State
            };

            _context.Kickstarters.Add(entity);
        }

        _context.SaveChanges();
    }
}
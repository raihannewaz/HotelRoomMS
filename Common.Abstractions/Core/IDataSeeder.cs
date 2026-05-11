namespace Common.Abstractions.Persistence;

public interface IDataSeeder
{
    Task SeedAllAsync();
}

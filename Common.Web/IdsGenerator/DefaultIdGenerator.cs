using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using IdGen;

namespace Common.Core.IdsGenerator;

public class SnowFlakIdGenerator : Abstractions.Core.IIdGenerator<long>
{
    public long New()
    {
        return NewId();
    }

    public static long NewId()
    {
        long id = _generator.CreateId();
        string idAsString = id.ToString();
        string truncatedNumberAsString = idAsString[..^1];
        return long.Parse(truncatedNumberAsString);
    }

    private static IdGenerator? _generator;

    public static void Configure(int generatorId)
    {
        Guard.Against.NegativeOrZero(generatorId, nameof(generatorId));

        var epoch = new DateTime(2022, 1, 17, 0, 0, 0, DateTimeKind.Local);

        var structure = new IdStructure(45, 2, 16);

        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

        _generator = new IdGenerator(0, options);
    }
}

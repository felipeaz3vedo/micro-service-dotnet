namespace FC.Codeflix.Catalog.UnitTests.Common;

using Bogus;

public abstract class BaseFixture
{
    public Faker Faker { get; set; } = new("pt_BR");
}
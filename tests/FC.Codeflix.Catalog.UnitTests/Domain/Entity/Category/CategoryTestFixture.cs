namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

using DomainEntity = Catalog.Domain.Entity;
using Common;

public class CategoryTestFixture : BaseFixture
{
    public string GenerateValidName()
    {
        var categoryName = "";

        while (categoryName.Length is < 3 or > 255)
        {
            categoryName = Faker.Commerce.Categories(1)[0];
        }
        
        return categoryName;
    }
    
    public string GenerateValidDescription()
    {
        var categoryDescription = "";

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        
        return categoryDescription;
    } 
    
    public DomainEntity.Category GetValidCategory()
        => new(
            GenerateValidName(),
            GenerateValidDescription());
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection 
    : ICollectionFixture<CategoryTestFixture>;

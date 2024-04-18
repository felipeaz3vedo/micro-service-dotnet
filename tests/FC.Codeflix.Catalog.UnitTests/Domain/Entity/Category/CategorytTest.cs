namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using DomainEntity = Catalog.Domain.Entity;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest(CategoryTestFixture categoryTestFixture)
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validCategory = categoryTestFixture.GetValidCategory();

        var dateTimeBefore = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        (category.IsActive).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validCategory = categoryTestFixture.GetValidCategory();

        var dateTimeBefore = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));
        
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string name)
    {
        Action action = () => _ = new DomainEntity.Category(name, "Category description");

        action.Should().Throw<EntityValidationException>()
              .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        Action action = () => _ = new DomainEntity.Category("Category", null!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should not be null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 6)]
    public void InstantiateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        Action action = () => _ = new DomainEntity.Category(invalidName, "Category valid description");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }
    
    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfTests)
    {
        var fixture = new CategoryTestFixture();
        
        for (var i = 0; i < numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;
            
            yield return new object[]
            {
                fixture.GenerateValidName()[..(isOdd ? 1 : 2)]
            };
        }
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNamIsGreaterThanThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNamIsGreaterThanThan255Characters()
    {
        string invalidName = new('a', 256);

        Action action = () => _ = new DomainEntity.Category(invalidName, "Category valid description");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        string invalidDescription = new('a', 10_001);

        Action action = () => _ = new DomainEntity.Category("Category valid name", invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validCategory = categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        (category.IsActive).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validCategory = categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        
        category.Deactivate();

        (category.IsActive).Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = categoryTestFixture.GetValidCategory();

        var newValues = new
        {
            Name = categoryTestFixture.GenerateValidName(),
            Description = categoryTestFixture.GenerateValidDescription()
        };

        category.Update(newValues.Name, newValues.Description);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var category = categoryTestFixture.GetValidCategory();

        var  newName = categoryTestFixture.GenerateValidName();
        var prevDescription = category.Description;

        category.Update(newName);

        category.Name.Should().Be(newName);
        category.Description.Should().Be(prevDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string invalidName)
    {
        var category = categoryTestFixture.GetValidCategory();

        var action = () => category.Update(
            invalidName, 
            categoryTestFixture.GenerateValidDescription());
        
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("Fe")]
    [InlineData("F")]
    [InlineData("12")]
    [InlineData("1")]
    public void UpdateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        var category = categoryTestFixture.GetValidCategory();

        var action = () => category.Update(
            invalidName,
            categoryTestFixture.GenerateValidDescription());

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThanThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThanThan255Characters()
    {
        var category = categoryTestFixture.GetValidCategory();

        var invalidName = categoryTestFixture.Faker.Random.String(256);

        var action = () => category.Update(
            invalidName, 
            categoryTestFixture.GenerateValidDescription());

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters long");
    }
    
    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThanThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThanThan10_000Characters()
    {
        var category = categoryTestFixture.GetValidCategory();

        var invalidDescription = categoryTestFixture.Faker.Random.String(10_001);

        var action = () => category.Update(
            categoryTestFixture.GenerateValidName(),
            invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters long");
    }
}

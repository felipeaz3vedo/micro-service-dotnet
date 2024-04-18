using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;

using FC.Codeflix.Catalog.Domain.Validations;
using FluentAssertions;
using Bogus;

public class DomainValidationTest
{
    private Faker Faker { get; } = new();

    [Fact(DisplayName = nameof(ShouldBeNotNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void ShouldBeNotNull()
    {
        var value = Faker.Commerce.ProductName();

        var action = () => DomainValidation.NoTNull(value, "Value");

        action.Should().NotThrow();
    }
    
    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string? value = null;

        var action = () => DomainValidation.NoTNull(value, "FieldName");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("FieldName should not be null");
    }
    
    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        var action = () => DomainValidation.NotNullOrEmpty(target, "FieldName");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("FieldName should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(ShouldBeNotNullOrEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void ShouldBeNotNullOrEmpty()
    {
        var target = Faker.Commerce.ProductName();
        
        var action = () => DomainValidation.NotNullOrEmpty(target, "FieldName");

        action.Should().NotThrow();
    }
    
    
    [Theory(DisplayName = nameof(MinLengthThrowWhenLessThanLimit))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10 )]
    public void MinLengthThrowWhenLessThanLimit(string target, int minLength)
    {
        var action = () => DomainValidation.MinLength(target, minLength, "FieldName" );

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"FieldName should be at least {minLength} characters long");
    }
    
    [Theory(DisplayName = nameof(MeetsMinLength))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesEqualOrGreaterThanMin), parameters: 10 )]
    public void MeetsMinLength(string target, int minLength)
    {
        var action = () => DomainValidation.MinLength(target, minLength, "FieldName" );

        action.Should().NotThrow();
    }
    
    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreaterThanLimit))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10 )]
    public void MaxLengthThrowWhenGreaterThanLimit(string target, int maxLength)
    {
        var action = () => DomainValidation.MaxLength(target, maxLength, "FieldName" );

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"FieldName should be less or equal {maxLength} characters long");
    }
    
    [Theory(DisplayName = nameof(MeetsMaxLengthMaxLimit))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesLessThanMax), parameters: 10 )]
    public void MeetsMaxLengthMaxLimit(string target, int maxLength)
    {
        var action = () => DomainValidation.MaxLength(target, maxLength, "FieldName" );

        action.Should().NotThrow();
    }
    
    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numberOfTests)
    {
        var faker = new Faker();
        
        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + (new Random()).Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }
    
    public static IEnumerable<object[]> GetValuesEqualOrGreaterThanMin(int numberOfTests)
    {
        var faker = new Faker();
        
        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, minLength };
        }
    }
    
    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests)
    {
        var faker = new Faker();

        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, maxLength };
        }
    }
    
    public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests)
    {
        var faker = new Faker();

        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length + (new Random()).Next(1, 20);
            yield return new object[] { example, maxLength };
        }
    }
}
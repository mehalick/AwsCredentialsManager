namespace AwsCredentialsManager.Core.Tests;

public class AwsAccountTests
{
    private const string ValidName = "[default]";

    [Fact]
    public void AwsAccount_ValidName_SetsIdAndName()
    {
        var account = new AwsAccount(ValidName);

        Assert.Equal("default", account.Id);
        Assert.Equal(ValidName, account.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("...")]
    public void AwsAccount_InvalidNames_ThrowsException(string name)
    {
        Assert.Throws<ArgumentException>(() => new AwsAccount(name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("[default]")]
    public void AddProperty_InvalidProperties_IgnoresProperty(string line)
    {
        var account = new AwsAccount(ValidName);

        account.AddProperty(line);

        Assert.Empty(account.Properties);
    }

    [Theory]
    [InlineData("property=value")]
    [InlineData("property = value")]
    public void AddProperty_ValidLine_AddsProperty(string line)
    {
        var account = new AwsAccount(ValidName);

        account.AddProperty(line);

        Assert.Equal("value", account.Properties["property"]);
    }

    [Fact]
    public void AddProperty_MissingValue_ThrowsException()
    {
        var account = new AwsAccount(ValidName);

        Assert.Throws<ArgumentException>(() => account.AddProperty("property-without-value"));
    }
}

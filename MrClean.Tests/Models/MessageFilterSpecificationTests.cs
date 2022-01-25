using MrClean.Models;
using NUnit.Framework;

namespace MrClean.Tests.Models;

public class MessageFilterSpecificationTests
{
    [Test]
    public void TestBuildingSpecificationFromNull()
    {
        var specification = new MessageFilterSpecification(null);
        
        Assert.That(specification.AllowedEntities, Is.Empty);
        Assert.That(specification.DeniedEntities, Is.Empty);
        Assert.That(specification.SpecificationString, Is.Null);
    }

    [Test]
    public void TestBuildingSpecificationFromEmptyString()
    {
        var specification = new MessageFilterSpecification("");
        
        Assert.That(specification.AllowedEntities, Is.Empty);
        Assert.That(specification.DeniedEntities, Is.Empty);
        Assert.That(specification.SpecificationString, Is.Null);
    }

    [Test]
    public void TestParsingEntitiesSourceString()
    {
        const string sourceString = "0;~1;2;3;~4";
        var specification = new MessageFilterSpecification(sourceString);

        Assert.That(specification.AllowedEntities.Count, Is.EqualTo(3));
        Assert.That(specification.AllowedEntities, Does.Contain(2));
        Assert.That(specification.AllowedEntities, Does.Contain(3));

        Assert.That(specification.DeniedEntities.Count, Is.EqualTo(2));
        Assert.That(specification.DeniedEntities, Does.Contain(1));
        Assert.That(specification.DeniedEntities, Does.Contain(4));
    }

    [Test]
    public void TestImplicitAllow()
    {
        var specification = new MessageFilterSpecification(null);
        
        Assert.That(specification.AllowsEntity(1), Is.True);
        Assert.That(specification.AllowsEntity(2), Is.True);
        Assert.That(specification.AllowsEntity(3), Is.True);
    }

    [Test]
    public void TestExplicitAllow()
    {
        var specification = new MessageFilterSpecification("1;2");
        
        Assert.That(specification.AllowsEntity(1), Is.True);
        Assert.That(specification.AllowsEntity(2), Is.True);
        Assert.That(specification.AllowsEntity(3), Is.False);
    }

    [Test]
    public void TestExplicitAllowAndDeny()
    {
        var specification = new MessageFilterSpecification("1;~2");
        
        Assert.That(specification.AllowsEntity(1), Is.True);
        Assert.That(specification.AllowsEntity(2), Is.False);
    }

    [Test]
    public void TestImplicitAllowAndDeny()
    {
        var specification = new MessageFilterSpecification("~2");
        
        Assert.That(specification.AllowsEntity(1), Is.True);
        Assert.That(specification.AllowsEntity(2), Is.False);
        Assert.That(specification.AllowsEntity(3), Is.True);
    }

    [Test]
    public void TestSpecificationString()
    {
        var source = new MessageFilterSpecification("~0;1;2;~3");
        var output = source.SpecificationString;
        
        Assert.IsNotNull(output);
        Assert.AreEqual("1;2;~0;~3", output);

        var empty = new MessageFilterSpecification("");
        
        Assert.That(empty.SpecificationString, Is.Null);
    }

    [Test]
    public void TestAddingAllowedEntities()
    {
        var source = new MessageFilterSpecification("~1");
        
        Assert.That(source.DeniedEntities, Is.Not.Empty);
        Assert.That(source.DeniedEntities, Does.Contain(1));
        
        Assert.That(source.AllowedEntities, Is.Empty);
        
        source.AddAllowedEntity(2);
        
        Assert.That(source.DeniedEntities, Is.Not.Empty);
        Assert.That(source.DeniedEntities, Does.Contain(1));
        Assert.That(source.DeniedEntities, Does.Not.Contain(2));
        
        Assert.That(source.AllowedEntities, Is.Not.Empty);
        Assert.That(source.AllowedEntities, Does.Not.Contain(1));
        Assert.That(source.AllowedEntities, Does.Contain(2));
        
        source.AddAllowedEntity(1);
        
        Assert.That(source.DeniedEntities, Is.Empty);
        
        Assert.That(source.AllowedEntities, Is.Not.Empty);
        Assert.That(source.AllowedEntities, Does.Contain(1));
        Assert.That(source.AllowedEntities, Does.Contain(2));
    }
    
    [Test]
    public void TestAddingDeniedEntities()
    {
        var source = new MessageFilterSpecification("1");
        
        Assert.That(source.AllowedEntities, Is.Not.Empty);
        Assert.That(source.AllowedEntities, Does.Contain(1));
        
        Assert.That(source.DeniedEntities, Is.Empty);
        
        source.AddDeniedEntity(2);
        
        Assert.That(source.AllowedEntities, Is.Not.Empty);
        Assert.That(source.AllowedEntities, Does.Contain(1));
        Assert.That(source.AllowedEntities, Does.Not.Contain(2));
        
        Assert.That(source.DeniedEntities, Is.Not.Empty);
        Assert.That(source.DeniedEntities, Does.Not.Contain(1));
        Assert.That(source.DeniedEntities, Does.Contain(2));
        
        source.AddDeniedEntity(1);
        
        Assert.That(source.AllowedEntities, Is.Empty);
        
        Assert.That(source.DeniedEntities, Is.Not.Empty);
        Assert.That(source.DeniedEntities, Does.Contain(1));
        Assert.That(source.DeniedEntities, Does.Contain(2));
    }
}
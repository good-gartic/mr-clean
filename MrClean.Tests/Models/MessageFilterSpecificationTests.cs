using MrClean.Models;
using NUnit.Framework;

namespace MrClean.Tests.Models;

public class MessageFilterSpecificationTests
{
    [Test]
    public void TestBuildingSpecificationFromNull()
    {
        var specification = new MessageFilterSpecification(null);
        
        Assert.IsEmpty(specification.AllowedEntities);
        Assert.IsEmpty(specification.DeniedEntities);
    }

    [Test]
    public void TestBuildingSpecificationFromEmptyString()
    {
        var specification = new MessageFilterSpecification("");
        
        Assert.IsEmpty(specification.AllowedEntities);
        Assert.IsEmpty(specification.DeniedEntities);
    }

    [Test]
    public void TestParsingEntitiesSourceString()
    {
        const string sourceString = "0;~1;2;3;~4";
        var specification = new MessageFilterSpecification(sourceString);

        Assert.AreEqual(3, specification.AllowedEntities.Count);
        Assert.Contains(0, specification.AllowedEntities);
        Assert.Contains(2, specification.AllowedEntities);
        Assert.Contains(3, specification.AllowedEntities);

        Assert.AreEqual(2, specification.DeniedEntities.Count);
        Assert.Contains(1, specification.DeniedEntities);
        Assert.Contains(4, specification.DeniedEntities);
    }

    [Test]
    public void TestImplicitAllow()
    {
        var specification = new MessageFilterSpecification(null);
        
        Assert.IsTrue(specification.AllowsEntity(1));
        Assert.IsTrue(specification.AllowsEntity(2));
        Assert.IsTrue(specification.AllowsEntity(3));
    }

    [Test]
    public void TestExplicitAllow()
    {
        var specification = new MessageFilterSpecification("1;2");
        
        Assert.IsTrue(specification.AllowsEntity(1));
        Assert.IsTrue(specification.AllowsEntity(2));
        Assert.IsFalse(specification.AllowsEntity(3));
    }

    [Test]
    public void TestExplicitAllowAndDeny()
    {
        var specification = new MessageFilterSpecification("1;~2");
        
        Assert.IsTrue(specification.AllowsEntity(1));
        Assert.IsFalse(specification.AllowsEntity(2));
    }

    [Test]
    public void TestImplicitAllowAndDeny()
    {
        var specification = new MessageFilterSpecification("~2");
        
        Assert.IsTrue(specification.AllowsEntity(1));
        Assert.IsFalse(specification.AllowsEntity(2));
        Assert.IsTrue(specification.AllowsEntity(3));
    }
}
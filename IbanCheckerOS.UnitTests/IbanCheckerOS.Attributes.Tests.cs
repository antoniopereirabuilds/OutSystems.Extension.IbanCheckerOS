using System.Reflection;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Guards the ODC contract: OSInterface metadata, icon resource and void/out action signatures.
/// </summary>
public class AttributesTests
{
    [Fact]
    public void Interface_HasOsInterfaceWithNameAndEmbeddedIcon()
    {
        var attr = typeof(IIbanNet).GetCustomAttribute<OSInterfaceAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("IbanCheckerOS", attr.Name);
        Assert.False(string.IsNullOrWhiteSpace(attr.Description));
        using var stream = typeof(IIbanNet).Assembly.GetManifestResourceStream(attr.IconResourceName);
        Assert.NotNull(stream);
    }

    [Fact]
    public void Actions_AreVoidWithDescriptionsAndIcon()
    {
        var iconName = typeof(IIbanNet).GetCustomAttribute<OSInterfaceAttribute>()!.IconResourceName;
        var methods = typeof(IIbanNet).GetMethods();

        Assert.Equal(9, methods.Length);
        Assert.All(methods, m =>
        {
            var a = m.GetCustomAttribute<OSActionAttribute>();
            Assert.NotNull(a);
            Assert.Equal(iconName, a.IconResourceName);
            Assert.False(string.IsNullOrWhiteSpace(a.Description));
            Assert.Equal(typeof(void), m.ReturnType);
            Assert.All(m.GetParameters(), p => Assert.NotNull(p.GetCustomAttribute<OSParameterAttribute>()));
        });
    }

    [Fact]
    public void GenerateTestIbans_DescriptionSaysForTestingOnly()
    {
        var a = typeof(IIbanNet).GetMethod(nameof(IIbanNet.GenerateTestIbans))!.GetCustomAttribute<OSActionAttribute>()!;

        Assert.Contains("for testing only", a.Description, StringComparison.OrdinalIgnoreCase);
    }
}

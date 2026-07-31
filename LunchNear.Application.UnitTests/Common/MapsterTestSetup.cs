namespace LunchNear.Application.UnitTests.Common;

using System.Runtime.CompilerServices;
using LunchNear.Application.Common.Mappings;
using Mapster;

/// <summary>
/// Handlers call the parameterless <c>.Adapt&lt;T&gt;()</c> extension, which reads from
/// <see cref="TypeAdapterConfig.GlobalSettings"/> - the same static config that production
/// populates once at startup via <c>AddApplicationServices()</c>. Test runs need the same
/// one-time registration; a module initializer runs exactly once when this test assembly loads.
/// </summary>
internal static class MapsterTestSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingRegister).Assembly);
    }
}

using Application.Common;
using Domain.Common;
using FluentAssertions;
using Infrastructure.Persistence;
using Optimization;
using NetArchTest.Rules;

namespace Architecture.Tests;

public sealed class LayerDependencyTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Outer_Layers()
    {
        var result = Types.InAssembly(typeof(AggregateRoot).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Application", "Infrastructure", "Optimization", "API", "Worker")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Hosts()
    {
        var result = Types.InAssembly(typeof(Result<>).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Infrastructure", "Optimization", "API", "Worker")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Hosts()
    {
        var result = Types.InAssembly(typeof(ApplicationDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Optimization", "API", "Worker")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    [Fact]
    public void Optimization_Should_Not_Depend_On_Infrastructure_Or_Hosts()
    {
        var result = Types.InAssembly(typeof(Optimization.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Infrastructure", "API", "Worker")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

}

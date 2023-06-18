using Base.Application.Common.Interfaces;
using Base.Infrastructure;
using Base.Infrastructure.Persistence.Interceptors;
using Common.UnitTest.Constants;
using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

public static class ApplicationDbContextFactory
{
    private static DbContextOptions<ApplicationDbContext> _options;
    private static IOptions<OperationalStoreOptions> _operationalStoreOptions;
    private static Mock<IMediator> _mediator;

    static ApplicationDbContextFactory()
    {
        _options = GetOptions();
        _operationalStoreOptions = Options.Create(new OperationalStoreOptions());
        _mediator = new Mock<IMediator>();
    }

    public static ApplicationDbContext CreateDbContext()
    {
        var mockUserService = new Mock<ICurrentUserService>();
        mockUserService.Setup(u => u.UserId).Returns(ContextConstants.UnitTestUserId);

        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor(mockUserService.Object, new Mock<IDateTime>().Object);

        var dbContext = new ApplicationDbContext(_options, _operationalStoreOptions, _mediator.Object, auditableEntitySaveChangesInterceptor);

        return dbContext;
    }

    private static DbContextOptions<ApplicationDbContext> GetOptions()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
        return options;
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Fdsd.Application.Master;
using Fdsd.Domain.Entities;
using Fdsd.Domain.Rules;
using Fdsd.Infrastructure.Persistence;
using Fdsd.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Fdsd.Tests;

public class GakkaChangeServiceTests
{
    private static FdsdDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FdsdDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FdsdDbContext(options);
    }

    private static GakkaChangeService CreateSut(FdsdDbContext db)
    {
        var repo = new GakkaChangeRepository(db);
        var userRepo = new UserRepository(db);
        var gakkaRepo = new GakkaRepository(db);
        var uow = new EfUnitOfWork(db);
        return new GakkaChangeService(repo, userRepo, gakkaRepo, uow);
    }

    [Fact]
    public async Task AddAsync_ClosesPreviousRecord_WhenContinuingMarkedAsMaxDate()
    {
        using var db = CreateContext();
        db.M_USER.Add(new M_User { USERID = 40, EmpName = "テスト" });
        db.T_GAKKA_CHANGE.Add(new T_Gakka_Change
        {
            ID = 1,
            USERID = 40,
            GAKKACD = 1101,
            DateOfArrival = new DateTime(2025, 4, 1),
            DateOfDeparture = DomainRules.MaxDate
        });
        await db.SaveChangesAsync();

        var sut = CreateSut(db);
        await sut.AddAsync(40, 1102, new DateTime(2026, 4, 1), null);

        var prev = db.T_GAKKA_CHANGE.Single(x => x.ID == 1);
        prev.DateOfDeparture.Should().Be(new DateTime(2026, 3, 31));

        var added = db.T_GAKKA_CHANGE.Single(x => x.GAKKACD == 1102);
        added.DateOfDeparture.Should().Be(DomainRules.MaxDate);
    }

    [Fact]
    public async Task AddAsync_ClosesPreviousRecord_WhenContinuingMarkedAsNull()
    {
        using var db = CreateContext();
        db.M_USER.Add(new M_User { USERID = 40, EmpName = "テスト" });
        db.T_GAKKA_CHANGE.Add(new T_Gakka_Change
        {
            ID = 1,
            USERID = 40,
            GAKKACD = 1101,
            DateOfArrival = new DateTime(2025, 4, 1),
            DateOfDeparture = null
        });
        await db.SaveChangesAsync();

        var sut = CreateSut(db);
        await sut.AddAsync(40, 1102, new DateTime(2026, 4, 1), null);

        db.T_GAKKA_CHANGE.Single(x => x.ID == 1).DateOfDeparture
            .Should().Be(new DateTime(2026, 3, 31));
    }
}

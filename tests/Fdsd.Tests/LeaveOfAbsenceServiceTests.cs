using System;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Application.Master;
using Fdsd.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Fdsd.Tests;

public class LeaveOfAbsenceServiceTests
{
    private readonly Mock<ILeaveOfAbsenceRepository> _repo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private LeaveOfAbsenceService CreateSut() => new(_repo.Object, _userRepo.Object, _uow.Object);

    private static T_Leave_Of_Absence NewEntity(short userId = 40) => new()
    {
        ID = 1,
        USERID = userId,
        StartDate = new DateTime(2026, 4, 1),
        EndDate = null
    };

    [Fact]
    public async Task AddAsync_WhenUserExists_PersistsEntity()
    {
        var entity = NewEntity();
        _userRepo.Setup(r => r.GetByIdAsync(entity.USERID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new M_User { USERID = entity.USERID });

        var sut = CreateSut();
        await sut.AddAsync(entity);

        _repo.Verify(r => r.Add(entity), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenUserMissing_ThrowsAndDoesNotPersist()
    {
        var entity = NewEntity(999);
        _userRepo.Setup(r => r.GetByIdAsync(entity.USERID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((M_User?)null);

        var sut = CreateSut();
        var act = () => sut.AddAsync(entity);

        await act.Should().ThrowAsync<NotFoundException>();
        _repo.Verify(r => r.Add(It.IsAny<T_Leave_Of_Absence>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserExists_PersistsEntity()
    {
        var entity = NewEntity();
        _userRepo.Setup(r => r.GetByIdAsync(entity.USERID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new M_User { USERID = entity.USERID });

        var sut = CreateSut();
        await sut.UpdateAsync(entity);

        _repo.Verify(r => r.Update(entity), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserMissing_ThrowsAndDoesNotPersist()
    {
        var entity = NewEntity(999);
        _userRepo.Setup(r => r.GetByIdAsync(entity.USERID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((M_User?)null);

        var sut = CreateSut();
        var act = () => sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<NotFoundException>();
        _repo.Verify(r => r.Update(It.IsAny<T_Leave_Of_Absence>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

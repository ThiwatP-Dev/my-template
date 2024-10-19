using Template.Service.Dto;
using Template.Service.Interfaces;
using Template.Service.src;

namespace Template.Test;

public class InstituteTest : BaseTest
{
    private readonly IInstituteService _service;

    public InstituteTest()
    {
        _service = new InstituteService(_unitOfWork);
    }

    [Fact]
    public async Task Add_ShouldSaveEntity()
    {
        // Arrange
        var request = new CreateInstituteDto
        {
            Name = "test"
        };

        // Act
        var id = await _service.CreateAsync(request, Guid.Empty);

        // Assert
        Assert.NotNull(_dbContext.Institutes.Find(id));
    }

    [Fact]
    public async Task GetAll_ShouldFindEntity()
    {
        // Arrange
        await InitEntity();

        // Act
        var response = await _service.GetAllAsync();

        // Assert
        Assert.Single(response);
    }

    [Fact]
    public async Task Search_ShouldFindEntity()
    {
        // Arrange
        await InitEntity();

        // Act
        var response = await _service.SearchAsync();

        // Assert
        Assert.Equal(1, response.TotalItem);
    }

    [Fact]
    public async Task GetById_ShouldFindEntity()
    {
        // Arrange
        var id = await InitEntity();

        // Act
        var response = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetById_ShouldNotFindEntity()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Task actual() => _service.GetByIdAsync(id);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(actual);
    }

    [Fact]
    public async Task Update_EntityShouldUpdate()
    {
        // Arrange
        var id = await InitEntity();

        // Act
        await _service.UpdateAsync(id, new CreateInstituteDto { Name = "test 2" }, Guid.Empty);
        var response = await _service.GetByIdAsync(id);

        // Assert
        Assert.Equal("test 2", response.Name);
    }

    [Fact]
    public async Task Delete_EntityShouldNotFound()
    {
        // Arrange
        var id = await InitEntity();

        // Act
        await _service.DeleteAsync(id);
        Task actual() => _service.GetByIdAsync(id);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(actual);
    }

    private async Task<Guid> InitEntity()
    {
        var request = new CreateInstituteDto
        {
            Name = "test"
        };

        var id = await _service.CreateAsync(request, Guid.Empty);

        return id;
    }
}
using CustomerCrud.Core;
using CustomerCrud.Repositories;
using CustomerCrud.Requests;

namespace CustomerCrud.Test;

public class CustomersControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<ICustomerRepository> _repositoryMock;

    public CustomersControllerTest(WebApplicationFactory<Program> factory)
    {
        _repositoryMock = new Mock<ICustomerRepository>();

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<ICustomerRepository>(st => _repositoryMock.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAllTest()
    {
        var customers = AutoFaker.Generate<Customer>(3);
        _repositoryMock.Setup(st => st.GetAll()).Returns(customers);

        var response = await _client.GetAsync("/controller");
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().BeEquivalentTo(customers);

        _repositoryMock.Verify(db => db.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        var customer = AutoFaker.Generate<Customer>();
        _repositoryMock.Setup(st => st.GetById(0)).Returns(customer);

        var response = await _client.GetAsync("/controller/0");
        var content = await response.Content.ReadFromJsonAsync<Customer>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().BeEquivalentTo(customer);

        _repositoryMock.Verify(db => db.GetById(0), Times.Once);
    }

    [Fact]
    public async Task CreateTest()
    {
        var request = AutoFaker.Generate<CustomerRequest>();
        _repositoryMock.Setup(st => st.GetNextIdValue()).Returns(1);
        _repositoryMock.Setup(st => st.Create(It.Is<Customer>(r => r.Id == 1))).Returns(true);

        var response = await _client.PostAsJsonAsync("/controller", request);
        var content = await response.Content.ReadFromJsonAsync<Customer>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Id.Should().Be(1);
        content.Name.Should().Be(request.Name);
        content.CPF.Should().Be(request.CPF);
        content.Transactions.Should().BeEquivalentTo(request.Transactions);

        _repositoryMock.Verify(db => db.GetNextIdValue(), Times.Once);
        _repositoryMock.Verify(db => db.Create(It.Is<Customer>(r => r.Id == 1)), Times.Once);
    }

    [Fact]
    public async Task UpdateTest()
    {
        var request = AutoFaker.Generate<CustomerRequest>();
        _repositoryMock.Setup(st => st.Update(1, request)).Returns(true);

        var response = await _client.PutAsJsonAsync("/controller/1", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        content.Should().Be("Customer 1 updated");
        _repositoryMock.Verify(db => db.Update(0, request), Times.Once);
    }

    [Fact]
    public async Task DeleteTest()
    {
        _repositoryMock.Setup(st => st.Delete(1)).Returns(true);

        var response = await _client.DeleteAsync("/controller/1");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _repositoryMock.Verify(db => db.Delete(1), Times.Once);
    }
}
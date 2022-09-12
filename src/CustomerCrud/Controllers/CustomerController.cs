using Microsoft.AspNetCore.Mvc;
using CustomerCrud.Core;
using CustomerCrud.Requests;
using CustomerCrud.Repositories;

namespace CustomerCrud.Controllers;

[ApiController]
[Route("controller")]
public class CustomerController : ControllerBase
{
    private ICustomerRepository CustomerRepository;
    public CustomerController(ICustomerRepository customerRepository)
    {
        CustomerRepository = customerRepository;
    }

    [HttpGet(Name = "GetAll")]
    public ActionResult GetAll()
    {
        var customers = CustomerRepository.GetAll();
        if (customers == null) return NotFound("Customers not found");
        return Ok(customers);
    }
}

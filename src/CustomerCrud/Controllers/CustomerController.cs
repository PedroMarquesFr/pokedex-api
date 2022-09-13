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

    [HttpGet("{id}", Name = "GetById")]
    public ActionResult GetById(int id)
    {
        var customer = CustomerRepository.GetById(id);
        if (customer == null) return NotFound("Customer not found");
        return Ok(customer);
    }

    [HttpPost(Name = "CreateCustomer")]
    public ActionResult CreateCustomer([FromBody] CustomerRequest request)
    {
        int id = CustomerRepository.GetNextIdValue();
        var customer = new Customer(id, request);
        var isCreated = CustomerRepository.Create(customer);

        string actionName = nameof(GetById);
        // var controllerName = "CustomerController";
        var routeValues = new { id, version = "1.0" };

        return CreatedAtAction(actionName, new { id = customer.Id }, customer);
    }

    [HttpPut("{id}", Name = "UpdateCustomer")]
    public ActionResult UpdateCustomer(int id, [FromBody] CustomerRequest request)
    {
        bool customerUpdated = CustomerRepository.Update(id, request);
        if (!customerUpdated) return NotFound("Customer not found");
        return Ok($"Customer {id} updated");
    }

    [HttpDelete("{id}", Name = "DeleteCustomer")]
    public ActionResult Delete(int id)
    {
        bool result = CustomerRepository.Delete(id);
        if (!result) return NotFound("Customer not found");
        return NoContent();
    }
}

using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinAPI;

Dictionary<string, Department> departments = new();
Dictionary<string, Employee> employees = new();


var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<DepartmentDb>(opt => opt.UseInMemoryDatabase("DepsList"));
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/departments", GetDepartments);

app.MapGet("/departments/{id}", GetDepartment);
    

app.MapPost("/departments", PostDepartment);

app.MapPut("/departments/{id}", PutDepartment);

app.MapDelete("/departments/{id}", DeleteDepartment);


app.MapGet("/employees", GetEmployees);

app.MapGet("/employees/{id}", GetEmployee);
   
app.MapPost("/employees", PostEmployee);

app.MapPut("/employees/{id}", PutEmployee);

app.MapDelete("/employees/{id}", DeleteEmployee);

app.UseMiddleware<ApiKeyAuthenticationFilter>();

app.Run();

IResult GetDepartments([FromQuery(Name = "page")] int page = 1,
    [FromQuery(Name = "pageSize")] int pageSize = 5)
{
    var pl = PaginatedList<Department>.ToPaginatedList(departments.Values, page, pageSize);
    var response = new PaginatedResponse<Department>(pl, page);
    return Results.Ok(response);
}
IResult GetDepartment(string id)
{
    return departments.TryGetValue(id, out var dep) ? Results.Ok(dep) : Results.NotFound();
}
IResult PostDepartment(Department dep)
{
    dep.id = RandIdGen.GenerateId();
    departments.Add(dep.id, dep);
    return Results.Created($"/departments/{dep.id}", dep.id);
}

IResult PutDepartment(string id, Department inputDep)
{
    if (!departments.TryGetValue(id, out var dep)) return Results.NotFound();

    dep.name = inputDep.name;

    return Results.NoContent();
}

IResult DeleteDepartment(string id)
{
    return departments.Remove(id) ? Results.NoContent() : Results.NotFound();
}

IResult GetEmployees([FromQuery(Name = "page")] int page = 1,
    [FromQuery(Name = "pageSize")] int pageSize = 5)
{
    var pl = PaginatedList<Employee>.ToPaginatedList(employees.Values, page, pageSize);
    var response = new PaginatedResponse<Employee>(pl, page);
    var json = JsonSerializer.Serialize(response);
    return Results.Ok(response);
}

IResult GetEmployee(string id)
{
    return employees.TryGetValue(id, out var emp) ? Results.Ok(emp) : Results.NotFound();
}

IResult PostEmployee(Employee emp)
{
    if (!departments.ContainsKey(emp.dep_id)) return Results.Conflict();
    emp.id = RandIdGen.GenerateId();
    employees.Add(emp.id, emp);
    return Results.Created($"/departments/{emp.id}", emp.id);
}

IResult PutEmployee(string id, Employee inputEmp)
{
    if (!employees.TryGetValue(id, out var emp)) return Results.NotFound();
    if (!departments.ContainsKey(inputEmp.dep_id)) return Results.Conflict();
    emp.dep_id = inputEmp.dep_id;
    emp.name = inputEmp.name;

    return Results.NoContent();
}

IResult DeleteEmployee(string id)
{
   return employees.Remove(id) ? Results.NoContent() : Results.NotFound();
}
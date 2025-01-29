// 
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Numerics;


// we want to manipulate photo so need to get a random photo for new person
// since it is async we cannot return string we need Task<string>


async Task<string> CreatePhoto(IHttpClientFactory httpClientFactory)

{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetStringAsync($"https://randomuser.me/api/");
    // parse json

    var json = JsonDocument.Parse(response);
    var photoUrl = json.RootElement
    .GetProperty("results")[0]
    .GetProperty("picture")
    .GetProperty("large")
    .GetString();

    return photoUrl ?? "/images/blank.webp";

}
;




var builder = WebApplication.CreateBuilder(args);

//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("allowAll", policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});

builder.Services.AddHttpClient();
var app = builder.Build();

app.UseCors("allowAll");
app.UseStaticFiles();

//routers index
app.MapGet("/", context =>
{
    context.Response.Redirect("/index.html");
    return Task.CompletedTask;
});


app.MapGet("/staff", context =>
{
    context.Response.Redirect("/table.html");
    return Task.CompletedTask;
});

app.MapPost("/staff", async (IHttpClientFactory httpClientFactory, [FromBody] PersonRequest personRequest) =>
{
    if (StaffManager.IsFull)
    {
        return Results.Json(StaffManager.GetStaff());
    }
    try
    {
        var client = httpClientFactory.CreateClient();

        // Bug 1: Typo in API URL
        var response = await client.GetFromJsonAsync<JsonElement>($"https://randomuser.me/ap/?results={personRequest.PersonNumber}");
        var results = response.GetProperty("results").EnumerateArray();

        // Bug 2: Incorrect index usage in Select()
        var newStaff = results.Select((person, index) => new Person
        {
            Id = index, // Should be index + 1
            FirstName = person.GetProperty("name").GetProperty("first").GetString() ?? "Unknown",
            LastName = person.GetProperty("name").GetProperty("last").GetString() ?? "Unknown",
            Email = person.GetProperty("email").GetString() ?? "unknown",
            Phone = person.GetProperty("phone").GetString() ?? "Unknown",
            Location = person.GetProperty("location").GetProperty("city").GetString() ?? "Unknown",
            PhotoUrl = person.GetProperty("picture").GetProperty("large").GetString() ?? "Unknown"
        }).ToList();

        StaffManager.AddStaff(newStaff);
        return Results.Json(newStaff);
    }
    catch (Exception exp)
    {
        return Results.BadRequest(new { error = exp.Message });
    }
});

app.MapPost("/addnewperson", async (IHttpClientFactory httpClientFactory, [FromBody] Person newPerson) =>
{

    try
    {

        int newId = StaffManager.GetStaff().Any() ? StaffManager.GetStaff().Max(p => p.Id) + 1 : 1;
        newPerson.PhotoUrl = await CreatePhoto(httpClientFactory);
        newPerson.Id = newId;

        // Bug 3: Calling a non-existing method
        StaffManager.AddPerson(newPerson); // Should be AddNewStaff
        return Results.Ok(new { Message = "person added" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = ex.Message });
    }
});

app.MapPost("/searchperson", ([FromBody] string firstName) =>
{

    try
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Results.BadRequest(new { Message = "firs name cannot be empty" });
        }
        var matchingFirstNames = StaffManager.FindByName(firstName);
        return Results.Ok(new { matchNames = matchingFirstNames });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = ex.Message });
    }

});

app.MapPost("/searchtext", ([FromBody] SearchRequest request) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.SearchText))
        {
            var allStaff = StaffManager.GetStaff();
            return Results.Ok(new { matchText = allStaff });
        }

        var matchingText = StaffManager.FindByPrefix(request.SearchText);
        return Results.Ok(new { matchText = matchingText });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = ex.Message });
    }
});


//delete a person
app.MapDelete("/deleteperson", ([FromBody] int id) =>
{
    try
    {
        // Bug 4: Incorrect method call (typo in method name)
        StaffManager.RemovePersonById(id); // Should be DeletePersonById
        return Results.Ok(new { Message = "Person deleted" });

    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = ex.Message });
    }

});

//update person 
app.MapPut("/updateperson", ([FromBody] Person updatedPerson) =>
{

    try
    {
        StaffManager.UpdatePersonById(updatedPerson.Id, updatedPerson);
        return Results.Ok(new { Message = "Person updated" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = ex.Message });
    }


});



app.Run();


//person request
public class PersonRequest
{
    public int PersonNumber { get; set; }
}

public class SearchRequest
{
    public string SearchText { get; set; } = "";
}

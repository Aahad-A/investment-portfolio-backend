using System.Linq;
using System.Text.Json;

using final_project_back_end_Aahad_A;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowAllOrigins",
    builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.MapGet("/initialize", () => {

    // using (var context = new RecipeContext())
    // {
    //     context.Database.EnsureDeleted();
    //     context.Database.EnsureCreated();
    // }

    using (var loginContext = new LoginContext())
    {
        loginContext.Database.EnsureDeleted();
        loginContext.Database.EnsureCreated();
    }

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    // string titleJson = File.ReadAllText("recipeTitleData.json");
    // RecipeTitle[]? titles = JsonSerializer.Deserialize<RecipeTitle[]>(titleJson, options);

    // string ingredientJson = File.ReadAllText("recipeIngredientsData.json");
    // RecipeIngredients[]? ingredients = JsonSerializer.Deserialize<RecipeIngredients[]>(ingredientJson, options);

    //Adding new data to tables
    // using (var context = new RecipeContext())
    // {
    //     //Create Recipe Titles
    //     foreach (var title in titles)
    //     {
    //         context.RecipeTitles.Add(title);
    //     }

    //     context.SaveChanges();
        

    //     //Loop through titles and assign id to recipe review
    //     List<RecipeTitle> fromdb = context.RecipeTitles.ToList();
    //     foreach(var item in fromdb)
    //     {
    //         //updating recipeReview to have the recipeTItle id association
    //         foreach(var review in context.RecipeReviews.ToList())
    //         {
    //             if(review.Id == item.Id)
    //             {
    //                 review.RecipeId = item.Id;
    //                 item.Review = review;
    //                 break;
    //             }
    //             continue;
    //         }

    //         //updating ingredients to have recipeTitle id association
    //         //RecipeTitle r = context.RecipeTitles.FirstOrDefault(rt => rt.Title == item.Title);
    //         List<RecipeIngredients> matched = ingredients.Where(ingredient => ingredient.RecipeTitle.ToLower() == item.Title.ToLower()).ToList();
    //         foreach (var ingredient in matched)
    //         {
    //             ingredient.RecipeId = item.Id;
    //             context.RecipeIngredients.Add(ingredient);
    //         }

            
    //     }

    //     context.SaveChanges();
    //     context.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint;");
    // }

    using(var loginContext = new LoginContext())
    {
        Login starter = new Login("brian", "password");
        loginContext.Logins.Add(starter);
        loginContext.SaveChanges();
        loginContext.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint;");
    }
    
}).WithName("Init").WithOpenApi();

app.MapPost("/newUser", (Login newUser) =>{
    using(var context = new LoginContext())
    {
        context.Logins.Add(newUser);
        context.SaveChanges();
        context.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint;");
    }
    return Results.Created($"/newUser/{newUser.Id}", newUser);
}).WithName("PostLogin").WithOpenApi();

app.MapPost("/login", (Login authenticatedUser) => {
    Console.WriteLine("Executing login: " + DateTime.Now.ToShortTimeString());
    using(var context = new LoginContext())
    {
        var user = context.Logins.FirstOrDefault(l => l.Username == authenticatedUser.Username);
        if(user == null){
            return Results.NotFound();
        }
        return Results.Ok(user);
    }
}).WithName("Login").WithOpenApi().RequireAuthorization(new AuthorizeAttribute() {AuthenticationSchemes="BasicAuthentication"});


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

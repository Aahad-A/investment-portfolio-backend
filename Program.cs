using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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

// app.MapGet("/initialize", () =>
// {

//     var options = new JsonSerializerOptions
//     {
//         PropertyNameCaseInsensitive = true
//     };

//     using (var context = new PortfolioContext())
//     {
//         context.Database.EnsureDeleted();
//         context.Database.EnsureCreated();

//         // ADDING HARDCODED INITIAL LOGIN CREDS
//         var starter = new Login("aahad", "password");
//         context.Logins.Add(starter);
//         context.SaveChanges();
//         context.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint;");

//         // ADDING HARDCODED INITIAL PORTFOLIO 
//         var techPortfolio = new Portfolio
//         {
//             PortfolioId = 1,
//             LoginId = starter.Id,
//             Name = "Tech Portfolio",
//             Investments = new List<Investment>
//             {
//                 new Investment
//                 {
//                     InvestmentId = 1,
//                     Name = "Apple Inc.",
//                     Ticker = "AAPL",
//                     Quantity = 10,
//                     PurchasePrice = 150
//                 },
//                 new Investment
//                 {
//                     InvestmentId = 2,
//                     Name = "Microsoft Corp.",
//                     Ticker = "MSFT",
//                     Quantity = 5,
//                     PurchasePrice = 200
//                 }
//             }
//         };

//         context.Portfolios.Add(techPortfolio);
//         context.SaveChanges();
//         context.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint;");
//     }

// }).WithName("Init").WithOpenApi();

app.MapGet("/initialize", () => {

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    Login starter;
    using (var loginContext = new LoginContext())
    {
        loginContext.Database.EnsureDeleted();
        loginContext.Database.EnsureCreated();

        // ADDING HARDCODED INITIAL LOGIN CREDS
        starter = new Login("aahad", "password");
        loginContext.Logins.Add(starter);
        loginContext.SaveChanges();
    }

    using (var portfolioContext = new PortfolioContext())
    {
        portfolioContext.Database.EnsureDeleted();
        portfolioContext.Database.EnsureCreated();
    }
    // ADDING HARDCODED INITIAL PORTFOLIO 

    using (var portfolioContext = new PortfolioContext())
    {
        var techPortfolio = new Portfolio
        {
            PortfolioId = 1,
            Name = "Tech Portfolio",
            Investments = new List<Investment>
            {
                new Investment
                {
                    InvestmentId = 1,
                    Name = "Apple Inc.",
                    Ticker = "AAPL",
                    Quantity = 10,
                    PurchasePrice = 150
                },
                new Investment
                {
                    InvestmentId = 2,
                    Name = "Microsoft Corp.",
                    Ticker = "MSFT",
                    Quantity = 5,
                    PurchasePrice = 200
                }
            }
        };
        var healthPortfolio = new Portfolio
        {
            PortfolioId = 2,
            Name = "Health Portfolio",
            Investments = new List<Investment>
            {
                new Investment
                {
                    InvestmentId = 3,
                    Name = "Pfizer Inc.",
                    Ticker = "PFE",
                    Quantity = 10,
                    PurchasePrice = 40
                },
                new Investment
                {
                    InvestmentId = 4,
                    Name = "Johnson & Johnson",
                    Ticker = "JNJ",
                    Quantity = 5,
                    PurchasePrice = 150
                }
            }
        };

        portfolioContext.Portfolios.Add(techPortfolio);
        portfolioContext.Portfolios.Add(healthPortfolio);
        portfolioContext.SaveChanges();
        portfolioContext.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint;");
    }

}).WithName("Init").WithOpenApi();

app.MapGet("/portfolios", () =>
{
    using (var context = new PortfolioContext())
    {
        var portfolios = context.Portfolios.Include(p => p.Investments).ToList();
        return Results.Ok(portfolios);
    }
}).WithName("GetAllPortfolios").WithOpenApi();

app.MapPost("/portfolios", async (Portfolio newPortfolio) =>
{
    using (var context = new PortfolioContext())
    {
        context.Portfolios.Add(newPortfolio);
        await context.SaveChangesAsync();

        return Results.Created($"/portfolios/{newPortfolio.PortfolioId}", newPortfolio);
    }
}).WithName("CreatePortfolio").WithOpenApi().RequireAuthorization();

app.MapPost("/investments", async (Investment newInvestment) =>
{
    using (var context = new PortfolioContext())
    {
        context.Investments.Add(newInvestment);
        await context.SaveChangesAsync();

        return Results.Created($"/investments/{newInvestment.InvestmentId}", newInvestment);
    }
}).WithName("CreateInvestment").WithOpenApi().RequireAuthorization();

// app.MapGet("/portfolios/{LoginId}", (int id) =>
// {
//     Console.WriteLine("Executing Get Portfolio: " + DateTime.Now.ToShortTimeString());
//     using (var context = new PortfolioContext())
//     {
//         var portfolio = context.Portfolios.FirstOrDefault(p => p.PortfolioId == id);
//         if (portfolio == null)
//         {
//             return Results.NotFound();
//         }
//         return Results.Ok(portfolio);
//     }
// }).WithName("GetUserPortfolio").WithOpenApi();

app.MapPost("/purchase", async (InvestmentUpdate update) =>
{
    using (var context = new PortfolioContext())
    {
        var investment = await context.Investments.FindAsync(update.InvestmentId);
        if (investment == null)
        {
            return Results.NotFound();
        }

        // Update the investment
        investment.Quantity += update.Quantity;
        investment.PurchasePrice = update.Price;

        // Save the changes
        await context.SaveChangesAsync();

        return Results.Ok(investment);
    }
}).WithName("PurchaseInvestment").WithOpenApi().RequireAuthorization(new AuthorizeAttribute() { AuthenticationSchemes = "BasicAuthentication" });

app.MapPut("/sell", async (InvestmentUpdate update) =>
{
    using (var context = new PortfolioContext())
    {
        var investment = await context.Investments.FindAsync(update.InvestmentId);
        if (investment == null)
        {
            return Results.NotFound();
        }

        // Check if the quantity to sell is more than the quantity owned
        if (update.Quantity > investment.Quantity)
        {
            return Results.BadRequest("Quantity to sell cannot be more than quantity owned");
        }

        // Update the investment
        investment.Quantity -= update.Quantity;
        investment.PurchasePrice = update.Price;

        // Save the changes
        await context.SaveChangesAsync();

        return Results.Ok(investment);
    }
}).WithName("SellInvestment").WithOpenApi();

// ---------------------------------------- NEW USER
app.MapPost("/newUser", (Login newUser) =>{
    Console.WriteLine("Executing New UserCreation: " + DateTime.Now.ToShortTimeString());
    using (var context = new LoginContext())
    {
        context.Logins.Add(newUser);
        context.SaveChanges();
        context.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint;");
    }
    return Results.Created($"/newUser/{newUser.Id}", newUser);
}).WithName("PostLogin").WithOpenApi();

// ----------------------------------------------------------- LOGIN EXISTING USER
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



app.Run();



using Microsoft.EntityFrameworkCore;
using MinimalShopingListApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseInMemoryDatabase("ShoppingListApi"));

var app = builder.Build();
app.MapGet("/shopingList",async (ApiDbContext db)=>await db.Groceries.ToListAsync());

app.MapGet("/shopingList/{id}", async (int id, ApiDbContext db) =>
{
    var grocery = await db.Groceries.FindAsync(id);
    return grocery != null ? Results.Ok(grocery) : Results.NotFound();
});

app.MapPost("/shopingList", async (Grocery grocery, ApiDbContext db) =>
{
    db.Groceries.Add(grocery);
    await db.SaveChangesAsync();
    return Results.Created($"/shoppinglist/{grocery.Id}", grocery);
});


app.MapPut("/shopingList/{id}", async (int id, Grocery grocery, ApiDbContext db) =>
{
    var groceryInDb = await db.Groceries.FindAsync(id);
    if (grocery != null)
    {
        groceryInDb.Name=grocery.Name;
        groceryInDb.Purchased=grocery.Purchased;
        await db.SaveChangesAsync();
        return Results.Ok(groceryInDb);
    }
    return Results.NoContent();
});

app.MapDelete("/shopingList/{id}", async (int id, ApiDbContext db) =>
{
    var grocery = await db.Groceries.FindAsync(id);
    if(grocery != null)
    {
        db.Groceries.Remove(grocery);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NoContent();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
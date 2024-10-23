using eShop.Basket.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IShoppingBasketService, ShoppingBasketService>();

builder.AddNpgsqlDbContext<BasketDbContext>("BasketDB");

builder
    .AddGraphQL()
    .AddTypes()
    .AddMutationConventions()
    .AddHttpRequestInterceptor<SessionHttpIntercetpor>()
    .AddGlobalObjectIdentification();

var app = builder.Build();

app.MapGraphQL();

await Seed(app.Services);

app.RunWithGraphQLCommands(args);

async Task Seed(IServiceProvider provider)
{
    var context = provider.CreateScope().ServiceProvider.GetRequiredService<BasketDbContext>();

    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    var basket = ShoppingBasket
        .Create(Guid.Parse("A18E607B-0C2A-410D-B3F5-C589A9776058"), "customerId");

    basket.AddItem(1, 109.99, 1);

    context.Baskets.Add(basket);

    await context.SaveChangesAsync();
}
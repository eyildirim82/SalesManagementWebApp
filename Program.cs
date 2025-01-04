var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnectionFactory>(sp =>
    new DbConnectionFactory(builder.Configuration.GetConnectionString("SalesManagementDB")));

builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<InvoiceRepository>();
builder.Services.AddScoped<InvoiceDetailRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<StockMovementRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); 
}

app.UseHttpsRedirection(); 
app.UseStaticFiles(); 

app.UseRouting(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();

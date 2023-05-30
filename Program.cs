using System.Text;
using LoginService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.UserService;

var builder = WebApplication.CreateBuilder(args);
//string mySecret = Environment.GetEnvironmentVariable("Secret") ?? "none";
//string myIssuer = Environment.GetEnvironmentVariable("Issuer") ?? "none";
string mySecret ="8292811195319825";
string myIssuer = "myIssuer";
var vaultSecrets = new VaultSecrets
{
    vaultSecret = mySecret.ToString(),
    vaultIssuer = myIssuer.ToString()

};
// Add services to the container.
builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = myIssuer.ToString(),
        ValidAudience = "http://localhost",
        IssuerSigningKey =
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret.ToString()))
    };

});
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("UserDatabase"));
builder.Services.AddControllers();
builder.Services.AddSingleton<userService>();
builder.Services.Configure<VaultSecrets>(
    builder.Configuration.GetSection("vaultSecrets")

    );
builder.Services.AddSingleton(vaultSecrets);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QLCHNT.DATA;
using QLCHNT.Entity;
using QLCHNT.Repository;
using QLCHNT.Service.Cart;
using QLCHNT.Service.Category;
using QLCHNT.Service.Product;
using QLCHNT.Service.User;
using System.Text;
using CloudinaryDotNet;
using QLCHNT.Dto.Product;
using QLCHNT.Service.Order;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add Generic Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICategoryServices,CategoryServices>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICartService,CartService>();
builder.Services.AddScoped<IOrderService,OrderService>();


// Add PasswordHasher
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// JWT config
var jwtSetting = builder.Configuration.GetSection("Jwt"); // Gọi config của JWT
var key = Encoding.ASCII.GetBytes(jwtSetting["Key"]);

// Xác thực

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(option =>
{
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSetting["Issuer"],
        ValidAudience = jwtSetting["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(); // Phân quyền


// Cấu hình Swagger UI để nó nhận được JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "QLCHNT",
            Version = "v1"
        }
    );
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập 'Bearer {Token}'"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    //.UseLazyLoadingProxies());


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()   // Cho phép tất cả domain
                .AllowAnyMethod()   // Cho phép tất cả phương thức (GET, POST, PUT, DELETE...)
                .AllowAnyHeader();  // Cho phép tất cả headers
        });
});

builder.Services.Configure<ProductImgCloudinary>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(cloud =>
{
    var config = builder.Configuration.GetSection("CloudinarySettings").Get<ProductImgCloudinary>();
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new CloudinaryDotNet.Cloudinary(account);
});


var app = builder.Build();

app.UseAuthorization();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();

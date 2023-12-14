using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Model;
using blog.common.Repository;
using blog.common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            o.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "The Authorization code to access the API",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };
            var requirement = new OpenApiSecurityRequirement
            {
        { scheme, new List<string>() }
            };
            o.AddSecurityRequirement(requirement);

        });

        builder.Services.ConfigureSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Blog API",
                Version = "v1"
            });
        });

        builder.Services.AddDbContext<DBContextBlog>(o => o.UseInMemoryDatabase("blog"));// UseMySQL("server=localhost;database=blog;user=admin;password=admin"));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPostRepository, PostRepository>();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPostService, PostService>();

        builder.Services.AddScoped<IMapper<PostDB, PostInput>, MapperPostInputToPostDB>();
        builder.Services.AddScoped<IMapper<CommentDB, CommentInput>, MapperCommentInputToCommentDB>();

        builder.Services.AddScoped<CurrentUser>();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
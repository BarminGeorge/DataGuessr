using Microsoft.AspNetCore.Builder;

namespace Tests.Application.Integration;

public class TestEntryPoint
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        TestProgram.ConfigureApplication(builder);
        
        var app = builder.Build();
        TestProgram.ConfigureApp(app);
        
        app.Run();
    }
}
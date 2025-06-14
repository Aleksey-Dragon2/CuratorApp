using Microsoft.AspNetCore.HttpLogging;
using System.Reflection.PortableExecutable;

namespace CuratorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpLogging(opts =>
                opts.LoggingFields = HttpLoggingFields.RequestProperties);

            builder.Logging.AddFilter(
                "Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseHttpLogging();
            }

            app.Run();
        }
    }
}

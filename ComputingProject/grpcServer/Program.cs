using grpcServer.Services;
using InfluxDB.Client;

namespace grpcServer
{
    public class Program
    {

        // TODO Move token to config file
        const string url = "http://localhost:8086";
        const string token = "mGKNA3ucyOT2rnIuxmGYQAbnrYkBGaT0Piiotl2AFurXVbCl8ExjRok5I9IKI5f94prJziCGSezz7J_JQUGkBg==";
        const string org = "defaultOrg";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddSingleton<IInfluxDBClient>(new InfluxDBClient(url, token));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevCorsPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseRouting();
            app.UseCors("DevCorsPolicy");
            app.UseGrpcWeb();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TelemetryDataService>().EnableGrpcWeb();
            });

            app.Run();
        }
    }
}
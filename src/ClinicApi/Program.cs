var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "ClinicApi" }));
app.MapGet("/patients", () => new[] {
    new { Id = 1, Name = "Ana Horvat", Active = true },
    new { Id = 2, Name = "Marko Kovac", Active = true }
});

app.Run();

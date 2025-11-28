using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
var magiconion = builder.Services.AddMagicOnion();
if (builder.Environment.IsDevelopment())
{
    magiconion.AddJsonTranscoding();
    builder.Services.AddMagicOnionJsonTranscodingSwagger();
}
builder.Services.AddSwaggerGen(options => {
    options.IncludeMagicOnionXmlComments(Path.Combine(AppContext.BaseDirectory, "bicycle_racing.Shared.xml"));
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "bicycle_racing",
        Description = "bicycle racing",
    });
});
builder.Services.AddMvcCore().AddApiExplorer();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "bicycle_racing");
    });
}
app.MapMagicOnionService();
app.MapGet("/", () => "");

app.Run();

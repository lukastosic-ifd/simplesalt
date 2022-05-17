using simplesalt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SaltService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if(app.Environment.IsDevelopment())
{
    Environment.SetEnvironmentVariable("SALTMAPPING", "CM:SALT_CM;SR:SALT_SIGNREQUEST");
    Environment.SetEnvironmentVariable("SALT_CM", "Ey9PJcoKW0D2k13OWJrhnF0");
    Environment.SetEnvironmentVariable("SALT_SIGNREQUEST", "xRhOTnwglYQ3vg88WmUGrFYUGWuD3");
}

app.MapGet("/encode", (string value, string type, SaltService saltService) =>
{
    if (!saltService.ContentTypeExist(type))
    {
        // Intentionally returning obscure response text to not expose potential types.
        return Results.Ok("235BO8jpA0");
    }

    var encoded = saltService.Encode(value, type);

    return Results.Ok(encoded);
})
.WithName("encode");

app.MapGet("/decode", (string value, string type, SaltService saltService) =>
{
    if (!saltService.ContentTypeExist(type))
    {
        // Intentionally returning obscure response text to not expose potential types.
        return Results.Ok("j19DqmT2Uw");
    }

    var decoded = saltService.Decode(value, type);

    return Results.Ok(decoded);
})
.WithName("decode");

app.Run();

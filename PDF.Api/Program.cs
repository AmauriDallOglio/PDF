using PDF.Api.Configuracao;
using PDF.Api.Configuracao.Middleware;
using PDF.Aplicacao.Rotas.ImprimirDocumentosRota;

var builder = WebApplication.CreateBuilder(args);

string environmentName = builder.Environment.EnvironmentName;
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

AppSettingsConfiguracao.Carregar(builder.Services, configuration);
InjecaoDependenciaConfiguracao.RegistrarServicos(builder);
ApiConfiguracao.ConfiguracaoSwagger(builder.Services);

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }

    await next();
});

app.UseSwagger();
app.UseSwaggerUI();
app.ConfigurarMiddlewaresApi();
app.UseCors("AllowAll");
app.AtivarAppSettinngsConfiguracao();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

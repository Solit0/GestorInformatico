namespace GestorInformatico.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
    {
        _logger.LogInformation("=== EMAIL SIMULADO ===");
        _logger.LogInformation("Para: {Destinatario}", destinatario);
        _logger.LogInformation("Asunto: {Asunto}", asunto);
        _logger.LogInformation("Cuerpo: {Cuerpo}", cuerpo);
        _logger.LogInformation("=====================");
        return Task.CompletedTask;
    }
}

namespace AgendaApp.Application.Interfaces;

public interface IRabbitMqService
{
    Task SendMessageAsync(string message);
}
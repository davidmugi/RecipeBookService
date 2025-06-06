using System.Text.Json;
using Confluent.Kafka;
using RecipeBookService.DTOs;

namespace RecipeBookService.Services;

public class KafkaRecipeListener : BackgroundService
{
    private readonly IConfiguration _config;

    private readonly ILogger<KafkaRecipeListener> _logger;

    private readonly IServiceProvider _serviceProvider;

    public KafkaRecipeListener(IConfiguration config, ILogger<KafkaRecipeListener> logger, IServiceProvider serviceProvider)
    {
        _config = config;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = _config["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore,string>(consumerConfig).Build();
        consumer.Subscribe(_config["Kafka:ConsumerTopic"]);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));
                    
                    if(consumeResult?.Message.Value != null && !consumeResult.Message.Value.Equals(""))
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var recipeService = scope.ServiceProvider.GetRequiredService<IRecipeService>();
                        var recipeDTO = JsonSerializer.Deserialize<CreateRecipeDTO>(consumeResult.Message.Value);

                        var response = await recipeService.CreateRecipeAsync(recipeDTO);
                    
                        _logger.LogInformation("Kafka message complete for recipe {Name} : with status {Status}",response.Data.Title,response.StatusCode);
                    }
                    
                    await Task.Delay(50, stoppingToken);
                }
                catch (Exception exception)
                {
                   _logger.LogError(exception,"An error occured while processing kafka message");
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
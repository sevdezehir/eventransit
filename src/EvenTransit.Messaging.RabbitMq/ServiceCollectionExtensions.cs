using System;
using System.Reflection;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            services.AddScoped<IEventConsumer, EventConsumer>();
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.AddScoped<IRabbitMqDeclaration, RabbitMqDeclaration>();
            services.AddScoped(typeof(IConnectionFactory), provider =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    Uri = new Uri(configuration["RabbitMq:Endpoint"]),
                    AutomaticRecoveryEnabled = true
                };

                return connectionFactory;
            });

            services.AddHealthChecks()
                .AddCheck<RabbitMqHealthCheck>("rabbitmq");
            
            return services;
        }
    }
}
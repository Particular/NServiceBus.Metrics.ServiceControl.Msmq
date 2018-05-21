using System;

namespace SampleEndpoint
{
    using System.Threading;
    using NServiceBus;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
    using NServiceBus.Metrics.ServiceControl;

    class Program
    {
        static void Main(string[] args)
        {
            var config = new BusConfiguration();
            config.EndpointName("SomeName");
            config.SendMetricDataToServiceControl("Particular.Monitoring");
            config.UsePersistence<InMemoryPersistence>();

            using (var bus = Bus.Create(config).Start())
            {
                Console.WriteLine("Bus Started");

                while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                {
                    bus.SendLocal(new SomeMessage());
                }

                Console.WriteLine("Bus Stopped");
            }
        }
    }

    class SomeMessage : ICommand
    {

    }

    class SomeMessageHandler : IHandleMessages<SomeMessage>
    {
        public void Handle(SomeMessage message)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }


    class AuditConfigProvider : IProvideConfiguration<AuditConfig>
    {

        public AuditConfig GetConfiguration() => new AuditConfig
        {
            QueueName = "audit"
        };
    }

    class ErrorConfigProvider : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    {
        public MessageForwardingInCaseOfFaultConfig GetConfiguration() => new MessageForwardingInCaseOfFaultConfig
        {
            ErrorQueue = "error"
        };
    }
}

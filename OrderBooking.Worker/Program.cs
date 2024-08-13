using System.Reflection;
using Azure.Core.Pipeline;
using MessageHandler.Runtime;
using MessageHandler.Runtime.AtomicProcessing;
using MessageHandler.Runtime.Licensing;
using OrderBooking.Api.Commands;
using OrderBooking.Events;
using OrderBooking.Worker;

var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddHostedService<Worker>();

builder.Services.AddMessageHandler(nameof(OrderBooking.Worker), handlerRuntime =>
{
    handlerRuntime.License(builder.Configuration["LicenseToken"]);
    var serviceBusConnection = builder.Configuration["ServiceBusConnection"];
    handlerRuntime.AtomicProcessingPipeline(pipeline =>
    {
        pipeline.PullMessagesFrom(p => p.Topic(name: "asynchandler-topic", subscription: "orderbooking.worker", serviceBusConnection));
        pipeline.DetectTypesInAssembly(typeof(BookingStarted).Assembly);
        pipeline.HandleMessagesWith<SendEmailNotification>();
    });
});



var emailSettings = builder.Configuration.GetSection(EmailSettings.Section).Get<EmailSettings>() ?? 
throw new Exception("Email settings not provided");

builder.Services.AddFluentEmail(emailSettings.FromAddress)
.AddSmtpSender(
    emailSettings.Smtp,
    emailSettings.Port,
    emailSettings.Username,
    emailSettings.Password);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.Section));

builder.Services.AddSingleton<IEmailService>(sp => new EmailService(sp));

var host = builder.Build();
host.Run();

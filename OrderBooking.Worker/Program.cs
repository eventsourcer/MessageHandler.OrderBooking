using OrderBooking.Worker;

var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddHostedService<Worker>();

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

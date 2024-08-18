using FluentEmail.Core;
using Microsoft.Extensions.Options;

namespace OrderBooking.Worker;

public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly EmailSettings _settings;

        public EmailService(IServiceProvider serviceProvider)
        {
            _fluentEmail = serviceProvider.GetRequiredService<IFluentEmail>();
            _settings = serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
        }

        public async Task SendAsync(string errorMessage)
        {
            var result = await _fluentEmail.To(_settings.ToList)
            .Subject(_settings.Subject)
            .Body($@"{_settings.Body}{errorMessage}")
            .SendAsync();
        }
        public async Task SendAsync(string sellerEmail, string buyeremail, string subject, string body)
        {
            var result = await _fluentEmail.SetFrom(sellerEmail)
            .To(buyeremail)
            .Subject(subject)
            .Body($@"{body}")
            .SendAsync();
        }
    }
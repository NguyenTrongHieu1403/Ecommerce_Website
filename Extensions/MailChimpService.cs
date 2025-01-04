using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using Microsoft.Extensions.Options;
using ecommerce_final.Models;

public class MailChimpService
{
    private readonly MailChimpSettings _mailChimpSettings;
    private readonly IMailChimpManager _mailChimpManager;

    public MailChimpService(IOptions<MailChimpSettings> mailChimpSettings)
    {
        _mailChimpSettings = mailChimpSettings.Value;
        _mailChimpManager = new MailChimpManager(_mailChimpSettings.ApiKey);
    }



    public async Task SendEmailAsync(string subject, string body, List<string> emailAddresses)
    {
        try
        {
            var campaign = new Campaign
            {
                Type = CampaignType.Regular,
                Recipients = new Recipient
                {
                    ListId = _mailChimpSettings.ListId
                },
                Settings = new Setting
                {
                    SubjectLine = subject,
                    FromName = "LobbyStu", // Thay bằng tên của bạn
                    ReplyTo = "lobbystu.education@gmail.com" // Email nhận phản hồi
                }
            };

            var createdCampaign = await _mailChimpManager.Campaigns.AddAsync(campaign);
            await _mailChimpManager.Content.AddOrUpdateAsync(createdCampaign.Id, new ContentRequest
            {
                Html = body
            });


            await _mailChimpManager.Campaigns.SendAsync(createdCampaign.Id);
        }
        catch (MailChimpException ex)
        {
            Console.WriteLine($"Lỗi khi tạo hoặc gửi chiến dịch: {ex.Detail}");
            throw;
        }
    }

}
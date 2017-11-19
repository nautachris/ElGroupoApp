//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Options;
//using ElGroupo.Web.Models.Configuration;
//using RestSharp;
//using RestSharp.Authenticators;
//namespace ElGroupo.Web.Services
//{
//    public class MailgunEmailSender : IEmailSender
//    {
//        public EmailConfigOptions Options { get; }
//        public MailgunEmailSender(IOptions<EmailConfigOptions> optionsAccessor)
//        {
//            Options = optionsAccessor.Value;
//        }

//        public class MailgunResponse
//        {
//            public string Id { get; set; }
//            public string Message { get; set; }
//        }
//        public async Task<EmailResponse> SendEmailAsync(string email, string subject, string message)
//        {
//            var client = new RestClient("https://api.mailgun.net/v3");
//            client.Authenticator = new HttpBasicAuthenticator("api", this.Options.MailgunKey);

//            var request = new RestRequest();
//            request.AddParameter("domain", this.Options.MailgunDomain, ParameterType.UrlSegment);
//            request.Resource = "{domain}/messages";
//            request.AddParameter("from", "ElGroupo <elgroupo@" + this.Options.MailgunDomain + ">");
//            request.AddParameter("to", email);
//            request.AddParameter("subject", subject);
//            request.AddParameter("text", message);
//            request.AddParameter("html", message);
//            request.Method = Method.POST;

//            //var resp = client.Execute(request);

//            return await Task.Run(() =>
//             {
//                 var t = new TaskCompletionSource<EmailResponse>();
//                 var handle = client.ExecuteAsync<MailgunResponse>(request, response =>
//                 {
//                     var res = new EmailResponse { Message = response.Data.Message };
//                     if (string.IsNullOrEmpty(response.Data.Id))
//                     {
//                         res.Success = false;
//                     }
//                     else
//                     {
//                         res.Success = true;
//                     }
//                     t.TrySetResult(res);

//                 });
//                 return t.Task;
//             });


//        }
//    }
//}

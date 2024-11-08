namespace Portail_OptiVille.Data.Utilities
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using System.Net;
    using System.Net.Mail;

    public class MailManager
    {
        private readonly DefaultMail _defaultMail;

        // Constructeur avec injection des paramètres de configuration
        public MailManager(IOptions<DefaultMail> defaultMail)
        {
            _defaultMail = defaultMail.Value;

        }
        public void SendMail(string _destinataire, string _objet, object _contenuMail)
        {
            // Envoie de mail
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_defaultMail.MailAddress);
            mail.To.Add(_destinataire);
            mail.Subject = _objet;
            mail.Body = _contenuMail.ToString();
            mail.IsBodyHtml = true;


            var client = new SmtpClient(_defaultMail.SmtpAddr, _defaultMail.SmtpPort)
            {
                Credentials = new System.Net.NetworkCredential(_defaultMail.CredMail, _defaultMail.CredPassApp),
                EnableSsl = true
                //EnableSsl = _sslEnable ?? _defaultMail.EnableSsl
            };
            client.Send(mail);
        }

        public class DefaultMail
        {
            public string MailAddress { get; set; }
            public string CredMail { get; set; }
            public string CredPassApp { get; set; }
            public string SmtpAddr { get; set; }
            public int SmtpPort { get; set; }
            public bool EnableSsl { get; set; }
        }
    }
}

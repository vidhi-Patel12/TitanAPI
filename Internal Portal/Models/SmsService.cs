using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Internal_Portal.Models
{
    public class SmsService
    {
        private const string AccountSid = "AC3adf91fc2934dbb9d2cfca2aa2fd367f";
        private const string AuthToken = "9b6fd135e7aaf9cec12de1f93e45fbdb";
        private const string FromPhoneNumber = "+18666979815"; // Twilio Sandbox Number

        public bool SendSmsOTP(long mobile, int otp)
        {
            try
            {
                TwilioClient.Init(AccountSid, AuthToken);

                string formattedMobile = $"+91{mobile}";

                var message = MessageResource.Create(
                    from: new PhoneNumber(FromPhoneNumber), // Twilio SMS number
                    to: new PhoneNumber(formattedMobile),   // Recipient phone number
                    body: $"Your OTP for login is: {otp}. It is valid for 5 minutes. Do not share this with anyone."
                );

                Console.WriteLine($"OTP sent to WhatsApp: {formattedMobile} (Message SID: {message.Sid})");
                return true;
            }
            catch (Twilio.Exceptions.ApiException ex)
            {
                Console.WriteLine($"Twilio API Exception: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return false;
            }
        }
    }
}

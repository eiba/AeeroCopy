using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeero.Models;
using Twilio;

namespace Aeero.Areas.Customer
{
    public class Twilio
    {
        public static string SendMessage(string to, string msg)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var contactEtc = db.Contact.First();
            TwilioRestClient twilio = new TwilioRestClient(contactEtc.TwilioAccountSid, contactEtc.TwilioAuthToken);

            if (!to.StartsWith("+47"))
            {
                to = "+47" + to;
            }
            if (to.Length != 11)
            {
                throw new InvalidOperationException("Invalid phone number");
            }

            var message = twilio.SendMessage("+15005550006", to, msg);

            if (message.RestException != null)
            {
                var error = message.RestException.Message;
                throw new InvalidOperationException(error);
            }

            return message.Sid;
        }
    }
}
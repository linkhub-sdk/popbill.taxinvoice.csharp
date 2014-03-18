using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace Linkhub
{
    public class auth
    {
        
        private const String URL = "https://demo.innopost.com";
        private String ServiceID;
        private String PartnerID;
        private String SecretKey;
        private String access_id;
        private List<String> scope;

        public auth(String ServiceID, String PartnerID, String SecretKey, String access_id, List<String> scope)
        {
            this.ServiceID = ServiceID;
            this.PartnerID = PartnerID;
            this.SecretKey = SecretKey;
            this.access_id = access_id;
            this.scope = scope;
        }

        private Token __token = null;

        private Token getToken()
        {
            if (__token == null || __token.IsExpired)
            {

                WebClient client = new WebClient();

                Uri uri = new Uri(URL + "/" + ServiceID + "/Token");

                TokenRequest T = new TokenRequest();

                T.access_id = access_id;
                T.scope = scope;

                String body = JsonConvert.SerializeObject(T);


                String date = DateTime.UtcNow.ToString("r") + "Z";


                String SignTarget = "POST\n";
                SignTarget += md5base64(body) + "\n";
                SignTarget += date + "\n";
                SignTarget += "/" + ServiceID + "/Token";

                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Authorization", "LINKHUB " + PartnerID + " " + hmacsha1_base64(SecretKey, SignTarget));
                client.Headers.Add("x-lh-date", date);


                String result = null;

                try
                {
                    byte[] btResult = client.UploadData(uri, "POST", Encoding.UTF8.GetBytes(body));

                    result = Encoding.UTF8.GetString(btResult);
                }
                catch (WebException we)
                {
                    BaseResponse response;

                    if (we.Response != null)
                    {
                        StreamReader sr = new StreamReader(we.Response.GetResponseStream());
                        response = JsonConvert.DeserializeObject<BaseResponse>( sr.ReadToEnd());
                    }
                    else
                    {
                        response = new BaseResponse();
                        response.message = we.Message;
                        response.code = -0;
                    }

                    response.throwException();
                }

                try
                {
                    __token = JsonConvert.DeserializeObject<Token>(result);

                }
                catch (Exception E)
                {
                    throw new LinkhubException(-0,E.Message);
                }
            }

            return __token;
        }

        public String getSession_Token()
        {
            return getToken().session_token;
        }

        private String md5base64(String input)
        {
            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
        private String hmacsha1_base64(String key, String input)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1(Convert.FromBase64String(key));

            return Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        private class TokenRequest
        {
            public String access_id;
            public List<String> scope;

        }

        private class Token
        {
            public String session_token = null;
            public String serviceID = null;
            public String partnerID = null;
            public String usercode = null;
            public List<String> scope = null;
            public String ipaddress = null;
            public String expiration = null;

            public bool IsExpired
            {
                get
                {
                    DateTime expiredDT = DateTime.Parse(this.expiration);
                    return DateTime.Compare(DateTime.Now, expiredDT) > 1;

                }
            }

        }

       
    }
}

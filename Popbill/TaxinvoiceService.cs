using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Linkhub;
using Newtonsoft.Json;
using System.Collections.Specialized;


namespace Popbill
{
    public class TaxinvoiceService
    {

        #region variables
        private Linkhub.auth auth;

       
        private String ServiceID_REAL = "POPBILL";
        private String ServiceID_TEST = "POPBILL_STAGE";
        //private String ServiceID_TEST = "POPBILL_TEST";

        private String ServiceURL_REAL = "https://popbill.linkhub.co.kr";
        private String ServiceURL_TEST = "https://dev-api.innopost.com";
        //private String ServiceURL_TEST = "https://popbill_test.linkhub.co.kr";

        private bool isTest=false;
        private String partnerID;
        private String secretKey;
      

        #endregion

        #region properties

        private auth Auth
        {
            get
            {
                if (auth == null)
                {
                    auth = new Linkhub.auth(ServiceID, partnerID, secretKey, access_id, new List<string> { "member", "110" });
                }
                return auth;
            }
        }

        public bool IsTest
        {
            get { return isTest; }
            set { isTest = value; }
        }

        private String ServiceID
        {
            get
            {
                return isTest ? ServiceID_TEST : ServiceID_REAL;
            }
        }
        private string ServiceURL
        {
            get
            {
                return isTest ? ServiceURL_TEST : ServiceURL_REAL;
            }
        }


        #endregion

        #region Constructor
        public TaxinvoiceService( String PartnerID, String SecretKey)
        {
            this.partnerID = PartnerID;
            this.secretKey = SecretKey;
        }

        #endregion

        public URLResponse GetPopbillURL(String CorpNum, String UserID, String Togo)
        {

            NameValueCollection headers = new NameValueCollection();

            headers.Add("x-pb-userid", UserID);

            NameValueCollection queries = new NameValueCollection();

            queries.Add("TG", Togo);

            String result = GetRes("/Taxinvoice", headers, queries);

            URLResponse response = JsonConvert.DeserializeObject<URLResponse>(result);

            return response;

        }

        private String GetRes(String URL , NameValueCollection headers, System.Collections.Specialized.NameValueCollection QueryStrings)
        {

            WebClient client = new WebClient();

            Uri uri = new Uri(ServiceURL + URL);

            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("Authorization", "Bearer " +  Auth.getSession_Token());

            if(headers != null) client.Headers.Add(headers);

            if (QueryStrings != null)
            {
                client.QueryString.Add(QueryStrings);
            }

            String result = null;

            try
            {

                return client.DownloadString(uri);

            }
            catch (WebException we)
            {
                BaseResponse response;

                if (we.Response != null)
                {
                    StreamReader sr = new StreamReader(we.Response.GetResponseStream());
                    response = JsonConvert.DeserializeObject<BaseResponse>(sr.ReadToEnd());
                }
                else
                {
                    response = new BaseResponse();
                    response.message = we.Message;
                    response.code = -0;
                }

                response.throwException();
            }

            return result;

        }
    }
}

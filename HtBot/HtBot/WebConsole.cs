using MinecraftClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace HtBot.HtBot
{
    class WebConsole
    {
        private Dictionary<String, String> SwListMessage = new Dictionary<String, String>();
        private Thread _thread;

        private int c = 0;

        public void process(String text)
        {
            c++;

            SwListMessage.Add("NULL" + c, text);

            SwStartThreadConsoleWeb();
        }

        public void SwStartThreadConsoleWeb()
        {
            if (_thread == null)
            {
                _thread = new Thread(SwRunnableConsoleWeb);
                _thread.Start();
            }
        }

        private void SwRunnableConsoleWeb(Object text)
        {
            while (true)
            {
                Dictionary<String, String> SwMessagesToPorcess = new Dictionary<String, String>(SwListMessage);

                if (SwMessagesToPorcess.Count > 0)
                {
                    SwListMessage.Clear();

                    JArray array = new JArray();

                    foreach (var kvp in SwMessagesToPorcess)
                    {
                        array.Add(kvp.Value);
                    }

                    JObject o = new JObject();

                    o["message"] = array;

                    String json = o.ToString();


                    try
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
                        var request = (HttpWebRequest)WebRequest.Create("https://cloudmc.snowdev.com.br/api/consolenewmessage");
                        request.Headers.Add("Authorization", "SnowRunescape");

                        var postData = "message=" + json;

                        var data = Encoding.UTF8.GetBytes(postData);

                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = data.Length;

                        using (var stream = request.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }

                        // Get the response.  
                        WebResponse response = request.GetResponse();

                        // Get the stream containing content returned by the server.  
                        Stream dataStream = response.GetResponseStream();

                        // Open the stream using a StreamReader for easy access.  
                        StreamReader reader = new StreamReader(dataStream);

                        reader.Close();
                        response.Close();
                    }
                    catch (System.Exception e)
                    {
                        ConsoleIO.WriteLineFormatted("[ERROR] " + e.StackTrace);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }

    }
}

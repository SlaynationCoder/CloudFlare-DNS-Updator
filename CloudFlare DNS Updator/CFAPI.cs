using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace CloudFlare_DNS_Updator
{
    class CFAPI
    {
        public string DOMAIN;
        public string KEY;
        public string EMAIL;

        public CFAPI(string domain, string key, string email)
        {
            DOMAIN = domain;
            KEY = key;
            EMAIL = email;
        }

        public dynamic getDNSINFO()
        {
            string url = "https://www.cloudflare.com/api_json.html";

            System.Net.WebClient wc = new System.Net.WebClient();
            //NameValueCollectionの作成
            System.Collections.Specialized.NameValueCollection ps = new System.Collections.Specialized.NameValueCollection();
            //送信するデータ（フィールド名と値の組み合わせ）を追加
            ps.Add("a", "rec_load_all");
            ps.Add("tkn", KEY);
            ps.Add("email", EMAIL);
            ps.Add("z", DOMAIN);
            //データを送信し、また受信する
            byte[] resData = wc.UploadValues(url, ps);
            wc.Dispose();

            //受信したデータを表示する
            string resText = System.Text.Encoding.UTF8.GetString(resData);

            var model = new JavaScriptSerializer().Deserialize<dynamic>(resText);

            for (int i = 0; i < model["response"]["recs"]["count"]; i++)
            {
                string type = model["response"]["recs"]["objs"][i]["type"];

                if (type == "A")
                {
                    return new
                    {
                        rec_id = model["response"]["recs"]["objs"][i]["rec_id"],
                        content = model["response"]["recs"]["objs"][i]["content"],
                        name = model["response"]["recs"]["objs"][i]["name"],
                        service_mode = model["response"]["recs"]["objs"][i]["service_mode"],
                        ttl = model["response"]["recs"]["objs"][i]["ttl"]
                    };
                }
                    

            }

            return null;
        }


        public bool UpdateDNSIP(string rec_id, string IP, string name, string service_mode, string ttl)
        {
            string url = "https://www.cloudflare.com/api_json.html";

            System.Net.WebClient wc = new System.Net.WebClient();
            //NameValueCollectionの作成
            System.Collections.Specialized.NameValueCollection ps = new System.Collections.Specialized.NameValueCollection();
            //送信するデータ（フィールド名と値の組み合わせ）を追加
            ps.Add("a", "rec_edit");
            ps.Add("tkn", KEY);
            ps.Add("id", rec_id);
            ps.Add("email", EMAIL);
            ps.Add("z", DOMAIN);
            ps.Add("type", "A");
            ps.Add("name", name);
            ps.Add("content", IP);
            ps.Add("service_mode", service_mode);
            ps.Add("ttl", ttl);

            //データを送信し、また受信する
            byte[] resData = wc.UploadValues(url, ps);
            wc.Dispose();
            string resText = System.Text.Encoding.UTF8.GetString(resData);
            Clipboard.SetText(resText);
            return false;
        }


    }
}

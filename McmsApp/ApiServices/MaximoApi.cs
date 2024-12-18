using System.Diagnostics;
using System.Text;
using McmsApp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace McmsApp.ApiServices
{
    public class MaximoApi : IMaximoApi
    {
        private RestClient client;
        string hostname { get; set; }
        public MaximoApi()
        {
            _ = loadHost();
        }

        async Task loadHost()
        {
            hostname = await SecureStorage.GetAsync("Hostname");
            hostname = string.Concat(hostname, "/maximo/oslc");
            //hostname = string.Concat(hostname, "/maxrest/oslc");
            client = new RestClient(hostname);
            client.Timeout = 60000;
        }

        public async Task<Apikey> Login(string username, string password)
        {
            client.Timeout = 60000;
            var authData = string.Format("{0}:{1}", username, password);
            var maxauth = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            var request = new RestRequest($"apitoken/create?lean=1", Method.POST);
            //client.Authenticator = new HttpBasicAuthenticator(username, password);
            request.AddHeader("Content-Type", "text/plain");
            request.AddHeader("maxauth", maxauth);
            //request.AddHeader("Authorization", "BASIC" + " "+client.Authenticator);
            request.AddJsonBody(new { expiration = -1 });
            var response = await client.ExecuteAsync<Apikey>(request);
            return response.Data;
        }

        public async Task<bool> Ping()
        {
            client.Timeout = 2000;
            var request = new RestRequest("ping", Method.GET);
            var response = await client.ExecuteAsync(request);
            Debug.WriteLine(response.ResponseStatus);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<RespDownloadWorkorder> GetWorkorder(string wonum, bool isattachment)
        {
            string doclinks = "doclinks{doclinksid,urltype,changeby,document,createdate,description,changedate,ownerid,ownertable,docinfoid,doctype,createby,weburl,getlatestversion,printthrulink,urlname,reference}";
            string paramwo = "";
            if (wonum != null)
            {
                paramwo = $" and wonum=%22{wonum}%22";
            }
            client.Timeout = 200000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var username = await SecureStorage.GetAsync("username");

            var request = new RestRequest("os/tnb_xam_wo?lean=1&inlinedoc=1&oslc.select=wonum,description,status,statusdate,worktype,workorderid,tnbvertical,tnbsubvertical,tnbworkscope,fcprojectid,fctaskid,tnbprojectnumber,tnbprojectphase,parent,tnbvoltagelevel,tnbbusinessarea,tnbzone,tnbstate,tnbsubzone,tnbstation,tnbflsubstationtype,location,lead,siteid,tnbpsiwochecklisttype,assetnum,schedstart,schedfinish,locations{locationsid,description,locationspec},asset{assetid,assetuid,assetnum,description,tnbmodelnum,serialnum,tnbmanufacturerpart,manufacturer,tnbmanufacturingcountry,tnbbrand,tnbitemnum,itemnum,tnbmanufdate,tnbdeliverydate,tnbvoltagelevel,tnbassetcondition,assetspec},workorderspec,plusgaudit{*," + doclinks + "},plusgpermitwork{*," + doclinks + "},worklog,tnbwometergroup," + doclinks + "&oslc.where=status!=%22[COMP,MPIATCOMP,TECO,PIATCOMP,SITEREADY,MSITEREADY,MSITERDCOMP]%22 and assignment.laborcode=%22" + username + "%22" + paramwo, Method.GET);
            if (isattachment)
            {
                request = new RestRequest("os/tnb_xam_wo?lean=1&inlinedoc=1&oslc.select=wonum,description,status,statusdate,worktype,workorderid,tnbvertical,tnbsubvertical,tnbworkscope,fcprojectid,fctaskid,tnbprojectnumber,tnbprojectphase,parent,tnbvoltagelevel,tnbbusinessarea,tnbzone,tnbstate,tnbsubzone,tnbstation,tnbflsubstationtype,location,lead,siteid,tnbpsiwochecklisttype,assetnum,schedstart,schedfinish,locations{locationsid,description,locationspec},asset{assetid,assetuid,assetnum,description,tnbmodelnum,serialnum,tnbmanufacturerpart,manufacturer,tnbmanufacturingcountry,tnbbrand,tnbitemnum,itemnum,tnbmanufdate,tnbdeliverydate,tnbvoltagelevel,tnbassetcondition,assetspec},workorderspec,plusgaudit,plusgpermitwork,worklog,tnbwometergroup,doclinks&oslc.where=status!=%22[COMP,MPIATCOMP,TECO,PIATCOMP,SITEREADY,MSITEREADY,MSITERDCOMP]%22 and assignment.laborcode=%22" + username + "%22" + paramwo, Method.GET);
            }
            request.AddHeader("apikey", apikey);
            var response = await client.ExecuteAsync<RespDownloadWorkorder>(request);
            Debug.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }
        //downloading single file
        public async Task<Doclinks> GetDoclinks(int? docid)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_attachment/{docid}?lean=1&inlinedoc=1&oslc.select=documentdata", Method.GET);
            request.AddHeader("apikey", apikey);
            var response = await client.ExecuteAsync<Doclinks>(request);
            Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<RespDownloadWorkorder> GetParentWorkorder(string parents, bool isattachment)
        {
            client.Timeout = 200000;
            string doclinks = "doclinks{doclinksid,urltype,changeby,document,createdate,description,changedate,ownerid,ownertable,docinfoid,doctype,createby,weburl,getlatestversion,printthrulink,urlname}";
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest("os/tnb_xam_wo?lean=1&inlinedoc=1&oslc.select=wonum,description,status,statusdate,worktype,workorderid,tnbvertical,tnbsubvertical,tnbworkscope,tnbprojectphase,parent,tnbvoltagelevel,tnbbusinessarea,tnbzone,tnbstate,tnbsubzone,tnbstation,tnbflsubstationtype,location,lead,siteid,tnbwochecklisthdr,plusgaudit{*," + doclinks + "},plusgpermitwork{*," + doclinks + "},worklog,tnbwometergroup," + doclinks + "&oslc.where=wonum in " + parents, Method.GET);

            if (isattachment)
            {
                request = new RestRequest($"os/tnb_xam_wo?lean=1&inlinedoc=1&oslc.select=wonum,description,status,statusdate,worktype,workorderid,tnbvertical,tnbsubvertical,tnbworkscope,tnbprojectphase,parent,tnbvoltagelevel,tnbbusinessarea,tnbzone,tnbstate,tnbsubzone,tnbstation,tnbflsubstationtype,location,lead,siteid,tnbwochecklisthdr,plusgaudit,plusgpermitwork,worklog,tnbwometergroup,doclinks&oslc.where=wonum in {parents}", Method.GET);
            }
            request.AddHeader("apikey", apikey);
            var response = await client.ExecuteAsync<RespDownloadWorkorder>(request);
            Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<RespDownloadWorkorder> GetWorkorderQuery(string select, string where)
        {
            client.Timeout = 200000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_wo?lean=1&oslc.select={select}&oslc.where=" + where, Method.GET);
            request.AddHeader("apikey", apikey);
            var response = await client.ExecuteAsync<RespDownloadWorkorder>(request);
            Debug.WriteLine("tos" + response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<byte[]> DownloadImage(string id)
        {
            //Debug.WriteLine("ini : id "+id);
            client.Timeout = 200000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"images/{id}", Method.GET);
            request.AddHeader("apikey", apikey);
            byte[] response = client.DownloadData(request);
            //Debug.WriteLine("ini : base " + Convert.ToBase64String(response));
            return response;
        }


        public async Task<UserProfile> whoAmI()
        {
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"whoami?lean=1", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            var response = await client.ExecuteAsync<UserProfile>(request);
            Console.WriteLine(response.Content);
            return response.Data;
        }

        public async Task<ResPerson> GetPersons()
        {
            client.Timeout = 200000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_person?oslc.select=personuid,personid,displayname&lean=1", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            var response = await client.ExecuteAsync<ResPerson>(request);
            Debug.WriteLine("ini tt" + response.Content);
            return response.Data;
        }

        public async Task<ResSQA> GetTemplateSQA()
        {
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_sqa?lean=1&oslc.where=template=true&oslc.select=*", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            var response = await client.ExecuteAsync<ResSQA>(request);
            Console.WriteLine(response.Content);
            return response.Data;
        }

        public async Task<ChildWORest> GetChildWO(string parent)
        {
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/mxwo?lean=1&oslc.select=*&oslc.where=parent=%22{parent}%22 and istask=false", Method.GET);
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            var response = await client.ExecuteAsync<ChildWORest>(request);
            return response.Data;
        }

        public async Task<Plusgaudit> AddSQA(Plusgaudit body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_sqa?lean=1&inlinedoc=1&oslc.where=template=true&oslc.select=*", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            request.AddHeader("properties", "*");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            Console.WriteLine(json);
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Plusgaudit>(request);
            Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<Plusgaudit> UpdateSQA(Plusgaudit body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_sqa/{body.plusgauditid}?lean=1&inlinedoc=1&oslc.where=template=true&oslc.select=*", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine("SQA Update " + json);
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Plusgaudit>(request);

            //Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<PermitWork> AddPermit(PermitWork body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_permit?lean=1&inlinedoc=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            request.AddHeader("properties", "*");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            Console.WriteLine("Add Permit : " + json);
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<PermitWork>(request);
            Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<Doclinks> AddAttachmnt(Doclinks body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_attachment?lean=1&inlinedoc=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            Console.WriteLine("Add Attachment" + json);
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Doclinks>(request);
            Debug.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<PermitWork> UpdatePermit(PermitWork body)
        {

            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_permit/{body.plusgpermitworkid}?lean=1&inlinedoc=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });

            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<PermitWork>(request);

            Debug.WriteLine("Permit Update : " + json);


            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }

        }

        public async Task<Tnbsignature> AddSignatures(Tnbsignature body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_signatures?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine("Add New Signature" + json);

            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Tnbsignature>(request);

            //Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }


        public async Task<Imglib> AddImgLib(Imglib body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/TNB_XAM_IMGLIB?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine("Add new Imaglib" + json);

            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Imglib>(request);

            //Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<Tnbsignature> UpdateSignatures(Tnbsignature body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_signatures/{body.tnbsignatureid}?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine("Signature Update: " + json);
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Tnbsignature>(request);

            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                if (response.ResponseStatus == ResponseStatus.Completed && body._action == "Delete")
                {
                    response.Data = body;
                    return response.Data;
                }
                else
                {
                    return response.Data;

                }
            }
        }

        public async Task<Tnbsignature> AddImglibSignatures(Tnbsignature body)
        {

            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });

            Debug.WriteLine("upload Image library Rest Call : " + json);
            Debug.WriteLine("Add Image :" + body.signature);

            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_signatures/{body.tnbsignatureid}?lean=1&action=system:addimage", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "_imagelibref");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("custom-encoding", "base");
            request.AddHeader("Slug", $"signature{body.tnbsignatureid}.png");
            request.AddHeader("Content-type", "image/png");
            request.AddParameter("application/binary", Convert.FromBase64String(body.signature), ParameterType.RequestBody);
            var response = await client.ExecuteAsync<Tnbsignature>(request);
            Debug.WriteLine("Response content=" + response.Content);
            Debug.WriteLine("Response status=" + response.ResponseStatus);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                if (response.ResponseStatus == ResponseStatus.Completed && body._action == "Delete")
                {
                    response.Data = body;
                    return response.Data;
                }
                else
                {
                    return response.Data;

                }
            }
        }

        public async Task<Tnbwochecklistsignature> AddSignature(Tnbwochecklistsignature body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_signature?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine("Add New Signature : " + json);
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Tnbwochecklistsignature>(request);

            //Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<Tnbwochecklistsignature> UpdateSignature(Tnbwochecklistsignature body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_signature/{body.tnbwochecklistsignatureid}?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");

            /*if (body.imglib != null)
            {
                Imglib sing = await AddImgLib(body.imglib[0]);
                Debug.WriteLine("Message from add new singnature : " + sing);
            }*/


            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            //Console.WriteLine(json);
            Debug.WriteLine("update Signature test form: " + json);
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Tnbwochecklistsignature>(request);
           


            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                if (response.ResponseStatus == ResponseStatus.Completed && body._action == "Delete")
                {
                    response.Data = body;
                    return response.Data;
                }
                else
                {
                    return response.Data;

                }
               
            }
           
        }

        public async Task<Tnbwochecklistsignature> AddImglibSignature(Tnbwochecklistsignature body)
        {



            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_signature/{body.tnbwochecklistsignatureid}?lean=1&action=system:addimage", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "_imagelibref");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("custom-encoding", "base");
            request.AddHeader("Slug", $"signature{body.tnbwochecklistsignatureid}.png");
            request.AddHeader("Content-type", "image/png");
            request.AddParameter("application/binary", Convert.FromBase64String(body.signature), ParameterType.RequestBody);
            var response = await client.ExecuteAsync<Tnbwochecklistsignature>(request);
            Debug.WriteLine("Response content=" + response.Content);
            Debug.WriteLine("Response status=" + response.ResponseStatus);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                if (response.ResponseStatus == ResponseStatus.Completed && body._action == "Delete")
                {
                    response.Data = body;
                    return response.Data;
                }
                else
                {
                    return response.Data;

                }
            }
        }


        public async Task<Tnbwochecklistsignature> UpdateImglibSignature(Tnbwochecklistsignature body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_signature/{body.tnbwochecklistsignatureid}?lean=1&action=system:updateimage", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "_imagelibref");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("custom-encoding", "base");
            request.AddHeader("Slug", $"signature{body.tnbwochecklistsignatureid}.png");
            Debug.WriteLine("Image is : " + body.signature);
            //request.AddHeader("Slug", "TESTIMAGE001.png");
            request.AddHeader("Content-type", "image/png");
            request.AddParameter("application/binary", Convert.FromBase64String(body.signature)/*body.signature*/, ParameterType.RequestBody);
            var response = await client.ExecuteAsync<Tnbwochecklistsignature>(request);
            Debug.WriteLine("Response content=" + response.Content);
            Debug.WriteLine("Response status=" + response.ResponseStatus);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                if (response.ResponseStatus == ResponseStatus.Completed && body._action == "Delete")
                {
                    response.Data = body;
                    return response.Data;
                }
                else
                {
                    return response.Data;

                }
            }
        }

        public async Task<UserProfile> DeleteImglibPerson(UserProfile body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_person/{body.personuid}?lean=1&action=system:deleteimage", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("x-method-override", "PATCH");
            var response = await client.ExecuteAsync<UserProfile>(request);
            Debug.WriteLine("Response content=" + response.Content);
            Debug.WriteLine("Response status=" + response.ResponseStatus);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<UserProfile> AddImglibPerson(UserProfile body)
        {
            await DeleteImglibPerson(body);
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_person/{body.personuid}?lean=1&action=system:updateimage", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "_imagelibref");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("custom-encoding", "base");
            request.AddHeader("Slug", $"profile{body.personid}.png");
            request.AddHeader("Content-type", "image/png");
            request.AddParameter("application/binary", Convert.FromBase64String(body.base64string), ParameterType.RequestBody);
            var response = await client.ExecuteAsync<UserProfile>(request);
            Debug.WriteLine("Response content=" + response.Content);
            Debug.WriteLine("Response status=" + response.ResponseStatus);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public async Task<Tnbwochecklisttype> UpdateChecklist(Tnbwochecklisttype body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_checklist/{body.tnbwochecklisttypeid}?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "tnbwochecklisttypeid");
            request.AddHeader("x-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });

            Debug.WriteLine("TNBCHECKLIST DATA : " + json);

            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Tnbwochecklisttype>(request);

            //Console.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }

        }

        //Doclinks Part

        public async Task<DoclinkRest> GetDoclink(string osname, int? refid)
        {
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/{osname}/{refid}/doclinks?lean=1&oslc.select=*", Method.GET);
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            var response = await client.ExecuteAsync<DoclinkRest>(request);
            Console.WriteLine(response.Content);
            return response.Data;
        }


        //update all wo data

        public async Task<Workorder> UpdateWO(Workorder body, string select)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_wo/{body.workorderid}?lean=1&inlinedoc=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);

            // Start Save SQA
            // request.AddHeader("apikey", "xxxx");
            string jsonPlusgudit = JsonConvert.SerializeObject(
                body.plusgaudit, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine("Test writeen plusgaudit: " + jsonPlusgudit);

            if (body.plusgaudit != null)
                if (body.plusgaudit.Count < 0)
                {
                    Debug.WriteLine("Message from Update workorder : There is not checklist");
                }
                else
                {
                    //!SelectedSQA.plusgauditid.HasValue

                    List<Plusgaudit> plusgaudits = body.plusgaudit;
                    /*List<Plusgaudit> plusgauditsUpdate = new List<Plusgaudit>();
                    List<Plusgaudit> plusgauditsAdd = new List<Plusgaudit>();*/

                    foreach (Plusgaudit plusgaudit in plusgaudits)
                    {
                        Tnbsignature res = new Tnbsignature();
                        Plusgaudit plus = new Plusgaudit();
                        if (!plusgaudit.plusgauditid.HasValue)
                        {
                            // plusgauditsAdd.Add(plusgaudit);
                            if (plusgaudit.status.Contains("ENTRY"))
                            {
                                plusgaudit.status = "DRAFT";
                                plus = await AddSQA(plusgaudit);
                                if (plus != null)
                                {
                                    plus.status = "ENTRY";
                                    plus.doclinks = null;
                                    await UpdateSQA(plus);
                                }
                            }
                            else
                            {
                                plus = await AddSQA(plusgaudit);
                            }

                            foreach (Tnbsignature tnbsignature in plusgaudit.tnbsignature)
                            {
                                /*foreach(Imglib imglib in tnbsignature.imglib)
                                {
                                    Debug.WriteLine("ImageLibrary" + imglib.refobjectid);
                                }*/

                                if (tnbsignature.tnbsignatureid.HasValue)
                                    res = await UpdateSignatures(tnbsignature);
                                else
                                {
                                    if (plus != null)
                                    {
                                        tnbsignature.tnbownerid = plus.plusgauditid;
                                        res = await AddSignatures(tnbsignature);
                                    }
                                }

                                if (tnbsignature._imagelibref == null && tnbsignature.signature != null)
                                {
                                    if (plus != null)
                                    {
                                        tnbsignature.tnbownerid = plus.plusgauditid;
                                        tnbsignature.tnbsignatureid = res.tnbsignatureid;
                                    }
                                    await AddImglibSignatures(tnbsignature);
                                }
                                else
                                {
                                    if (plus != null)
                                    {
                                        tnbsignature.tnbownerid = plus.plusgauditid;
                                        tnbsignature.tnbsignatureid = res.tnbsignatureid;
                                    }
                                    await AddImglibSignatures(tnbsignature);
                                }
                            }
                        }
                        else
                        {
                            await UpdateSQA(plusgaudit);
                            foreach (Tnbsignature tnbsignature in plusgaudit.tnbsignature)
                            {
                                if (tnbsignature.tnbsignatureid.HasValue)
                                    res = await UpdateSignatures(tnbsignature);
                                else
                                    res = await AddSignatures(tnbsignature);

                                if (tnbsignature._imagelibref == null && tnbsignature.signature != null)
                                {
                                    tnbsignature.tnbsignatureid = res.tnbsignatureid;
                                    await AddImglibSignatures(tnbsignature);
                                }
                                else
                                {
                                    await AddImglibSignatures(tnbsignature);
                                }
                            }
                        }
                    }
                    //remove it
                    body.plusgaudit = null;
                }
            // End Save SQA

            // Start Save Permit
            //update permit seprately
            if (body.plusgpermitwork != null)
            {
                await funUpdateandAddPermit(body.plusgpermitwork);
                //remove permit from request
                body.plusgpermitwork = null;
            }
            // End Save Permit


            //start saving Testform Signature

            if (body.tnbpsiwochecklisttype != null)
            {
                List<Tnbpsiwochecklisttype> tnbpsiwochecklisttypes = body.tnbpsiwochecklisttype;
                foreach (Tnbpsiwochecklisttype tnbpsiwochecklisttype in tnbpsiwochecklisttypes)
                {
                    foreach (Tnbwochecklisttype tnbwochecklisttype in tnbpsiwochecklisttype.tnbwochecklisttype)
                    {
                        foreach (Tnbwochecklistsignature tnbwochecklistsignature in tnbwochecklisttype.tnbwochecklistsignature.Where(x => x.pendingupdate==true))
                        {
                            Tnbwochecklistsignature res2 = await UpdateSignature(tnbwochecklistsignature);
                            if (tnbwochecklistsignature.signature != null)
                            {
                                Tnbwochecklistsignature res = await UpdateImglibSignature(tnbwochecklistsignature);
                            }
                        }
                        tnbwochecklisttype.tnbwochecklistsignature = null;
                    }
                }
            }


            // Start Save Workorder
            // request.AddHeader("apikey", "xxxx");
            if(body.asset!=null)
            foreach (Asset m in body.asset)
            {

               Asset getdata =  await UpdateAsset(m);
                    if (getdata != null)
                        return null;
            }
            body.asset = null;

            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });

            Debug.WriteLine("Sync Workorder : " + json);


            request.AddHeader("properties", select);
            request.AddHeader("X-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Workorder>(request);
            Debug.WriteLine("update work order response "+response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
            // End Save Workorder
        }

        private async Task funUpdateandAddPermit(List<PermitWork> plusgpermitwork)
        {
            foreach (PermitWork per in plusgpermitwork)
            {
                if (per.permitworknum == null)
                    await AddPermit(per);
                else
                    await UpdatePermit(per);
            }
        }

        public async Task<Asset> UpdateAsset(Asset body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_asset/{body.assetuid}?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);

            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine(json);
            request.AddHeader("properties", "assetid,assetuid,assetnum,description,tnbmodelnum,serialnum,tnbmanufacturerpart,manufacturer,tnbmanufacturingcountry,tnbbrand,tnbitemnum,itemnum,tnbmanufdate,tnbdeliverydate,tnbvoltagelevel,tnbassetcondition,assetspec");
            request.AddHeader("X-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Asset>(request);
            Debug.WriteLine("Update Asset" + json);
            Debug.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }


        public async Task<Locations> UpdateLocation(Locations body)
        {
            client.Timeout = 60000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_location/{body.locationsid}?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            string json = JsonConvert.SerializeObject(
                body, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
            Debug.WriteLine(json);
            request.AddHeader("properties", "locationsid,description,locationspec");
            request.AddHeader("X-method-override", "PATCH");
            request.AddHeader("patchtype", "MERGE");
            request.AddJsonBody(json);
            var response = await client.ExecuteAsync<Locations>(request);
            Debug.WriteLine(response.Content);
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }


        public async Task<MaxDomains> GetDomain()
        {
            client.Timeout = 120000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_domain?lean=1&oslc.where=domainid%20in%20[%22TNBBRAND%22,%22TNBMODELNUM%22,%22TNBMANUFCOUNTRY%22,%22TNBVOLTAGELEVEL%22,%22TNBASSETCONDITION%22,%22TNBCHECKLISTLABEL%22,%22TNBPERMITTYPE%22]&oslc.select=domainid,description,alndomain,domaintype", Method.GET);
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            var response = await client.ExecuteAsync<MaxDomains>(request);
            Debug.WriteLine(response.Content);
            var mxdom = response.Data;
            mxdom.size = response.ContentLength;
            return mxdom;
        }

        public async Task<ResCompanies> GetCompanies()
        {
            client.Timeout = 120000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_companies?lean=1&oslc.select=companiesid,company,name&oslc.where=type=%22V%22&oslc.pageSize=1", Method.GET);
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            var response = await client.ExecuteAsync<ResCompanies>(request);
            Debug.WriteLine(response.Content);
            return response.Data;
        }

        public async Task<Restnbchecklistval> GetChecklistValue()
        {
            client.Timeout = 120000;
            var apikey = await SecureStorage.GetAsync("apikey");
            var request = new RestRequest($"os/tnb_xam_checklistval?lean=1&oslc.select=*", Method.GET);
            request.AddHeader("apikey", apikey);
            // request.AddHeader("apikey", "xxxx");
            var response = await client.ExecuteAsync<Restnbchecklistval>(request);
            Debug.WriteLine(response.Content);
            return response.Data;
        }

        public async Task<List<Lbslocation>> Updatelbslocation(Lbslocation body)
        {
            var apikey = await SecureStorage.GetAsync("apikey");
            var username = await SecureStorage.GetAsync("username");
            var request = new RestRequest("os/tnb_xam_lbs?lean=1", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("apikey", apikey);
            request.AddHeader("properties", "*");
            request.AddJsonBody(new
            {
                refobject = "LABOR",
                key1 = "TNB",
                key2 = username,
                body.latitude,
                body.longitude,
                wonum = "",
                siteid = "",
                lastupdate = DateTime.Now
            });
            var response = await client.ExecuteAsync<List<Lbslocation>>(request);
            Console.WriteLine(response.Content);
            return response.Data;
        }
    }
}

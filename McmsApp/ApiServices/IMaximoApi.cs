using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McmsApp.Models;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ApiServices
{
    public interface IMaximoApi
    {
        //System
        Task<bool> Ping();
        Task<Apikey> Login(string username, string password);
        Task<UserProfile> whoAmI();
        //Get OS
        Task<ResPerson> GetPersons();
        Task<RespDownloadWorkorder> GetWorkorder(string wonum,bool isattachment);
        Task<RespDownloadWorkorder> GetParentWorkorder(string parents, bool isattachment);
        Task<RespDownloadWorkorder> GetWorkorderQuery(string select, string where);
        Task<ResSQA> GetTemplateSQA();
        Task<ChildWORest> GetChildWO(string parent);
        Task<DoclinkRest> GetDoclink(string osname, int? refid);
        Task<MaxDomains> GetDomain();
        Task<ResCompanies> GetCompanies();
        Task<Restnbchecklistval> GetChecklistValue();
        Task<Doclinks> GetDoclinks(int? docid);
        Task<byte[]> DownloadImage(string id);
        //Push OS
        Task<Plusgaudit> AddSQA(Plusgaudit body);
        Task<Doclinks> AddAttachmnt(Doclinks body);
        Task<Plusgaudit> UpdateSQA(Plusgaudit body);
        Task<PermitWork> AddPermit(PermitWork body);
        Task<PermitWork> UpdatePermit(PermitWork body);
        Task<Workorder> UpdateWO(Workorder body,string select);
        Task<Asset> UpdateAsset(Asset body);
        Task<Locations> UpdateLocation(Locations body);
        Task<List<Lbslocation>> Updatelbslocation(Lbslocation body);
        Task<Tnbwochecklistsignature> AddSignature(Tnbwochecklistsignature body);
        Task<Tnbwochecklistsignature> AddImglibSignature(Tnbwochecklistsignature body);
        Task<Tnbwochecklistsignature> UpdateImglibSignature(Tnbwochecklistsignature body);
        Task<Tnbwochecklistsignature> UpdateSignature(Tnbwochecklistsignature body);
        Task<Tnbsignature> AddSignatures(Tnbsignature body);
        Task<Tnbsignature> AddImglibSignatures(Tnbsignature body);
        Task<Tnbsignature> UpdateSignatures(Tnbsignature body);
        Task<Tnbwochecklisttype> UpdateChecklist(Tnbwochecklisttype body);
        Task<UserProfile> AddImglibPerson(UserProfile body);
        Task<Imglib> AddImgLib(Imglib body);
    }
}


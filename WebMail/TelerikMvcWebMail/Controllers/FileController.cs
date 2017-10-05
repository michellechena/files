using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TelerikMvcWebMail.Filter;
using TelerikMvcWebMail.Models;

namespace TelerikMvcWebMail.Controllers
{
   // [Authorize]
    //[SessionExpire]
    [Authorize]
   // [Authorize]
    //[SessionExpire]
    public class FileController : Controller
    {
        
        public ActionResult FromOtherPageRequest(string MailBoxId = null)
        {
            TempData["SelectedMailBoxId"] = MailBoxId;
            return  RedirectToAction("Index");
        } 
        public ActionResult Index()
        {
           

            int MailBoxid = 0;
            try
            {
                MailBoxid = Convert.ToInt32(TempData["SelectedMailBoxId"]);
            }
            catch (Exception ex)
            {

            }
            if (MailBoxid != 0)
            {
                var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/GetFirstFolderId?UserId=" + Session["UserId"]+ "&MailBoxId="+ MailBoxid, RestSharp.Method.GET);
                string FirstFolderId = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<string>(Data);
                ViewBag.FirstFolderId = FirstFolderId;
            }
            else
            {
                var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/GetFirstFolderId?UserId=" + Session["UserId"], RestSharp.Method.GET);
                string FirstFolderId = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<string>(Data);
                ViewBag.FirstFolderId = FirstFolderId;
            }

            return View();
        }

        

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return PartialView();
        }
            
     
        [ValidateInput(false)]
        public ActionResult ReadMailDetails(string MailId)
        {
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/ReadMailDetails?MailId=" + MailId, RestSharp.Method.GET);
            FilesViewModel Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<FilesViewModel>(Data);           
            return PartialView("EmailDetailes", Result);

        }
        [HttpPost]
        public ActionResult UpdateFile(FilesViewModel _MailViewModel)
        {
            if (string.IsNullOrEmpty(_MailViewModel.Path))
            {
                _MailViewModel.IsValid = false;
            }
            else
            {
                Uri uriResult;
                bool result = Uri.TryCreate(_MailViewModel.Path, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (result)
                {
                    _MailViewModel.IsValid = true;
                }
                else
                {
                    _MailViewModel.IsValid = false;
                }
            }
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/UpdateFile", RestSharp.Method.POST, _MailViewModel);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MailBoxFolders()
        {
            
            return View();
        }
        
        public ActionResult returnFolderPartalView(string FolderId)
        {
            FoldersViewmodel Model = new FoldersViewmodel();            
            List<SelectListItem> List = new List<SelectListItem>() {
                new SelectListItem {
                    Text="Select",
                    Value=""
                }
            };
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/MailBoxList", RestSharp.Method.GET);
            List<SelectListItem> Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<SelectListItem>>(Data);
            List.AddRange(Result);
            ViewBag.FolderList = List;
           
            if (!string.IsNullOrEmpty(FolderId))
            {
                var _MailBoxFolderModel = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/GetFolderDeatiles?FolderId=" + FolderId, RestSharp.Method.GET);
                Model = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<FoldersViewmodel>(_MailBoxFolderModel);
                             
            }
            
            return View("_PartialAddEditFolder", Model);

        }


        public ActionResult AddEditFolder(FoldersViewmodel Model)
        {
            
                    
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/AddEditFolder", RestSharp.Method.POST, Model);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);

        }
       
        public ActionResult getFolderlistBySelectedmailBox(long MailBoxId)
        {
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/MailBoxFolderListForFolderPage?MailBoxId=" + MailBoxId+ "&UserId="+ Session["UserId"].ToString(), RestSharp.Method.GET);
            List<FoldersViewmodel> Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<FoldersViewmodel>>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);            
           
        }
        public ActionResult GetNewEmail()
        {
            return PartialView();
        }
        public ActionResult UploadFile()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult SubmitUploadFile(IEnumerable<HttpPostedFileBase> attachments,string Folderid)
        {
            string Message = string.Empty;
            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    FileViewmodel Model = new FileViewmodel();
                    string FileContent = new StreamReader(file.InputStream).ReadToEnd();
                    Model.FileContent = FileContent;                    
                    Model.FileName = file.FileName;
                    Model.FolderId = Folderid;
                    var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/SubmitUploadFile", RestSharp.Method.POST, Model);
                    bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
                    if(Result)
                    {
                        Message = Message+ "<span > " + file.FileName + " Upload Sucessfully<span><br>";
                    }
                    else
                    {
                        Message = Message + "<span style='color:red'> " + file.FileName + " Error in upload<span><br>";
                    }
                }
            }

            return Json(Message,JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveNewFile(FilesViewModel _MailViewModel)
        {
            if (string.IsNullOrEmpty(_MailViewModel.Path))
            {
                _MailViewModel.IsValid = false;
            }
            else
            {
                Uri uriResult;
                bool result = Uri.TryCreate(_MailViewModel.Path, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (result)
                {
                    _MailViewModel.IsValid = true;
                }
                else
                {
                    _MailViewModel.IsValid = false;
                }
            }
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/SaveNewFile", RestSharp.Method.POST, _MailViewModel);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }



    }
}

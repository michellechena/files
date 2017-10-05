using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Http.ModelBinding;

using System.Web.Http.Cors;
using APIs.Entity;
using APIs.Models;
using System.Web.WebPages.Html;
using System.Data;


namespace APIs.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ApiHomeController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ValidateUser(SignIn Model)
        {
            UserApIModel _User = new UserApIModel();
            string Result = string.Empty;
            using (var Entity = new WebMailEntities())
            {
                _User= Entity.Users.Where(s => s.Email.ToUpper() == Model.UserName.ToUpper()
              ).Select(x => new UserApIModel
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    id = x.id
                }).FirstOrDefault();

            }
            return Json(_User);
        }


        [HttpGet]
        public List<System.Web.Mvc.SelectListItem> UserMailBox(int UserId, string request)
        {
            List<UserMailBoxApiModel> _Model = new List<UserMailBoxApiModel>();           
            List<System.Web.Mvc.SelectListItem> _MainBoxList = new List<System.Web.Mvc.SelectListItem>();
            string Result = string.Empty;
            using (var Entity = new WebMailEntities())
            {
                _Model = Entity.UserMailBoxes.Where(x => x.UserId == UserId && x.PermitionLevel !="0").Select(s => new UserMailBoxApiModel
                {
                    MailboxId = s.MailboxId,
                    FullName = s.MailBox.FullName,
                    ShortName = s.MailBox.ShortName,
                    IsDefoultMailBox=s.IsDefoultMailBox,
                    IsMainContact = s.IsMainContact,
                    PermitionLevel =s.PermitionLevel,
                  
                }).OrderByDescending(x=>x.IsDefoultMailBox).ToList();
            }           
            
            if (request == "DRP")
            {
                _MainBoxList = _Model.Select(s => new System.Web.Mvc.SelectListItem
                {
                    Text = s.FullName,
                    Value = s.MailboxId.ToString(),
                    Selected = s.IsDefoultMailBox == true ? true : false
                }).ToList();
            };

            return _MainBoxList;
        }

        [HttpGet]
        public string CheckMailBoxPermition(string MailBoxId, string UserId)
        {
            long _UserId = Convert.ToInt32(UserId);
            long _MailBoxId = Convert.ToInt32(MailBoxId);
            string result = string.Empty;
            using (var Entity = new WebMailEntities())
            {
                result = Entity.UserMailBoxes.Where(s => s.UserId == _UserId && s.MailboxId == _MailBoxId).Select(s => s.PermitionLevel).FirstOrDefault();
            }
            return result;
        }

        [HttpGet]
        public IHttpActionResult GetMailBoxFolderList(long MailBoxId, string Defoult = null, string SerchedFolderString = null, string UserId = null)
        {

            long _UserId = Convert.ToInt32(UserId);
            long DefoultFolderId = 0;
            List<FolderApiModel> Model = new List<FolderApiModel>();
            using (var Entity = new WebMailEntities())
            {
                Model = Entity.Folders.Where(x => x.MailBoxId == MailBoxId && x.StatusId !=0 ).Select(s => new FolderApiModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    MailBoxId = s.MailBoxId.ToString(),
                    TypeId = s.TypeId.ToString(),
                    StatusId = s.StatusId.ToString(),
                    MailBoxName = s.MailBox.FullName,
                }).OrderBy(x=>x.TypeId).ThenBy(z=>z.Name).ToList();

            }

            List<Folders> _FolderList = new List<Folders>();
            if (string.IsNullOrEmpty(SerchedFolderString))
            {
                _FolderList = Model.Select(x => new Folders
                {
                    MailBoxName = MailBoxId.ToString(),
                    text = x.Name,
                    value = x.Id.ToString(),                   
                }).ToList();
            }
            else
            {
                _FolderList = Model.Where(x => x.Name.ToUpper().Contains(SerchedFolderString.ToUpper())).Select(x => new Folders
                {
                    MailBoxName = x.MailBoxName,
                    text = x.Name,
                    value = x.Id.ToString(),                    
                }).ToList();
            }
            if (!string.IsNullOrEmpty(Defoult))
            {
                DefoultFolderId = Model.Select(x => x.Id).Take(1).FirstOrDefault();
                return Json(DefoultFolderId);
            }
            else
            {
                return Json(_FolderList);
            }


        }

        public string GetFirstFolderId(string UserId, int MailBoxId = 0)
        {
            long _UserId = Convert.ToInt32(UserId);
            int _MailBoxId = Convert.ToInt32(MailBoxId);
            int FolderId = 0;
            if (_MailBoxId == 0)
            {
                using (var Entity = new WebMailEntities())
                {
                    _MailBoxId = Entity.UserMailBoxes.Where(s => s.IsDefoultMailBox == true && s.UserId== _UserId).Select(z => z.MailboxId).FirstOrDefault();

                }
            }
            using (var Entity = new WebMailEntities())
            {
                FolderId = Entity.Folders.Where(s => s.MailBoxId == _MailBoxId && s.TypeId==0).Select(z => z.Id).FirstOrDefault();

            }
            return FolderId.ToString();
        }

        public IHttpActionResult GetEmailList([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request, string MailBoxId, string UserId, string AjaxRequest = null)
        {
            List<FilesApiModel> result = new List<FilesApiModel>();
            try
            {
                long _UserId = Convert.ToInt32(UserId);
                long _MailBoxId = Convert.ToInt32(MailBoxId);                
                using (var Entity = new WebMailEntities())
                {
                   
                    result = Entity.Files.Where(x => x.Folder.MailBoxId == _MailBoxId).Select(message => new FilesApiModel
                    {
                        FolderId = message.FolderId.ToString(),
                        Id = message.Id,
                        IsValid = message.IsValid,
                        Name = message.Name,
                        Path = message.Path,
                        StatusId = message.StatusId,
                        TypeId = message.TypeId,
                        PermitionLevel = Entity.UserMailBoxes.Where(z => z.UserId == _UserId && z.MailboxId == _MailBoxId).Select(a => a.PermitionLevel).FirstOrDefault()

                    }).ToList();
                }
            }
            catch(Exception ex)
            {

            }
            return Json(result.ToDataSourceResult(request));

        }
        

        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public IHttpActionResult UpdateEmailDetailes([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request, FilesApiModel _FilesApiModel)
        {
            if (_FilesApiModel.FolderId == "Disable")
            {
                using (var Entity = new WebMailEntities())
                {
                    File Files = Entity.Files.Where(s => s.Id == _FilesApiModel.Id).FirstOrDefault();
                    if (Files != null)
                    {
                        Files.StatusId =0;
                        Entity.SaveChanges();
                    }
                }
            }
            else if (_FilesApiModel.FolderId == "Deleted")
            {
                using (var Entity = new WebMailEntities())
                {
                    File Mail = Entity.Files.Where(s => s.Id == _FilesApiModel.Id).FirstOrDefault();
                    if (Mail != null)
                    {
                        Entity.Files.Remove(Mail);
                        Entity.SaveChanges();
                    }
                }
            }
            else
            {

                using (var Entity = new WebMailEntities())
                {
                    File target = Entity.Files.Where(x => x.Id == _FilesApiModel.Id).FirstOrDefault();
                    if (target != null)
                    {
                        int _FolderId = Convert.ToInt32(_FilesApiModel.FolderId);
                        target.FolderId = _FolderId;
                        target.IsValid = _FilesApiModel.IsValid;
                        target.Name = _FilesApiModel.Name;
                        target.Path = _FilesApiModel.Path;
                      //  target.StatusId = _FilesApiModel.StatusId;
                        target.TypeId = _FilesApiModel.TypeId;                      
                        Entity.SaveChanges();
                    }
                }
            }

            return Json(new[] { _FilesApiModel }.ToDataSourceResult(request));
        }

        [HttpGet]
        public bool UpdateFileStatus(string Id, string StatusId)
        {
            try
            {
                int _Id = Convert.ToInt32(Id);
                byte _StatusId = Convert.ToByte(StatusId);
                using (var Entity = new WebMailEntities())
                {
                    File _File = Entity.Files.Where(x => x.Id == _Id).FirstOrDefault();
                    if (_File != null)
                    {
                        _File.StatusId = _StatusId;
                        Entity.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }


        [HttpPost]
        public bool SaveNewFile(FilesApiModel _FilesApiModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    File _File = new File();
                    _File.FolderId = Convert.ToInt32(_FilesApiModel.FolderId);
                    _File.IsValid = _FilesApiModel.IsValid;
                    _File.Name = _FilesApiModel.Name;
                    _File.Path = _FilesApiModel.Path;
                    _File.StatusId = 1;
                    using (var Entity = new WebMailEntities())
                    {
                        Entity.Files.Add(_File);
                        Entity.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public IHttpActionResult FolderList([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            
            List<FolderApiModel> Model = new List<FolderApiModel>();
            using (var Entity = new WebMailEntities())
            {
                Model = Entity.Folders.Select(s => new FolderApiModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    MailBoxId = s.MailBoxId.ToString(),
                    MailBoxName = s.MailBox.FullName.ToString(),
                    StatusId = s.StatusId.ToString(),
                    TypeId = s.TypeId.ToString()
                }).OrderBy(x => x.MailBoxName).ThenBy(x => x.Name).ToList();

            }
            return Json(Model.ToDataSourceResult(request));

        }


        [HttpGet]
        public List<SelectListItem> MailBoxList()
        {
            List<SelectListItem> _MainBoxList = new List<SelectListItem>();            
            using (var Entity = new WebMailEntities())
            {
                _MainBoxList = Entity.MailBoxes.Select(s => new SelectListItem
                {
                    Text = s.FullName,
                    Value = s.id.ToString(),
                }).ToList();

            }
            return _MainBoxList;
        }


        [HttpGet]
        public FolderApiModel GetFolderDeatiles(string FolderId)
        {
            long _FolderId = Convert.ToInt32(FolderId);
            FolderApiModel Model = new FolderApiModel();
            using (var Entity = new WebMailEntities())
            {
                Model = Entity.Folders.Where(x => x.Id == _FolderId).Select(s => new FolderApiModel
                {
                    Id = s.Id,
                    MailBoxName = s.MailBox.FullName,
                    MailBoxId = s.MailBoxId.ToString(),
                    Name = s.Name,
                    StatusId = s.StatusId.ToString(),
                    TypeId = s.TypeId.ToString(),
                }).FirstOrDefault();

            }
            return Model;
        }


        [HttpPost]
        public bool AddEditFolder(FolderApiModel Model)
        {
            try
            {

                if (Model.TypeId == "0")
                {
                    using (var Entity = new WebMailEntities())
                    {
                        int _MailBoxid = Convert.ToInt32(Model.MailBoxId);
                        List<Folder> _FolderList = Entity.Folders.Where(s => s.MailBoxId == _MailBoxid).ToList();
                        _FolderList.ForEach(s => s.TypeId = 1);
                        Entity.SaveChanges();


                    }
                }
                Folder _Folders = new Folder();
                //For Add
                if (string.IsNullOrEmpty(Model.Id.ToString()) || Model.Id == 0)
                {
                    using (var Entity = new WebMailEntities())
                    {
                        _Folders.MailBoxId = Convert.ToInt32(Model.MailBoxId);
                        _Folders.Name = Model.Name;
                        _Folders.StatusId =Convert.ToByte(Model.StatusId);
                        _Folders.TypeId = Convert.ToByte(Model.TypeId);
                        _Folders.StatusId = 1;
                        Entity.Folders.Add(_Folders);
                        Entity.SaveChanges();
                    }
                }
                else
                {
                    using (var Entity = new WebMailEntities())
                    {
                        Folder _FoldersUpdate = Entity.Folders.Where(s => s.Id == Model.Id).FirstOrDefault();
                        if (_FoldersUpdate != null)
                        {
                            _FoldersUpdate.MailBoxId = Convert.ToInt32(Model.MailBoxId);
                            _FoldersUpdate.Name = Model.Name;
                            _FoldersUpdate.StatusId = 1;
                            _FoldersUpdate.TypeId = Convert.ToByte(Model.TypeId);   
                            Entity.SaveChanges();
                        }
                    }
                }
               
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        [HttpGet]
        public bool DeleteFolder(string Id)
        {
            try
            {
                long _Id = Convert.ToInt32(Id);
                Folder _Folder = new Folder();

                using (var Entity = new WebMailEntities())
                {
                    _Folder = Entity.Folders.Where(s => s.Id == _Id).FirstOrDefault();
                    if (_Folder != null)
                    {
                        _Folder.StatusId =0;
                        Entity.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public FilesApiModel ReadMailDetails(string MailId)
        {
            long _MailId = Convert.ToInt32(MailId);
            FilesApiModel result = new Models.FilesApiModel();

            using (var Entity = new WebMailEntities())
            {
                result = Entity.Files.Where(x => x.Id == _MailId).Select(message => new FilesApiModel
                {
                    Id = message.Id,
                    Name = message.Name,
                    Path = message.Path,
                    TypeId = message.TypeId,
                    StatusId=message.StatusId                   
                }).FirstOrDefault();
            }
            return result;
        }


        [HttpPost]
        public bool UpdateFile(FilesApiModel _FilesApiModel)
        {
            try
            {
                File _File = new File();
                using (var Entity = new WebMailEntities())
                {
                    int _id = _FilesApiModel.Id;
                    _File = Entity.Files.Where(z => z.Id == _id).FirstOrDefault();
                    if (_File != null)
                    {
                        _File.IsValid = _FilesApiModel.IsValid;
                        _File.Name = _FilesApiModel.Name;
                        _File.Path = _FilesApiModel.Path;
                        _File.TypeId = _FilesApiModel.TypeId;
                        Entity.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        //[HttpGet]
        //public List<MailBoxFolderModel> MailBoxFolderListForFolderPage(long MailBoxId, string UserId)
        //{
        //    long _UserId = Convert.ToInt32(UserId);
        //    List<MailBoxFolderModel> Model = new List<MailBoxFolderModel>();
        //    using (var Entity = new WebMailEntities())
        //    {
        //        Model = Entity.MailBoxFolders.Where(x => x.MailBoxId == MailBoxId && x.IsActive == true).Select(s => new MailBoxFolderModel
        //        {
        //            MailBoxFolderId = s.MailBoxFolderId,
        //            MailBoxFolderName = s.MailBoxFolderName,
        //            MailBoxId = s.MailBoxId,
        //            Owner = s.MailBox.UserId == _UserId ? "YES" : "NO",
        //            Sequence = s.Sequence,
        //        }).OrderBy(x => x.Sequence).ToList();

        //    }
        //    return Model;
        //}


        public bool SubmitUploadFile(FileAPImodel model)
        {
            try
            {
                DataTable tbl = new DataTable();                
                    tbl.Columns.Add("FileName");
                    tbl.Columns.Add("FilePath");

                List<File> FilemodelfromtextFile = new List<File>();
                string[] lines = model.FileContent.Split(new[] { Environment.NewLine },
                                     StringSplitOptions.RemoveEmptyEntries);

                foreach(var line in lines)
                {
                    var LineContent = line.Split(',');
                    if(LineContent.Count()>2)
                    {
                        return false;
                    }
                    else
                    {
                        File _File1 = new File();
                        _File1.Name = LineContent[0];
                        _File1.Path = LineContent[1];
                        _File1.StatusId = 1;
                        _File1.FolderId = Convert.ToInt32(model.FolderId);
                        FilemodelfromtextFile.Add(_File1);
                    }
                }

                FilemodelfromtextFile.ForEach(x => x.IsValid = CheckFilePathCurrect(x.Path));
                
                 using (var Entity = new WebMailEntities())
                 {
                        Entity.Files.AddRange(FilemodelfromtextFile);
                        Entity.SaveChanges();
                 }
                 return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
       public bool CheckFilePathCurrect(string Path)
        {
            bool result;
            Uri uriResult;
            result = Uri.TryCreate(Path, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
        [HttpPost]
        public bool AddFeedBack(string FeedBack,string userEmail)
        {
            try
            {
                FeedBack _FeedBack = new APIs.Entity.FeedBack();
                using (var Entity = new WebMailEntities())
                {
                    
                    _FeedBack.DateTime = DateTime.Now;
                    _FeedBack.Email = userEmail;
                    _FeedBack.FeedBack1 = FeedBack;
                    Entity.FeedBacks.Add(_FeedBack);
                    Entity.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

    }
}

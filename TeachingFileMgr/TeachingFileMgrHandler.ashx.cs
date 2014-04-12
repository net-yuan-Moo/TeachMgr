using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helper;
using System.Data;
using System.Web.SessionState;

namespace TeachMgr.TeachingFileMgr
{
    /// <summary>
    /// TeachingFileMgrHandler 的摘要说明
    /// </summary>
    public class TeachingFileMgrHandler : IHttpHandler, IRequiresSessionState 
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string strResult = string.Empty;
            try
            {
                string action = context.Request.Form["action"];
                switch (action)
                {
                    case "getFile":
                        strResult = GetUploadFile(context);
                        break;
                    case "getList":
                        strResult = GetList();
                        break;
                    default:
                        break;
                }
                strResult = JsonHelper.SuccessReturn(strResult);
            }
            catch
            {
                strResult = JsonHelper.FailReturn("error");
            }
            context.Response.Write(strResult);
        }

        public string GetUploadFile(HttpContext context)
        {
            HttpPostedFile file = context.Request.Files["fileToUpload"];
            if (file != null && string.IsNullOrEmpty(file.FileName) == false)
            {
                file.SaveAs(context.Server.MapPath("~/TeachingFileMgr/TeachFiles/") + file.FileName);

                BLL.TeachFilesBLL bll = new BLL.TeachFilesBLL();
                Model.TeachFilesModel mod = new Model.TeachFilesModel();
                mod.CreateTime = DateTime.Now;
                mod.PersonName = context.Session[0].ToString();
                mod.FileDesc = context.Request.Form["filedesc"];
                mod.FileName = file.FileName;
                mod.FilePath = "/TeachingFileMgr/TeachFiles/" + file.FileName;
                bool b = bll.Add(mod);
                if (b)
                {
                    return "上传教学文件成功！".ToJson();
                }
                else
                {
                    return "上传教学文件失败！".ToJson();
                }
            }
            else
            {
                return "请选择文件!".ToJson();
            }
        }

        public string GetList()
        {
            BLL.TeachFilesBLL bll = new BLL.TeachFilesBLL();
            DataSet ds = bll.GetAllList();
            return ds.Tables[0].ToJson();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
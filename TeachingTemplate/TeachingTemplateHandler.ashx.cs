using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helper;
using BLL;
using System.Data;
using System.IO;
using System.Web.SessionState;

namespace TeachMgr.TeachingTemplate
{
    /// <summary>
    /// TeachingTemplateHandler 的摘要说明
    /// </summary>
    public class TeachingTemplateHandler : IHttpHandler, IRequiresSessionState 
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
                    case "deleteTemp":
                        int id = Convert.ToInt32(context.Request.Form["id"]);
                        string filename = context.Request.Form["filename"];
                        string path = context.Server.MapPath("~/TeachingTemplate/TempFiles/") + filename;
                        strResult = DeleteTemp(id,path);
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
                file.SaveAs(context.Server.MapPath("~/TeachingTemplate/TempFiles/") + file.FileName);

                BLL.TempFileBLL bll = new BLL.TempFileBLL();
                Model.TempFileModel mod = new Model.TempFileModel();
                mod.CreateTime = DateTime.Now;
                mod.PersonName = context.Session[0].ToString();
                mod.TempDesc = context.Request.Form["tempdesc"];
                mod.TempName = file.FileName;
                mod.TempPath = "/TeachingTemplate/TempFiles/" + file.FileName; //存储相对路径
                bool b = bll.Add(mod);
                if (b)
                {
                    return "上传模板文件成功！".ToJson();
                }
                else
                {
                    return "上传模板文件失败！".ToJson();
                }
            }
            else
            {
                return "请选择文件!".ToJson();
            }
        }

        public string GetList()
        {
            TempFileBLL bll = new TempFileBLL();
            DataSet ds =  bll.GetAllList();
            return ds.Tables[0].ToJson();
        }

        public string DeleteTemp(int id,string path)
        {
            
            TempFileBLL bll = new TempFileBLL();
            bool b = bll.Delete(id);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (b)
            {
                return "删除模板成功！".ToJson();
            }
            else
            {
                return "删除模板失败!".ToJson();
            }
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
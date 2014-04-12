using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helper;
using Model;
using BLL;
using System.Data;
using System.Web.SessionState;

namespace TeachMgr.TeachingInfo
{
    /// <summary>
    /// TeachingInfoHandler 的摘要说明
    /// </summary>
    public class TeachingInfoHandler : IHttpHandler, IRequiresSessionState 
    {

        public void ProcessRequest(HttpContext context)
        {
            string strResult = string.Empty;
            try
            {
                string action = context.Request.Form["action"];
                switch (action)
                {
                    case "getTeacherList":
                        strResult = GetTeachInfoList();
                        break;
                    case "addTeachInfo":
                        TeachingcontentModel mod = new TeachingcontentModel();
                        mod.title = context.Request.Form["title"];
                        mod.createTime = DateTime.Now;
                        string strContent = context.Request.Form["content"];
                        mod.content = context.Server.HtmlDecode(strContent);
                        mod.content = mod.content.Replace(">nbsp;", "&nbsp;");
                        mod.personName = context.Session[0].ToString();
                        strResult = AddTeachInfo(mod);
                        break;
                    case "getContent":
                        int id = Convert.ToInt32(context.Request.Form["id"]);
                        strResult = GetContent(id);
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

        public string AddTeachInfo(TeachingcontentModel mod)
        {
            TeachingcontentBLL bll = new TeachingcontentBLL();
            bool result = bll.Add(mod);
            if (result)
            {
                return "添加教学信息成功！".ToJson();
            }
            else
            {
                return "添加教学信息失败！".ToJson();
            }
        }

        public string GetTeachInfoList()
        {
            TeachingcontentBLL bll = new TeachingcontentBLL();
            DataSet ds = bll.GetAllList();
            return ds.Tables[0].ToJson();
        }

        public string GetContent(int id)
        {
            TeachingcontentBLL bll = new TeachingcontentBLL();
            TeachingcontentModel mod =  bll.GetModel(id);
            return mod.ToJson();
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
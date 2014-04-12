using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using Helper;
using System.Data;
using System.Web.SessionState;

namespace TeachMgr.Handler
{
    /// <summary>
    /// LoginHandler 的摘要说明
    /// </summary>
    public class LoginHandler : IHttpHandler, IRequiresSessionState 
    {
        BLL.UserBLL bll = new BLL.UserBLL();
        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            string strResult = string.Empty;
            try
            {
                string action = context.Request.Form["action"];
                switch(action)
                {
                    case "login":
                        string strName=context.Request.Form["loginname"];
                        string strPwd=context.Request.Form["pwd"];
                        string strValid = context.Request.Form["valid"];
                        strResult = LoginSys(strName, strPwd, strValid,context);
                        break;
                    case "getRights":
                        string str = context.Session[0].ToString();
                        int roleId = Convert.ToInt32(context.Session[2].ToString());
                        strResult = GetRights(roleId);
                        break;
                    case "getUser":
                        strResult = context.Session[0].ToString().ToJson();
                        break;
                    case "getRole":
                        strResult = context.Session[2].ToString().ToJson();
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

        public string LoginSys(string strName, string strPwd,string strValid, HttpContext context)
        {
            if (strValid != "DDZTJ")
            {
                return "验证码错误".ToJson();
            }
            bool b = bll.Login(strName,strPwd);
            if (b)
            {
                
                context.Session["LoginName"] = strName;
                context.Session["Pwd"] = strName;
                context.Session["RoleId"] = bll.GetCurrentRole(strName,strPwd);
                return "登录成功".ToJson();
            }
            else
            {
                return "用户名或者密码不正确".ToJson();
            }
        }

        public string GetRights(int roleId)
        {
            DataTable dt = bll.GetRights(roleId);
            return dt.ToJson();
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
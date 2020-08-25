using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Master_AdminMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.User.Identity.Name == string.Empty)
        {
            Response.Redirect("~/PageCommon/login.aspx");

        }
        else
        {
            bool type = Page.User.IsInRole("Admin");
            LoginView logInView = (LoginView)this.Master.FindControl("HeadLoginView");
            HyperLink rdirect = (HyperLink)logInView.FindControl("HyperLink1");
            if (!type)
           {
               Response.Redirect("~/PageCommon/error.aspx");
               rdirect.NavigateUrl = "~/PageUser/MyProfile.aspx";
           }
           else
           {
                
                rdirect.NavigateUrl = "~/PageAdmin/Admin.aspx";
           }
        }
    }
}

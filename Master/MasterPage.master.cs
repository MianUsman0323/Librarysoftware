using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Master_MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.User.Identity.Name != string.Empty)
        {
          HyperLink hyp=(HyperLink)HeadLoginView.FindControl("HyperLink1");
          bool type = Page.User.IsInRole("Admin");
          if (type)
          {
              hyp.NavigateUrl = "~/PageAdmin/Admin.aspx";
          }
          else
          {
              hyp.NavigateUrl = "~/PageUser/MyProfile.aspx";
          }
         
        }
       
           
    }
}

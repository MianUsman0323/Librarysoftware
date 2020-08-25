using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Drawing;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
public partial class PageAdmin_AsignRole : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindRolesDetails();

            BindUserDetails();
            BindRoles();
        }
    }
    protected void BindUserDetails()
    {
        ddlUsers.DataSource = Membership.GetAllUsers();
        ddlUsers.DataTextField = "UserName";
        ddlUsers.DataValueField = "UserName";
        ddlUsers.DataBind();
        ddlUsers.Items.Insert(0, new ListItem("--Select User--", "0"));
    }

    protected void BindRoles()
    {
        GridView1.DataSource = Roles.GetAllRoles();
        GridView1.DataBind();
    }


    // This Method is used to bind roles
    protected void BindRolesDetails()
    {
        gvRoles.DataSource = Roles.GetAllRoles();
        gvRoles.DataBind();
    }
    // This Button Click event is used to Create new Role
    protected void btnCreate_Click(object sender, EventArgs e)
    {
        string roleName = txtRole.Text.Trim();
        if (!Roles.RoleExists(roleName))
        {
            Roles.CreateRole(roleName);
            lblResult.Text = roleName + " Role Created Successfully";
            lblResult.ForeColor = Color.Green;
            txtRole.Text = string.Empty;
            BindRolesDetails();
            Response.Redirect("~/PageAdmin/AsignRole.aspx");
        }
        else
        {
            txtRole.Text = string.Empty;
            lblResult.Text = roleName + " Role already exists";
            lblResult.ForeColor = Color.Red;
        }
    }
    // This RowDeleting event is used to delete Roles
    protected void gvRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lableRole = (Label)gvRoles.Rows[e.RowIndex].FindControl("lblRole");
        Roles.DeleteRole(lableRole.Text, false);
        lblResult.ForeColor = Color.Green;
        lblResult.Text = lableRole.Text + " Role Deleted Successfully";
        BindRolesDetails();
        Response.Redirect("~/PageAdmin/AsignRole.aspx");
    }

    protected void btnAssign_Click(object sender, EventArgs e)
    {
        string userName = ddlUsers.SelectedItem.Text;
        string[] userRoles = Roles.GetRolesForUser(userName);
        string errorMessage = string.Empty;
        string successMessage = string.Empty;
        string roleName = string.Empty;
        int i = 0;
        int j = 0;
        foreach (GridViewRow gvrow in GridView1.Rows)
        {
            CheckBox chk = (CheckBox)gvrow.FindControl("chkRole");
            Label lbl = (Label)gvrow.FindControl("lblRole");
            roleName = lbl.Text;
            if (chk.Checked)
            {
                int index = Array.IndexOf(userRoles, roleName);
                if (index == -1)
                {
                    Roles.AddUserToRole(userName, roleName);
                    successMessage += roleName + ", ";
                    j++;
                }
            }
            else
            {
                int index = Array.IndexOf(userRoles, roleName);
                if (index > -1)
                {
                    // Remove the user from the role
                    string logName = Page.User.Identity.Name;
                    if(userName == logName)
                    {
                        lblError.Text = "Current user Can't be remove from role";
                        i++;
                    }
                    else
                    {
                      Roles.RemoveUserFromRole(userName, roleName);
                      errorMessage += roleName + ", ";
                      i++;
                    }
                }
            }
        }
        if (errorMessage != string.Empty)
        {
            if (i > 1)
            {
                lblError.Text = userName + " was removed from roles " + errorMessage.Substring(0, errorMessage.Length - 2);
            }
            else
            {
                lblError.Text = userName + " was removed from role " + errorMessage.Substring(0, errorMessage.Length - 2);
            }
            lblError.ForeColor = Color.Red;
        }
        else
        {
            lblError.Text = string.Empty;
        }
        if (successMessage != string.Empty)
        {
            if (j > 1)
            {
                lblSuccess.Text = successMessage.Substring(0, successMessage.Length - 2) + " roles assigned to " + userName;
            }
            else
            {
                lblSuccess.Text = successMessage.Substring(0, successMessage.Length - 2) + " role assigned to " + userName;
            }
            lblSuccess.ForeColor = Color.Green;
        }
        else
        {
            lblSuccess.Text = string.Empty;
        }
    }


    protected void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblSuccess.Text = string.Empty;
        lblError.Text = string.Empty;
        string userName = ddlUsers.SelectedItem.Text;
        string[] userRoles = Roles.GetRolesForUser(userName);
        string rolename = string.Empty;
        foreach (GridViewRow gvrow in GridView1.Rows)
        {
            CheckBox chk = (CheckBox)gvrow.FindControl("chkRole");
            Label lbl = (Label)gvrow.FindControl("lblRole");
            rolename = lbl.Text;
            int index = Array.IndexOf(userRoles, rolename);
            if (index > -1)
            {
                chk.Checked = true;
            }
            else
            {
                chk.Checked = false;
            }
        }

    }
    protected void btnBackup_Click(object sender, EventArgs e)
    {
        BackupData("OnlineLibrary", "sa", "12345", "AMIT-PC", "C:\\back");
    }
    protected void BackupData(String databaseName, String userName, String password, String serverName, String destinationPath)
    {
        Backup sqlBackup = new Backup();

        sqlBackup.Action = BackupActionType.Database;
        sqlBackup.BackupSetDescription = "ArchiveDataBase:" +
                                         DateTime.Now.ToShortDateString();
        sqlBackup.BackupSetName = "Archive";

        sqlBackup.Database = databaseName;

        BackupDeviceItem deviceItem = new BackupDeviceItem(destinationPath, DeviceType.File);
        ServerConnection connection = new ServerConnection(serverName, userName, password);
        Server sqlServer = new Server(connection);

        Database db = sqlServer.Databases[databaseName];

        sqlBackup.Initialize = true;
        sqlBackup.Checksum = true;
        sqlBackup.ContinueAfterError = true;

        sqlBackup.Devices.Add(deviceItem);
        sqlBackup.Incremental = false;

        sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);
        sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

        sqlBackup.FormatMedia = false;

        sqlBackup.SqlBackup(sqlServer);
    }
}
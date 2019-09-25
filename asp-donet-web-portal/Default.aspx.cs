// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE.txt in the project root for license information.
using findmydocs.Models;
using Microsoft.Azure.OperationalInsights;
using Microsoft.Rest.Azure.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace findmydocs
{
    public partial class _Default : Page
    {
        private static string FirstFunctionKey = ConfigurationManager.AppSettings["ida:FirstAzFunctionKey"];
        private static string SecondFunctionKey = ConfigurationManager.AppSettings["ida:SecondAzFunctionKey"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Label2.Visible = false;
                // Log Analytics API Docs: 
                // https://dev.loganalytics.io/documentation/Tools/CSharp-Sdk
                GetAIPFiles();

            }

        }

        public void GetAIPFiles()
        {
            WebClient client = new WebClient();
            string strUri = "https://<APP_NAME>.azurewebsites.net/api/FUNCTION_NAME?name=USERUPN@DOMAIN&code=" + FirstFunctionKey;
            string strNewUri = strUri.Replace("USERUPN@DOMAIN", HttpContext.Current.User.Identity.Name.ToLower());

            string results = client.DownloadString(strNewUri);
            var data = JsonConvert.DeserializeObject<AIPFile.Table>(results);

            if (data.rows.Count() > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("File Name", typeof(string));
                dt.Columns.Add("Activity", typeof(string));
                dt.Columns.Add("Label Name", typeof(string));
                dt.Columns.Add("Time Generated", typeof(DateTime));
                dt.Columns.Add("Protected", typeof(string));
                dt.Columns.Add("Machine Name", typeof(string));

                for (int i = 0; i < data.rows.Count(); i++)
                {
                    string file = data.rows[i][0].ToString();
                    string activity = data.rows[i][1].ToString();
                    string labelname = data.rows[i][2].ToString();
                    DateTime TimeGenerated = Convert.ToDateTime(data.rows[i][3]);
                    string protectionstatus = data.rows[i][4].ToString();
                    string machinename = data.rows[i][5].ToString();

                    DataRow row = dt.NewRow();
                    row[0] = file;
                    row[1] = activity;
                    row[2] = labelname;
                    row[3] = TimeGenerated;
                    row[4] = protectionstatus;
                    row[5] = machinename;
                    dt.Rows.Add(row);
                }

                GridView1.DataSource = dt;
                GridView1.DataBind();
                Label3.Visible = true;

            }
            else
            {
                Label1.Text = "No protected documents found at the moment.";
                Label1.Visible = true;

            }

        }

        // Get list of AIP activity for the selected file
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label1.Text = "";
            Label1.Visible = false;
            GridView2.DataSource = null;
            GridView2.DataBind();

            // FILE TRACKING SITE:
            WebClient client = new WebClient();
            string strUri = "https://<APP_NAME>.azurewebsites.net/api/<FUNCTION_NAME>?name=AIPDOCUMENTNAME&code=" + SecondFunctionKey;
            string newStringUri = strUri.Replace("AIPDOCUMENTNAME", GridView1.SelectedRow.Cells[1].Text.Trim());

            string results = client.DownloadString(newStringUri);

            //var objs = JsonConvert.DeserializeObject(results);

            var data = JsonConvert.DeserializeObject<AIPFile.Table>(results);

            if (data.rows.Count() < 1)
            {
                Label1.Text = "No data found for " + GridView1.SelectedRow.Cells[1].Text;
                Label1.Visible = true;

            } else
            {
                DataTable dt = new DataTable();
                //dt.Columns.Add("ContentId", typeof(string));
                //dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("Accessed By", typeof(string));
                dt.Columns.Add("Activity", typeof(string));
                dt.Columns.Add("Protected By", typeof(string));
                dt.Columns.Add("Event Time", typeof(DateTime));
                dt.Columns.Add("Protection Time", typeof(DateTime));
                dt.Columns.Add("Application", typeof(string));
                dt.Columns.Add("IPAddress", typeof(string));
                dt.Columns.Add("Machine Name", typeof(string));

                for (int i = 0; i < data.rows.Count(); i++)
                {
                    //string contentid = data.rows[i][0].ToString();
                    //string filename = data.rows[i][1].ToString();
                    string accessedby = data.rows[i][2].ToString();
                    string activity = data.rows[i][3].ToString();
                    string protectedby = data.rows[i][4].ToString();
                    DateTime eventtime = Convert.ToDateTime(data.rows[i][5]);
                    DateTime protectiontime = Convert.ToDateTime(data.rows[i][6]);
                    string appname = data.rows[i][7].ToString();
                    string ipaddress = data.rows[i][8].ToString();
                    string devicename = data.rows[i][9].ToString();

                    DataRow row = dt.NewRow();
                     //row[0] = contentid;
                    //row[1] = filename;
                    row[0] = accessedby;
                    row[1] = activity;
                    row[2] = protectedby;
                    row[3] = eventtime;
                    row[4] = protectiontime;
                    row[5] = appname;
                    row[6] = ipaddress;
                    row[7] = devicename;

                    dt.Rows.Add(row);
 
                    GridView2.DataSource = dt;
                    GridView2.DataBind();
                    Label2.Text = "File activity for " + GridView1.SelectedRow.Cells[1].Text;
                    Label2.Visible = true;
                }

            }


        }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[1].Text == "AccessDenied")
                {
                    e.Row.BackColor = System.Drawing.Color.Yellow;
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    e.Row.Font.Bold = true;
                }
            }
        }

    }
}
using EcommerceProject.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EcommerceProject
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Report2WithSqlConn();
        }
        private void Report2WithSqlConn()
        {
            string query = @"SELECT TOP (1000) em.[proID]
                  ,em.[proName]
                  ,em.[Price]
                  ,em.[salesDate]
                  ,em.[InOut]
                  ,em.[FilePath]
                  ,dg.[BrandsName] BrandsID
              FROM [EcommerceDB].[dbo].[products] em
              left join productBrands dg on em.BrandsID=dg.BrandsID WHERE em.[proID] = " + Form1.proID;
            string connectionString = "server=DESKTOP-4DJP6OT;Initial Catalog=EcommerceDB;Integrated Security=True;";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds, "products");
            for (var i = 0; i < ds.Tables["products"].Rows.Count; i++)
            {
                if (ds.Tables["products"].Rows[i]["FilePath"] != null)
                {
                    if (!string.IsNullOrEmpty(ds.Tables["products"].Rows[i]["FilePath"].ToString()))
                    {
                        string strFilePath = Application.StartupPath + ds.Tables["products"].Rows[i]["FilePath"].ToString();
                        if (File.Exists(strFilePath))
                        {
                            ds.Tables["products"].Rows[i]["FilePath"] = strFilePath;
                        }
                    }
                }
            }

            CrystalReport1 cr2 = new CrystalReport1();
            cr2.SetDataSource(ds);
            crystalReportViewer1.ReportSource = cr2;
            con.Close();
            crystalReportViewer1.Refresh();
        }
    }
}

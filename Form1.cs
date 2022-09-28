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
    public partial class Form1 : Form
    {
        SqlCommand cmd = null;
        SqlDataAdapter adapt = null;
        public static int proID = 0;
        public Form1()
        {
            InitializeComponent();
            AddButtonColumn();
            LoadEmp();
            ResetProduct();
        }
        SqlConnection con = new SqlConnection("data source=DESKTOP-4DJP6OT; database =EcommerceDB; integrated security = true");
        int brandsID = 0;
        public DataTable GetBrands()
        {
            DataSet dsData = new DataSet();
            {
                con.Open();
                string query = "SELECT [BrandsID],[BrandsName] FROM [productBrands]";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dsData);
                con.Close();
                return dsData.Tables[0];
            }
        }

        private void BtnBSave_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = null;
            con.Open();
            string insertQ = "INSERT INTO [productBrands]([BrandsName]) VALUES('" + txtBName.Text.Trim() + "')";
            cmd = new SqlCommand(insertQ, con);
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                MessageBox.Show("INSERT Perform Succesfully.");
            }
            else
            {
                MessageBox.Show("Operation failed");
            }
            con.Close();

            ResetBrand();
            FillCmbProduct();
        }

        private void BtnBUpdate_Click(object sender, EventArgs e)
        {
            string selectQ = "SELECT [BrandsID],[BrandsName] FROM [productBrands] WHERE [BrandsID] = " + brandsID;
            SqlCommand cmd = new SqlCommand(selectQ, con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    brandsID = Convert.ToInt32(reader["BrandsID"]);
                }
                reader.Close();
                string updateQ = "UPDATE [productBrands] SET [BrandsName] = '" + txtBName.Text.Trim() + "' WHERE [BrandsID] = " + brandsID;
                cmd = new SqlCommand(updateQ, con);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("UPDATE Perform Succesfully.");
                }
                else
                {
                    MessageBox.Show("Operation failed.");
                }
            }
            else
            {
                MessageBox.Show("No data found.");
            }
            con.Close();
            ResetBrand();
            FillCmbProduct();
        }
        private void ResetBrand()
        {
            btnBSave.Show();
            btnBUpdate.Hide();
            txtBName.Text = "";
            dataGridView1.DataSource = GetBrands();
            brandsID = 0;
            string q = "SELECT e.[proID],e.[proName],d.[BrandsName] FROM [products] e JOIN [productBrands] d ON e.BrandsID = d.BrandsID";           
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds, "products");
        }

        private void BtnBDelete_Click(object sender, EventArgs e)
        {
            ResetBrand();
            FillCmbProduct();
        }
        private void AddButtonColumn()
        {
            DataGridViewButtonColumn btnReport = new DataGridViewButtonColumn();
            btnReport.HeaderText = "#";
            btnReport.Text = "Report";
            btnReport.Name = "btnReport";
            btnReport.UseColumnTextForButtonValue = true;
            dataGridView3.Columns.Add(btnReport);

            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.HeaderText = "#";
            btnEdit.Text = "Edit";
            btnEdit.Name = "btnEdit";
            btnEdit.UseColumnTextForButtonValue = true;
            dataGridView3.Columns.Add(btnEdit);

            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
            btnDelete.HeaderText = "#";
            btnDelete.Text = "Delete";
            btnDelete.Name = "btnDelete";
            btnDelete.UseColumnTextForButtonValue = true;
            dataGridView3.Columns.Add(btnDelete);
        }
        private string UpdateFile()
        {
            string strFilePath = string.Empty;
            if (isNewFile)
            {
                strFilePath = "\\images\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                File.Copy(newFilePath, Application.StartupPath + strFilePath);

                //remove old file
                RemoveFile(Application.StartupPath + oldFilePath);
            }
            else
            {
                strFilePath = oldFilePath;
            }

            return strFilePath;
        }
        private void RemoveFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (!filePath.Contains("default"))
                {
                    File.Delete(filePath);
                }
                pictureBox1.Image = null;
            }
        }
        private void LoadEmp()
        {
            con.Open();
            DataTable dt = new DataTable();
            string query = @"SELECT e.[proID],e.[proName],e.[Price],e.[SalesDate],e.[InOut],e.[FilePath],d.[BrandsName] FROM [products] e JOIN [productBrands] d ON e.BrandsID = d.BrandsID";
            adapt = new SqlDataAdapter(query, con);
            adapt.Fill(dt);
            dataGridView3.DataSource = dt;
            con.Close();
        }
        private void AddNew()
        {
            // save image
            string strFilePath = AddFile();
            string query = @"INSERT INTO [dbo].[products]
                ([proName]
               ,[Price]
               ,[SalesDate]
               ,[InOut]
               ,[FilePath]
               ,[BrandsID])
                VALUES
               (@proName
               ,@Price
               ,@SalesDate
               ,@InOut
               ,@FilePath
               ,@BrandsID)";
            cmd = new SqlCommand(query, con);
            con.Open();
            cmd.Parameters.AddWithValue("@proID", txtID.Text.Trim());
            cmd.Parameters.AddWithValue("@proName", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());
            cmd.Parameters.AddWithValue("@SalesDate", pickDoB.Value);
            cmd.Parameters.AddWithValue("@InOut", radioIn.Checked == true ? "StockIn" : "StockOut");
            cmd.Parameters.AddWithValue("@FilePath", strFilePath);
            cmd.Parameters.AddWithValue("@BrandsID", cmbPro.SelectedValue.ToString());
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Data added successfully.");
        }
        private void Updates()
        {
            // save image
            string strFilePath = UpdateFile();

            cmd = new SqlCommand(@"UPDATE [products]
               SET [proName] = @proName
                  ,[Price] = @Price
                  ,[SalesDate] = @SalesDate
                  ,[InOut] = @InOut
                  ,[BrandsID] = @BrandsID
             WHERE [proID] = @proID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@proID", proID);
            cmd.Parameters.AddWithValue("@proName", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());
            cmd.Parameters.AddWithValue("@SalesDate", pickDoB.Value);
            cmd.Parameters.AddWithValue("@InOut", radioIn.Checked == true ? "StockIn" : "StockOut");
            cmd.Parameters.AddWithValue("@FilePath", strFilePath);
            cmd.Parameters.AddWithValue("@BrandsID", cmbPro.SelectedValue.ToString());
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Data updated successfully.");
        }
        private void Edit()
        {
            con.Open();
            DataTable dt = new DataTable();
            string query = @"SELECT [proID]
                  ,[proName]
                  ,[Price]
                  ,[SalesDate]
                  ,[InOut]
                  ,[FilePath]
                  ,[BrandsID]
              FROM [products] WHERE [proID] = " + proID;
            adapt = new SqlDataAdapter(query, con);
            adapt.Fill(dt);
            con.Close();

            if (dt.Rows.Count > 0)
            {
                btnAdd.Text = "Update";

                proID = Convert.ToInt32(dt.Rows[0]["proID"].ToString());
                txtName.Text = dt.Rows[0]["proName"].ToString();
                txtPrice.Text = dt.Rows[0]["Price"].ToString();
                pickDoB.Value = Convert.ToDateTime(dt.Rows[0]["SalesDate"].ToString());
                radioIn.Checked = dt.Rows[0]["InOut"].ToString() == "StockIn" ? true : false;
                radioOut.Checked = dt.Rows[0]["InOut"].ToString() == "StockIn" ? false : true;
                // set image
                if (dt.Rows[0]["FilePath"].ToString() != null)
                {
                    if (File.Exists(Application.StartupPath + dt.Rows[0]["FilePath"].ToString()))
                    {
                        using (var img = new Bitmap(Application.StartupPath + dt.Rows[0]["FilePath"].ToString()))
                        {
                            pictureBox1.Image = new Bitmap(img);
                            lblFile.Text = dt.Rows[0]["FilePath"].ToString();
                            isNewFile = false;
                            oldFilePath = dt.Rows[0]["FilePath"].ToString();
                        }
                    }
                    else
                    {
                        using (var img = new Bitmap(Application.StartupPath + "\\images\\default_img.jpg"))
                        {
                            pictureBox1.Image = new Bitmap(img);
                            lblFile.Text = "\\images\\default_img.jpg";
                        }
                    }
                }
                else
                {
                    using (var img = new Bitmap(Application.StartupPath + "\\images\\default_img.jpg"))
                    {
                        pictureBox1.Image = new Bitmap(img);
                        lblFile.Text = "\\images\\default_img.jpg";
                    }
                }
                cmbPro.SelectedValue = dt.Rows[0]["BrandsID"].ToString();
            }
        }
        private void Delete()
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure to remove?", "Confirm Message", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                con.Open();
                DataTable dt = new DataTable();
                string query = @"SELECT [proID]
                      ,[proName]
                      ,[Price]
                      ,[SalesDate]
                      ,[InOut]
                      ,[FilePath]
                      ,[BrandsID]
                  FROM [products] WHERE [proID] = " + proID;
                adapt = new SqlDataAdapter(query, con);
                adapt.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["FilePath"] != null)
                    {
                        // remove old file
                        RemoveFile(Application.StartupPath + dt.Rows[0]["FilePath"].ToString());
                    }

                    string q = @"DELETE FROM [products]
                    WHERE proID = @proID";
                    cmd = new SqlCommand(q, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@proID", proID);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    ResetProduct();
                    LoadEmp();
                    MessageBox.Show("Data removed successfully.");

                }
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (proID > 0)
            {
                Updates();
            }
            else
            {
                AddNew();
            }
            ResetProduct();
            LoadEmp();
        }
        private void ResetProduct()
        {
            proID = 0;
            btnAdd.Text = "Add";
            newFilePath = "";
            pictureBox1.Image = null;
            using (var img = new Bitmap(Application.StartupPath + "\\images\\default_img.jpg"))
            {
                pictureBox1.Image = new Bitmap(img);
                lblFile.Text = "\\images\\default_img.jpg";
            }           
            txtName.Text = "";
            txtPrice.Text = "";
            pickDoB.Text = "";
            radioIn.Checked = true;
            cmbPro.Text = "";
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetProduct();
        }
        private void FillCmbProduct()
        {
            var brands = GetBrands();
            cmbPro.DataSource = brands;
            cmbPro.DisplayMember = "BrandsName";
            cmbPro.ValueMember = "BrandsID";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetBrand();
            ResetProduct();
            FillCmbProduct();
        }

        private void DataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1 && dataGridView3.Rows.Count > e.RowIndex + 1)
            {
                var v = dataGridView3.Rows[e.RowIndex].Cells["proID"].Value;
                proID = dataGridView3.Rows[e.RowIndex].Cells["proID"].Value == null ? 0 : Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["proID"].Value);
                if ("Report" == dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                {
                    Form2 f2 = new Form2();
                    f2.Show();
                }
                if ("Edit" == dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                {
                    Edit();
                }
                if ("Delete" == dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                {
                    Delete();
                }
            }
        }      

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            SelectFile();
        }
        private void SelectFile()
        {
            open.Filter = "JPG (*.JPG)|*.jpg";
            if (open.ShowDialog() == DialogResult.OK)
            {
                using (var img = new Bitmap(open.FileName))
                {
                    pictureBox1.Image = new Bitmap(img);
                }
                // image file path
                newFilePath = open.FileName;
                isNewFile = true;
            }
        }
        string newFilePath = string.Empty;
        string oldFilePath = string.Empty;
        bool isNewFile = true;
        OpenFileDialog open = new OpenFileDialog();

        private string AddFile()
        {
            string strFilePath = string.Empty;
            if (isNewFile)
            {
                strFilePath = "\\images\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                File.Copy(newFilePath, Application.StartupPath + strFilePath);
            }

            return strFilePath;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0) // edit button
            {
                btnBSave.Hide();
                btnBUpdate.Show();
                string BrandsID = dataGridView1.Rows[e.RowIndex].Cells["BrandsID"].Value.ToString();
                string BrandsName = dataGridView1.Rows[e.RowIndex].Cells["BrandsName"].Value.ToString();
                Int32.TryParse(BrandsID, out brandsID);
                txtBName.Text = BrandsName;
            }

            if (e.ColumnIndex == 1) // delete button
            {
                string BrandsID = dataGridView1.Rows[e.RowIndex].Cells["BrandsID"].Value.ToString();
                string BrandsName = dataGridView1.Rows[e.RowIndex].Cells["BrandsName"].Value.ToString();
                Int32.TryParse(BrandsID, out brandsID);

                // example of connected architechture
                if (MessageBox.Show("Are you sure to delete '" + BrandsName + "'", "Delete confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string selectQ = "SELECT [BrandsID],[BrandsName] FROM [productBrands] WHERE [BrandsID] = " + brandsID;
                    SqlCommand cmd = new SqlCommand(selectQ, con);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            brandsID = Convert.ToInt32(reader["BrandsID"]);
                        }
                        reader.Close();

                        string deleteQ = "DELETE FROM [productBrands] WHERE [BrandsID] = " + brandsID;
                        cmd = new SqlCommand(deleteQ, con);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("DELETE Perform Succesfully.");
                        }
                        else
                        {
                            MessageBox.Show("Operation failed.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No data found.");
                    }
                    con.Close();
                    ResetBrand();
                }
            }
        }        
    }
}

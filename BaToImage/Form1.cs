using BaToImage.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaToImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public static string connectionString;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //frmServerLogin frm = new frmServerLogin();
            //frm.ShowDialog();

            if (cmbAuthentication.Text == "Windows Authentication")
            {
                connectionString = "Data Source=" + txtServer.Text + "; Integrated Security=True;";
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        DataTable databases = con.GetSchema("Databases");
                        cmbDatabase.DataSource = databases;
                        cmbDatabase.DisplayMember = "database_name";
                        cmbDatabase.ValueMember = "dbid";

                    }
                }
                catch (Exception)
                {

                }
            }else if(cmbAuthentication.Text=="SQL Server Authentication")
            {
                connectionString = "Data Source=" + txtServer.Text + "; Integrated Security=False; User ID="+txtUserId.Text+";Password="+txtPassword.Text;
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        DataTable databases = con.GetSchema("Databases");
                        cmbDatabase.DataSource = databases;
                        cmbDatabase.DisplayMember = "database_name";
                        cmbDatabase.ValueMember = "dbid";

                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                MessageBox.Show("Sorry, No Authentication Type Selected!");
            }

        }

        private void cmbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAuthentication.Text == "Windows Authentication")
            {
                try
                {

                    connectionString = "Data Source=" + txtServer.Text + "; Integrated Security=True; Initial Catalog =" + cmbDatabase.Text;
                    cmbTables.Items.Clear();
                    cmbTables.Text = String.Empty;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DataTable table = connection.GetSchema("Tables");
                        //List<string> TableNames = new List<string>();
                        foreach (DataRow row in table.Rows)
                        {
                            cmbTables.Items.Add(row[2].ToString());
                        }


                    }

                }
                catch (Exception)
                {

                }

            }else if(cmbAuthentication.Text=="SQL Server Authentication")
            {
                try
                {

                    connectionString = "Data Source=" + txtServer.Text + "; Integrated Security=False;Initial Catalog =" + cmbDatabase.Text + "; User ID=" + txtUserId.Text + ";Password=" + txtPassword.Text;
                    cmbTables.Items.Clear();
                    cmbTables.Text = String.Empty;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DataTable table = connection.GetSchema("Tables");
                        //List<string> TableNames = new List<string>();
                        foreach (DataRow row in table.Rows)
                        {
                            cmbTables.Items.Add(row[2].ToString());
                        }


                    }

                }
                catch (Exception)
                {

                }
            }
            else
            {
                MessageBox.Show("Sorry, No Authentication Type Selected!");
            }

        }

        private void cmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbColumnName.Items.Clear();
            cmbColumn.Items.Clear();

            try
            {

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();
                DataTable schemaTable;
                SqlDataReader reader;

                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM " + cmbTables.Text;
                reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);

                schemaTable = reader.GetSchemaTable();

                    foreach (DataRow dr in schemaTable.Rows)
                    {
                        cmbColumn.Items.Add(dr["ColumnName"].ToString());
                        cmbColumnName.Items.Add(dr["ColumnName"].ToString());
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fldDialog = new FolderBrowserDialog())
            {
                if (fldDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFile.Text= fldDialog.SelectedPath;
                }
            }
        }

        public void SaveImage(Image img, string path)
        {
            img.Save(path);
        }

        public Image ConvertByteArrayToImage(byte[] bytesA)
        {
            System.IO.MemoryStream strm = new System.IO.MemoryStream(bytesA);
            return Image.FromStream(strm);
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                var table = new DataTable();
                using (var da = new SqlDataAdapter("SELECT " + cmbColumn.Text + "," + cmbColumnName.Text + " FROM " + cmbTables.Text, connectionString))
                {
                    da.Fill(table);
                }
                foreach (DataRow row in table.Rows)
                {
                    byte[] rawImage = (byte[])row[cmbColumn.Text];
                    string imageName = row[cmbColumnName.Text].ToString();
                    SaveImage(ConvertByteArrayToImage(rawImage), @"" + txtFile.Text +"\\"+ imageName + ".jpg");

                }
                MessageBox.Show("Convertion Successfull");
            }catch(Exception)
            {

            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            About frm = new About();
            frm.ShowDialog();
        }

        private void cmbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            tableLayoutPanelUserLogin.Enabled = cmbAuthentication.Text == "SQL Server Authentication";
        }
    }
}

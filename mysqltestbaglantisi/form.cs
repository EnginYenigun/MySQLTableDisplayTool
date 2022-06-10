using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace mysqltestbaglantisi
{
    public partial class form : Form
    {
        public form()
        {
            InitializeComponent();
        }
        string constring, table;
        MySqlConnection mscon;
        bool move, maximize = false;
        Point pt;
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "EXIT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (maximize == false)
            {
                this.WindowState = FormWindowState.Maximized;
                maximize = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                maximize = false;
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void pnlTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            pt = e.Location;
        }
        private void pnlTopBar_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }
        private void pnlTopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                Point csp = PointToScreen(e.Location);
                Location = new Point(csp.X - pt.X, csp.Y - pt.Y);
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (txtServer.Text == "" || txtServer.Text == null || txtDatabase.Text == "" || txtDatabase.Text == null || txtUid.Text == "" || txtUid.Text == null || txtPass.Text == "" || txtPass.Text == " " || txtPass.Text == null)
            {
                MessageBox.Show("Please do not leave connection information blank!", "EMPTY INFO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    constring = "Server=" + txtServer.Text + ";Database=" + txtDatabase.Text + ";Uid=" + txtUid.Text + ";Pwd='" + txtPass.Text + "';";
                    mscon = new MySqlConnection(constring);
                    Properties.Settings.Default.server = txtServer.Text;
                    Properties.Settings.Default.database = txtDatabase.Text;
                    Properties.Settings.Default.uid = txtUid.Text;
                    Properties.Settings.Default.pass = txtPass.Text;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Connection Succesful!", "SUCCES", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTable.Enabled = true;
                    btnGet.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection Failed!\n\n" + ex, "FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTable.Enabled = false;
                    btnGet.Enabled = false;
                }
            }
        }
        private void txtServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = char.IsWhiteSpace(e.KeyChar);
        }
        private void form_Load(object sender, EventArgs e)
        {
            txtDatabase.Text = Properties.Settings.Default.database;
            txtServer.Text = Properties.Settings.Default.server;
            txtUid.Text = Properties.Settings.Default.uid;
            txtPass.Text = Properties.Settings.Default.pass;
            txtTable.Text = Properties.Settings.Default.table;
        }
        private void btnGet_Click(object sender, EventArgs e)
        {
            if (txtTable.Text == null || txtTable.Text == "" || txtTable.Text == " ")
            {
                MessageBox.Show("Please do not leave the table name black!", "EMPTY INFO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                table = txtTable.Text;
                try
                {
                    if (mscon.State == ConnectionState.Closed)
                    {
                        mscon.Open();
                    }
                    string cmdtext = "select * from " + txtTable.Text;
                    MySqlCommand cmd = new MySqlCommand(cmdtext, mscon);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    datagrid.DataSource = dt;
                    mscon.Close();
                    Properties.Settings.Default.table = txtTable.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception err)
                {
                    mscon.Close();
                    MessageBox.Show("Couldn't get a table!\n\n" + err.Message, "FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                mscon.Close();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Microsoft.Win32;

namespace TicketClientApp
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "نام کاربری...")
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "نام کاربری...";
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "رمز عبور...")
            {
                textBox2.Text = "";
            }

        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                textBox2.Text = "رمز عبور...";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();         
        }

        private void Login()
        {
            try
            {

                if (string.IsNullOrEmpty(textBox1.Text.Trim()) || textBox1.Text == "نام کاربری...")
                    textBox1.Focus();
                else
                if (string.IsNullOrEmpty(textBox2.Text.Trim()) || textBox2.Text == "رمز عبور...")
                    textBox2.Focus();
                else
                {
                    TicketWebService.AD_AuthenticateWebService TicketService = new TicketWebService.AD_AuthenticateWebService();

                    var authenticate_ret = TicketService.UserAuthenticate(textBox1.Text.Trim(), textBox2.Text.Trim());

                    if (authenticate_ret != null)
                    {
                        if (!String.IsNullOrEmpty(authenticate_ret.message))
                            MessageBox.Show(authenticate_ret.message, "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            var token = authenticate_ret.token;

                            if (chk_remind.Checked)
                            {
                                ClsFuncs.WriteTotextFile(authenticate_ret);

                                _tk = token.ToString();
                                san = authenticate_ret.user_san;
                                cn = authenticate_ret.user_cn;
                                img = authenticate_ret.user_image;

                                RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                                reg.SetValue("TicketClientApp", Application.ExecutablePath.ToString());
                                //MessageBox.Show("You have been successfully saved.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                            else
                            {
                                string fileName = Application.StartupPath + "\\Ticket.txt";
                                FileInfo fi = new FileInfo(fileName);

                                if (fi.Exists)
                                {
                                    //fi.Delete();
                                    System.IO.File.WriteAllText(fileName, String.Empty);
                                }

                            }

                            this.Hide();
                            Form1 frm = new Form1();
                            frm.ShowDialog();
                        }
                    }
                    else
                        MessageBox.Show("عدم خروجی وب سرویس", "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
            catch (Exception em)
            {
                MessageBox.Show(em.Message, "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string _tk = "";
        public static string san = "";
        public static string cn = "";
        public static string img = "";


        private void FrmLogin_Load(object sender, EventArgs e)
        {
            textBox1.Text = "نام کاربری...";
            textBox2.Text = "رمز عبور...";

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (!string.IsNullOrEmpty(textBox1.Text.Trim()) && textBox1.Text != "نام کاربری...")
                    textBox2.Focus();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (!string.IsNullOrEmpty(textBox2.Text.Trim()) && textBox2.Text != "رمز عبور...")
                {
                    btnLogin.Focus();

                    if (!string.IsNullOrEmpty(textBox1.Text.Trim()) && textBox1.Text != "نام کاربری...")
                    {
                        Login();
                    }
                }
        }

        private void FrmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

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
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Media;
using System.Drawing.Drawing2D;

namespace TicketClientApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {

            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            //this.Location = new Point(0, 0);

            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - Size.Width,
                                      Screen.PrimaryScreen.WorkingArea.Bottom - Size.Height);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized
            //hide it from the task bar
            //and show the system tray icon (represented by the NotifyIcon control)
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }


        public static string _tk = "";
        public static SoundPlayer sndplayr = new SoundPlayer(TicketClientApp.Properties.Resources._3);

        public static int CurrentNewMessageCount = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMessageFromWebsercice();

            //pictureBox1.Image = global::TicketClientApp.Properties.Resources._25;
            GraphicsPath gp2 = new GraphicsPath();
            gp2.AddEllipse(pictureBox1.DisplayRectangle);
            pictureBox1.Region = new Region(gp2);

        }

        private void LoadMessageFromWebsercice()
        {
            string fileName = Application.StartupPath + "\\temp.txt";
            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string user_san = "";
                    string user_cn = "";
                    string token = "";
                    string user_image = "";

                    string line1 = sr.ReadLine();
                    if (line1 != null)
                    {
                        user_san = ClsFuncs.DESDecrypt(line1);

                        string line2 = sr.ReadLine();
                        if (line2 != null)
                        {
                            user_cn = ClsFuncs.DESDecrypt(line2);

                            string line3 = sr.ReadLine();
                            if (line3 != null)
                            {
                                token = line3;

                                string line4 = sr.ReadLine();
                                if (line4 != null)
                                    user_image = line4;

                                if (!String.IsNullOrEmpty(token))
                                {
                                    _tk = token;

                                    lblName.Text = user_cn;

                                    #region profile info

                                    if (!String.IsNullOrEmpty(user_image))
                                    {
                                        string sURL = "https://ticket.nkums.ac.ir/get_file.aspx?fn=" + user_image + "&width=200";

                                        WebRequest req = WebRequest.Create(sURL);

                                        WebResponse res = req.GetResponse();

                                        Stream imgStream = res.GetResponseStream();

                                        Image img1 = Image.FromStream(imgStream);

                                        imgStream.Close();

                                        pictureBox1.Image = img1;
                                    }
                                    else
                                        pictureBox1.Image = global::TicketClientApp.Properties.Resources.user;

                                    #endregion


                                    CurrentNewMessageCount = LoadMessages(token);


                                    timer1.Enabled = true;


                                }

                            }
                        }
                    }
                }
            }
            else
            {
                this.Hide();
                FrmLogin frmLogin = new FrmLogin();
                frmLogin.ShowDialog();
            }
        }

        private int LoadMessages(string token)
        {
            int NewMessageCount = 0;

            TicketWebService.AD_AuthenticateWebService TicketService = new TicketWebService.AD_AuthenticateWebService();

            var Service_retval = TicketService.FetchNewConversations(token);

            if (Service_retval != null)
            {

                //if (authenticate_ret.Type == "ExpireSession")
                //{
                //    ReCreateToken_withNewLogin();
                //}
                //else
                //{

                if (!String.IsNullOrEmpty(Service_retval.Msg))
                    MessageBox.Show(Service_retval.Msg, "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    int location_y = 0;
                    int cnt = 0;

                    var LstConversations = Service_retval.LstConversations;
                    if (LstConversations.Count() == 0)
                    {
                        #region ziroMessages

                        Label lbl_ziroMessages = new Label();
                        // 
                        // lbl_ziroMessages
                        // 
                        lbl_ziroMessages.Dock = System.Windows.Forms.DockStyle.Top;
                        lbl_ziroMessages.Font = new System.Drawing.Font("B Mitra", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        lbl_ziroMessages.ForeColor = System.Drawing.Color.LimeGreen;
                        lbl_ziroMessages.Location = new System.Drawing.Point(0, 0);
                        lbl_ziroMessages.Name = "lbl_ziroMessages";
                        lbl_ziroMessages.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                        lbl_ziroMessages.Size = new System.Drawing.Size(294, 36);
                        lbl_ziroMessages.TabIndex = 0;
                        lbl_ziroMessages.Text = "پیام  جدیدی یافت نگردید";
                        lbl_ziroMessages.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                        pnlMessages.Controls.Add(lbl_ziroMessages);

                        #endregion
                    }
                    else
                    {
                        foreach (var item in LstConversations)
                        {
                            AddNewConversatinItem(item, cnt, location_y);
                            location_y += 66;

                            NewMessageCount += item.NewMessagesCount;
                        }
                    }


                    if (Service_retval.UserAuthenticateRet != null)
                    {
                        ClsFuncs.WriteTotextFile(Service_retval.UserAuthenticateRet);
                        _tk = Service_retval.UserAuthenticateRet.token.ToString();
                    }


                }

                //}
            }
            else
                MessageBox.Show("عدم خروجی وب سرویس", "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);


            return NewMessageCount;

        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void tokenLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string fileName = Application.StartupPath + "\\temp.txt";
            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                fi.Delete();
            }

            Application.Exit();
            //try
            //{
            //    tokenLink.LinkVisited = true;

            //    string url = "https://ticket.nkums.ac.ir/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk;
            //    //string url = "http://localhost:9609/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk;

            //    System.Diagnostics.Process.Start(url);

            //    this.WindowState = FormWindowState.Minimized;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Unable to open link that was clicked.");
            //}
        }

        public void AddNewConversatinItem(TicketWebService.ConversationInfo item, int i, int location_y)
        {

            Panel pnl_msgContainer = new Panel();
            Label lbl_bottom_line_left = new Label();
            LinkLabel lbl_msg_count = new LinkLabel();
            Label lbl_sender_cn = new Label();
            Panel pnl_senderImage_container = new Panel();
            Label lbl_bottom_line_right = new Label();
            PictureBox pictureBox_sender_img = new PictureBox();

            pnlMessages.SuspendLayout();
            pnl_msgContainer.SuspendLayout();
            pnl_senderImage_container.SuspendLayout();

            // 
            // pnl_msgContainer
            // 
            pnl_msgContainer.BackColor = Color.Transparent;
            pnl_msgContainer.Dock = DockStyle.Top;
            pnl_msgContainer.Location = new Point(10, location_y);
            pnl_msgContainer.Name = "pnl_msgContainer_" + i;
            pnl_msgContainer.Size = new Size(290, 66);
            pnl_msgContainer.TabIndex = 1;
            pnl_msgContainer.Controls.Add(lbl_bottom_line_left);
            pnl_msgContainer.Controls.Add(lbl_msg_count);
            pnl_msgContainer.Controls.Add(lbl_sender_cn);
            pnl_msgContainer.Controls.Add(pnl_senderImage_container);
            // 
            // lbl_bottom_line_left
            // 
            lbl_bottom_line_left.BackColor = Color.Goldenrod;
            lbl_bottom_line_left.Dock = DockStyle.Bottom;
            lbl_bottom_line_left.Location = new Point(10, 64);
            lbl_bottom_line_left.Name = "lbl_bottom_line_left_" + i;
            lbl_bottom_line_left.Size = new Size(227, 1);
            lbl_bottom_line_left.TabIndex = 2;
            // 
            // lbl_msg_count
            // 
            lbl_msg_count.Cursor = Cursors.Hand;
            lbl_msg_count.Font = new Font("B Mitra", 11F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(178)));
            lbl_msg_count.LinkBehavior = LinkBehavior.NeverUnderline;
            lbl_msg_count.Location = new Point(3, 34);
            lbl_msg_count.Name = "lbl_msg_count_" + i;
            lbl_msg_count.Padding = new Padding(0, 0, 0, 4);
            lbl_msg_count.RightToLeft = RightToLeft.Yes;
            lbl_msg_count.Size = new Size(224, 24);
            lbl_msg_count.TabIndex = 3;
            lbl_msg_count.TabStop = true;
            lbl_msg_count.Text = item.NewMessagesCount + " پیام خوانده نشده";
            lbl_msg_count.Tag = item.SenderIsAGroup + "_" + item.Sender;
            lbl_msg_count.TextAlign = ContentAlignment.MiddleLeft;
            lbl_msg_count.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkedLabelClicked);

            // 
            // lbl_sender_cn
            // 
            lbl_sender_cn.BackColor = Color.Transparent;
            lbl_sender_cn.Dock = DockStyle.Top;
            lbl_sender_cn.Font = new Font("B Mitra", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(178)));
            lbl_sender_cn.ForeColor = Color.DarkOrange;
            lbl_sender_cn.Location = new Point(0, 0);
            lbl_sender_cn.Name = "lbl_sender_cn_" + i;
            lbl_sender_cn.Size = new Size(227, 32);
            lbl_sender_cn.TabIndex = 1;
            lbl_sender_cn.Text = item.SenderCN;
            lbl_sender_cn.RightToLeft = RightToLeft.Yes;
            lbl_sender_cn.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnl_senderImage_container
            // 
            pnl_senderImage_container.BackColor = SystemColors.ButtonFace;
            pnl_senderImage_container.Dock = DockStyle.Right;
            pnl_senderImage_container.Padding = new System.Windows.Forms.Padding(0);
            pnl_senderImage_container.Location = new Point(237, 0);
            pnl_senderImage_container.Name = "pnl_senderImage_container_" + i;
            pnl_senderImage_container.Size = new Size(55, 66);
            pnl_senderImage_container.TabIndex = 0;
            pnl_senderImage_container.Controls.Add(lbl_bottom_line_right);
            pnl_senderImage_container.Controls.Add(pictureBox_sender_img);
            // 
            // lbl_bottom_line_right
            // 
            lbl_bottom_line_right.BackColor = Color.Goldenrod;
            lbl_bottom_line_right.Dock = DockStyle.Bottom;
            lbl_bottom_line_right.Location = new Point(0, 64);
            lbl_bottom_line_right.Name = "lbl_bottom_line_right_" + i;
            lbl_bottom_line_right.Size = new Size(67, 1);
            lbl_bottom_line_right.TabIndex = 3;
            // 
            // pictureBox_sender_img
            // 
            pictureBox_sender_img.Dock = DockStyle.Fill;
            //pictureBox_sender_img.Image = global::TicketClientApp.Properties.Resources.user;
            pictureBox_sender_img.Location = new Point(0, 5);
            pictureBox_sender_img.Name = "pictureBox_sender_img_" + i;
            pictureBox_sender_img.Size = new Size(50, 50);
            pictureBox_sender_img.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox_sender_img.TabIndex = 0;
            pictureBox_sender_img.TabStop = false;

            if (item.SenderIsAGroup)
            {
                pictureBox_sender_img.Image = global::TicketClientApp.Properties.Resources.chanel;
            }
            else
            {

                if (!String.IsNullOrEmpty(item.SenderImg))
                {
                    string sURL = "https://ticket.nkums.ac.ir/get_file.aspx?fn=" + item.SenderImg + "&width=200";

                    WebRequest req = WebRequest.Create(sURL);

                    WebResponse res = req.GetResponse();

                    Stream imgStream = res.GetResponseStream();

                    Image img1 = Image.FromStream(imgStream);

                    imgStream.Close();

                    pictureBox_sender_img.Image = img1;
                }
                else
                {
                    pictureBox_sender_img.Image = global::TicketClientApp.Properties.Resources.user;
                }
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(pictureBox_sender_img.DisplayRectangle);
            pictureBox_sender_img.Region = new Region(gp);

            //pnlMessages.Controls.Add(pnl_msgContainer);
            flowLayoutPanel1.Controls.Add(pnl_msgContainer);

        }

        public void temp1()
        {
            int location_y = 0;

            for (int i = 0; i < 5; i++)
            {

                Panel pnl_msgContainer = new System.Windows.Forms.Panel();
                Label lbl_bottom_line_left = new System.Windows.Forms.Label();
                LinkLabel lbl_msg_count = new System.Windows.Forms.LinkLabel();
                Label lbl_sender_cn = new System.Windows.Forms.Label();
                Panel pnl_senderImage_container = new System.Windows.Forms.Panel();
                Label lbl_bottom_line_right = new System.Windows.Forms.Label();
                PictureBox pictureBox_sender_img = new System.Windows.Forms.PictureBox();

                pnlMessages.SuspendLayout();
                pnl_msgContainer.SuspendLayout();
                pnl_senderImage_container.SuspendLayout();

                // 
                // pnl_msgContainer
                // 
                pnl_msgContainer.BackColor = System.Drawing.Color.Transparent;
                pnl_msgContainer.Dock = System.Windows.Forms.DockStyle.Top;
                pnl_msgContainer.Location = new System.Drawing.Point(0, location_y);
                pnl_msgContainer.Name = "pnl_msgContainer" + i;
                pnl_msgContainer.Size = new System.Drawing.Size(294, 66);
                pnl_msgContainer.TabIndex = 1;
                pnl_msgContainer.Controls.Add(lbl_bottom_line_left);
                pnl_msgContainer.Controls.Add(lbl_msg_count);
                pnl_msgContainer.Controls.Add(lbl_sender_cn);
                pnl_msgContainer.Controls.Add(pnl_senderImage_container);
                // 
                // lbl_bottom_line_left
                // 
                lbl_bottom_line_left.BackColor = System.Drawing.Color.Goldenrod;
                lbl_bottom_line_left.Dock = System.Windows.Forms.DockStyle.Bottom;
                lbl_bottom_line_left.Location = new System.Drawing.Point(0, 64);
                lbl_bottom_line_left.Name = "lbl_bottom_line_left" + i;
                lbl_bottom_line_left.Size = new System.Drawing.Size(227, 1);
                lbl_bottom_line_left.TabIndex = 2;
                // 
                // lbl_msg_count
                // 
                lbl_msg_count.Cursor = System.Windows.Forms.Cursors.Hand;
                lbl_msg_count.Font = new System.Drawing.Font("B Mitra", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                lbl_msg_count.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                lbl_msg_count.Location = new System.Drawing.Point(3, 34);
                lbl_msg_count.Name = "lbl_msg_count" + i;
                lbl_msg_count.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
                lbl_msg_count.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                lbl_msg_count.Size = new System.Drawing.Size(224, 24);
                lbl_msg_count.TabIndex = 3;
                lbl_msg_count.TabStop = true;
                lbl_msg_count.Text = "10 پیام جدید";
                lbl_msg_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                // 
                // lbl_sender_cn
                // 
                lbl_sender_cn.BackColor = System.Drawing.Color.Transparent;
                lbl_sender_cn.Dock = System.Windows.Forms.DockStyle.Top;
                lbl_sender_cn.Font = new System.Drawing.Font("B Mitra", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                lbl_sender_cn.ForeColor = System.Drawing.Color.DarkOrange;
                lbl_sender_cn.Location = new System.Drawing.Point(0, 0);
                lbl_sender_cn.Name = "lbl_sender_cn" + i;
                lbl_sender_cn.Size = new System.Drawing.Size(227, 32);
                lbl_sender_cn.TabIndex = 1;
                lbl_sender_cn.Text = "بیمارستان امام علی ع";
                lbl_sender_cn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                // 
                // pnl_senderImage_container
                // 
                pnl_senderImage_container.BackColor = System.Drawing.SystemColors.ButtonFace;
                pnl_senderImage_container.Dock = System.Windows.Forms.DockStyle.Right;
                pnl_senderImage_container.Location = new System.Drawing.Point(227, 0);
                pnl_senderImage_container.Name = "pnl_senderImage_container" + i;
                pnl_senderImage_container.Size = new System.Drawing.Size(67, 66);
                pnl_senderImage_container.TabIndex = 0;
                pnl_senderImage_container.Controls.Add(lbl_bottom_line_right);
                pnl_senderImage_container.Controls.Add(pictureBox_sender_img);
                // 
                // lbl_bottom_line_right
                // 
                lbl_bottom_line_right.BackColor = System.Drawing.Color.Goldenrod;
                lbl_bottom_line_right.Dock = System.Windows.Forms.DockStyle.Bottom;
                lbl_bottom_line_right.Location = new System.Drawing.Point(0, 64);
                lbl_bottom_line_right.Name = "lbl_bottom_line_right" + i;
                lbl_bottom_line_right.Size = new System.Drawing.Size(67, 1);
                lbl_bottom_line_right.TabIndex = 3;
                // 
                // pictureBox_sender_img
                // 
                pictureBox_sender_img.Dock = System.Windows.Forms.DockStyle.Fill;
                pictureBox_sender_img.Image = global::TicketClientApp.Properties.Resources.user;
                pictureBox_sender_img.Location = new System.Drawing.Point(0, 0);
                pictureBox_sender_img.Name = "pictureBox_sender_img" + i;
                pictureBox_sender_img.Size = new System.Drawing.Size(67, 66);
                pictureBox_sender_img.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                pictureBox_sender_img.TabIndex = 0;
                pictureBox_sender_img.TabStop = false;


                pnlMessages.Controls.Add(pnl_msgContainer);

                location_y += 66;
            }
        }


        private void LinkedLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LinkLabel current_linkLabel = sender as LinkLabel;

                string tag = current_linkLabel.Tag.ToString();
                //MessageBox.Show(tag);

                current_linkLabel.LinkVisited = true;

                string url = "https://ticket.nkums.ac.ir/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk;
                //string url = "http://localhost:9609/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk;

                System.Diagnostics.Process.Start(url);

                this.WindowState = FormWindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("tick");
            pnlMessages.Controls.Clear();

            if (!string.IsNullOrEmpty(_tk))
            {
                int NewMsgCount = LoadMessages(_tk);

                if (NewMsgCount > CurrentNewMessageCount)
                {
                    //SoundPlayer sndplayr = new SoundPlayer(TicketClientApp.Properties.Resources._3);
                    sndplayr.Play();

                    CurrentNewMessageCount = NewMsgCount;
                }
            }
        }

        private void playWave()
        {

            //string bleeb_url4 = Application.StartupPath + "\\4.wav";
            //SoundPlayer player4 = new SoundPlayer(bleeb_url4);
            //player4.Play();


            SoundPlayer sndplayr = new SoundPlayer(TicketClientApp.Properties.Resources._3);
            sndplayr.Play();

        }


        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = Application.StartupPath + "\\temp.txt";
            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                fi.Delete();
            }

            Application.Exit();

        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {

        }

    }
}

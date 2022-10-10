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
//using Alvas.Audio;

namespace TicketClientApp
{
    public partial class Form1 : Form
    {
        public static string site_url = "https://ticket.nkums.ac.ir";
        public Form1()
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

        public class LineItem
        {
            public string Sender { get; set; }
            public int NewMesageCount { get; set; }
            public int Notifycount { get; set; }
        }

        public static List<LineItem> CurrentLineItems = new List<LineItem>();
        public static List<LineItem> NotifiedLineItems = new List<LineItem>();

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMessageFromWebsercice();

            //pictureBox1.Image = global::TicketClientApp.Properties.Resources._25;
            GraphicsPath gp2 = new GraphicsPath();
            gp2.AddEllipse(pictureBox1.DisplayRectangle);
            pictureBox1.Region = new Region(gp2);

            pnlMessages.Height = 382;

            //CreateContextMenu();
        }


        private void LoadMessageFromWebsercice()
        {
            bool TicketServerConnectionError = false;

            string fileName = Application.StartupPath + "\\Ticket.txt";
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

                            if (user_san != "ExeptionError" && user_cn != "ExeptionError")
                            {
                                string line3 = sr.ReadLine();
                                if (line3 != null)
                                {
                                    token = line3;

                                    string line4 = sr.ReadLine();
                                    if (line4 != null)
                                        user_image = line4;

                                    if (!String.IsNullOrEmpty(token))
                                    {
                                        sr.Close();

                                        _tk = token;

                                        lblName.Text = user_cn;

                                        #region profile info

                                        if (!String.IsNullOrEmpty(user_image))
                                        {
                                            try
                                            {

                                                string sURL = site_url+"/get_file.aspx?fn=" + user_image + "&width=200";

                                                WebRequest req = WebRequest.Create(sURL);

                                                WebResponse res = req.GetResponse();

                                                Stream imgStream = res.GetResponseStream();


                                                Image img1 = Image.FromStream(imgStream);
                                                imgStream.Close();
                                                pictureBox1.Image = img1;



                                            }
                                            catch (Exception em)
                                            {
                                                if (em.Message.Contains("The remote name could not be resolved"))
                                                {
                                                    TicketServerConnectionError = true;
                                                }

                                                pictureBox1.Image = global::TicketClientApp.Properties.Resources.user;
                                            }

                                        }
                                        else
                                            pictureBox1.Image = global::TicketClientApp.Properties.Resources.user;

                                        #endregion

                                        if (!TicketServerConnectionError)
                                        {
                                            CurrentLineItems = LoadMessages(token);

                                            timer1.Enabled = true;
                                        }
                                        else
                                        {
                                            #region TicketServerConnectionError message

                                            flowLayoutPanel1.Controls.Clear();

                                            Label lbl_connectionError = new Label();
                                            // 
                                            // lbl_connectionError
                                            // 
                                            lbl_connectionError.BackColor = Color.OrangeRed;
                                            lbl_connectionError.Font = new Font("B Mitra", 11F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(178)));
                                            lbl_connectionError.ForeColor = Color.White;
                                            lbl_connectionError.Location = new Point(3, 15);
                                            lbl_connectionError.Name = "lbl_connectionError";
                                            lbl_connectionError.Padding = new Padding(8, 0, 8, 0);
                                            lbl_connectionError.Size = new Size(240, 111);
                                            lbl_connectionError.TabIndex = 0;
                                            lbl_connectionError.Text = "متاسفانه ارتباط برنامه با سرور سامانه تیکت میسر نمی باشد. لطفا اتصال سیستم خود به" +
                                                " اینترنت را بررسی نمایید";
                                            lbl_connectionError.TextAlign = ContentAlignment.MiddleCenter;

                                            flowLayoutPanel1.Controls.Add(lbl_connectionError);

                                            timer_connectionError.Enabled = true;

                                            #endregion
                                        }

                                    }

                                }
                            }
                            else
                            {
                                //ExeptionError = true;

                                //fi.Delete();

                                System.IO.File.WriteAllText(fileName, String.Empty);

                                DialogResult d = MessageBox.Show("متاسفانه اختلالی در تنظیمات کاربری شما رخ داده است، لطفا مجددا لاگین فرمایید.", "خطای تنظیمات کاربری", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (d == DialogResult.OK)
                                    Application.Exit();
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


            //if (ExeptionError)
            //{
            //    fi.Delete();

            //    DialogResult d = MessageBox.Show("متاسفانه اختلالی در تنظیمات کاربری شما رخ داده است، لطفا مجددا لاگین فرمایید.", "خطای تنظیمات کاربری", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    if (d == DialogResult.OK)
            //        Application.Exit();

            //}

        }


        public class AddLine_retVal
        {
            public bool PlaySound { get; set; }

            public LineItem lineItem { get; set; }
        }

        private List<LineItem> LoadMessages(string token)
        {
            bool PlaySound = false;

            List<LineItem> lstRet = new List<LineItem>();

            TicketWebService.AD_AuthenticateWebService TicketService = new TicketWebService.AD_AuthenticateWebService();

            var Service_retval = TicketService.FetchNewConversations(token);

            if (Service_retval != null)
            {
                if (!String.IsNullOrEmpty(Service_retval.Msg))
                {
                    DialogResult d = MessageBox.Show(Service_retval.Msg, "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //if (d == DialogResult.OK)
                    //{
                    string fileName = Application.StartupPath + "\\Ticket.txt";
                    FileInfo fi = new FileInfo(fileName);

                    if (fi.Exists)
                    {
                        //fi.Delete();

                        System.IO.File.WriteAllText(fileName, String.Empty);
                    }

                    Application.Restart();
                    //}
                }
                else
                {
                    int location_y = 0;

                    var LstConversations = Service_retval.LstConversations.ToList();
                    if (LstConversations.Count == 0)
                    {
                        #region ziroMessages

                        flowLayoutPanel1.Controls.Clear();

                        // 
                        // linkLabel_ziroMessages
                        // 
                        LinkLabel linkLabel_ziroMessages1 = new LinkLabel();
                        linkLabel_ziroMessages1.Font = new Font("B Mitra", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(178)));
                        linkLabel_ziroMessages1.Location = new Point(3, 0);
                        linkLabel_ziroMessages1.Name = "linkLabel_ziroMessages";
                        linkLabel_ziroMessages1.Size = new Size(240, 36);
                        linkLabel_ziroMessages1.TabIndex = 0;
                        linkLabel_ziroMessages1.TabStop = true;
                        //linkLabel_ziroMessages1.LinkColor = Color.MediumSeaGreen;

                        linkLabel_ziroMessages1.BackColor = Color.MediumSeaGreen;
                        linkLabel_ziroMessages1.LinkColor = Color.White;
                        //linkLabel_ziroMessages1.Padding = new Padding(2);

                        linkLabel_ziroMessages1.Text = "پیام جدیدی یافت نگردید.";
                        linkLabel_ziroMessages1.TextAlign = ContentAlignment.MiddleCenter;
                        linkLabel_ziroMessages1.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel_ziroMessages_LinkClicked);


                        flowLayoutPanel1.Controls.Add(linkLabel_ziroMessages1);

                        #endregion
                    }
                    else
                    {
                        int cnt = 0;
                        bool HasScroll = false;
                        if (LstConversations.Count > 6)
                            HasScroll = true;

                        foreach (var item in LstConversations)
                        {
                            var ret_AddlineItem = AddNewConversatinItem(item, cnt, location_y, HasScroll);
                            location_y += 60;

                            lstRet.Add(ret_AddlineItem.lineItem);

                            if (ret_AddlineItem.PlaySound)
                                PlaySound = true;

                            cnt++;
                        }

                        if (PlaySound)
                            sndplayr.Play();

                    }


                    if (Service_retval.UserAuthenticateRet != null)
                    {
                        ClsFuncs.WriteTotextFile(Service_retval.UserAuthenticateRet);
                        _tk = Service_retval.UserAuthenticateRet.token.ToString();
                    }


                }

            }
            else
            {
                MessageBox.Show("عدم خروجی وب سرویس", "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
            }


            return lstRet;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //sndplayr.Play();

            flowLayoutPanel1.Controls.Clear();

            if (!string.IsNullOrEmpty(_tk))
                CurrentLineItems = LoadMessages(_tk);

        }


        public AddLine_retVal AddNewConversatinItem(TicketWebService.ConversationInfo item, int i, int location_y, bool HasScroll)
        {
            AddLine_retVal ret = new AddLine_retVal();

            LineItem lineItem = new LineItem();

            int height = 60;

            #region CREATE CONTROLS

            Panel pnl_msgContainer = new Panel();
            Label lbl_bottom_line_left = new Label();
            LinkLabel lbl_msg_count = new LinkLabel();
            Label lbl_sender_cn = new Label();
            Panel pnl_senderImage_container = new Panel();
            Label lbl_bottom_line_right = new Label();
            PictureBox pictureBox_sender_img = new PictureBox();
            LinkLabel linkLabel_ajaxNewMsg = new LinkLabel();

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
            pnl_msgContainer.Size = new Size(222, height);
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
            lbl_bottom_line_left.Location = new Point(0, 58);
            lbl_bottom_line_left.Name = "lbl_bottom_line_left_" + i;
            lbl_bottom_line_left.Size = new Size(227, 1);
            lbl_bottom_line_left.TabIndex = 2;
            // 
            // lbl_msg_count
            // 
            lbl_msg_count.Cursor = Cursors.Hand;
            lbl_msg_count.Font = new Font("B Mitra", 11F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(178)));
            lbl_msg_count.LinkBehavior = LinkBehavior.NeverUnderline;
            lbl_msg_count.Location = new Point(33, 30);
            lbl_msg_count.Name = "lbl_msg_count_" + i;
            lbl_msg_count.Padding = new Padding(0, 0, 0, 4);
            lbl_msg_count.RightToLeft = RightToLeft.Yes;
            lbl_msg_count.Size = new Size(138, 24);
            lbl_msg_count.TabIndex = 3;
            lbl_msg_count.TabStop = true;
            lbl_msg_count.Tag = item.SenderIsAGroup + "_" + item.Sender + "_" + i + "_" + item.SenderIsAPool;
            lbl_msg_count.TextAlign = ContentAlignment.MiddleLeft;
            lbl_msg_count.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkedLabelClicked);

            if (item.SenderIsAPool)
            {
                lbl_msg_count.Text = item.NewMessagesCount + " تیکت برداشته نشده";

                lbl_msg_count.LinkColor = System.Drawing.Color.OrangeRed;
            }
            else
            {
                lbl_msg_count.Text = item.NewMessagesCount + " پیام خوانده نشده";

                lbl_msg_count.LinkColor = System.Drawing.Color.DarkOrange;
            }

            // 
            // lbl_sender_cn
            // 
            lbl_sender_cn.BackColor = Color.Transparent;
            lbl_sender_cn.Dock = DockStyle.Top;
            lbl_sender_cn.Font = new Font("B Mitra", 11F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(178)));
            lbl_sender_cn.ForeColor = Color.SteelBlue;
            lbl_sender_cn.Location = new Point(0, 0);
            lbl_sender_cn.Name = "lbl_sender_cn_" + i;
            lbl_sender_cn.Size = new Size(170, 32);
            lbl_sender_cn.TabIndex = 1;
            lbl_sender_cn.RightToLeft = RightToLeft.Yes;
            lbl_sender_cn.TextAlign = ContentAlignment.MiddleLeft;

            if (item.SenderIsAPool)
                lbl_sender_cn.Text = "استخر " + item.SenderCN;
            else
                lbl_sender_cn.Text = item.SenderCN;
            // 
            // pnl_senderImage_container
            // 
            pnl_senderImage_container.BackColor = SystemColors.ButtonFace;
            pnl_senderImage_container.Dock = DockStyle.Right;
            pnl_senderImage_container.Padding = new System.Windows.Forms.Padding(0);
            pnl_senderImage_container.Location = new Point(170, 0);
            pnl_senderImage_container.Name = "pnl_senderImage_container_" + i;
            pnl_senderImage_container.Size = new Size(55, height);
            pnl_senderImage_container.TabIndex = 0;
            pnl_senderImage_container.Controls.Add(lbl_bottom_line_right);
            pnl_senderImage_container.Controls.Add(pictureBox_sender_img);
            // 
            // lbl_bottom_line_right
            // 
            lbl_bottom_line_right.BackColor = Color.Goldenrod;
            lbl_bottom_line_right.Dock = DockStyle.Bottom;
            lbl_bottom_line_right.Location = new Point(0, 58);
            lbl_bottom_line_right.Name = "lbl_bottom_line_right_" + i;
            lbl_bottom_line_right.Size = new Size(67, 1);
            lbl_bottom_line_right.TabIndex = 3;
            // 
            // pictureBox_sender_img
            // 
            pictureBox_sender_img.Dock = DockStyle.Fill;
            pictureBox_sender_img.Location = new Point(0, 1);
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
                    string sURL = site_url+"/get_file.aspx?fn=" + item.SenderImg + "&width=200";

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


            #endregion




            int Notifycount = 0;

            #region FIND FONIFY COUNT

            var findeLine = CurrentLineItems.Where(x => x.Sender == item.Sender).FirstOrDefault();
            if (findeLine != null)
            {
                if (item.NewMessagesCount > findeLine.NewMesageCount)
                {
                    if (findeLine.Notifycount == 0)
                    {
                        Notifycount = item.NewMessagesCount - findeLine.NewMesageCount;
                    }
                    else if (findeLine.Notifycount > 0)
                    {
                        Notifycount = item.NewMessagesCount - findeLine.NewMesageCount + findeLine.Notifycount;
                    }

                    lineItem = new LineItem()
                    {
                        Sender = item.Sender,
                        NewMesageCount = item.NewMessagesCount,
                        Notifycount = Notifycount
                    };

                }
                else if (item.NewMessagesCount == findeLine.NewMesageCount)
                {
                    if (findeLine.Notifycount > 0)
                        Notifycount = findeLine.Notifycount;

                    lineItem = findeLine;
                }
                else if (item.NewMessagesCount < findeLine.NewMesageCount)
                {
                    lineItem = new LineItem()
                    {
                        Sender = item.Sender,
                        NewMesageCount = item.NewMessagesCount,
                        Notifycount = 0
                    };
                }

            }
            else
            {
                lineItem = new LineItem()
                {
                    Sender = item.Sender,
                    NewMesageCount = item.NewMessagesCount,
                    Notifycount = 0
                };
            }

            #endregion


            if (Notifycount > 0)
            {
                #region add linkLabel_ajaxNewMsg

                // 
                // linkLabel_ajaxNewMsg
                // 
                linkLabel_ajaxNewMsg.Image = global::TicketClientApp.Properties.Resources.newMsg_green;
                linkLabel_ajaxNewMsg.LinkBehavior = LinkBehavior.NeverUnderline;
                linkLabel_ajaxNewMsg.LinkColor = Color.White;
                linkLabel_ajaxNewMsg.Location = new Point(2, 32);
                linkLabel_ajaxNewMsg.Name = "linkLabel_ajaxNewMsg_" + i;
                linkLabel_ajaxNewMsg.Size = new Size(24, 24);
                linkLabel_ajaxNewMsg.TabIndex = 1;
                linkLabel_ajaxNewMsg.TabStop = true;
                linkLabel_ajaxNewMsg.Text = Notifycount.ToString();
                linkLabel_ajaxNewMsg.TextAlign = ContentAlignment.MiddleCenter;

                pnl_msgContainer.Controls.Add(linkLabel_ajaxNewMsg);

                #endregion

                ret.PlaySound = true;
            }


            if (!HasScroll)
            {
                #region CREATE CONTROLS CONSIDER NO SCROLL

                pnl_msgContainer.Location = new Point(0, location_y);

                pnl_msgContainer.Size = new Size(240, height);
                //pnl_msgContainer.BackColor = Color.Blue;


                lbl_bottom_line_left.Location = new Point(0, 58);
                lbl_bottom_line_left.Size = new Size(190, 1);



                lbl_msg_count.Location = new Point(38, 30);
                lbl_msg_count.Padding = new Padding(0, 0, 0, 4);
                lbl_msg_count.Size = new Size(150, 24);
                //lbl_msg_count.BackColor = Color.LawnGreen;


                lbl_sender_cn.Location = new Point(0, 0);
                lbl_sender_cn.Size = new Size(186, 32);
                //lbl_sender_cn.BackColor = Color.Aqua;



                pnl_senderImage_container.Location = new Point(186, 0);
                pnl_senderImage_container.Size = new Size(65, height);



                lbl_bottom_line_right.Location = new Point(0, 58);
                lbl_bottom_line_right.Size = new Size(67, 1);



                pictureBox_sender_img.Location = new Point(5, 1);
                pictureBox_sender_img.Size = new Size(50, 50);


                linkLabel_ajaxNewMsg.Location = new Point(5, 31);
                linkLabel_ajaxNewMsg.Size = new Size(24, 24);

                #endregion



                #region BC ---- CREATE CONTROLS CONSIDER NO SCROLL 

                ////pnl_msgContainer.Location = new Point(10, location_y);
                ////pnl_msgContainer.Size = new Size(290, height);
                //pnl_msgContainer.Location = new Point(0, location_y);
                //pnl_msgContainer.Size = new Size(300, height);


                ////lbl_bottom_line_left.Location = new Point(10, 58);
                ////lbl_bottom_line_left.Size = new Size(227, 1);
                //lbl_bottom_line_left.Location = new Point(0, 58);
                //lbl_bottom_line_left.Size = new Size(237, 1);



                ////lbl_msg_count.Location = new Point(50, 30);
                ////lbl_msg_count.Padding = new Padding(0, 0, 0, 4);
                ////lbl_msg_count.Size = new Size(178, 24);
                //lbl_msg_count.Location = new Point(60, 30);
                //lbl_msg_count.Padding = new Padding(0, 0, 0, 4);
                //lbl_msg_count.Size = new Size(178, 24);



                ////lbl_sender_cn.Location = new Point(0, 0);
                ////lbl_sender_cn.Size = new Size(227, 32);
                //lbl_sender_cn.Location = new Point(0, 0);
                //lbl_sender_cn.Size = new Size(227, 32);



                ////pnl_senderImage_container.Location = new Point(232, 0);
                ////pnl_senderImage_container.Size = new Size(55, height);
                //pnl_senderImage_container.Location = new Point(232, 0);
                //pnl_senderImage_container.Size = new Size(65, height);



                ////lbl_bottom_line_right.Location = new Point(0, 58);
                ////lbl_bottom_line_right.Size = new Size(67, 1);
                //lbl_bottom_line_right.Location = new Point(0, 58);
                //lbl_bottom_line_right.Size = new Size(67, 1);




                ////pictureBox_sender_img.Location = new Point(0, 1);
                ////pictureBox_sender_img.Size = new Size(50, 50);
                //pictureBox_sender_img.Location = new Point(5, 1);
                //pictureBox_sender_img.Size = new Size(50, 50);


                #endregion
            }


            flowLayoutPanel1.Controls.Add(pnl_msgContainer);

            ret.lineItem = lineItem;

            return ret;
        }

        private void LinkedLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = "";
                string qs = "";

                LinkLabel current_linkLabel = sender as LinkLabel;


                string tag = current_linkLabel.Tag.ToString();

                var arr = tag.Split('_').ToArray();
                if (arr[0].ToLower() == "true")//is group
                {
                    qs = "&gid=" + arr[1];
                }
                else if (arr[0].ToLower() == "false")//is san
                {
                    qs = "&san=" + arr[1];
                }


                current_linkLabel.LinkVisited = true;

                var findeLine = CurrentLineItems.Where(x => x.Sender == arr[1]).FirstOrDefault();
                if (findeLine != null)
                    findeLine.Notifycount = 0;

                string notify_control_name = "linkLabel_ajaxNewMsg_" + arr[2];

                var notifyControl = flowLayoutPanel1.Controls.Find(notify_control_name, true).FirstOrDefault();
                if (notifyControl != null)
                    notifyControl.Visible = false;
                //flowLayoutPanel1.Controls.Remove(notifyControl);

                if (arr[3].ToLower() == "true")//SenderIsAPool
                    url = site_url + "/home?token=" + _tk;
                else
                    url = site_url + "/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk + qs;

                System.Diagnostics.Process.Start(url);

                this.WindowState = FormWindowState.Minimized;

            }
            catch
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = Application.StartupPath + "\\Ticket.txt";
            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                //fi.Delete();

                System.IO.File.WriteAllText(fileName, String.Empty);
            }

            Application.Exit();

        }

        private void tokenLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string fileName = Application.StartupPath + "\\Ticket.txt";
            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                //fi.Delete();
                System.IO.File.WriteAllText(fileName, String.Empty);
            }

            Application.Exit();
            //try
            //{
            //    tokenLink.LinkVisited = true;

            //    string url = site_url+"/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk;
            //    //string url = site_url+"http://localhost:9609/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk;

            //    System.Diagnostics.Process.Start(url);

            //    this.WindowState = FormWindowState.Minimized;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Unable to open link that was clicked.");
            //}
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void playWave_()
        {

            //string bleeb_url4 = Application.StartupPath + "\\4.wav";
            //SoundPlayer player4 = new SoundPlayer(bleeb_url4);
            //player4.Play();


            SoundPlayer sndplayr = new SoundPlayer(TicketClientApp.Properties.Resources._3);
            sndplayr.Play();

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_cms_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void linkLabel_ziroMessages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            string url = site_url + "/ticketChatBox.aspx?viewMode=fullscreen&token=" + _tk;
            System.Diagnostics.Process.Start(url);

            this.WindowState = FormWindowState.Minimized;
        }

        private void timer_connectionError_Tick(object sender, EventArgs e)
        {
            try
            {
                string sURL = site_url + "/get_file.aspx?fn=";

                WebRequest req = WebRequest.Create(sURL);

                WebResponse res = req.GetResponse();

                timer_connectionError.Enabled = false;
                timer1.Enabled = true;
            }
            catch
            {
            }
        }

    }
}

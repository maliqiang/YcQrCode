using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

using Yc.QrCodeLib;
using Yc.QrCodeLib.Data;
using Yc.QrCodeLib.Util;
using Yc.QrCodeLib.custom;
using Yc.QrCodeLib.Angry_Birds;

namespace Yc.QrCode.Test
{
    public partial class QrCodeSampleApp : Form
    {

        public QrCodeSampleApp()
        {
            InitializeComponent();
            //双缓存
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);


            IniHelper _iniHelper = new IniHelper();
            _iniHelper.ls_iniFileFullPath = Application.StartupPath + "\\" + "USER.yc";
            string _key = _iniHelper.Read("USER", "key");

            _QrEncode = new QrEncode(_key);
        }

        private QrEncode _QrEncode;

        private Bitmap M_map_bufferpic;//加快GDI读取用缓存图片

        private Image picImage;

        /// <summary>
        /// 传入内存缓存中
        /// </summary>
        /// <param name="P_str_path">图片地址 </param>
        private void inPutBuffer(string P_str_path)
        {
            M_map_bufferpic = new Bitmap(P_str_path);
            this.picEncode.Image = M_map_bufferpic;
        }

        private void frmSample_Load(object sender, EventArgs e)
        {
            cboEncoding.SelectedIndex = 2;
            cboVersion.SelectedIndex = 0;
            cboCorrectionLevel.SelectedIndex = 3;

            txtEncodeData_TextChanged(null, null);

            for (int i = 0; i < 128; i++)
            {
                this.cbbImageStyle.DataSource = System.Enum.GetNames(typeof(ImageFix.STYLE));
            }

        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// 设置二维码参数
        /// </summary>
        private QRCodeEncoder SetQrCodeParam()
        {
            QRCodeEncoder _qrCodeEncoder = new QRCodeEncoder();
            string encoding = cboEncoding.Text;
            if (encoding == "Byte")
            {
                _qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                _qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                _qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(txtSize.Text);
                _qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                MessageBox.Show("无效的大小!");
                return null;
            }
            try
            {
                //版本为0时，自动识别
                int version = 0;
                //int version = Convert.ToInt16(cboVersion.Text);
                _qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                MessageBox.Show("无效的版本!");
            }

            string errorCorrect = cboCorrectionLevel.Text;//类型框
            if (errorCorrect == "L")
                _qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                _qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                _qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                _qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            return _qrCodeEncoder;
        }

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private void btnEncode_Click_1(object sender, EventArgs e)
        {
            stopwatch.Start();
            if (txtEncodeData.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Data must not be empty.");
                return;
            }

            QRCodeEncoder qrCodeEncoder = SetQrCodeParam();

            string data = txtEncodeData.Text;

            _QrEncode.QrCodeEncoder = qrCodeEncoder;
            _QrEncode.EnCoding = Encoding.UTF8;//编码自订，减少内容判断，加进生成效率

            picImage = _QrEncode.Encode(data);


            picEncode.Image = picImage;
            GC.Collect();

            stopwatch.Stop();
            MessageBox.Show("用时:" + stopwatch.ElapsedMilliseconds.ToString());
            stopwatch.Reset();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.FileName = string.Empty;
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        picImage.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        picImage.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        picImage.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Gif);
                        break;

                    case 4:
                        picImage.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }

                fs.Close();
            }

            //openFileDialog1.InitialDirectory = "c:\\";
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //openFileDialog1.FilterIndex = 2;
            //openFileDialog1.RestoreDirectory = true;

            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    MessageBox.Show(openFileDialog1.FileName); 
            //}

        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            DialogResult r = printDialog1.ShowDialog();
            if (r == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(picEncode.Image, 0, 0);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String fileName = openFileDialog1.FileName;
                picDecode.Image = new Bitmap(fileName);

            }
        }

        private void btnDecode_Click_1(object sender, EventArgs e)
        {
            try
            {
                QRCodeDecoder decoder = new QRCodeDecoder();
                //QRCodeDecoder.Canvas = new ConsoleCanvas();
                String decodedString = decoder.decode(new QRCodeBitmapImage(new Bitmap(picDecode.Image)));
                txtDecodedData.Text = decodedString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtEncodeData_TextChanged(object sender, EventArgs e)
        {
            QRCodeEncoder _qrCodeEncoder = SetQrCodeParam();

            _qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            _qrCodeEncoder.QRCodeVersion = 1;


            String data = txtEncodeData.Text;
            lblCount.Text = txtEncodeData.Text.Trim().Length.ToString();



            List<QRCodeEncoder.ERROR_CORRECTION> _errorCorrect_list = new List<QRCodeEncoder.ERROR_CORRECTION>();
            _errorCorrect_list.Add(QRCodeEncoder.ERROR_CORRECTION.L);
            _errorCorrect_list.Add(QRCodeEncoder.ERROR_CORRECTION.M);
            _errorCorrect_list.Add(QRCodeEncoder.ERROR_CORRECTION.Q);
            _errorCorrect_list.Add(QRCodeEncoder.ERROR_CORRECTION.H);

            for (int i = 0; i < 40; i++)
            {
                //try
                //{
                //    _qrCodeEncoder.QRCodeVersion = i;
                //    _qrCodeEncoder.calQrcode(Encoding.UTF8.GetBytes(data));
                //    cboVersion.SelectedIndex = _qrCodeEncoder.QRCodeVersion;
                //    break;
                //}
                //catch
                //{
                //    continue;
                //}
                _qrCodeEncoder.QRCodeVersion = i;
                if (!isQrCodeError(_qrCodeEncoder, data))
                {
                    cboVersion.SelectedIndex = _qrCodeEncoder.QRCodeVersion - 1;
                    break;
                }
            }
        }

        private bool isQrCodeError(QRCodeEncoder qrCodeEncoder, String data)
        {
            try
            {
                qrCodeEncoder.calQrcode(Encoding.UTF8.GetBytes(data));
                return false;
            }
            catch
            {
                return true;
            }
        }


        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }




    }
}
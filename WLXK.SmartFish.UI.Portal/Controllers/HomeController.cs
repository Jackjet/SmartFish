using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using WLXK.SmartFish.IBll;
using WLXK.SmartFish.Model;
using WLXK.SmartFish.UI.Portal.Common;

namespace WLXK.SmartFish.UI.Portal.Controllers
{
    public class HomeController : Controller
    {
        public IReceiveDatasServices ReceiveDatasServices { get; set; }
        public IControlDataServices ControlDataServices { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ControlCenter()
        {
            ViewData["model"] = ControlDataServices.LoadEntities(u => true).LastOrDefault();

            return View();
        }

        public ActionResult IndexWater()
        {
            ViewData["model"] = ControlDataServices.LoadEntities(u => true).LastOrDefault();
            return View();
        }
        public ActionResult SelectHistory()
        {
            int total = 0;
            ViewData["receive"] = ReceiveDatasServices.LoadPageEntities(1, 18, out total, u => true, u => u.SubTime, false).ToList();

            ViewData["today"] = DateTime.Now.Date.ToString("yyyy-MM-dd");
            ViewData["month"] = DateTime.Now.Hour;


            return View();
        }
        [HttpPost]
        public ActionResult SelectHistory(string today, string month)
        {
            DateTime Time = DateTime.Parse(today + " " + month + ":00:00");

            int total = 0;
            ViewData["receive"] = ReceiveDatasServices.LoadPageEntities(1, 18, out total, u => (u.SubTime - Time).TotalMinutes <= 60 && (u.SubTime - Time).TotalMinutes > 0, u => u.SubTime, false).ToList();

            ViewData["today"] = today;
            ViewData["month"] = month;


            return View();
        }



        public ActionResult GetAllValues()
        {
            var receives = ReceiveDatasServices.LoadEntities(u => true).LastOrDefault();

            return Json(receives, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetTurbidity(double value)
        {
            var datas = ControlDataServices.LoadEntities(u => true).LastOrDefault();

            if (datas == null)
            {
                ControlData dat = new ControlData();
                dat.MaxTemperate = 0;
                dat.MinTemperate = 0;
                dat.Oxygen = 0;
                dat.PhValues = value;

                try
                {
                    ControlDataServices.Add(dat);
                    return Content("设置成功");
                }
                catch (Exception)
                {
                    return Content("设置失败，请检查您输入的格式");
                }
            }
            else
            {
                datas.PhValues = value;

                try
                {
                    ControlDataServices.Add(datas);
                    return Content("设置成功");
                }
                catch (Exception)
                {
                    return Content("设置失败，请检查您输入的格式");
                }
            }
        }
        //设置氧气
        public ActionResult SetYangQi(double value)
        {
            var datas = ControlDataServices.LoadEntities(u => true).LastOrDefault();

            if (datas == null)
            {
                ControlData dat = new ControlData();
                dat.MaxTemperate = 0;
                dat.MinTemperate = 0;
                dat.Oxygen = value;
                dat.PhValues = 0;

                try
                {
                    ControlDataServices.Add(dat);
                    return Content("设置成功");
                }
                catch (Exception)
                {
                    return Content("设置失败，请检查您输入的格式");
                }
            }
            else
            {
                datas.Oxygen = value;

                try
                {
                    ControlDataServices.Add(datas);
                    return Content("设置成功");
                }
                catch (Exception)
                {
                    return Content("设置失败，请检查您输入的格式");
                }
            }
        }

        public ActionResult SetTemp(double value1, double value2)
        {
            if (value2 - value1 < 0)
            {
                return Content("请核对您输入的温度范围");
            }

            var datas = ControlDataServices.LoadEntities(u => true).LastOrDefault();

            if (datas == null)
            {
                ControlData dat = new ControlData();
                dat.MaxTemperate = value1;
                dat.MinTemperate = value2;
                dat.Oxygen = 0;
                dat.PhValues = 0;

                try
                {
                    ControlDataServices.Add(dat);
                    return Content("设置成功");
                }
                catch (Exception)
                {
                    return Content("设置失败，请检查您输入的格式");
                }
            }
            else
            {
                datas.MaxTemperate = value2;
                datas.MinTemperate = value1;

                try
                {
                    ControlDataServices.Add(datas);
                    return Content("设置成功");
                }
                catch (Exception)
                {
                    return Content("设置失败，请检查您输入的格式");
                }
            }
        }

        public ActionResult PaiShui(int type)
        {

            if (type == 1)
            {
                //开始排水

                SerialPortHelper.SendData(true, new byte[] {0xFC,0xB1, 0x11 });
                return Content("正在排水...");

            }
            else if (type == 0)
            {
                //停止排水
                SerialPortHelper.SendData(true, new byte[] { 0xFC, 0xB1, 0x12 });
                return Content("排水停止中...");
            }
            return Content("请稍候...");
        }

        public ActionResult Jinshui(int type)
        {
            if (type == 1)
            {
                //开始进水

                SerialPortHelper.SendData(true, new byte[] { 0xFC, 0xB2, 0x21 });
                return Content("正在进水...");

            }
            else if (type == 0)
            {
                //停止进水
                SerialPortHelper.SendData(true, new byte[] { 0xFC, 0xB2, 0x22 });
                return Content("进水停止中...");
            }
            return Content("请稍候...");
        }


        public ActionResult YangQi(int type)
        {
            if (type == 1)
            {
                //开始增氧
                SerialPortHelper.SendData(true, new byte[] { 0xFC, 0xB1, 0x21 });
                return Content("增加氧气中...");

            }
            else if (type == 0)
            {
                //停止增氧
                SerialPortHelper.SendData(true, new byte[] { 0xFC, 0xB1, 0x22 });
                return Content("停止增氧中...");
            }
            return Content("请稍候...");
        }

        public ActionResult WenDu(int type)
        {
            if (type == 1)
            {
                //增加温度
                SerialPortHelper.SendData(true, new byte[] { 0xFC, 0xB2, 0x11 });
                return Content("温度提升中...");

            }
            else if (type == 0)
            {
                //降低温度
                SerialPortHelper.SendData(true, new byte[] { 0xFC, 0xB2, 0x12 });
                return Content("停止增温中...");
            }
            return Content("请稍候...");
        }

        //获取得到的数据
        public ActionResult GetSetValue()
        {
            var control = ControlDataServices.LoadEntities(u => true).LastOrDefault();
            return Json(control, JsonRequestBehavior.AllowGet);
        }

        //创建折线图
        public ActionResult CreateImage(int jiedian, int data, DateTime today)
        {
            int height = 480, width = 700;
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);

            try
            {
                //清空图片背景色
                g.Clear(Color.White);

                Font font = new System.Drawing.Font("Arial", 9, FontStyle.Regular);
                Font font1 = new System.Drawing.Font("宋体", 20, FontStyle.Regular);
                Font font2 = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
                LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Blue, 1.2f, true);
                g.FillRectangle(Brushes.AliceBlue, 0, 0, width, height);
                Brush brush1 = new SolidBrush(Color.Blue);
                Brush brush2 = new SolidBrush(Color.SaddleBrown);

                g.DrawString(today.ToString("yyyy-MM-dd   ") + jiedian + "号节点" + "数据统计图", font1, brush1, new PointF(100, 30));
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Blue), 0, 0, image.Width - 1, image.Height - 1);

                Pen mypen = new Pen(brush, 1);
                Pen mypen2 = new Pen(Color.Red, 2);

                Pen mypen3 = new Pen(Color.Blue, 2);
                //绘制线条
                //绘制纵向线条
                int x = 60;
                for (int i = 0; i < 8; i++)
                {
                    g.DrawLine(mypen, x, 80, x, 340);
                    x = x + 80;
                }
                Pen mypen1 = new Pen(Color.Blue, 3);
                x = 60;
                g.DrawLine(mypen1, x, 82, x, 340);

                //绘制横向线条
                int y = 106;
                for (int i = 0; i < 10; i++)
                {
                    g.DrawLine(mypen, 60, y, 620, y);
                    y = y + 26;
                }
                // y = 106;
                g.DrawLine(mypen1, 60, y - 26, 620, y - 26);

                //x轴
                String[] n = { " 0时", " 4时", " 8时", " 12时", " 14时", " 16时", " 20时" };
                x = 50;
                for (int i = 0; i < 7; i++)
                {
                    g.DrawString(n[i].ToString(), font, Brushes.Blue, x, 348); //设置文字内容及输出位置
                    x = x + 77;
                }
                //查询数据
                var source = ReceiveDatasServices.LoadEntities(u => DateTime.Equals(u.SubTime.Date, today.Date)).ToList();


                if (data == 0)
                {
                    #region 温度和水温的折线图



                    //y轴
                    String[] m = { " 45℃", " 40℃", " 35℃", " 30℃", " 25℃", " 20℃", " 15℃", " 10℃",
"  5℃"};
                    y = 100;
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawString(m[i].ToString(), font, Brushes.Blue, 10, y); //设置文字内容及输出位置
                        y = y + 26;
                    }
                    //温度
                    int[] Count1 = new int[8];
                    int[] Count2 = new int[8];

                    #region 数据处理

                    foreach (var item in source)
                    {
                        double hourspan = (item.SubTime - today).TotalHours;

                        if (hourspan >= 0 && hourspan <= 3)
                        {
                            if (jiedian == 1)
                            {
                                Count1[0] = (int)item.EnviroTemperate;
                                Count2[0] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[0] = (int)item.EnviroTemperate2;
                                Count2[0] = (int)item.FishTemperate2;
                            }

                        }
                        else if (hourspan >= 4 && hourspan <= 7)
                        {
                            if (jiedian == 1)
                            {
                                Count1[1] = (int)item.EnviroTemperate;
                                Count2[1] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[1] = (int)item.EnviroTemperate2;
                                Count2[1] = (int)item.FishTemperate2;
                            }
                        }
                        else if (hourspan >= 8 && hourspan <= 11)
                        {

                            if (jiedian == 1)
                            {
                                Count1[2] = (int)item.EnviroTemperate;
                                Count2[2] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[2] = (int)item.EnviroTemperate2;
                                Count2[2] = (int)item.FishTemperate2;
                            }
                        }
                        else if (hourspan >= 12 && hourspan <= 13)
                        {
                            if (jiedian == 1)
                            {
                                Count1[3] = (int)item.EnviroTemperate;
                                Count2[3] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[3] = (int)item.EnviroTemperate2;
                                Count2[3] = (int)item.FishTemperate2;
                            }
                        }
                        else if (hourspan >= 14 && hourspan <= 15)
                        {
                            if (jiedian == 1)
                            {
                                Count1[4] = (int)item.EnviroTemperate;
                                Count2[4] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[4] = (int)item.EnviroTemperate2;
                                Count2[4] = (int)item.FishTemperate2;
                            }
                        }
                        else if (hourspan >= 16 && hourspan <= 19)
                        {
                            if (jiedian == 1)
                            {
                                Count1[5] = (int)item.EnviroTemperate;
                                Count2[5] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[5] = (int)item.EnviroTemperate2;
                                Count2[5] = (int)item.FishTemperate2;
                            }
                        }
                        else if (hourspan >= 20 && hourspan <= 22)
                        {
                            if (jiedian == 1)
                            {
                                Count1[6] = (int)item.EnviroTemperate;
                                Count2[6] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[6] = (int)item.EnviroTemperate2;
                                Count2[6] = (int)item.FishTemperate2;
                            }
                        }
                        else if (hourspan >= 22 && hourspan <= 23)
                        {
                            if (jiedian == 1)
                            {
                                Count1[7] = (int)item.EnviroTemperate;
                                Count2[7] = (int)item.FishTemperate;
                            }
                            else if (jiedian == 2)
                            {
                                Count1[7] = (int)item.EnviroTemperate2;
                                Count2[7] = (int)item.FishTemperate2;
                            }
                        }
                    }
                    #endregion

                    #region 温度


                    //显示折线效果
                    Font font3 = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                    SolidBrush mybrush = new SolidBrush(Color.Red);
                    Point[] points1 = new Point[8];
                    points1[0].X = 60; points1[0].Y = 5 * (50 - Count1[0]) + 80; //从106纵坐标开始, 到(0, 0)坐标时
                    points1[1].X = 140; points1[1].Y = 5 * (50 - Count1[1]) + 80;
                    points1[2].X = 220; points1[2].Y = 5 * (50 - Count1[2]) + 80;
                    points1[3].X = 300; points1[3].Y = 5 * (50 - Count1[3]) + 80;

                    points1[4].X = 380; points1[4].Y = 5 * (50 - Count1[4]) + 80;
                    points1[5].X = 460; points1[5].Y = 5 * (50 - Count1[5]) + 80;

                    points1[6].X = 540; points1[6].Y = 5 * (50 - Count1[6]) + 80;
                    points1[7].X = 620; points1[7].Y = 5 * (50 - Count1[7]) + 80;
                    g.DrawLines(mypen2, points1); //绘制折线

                    //绘制数字
                    g.DrawString(Count1[0].ToString(), font3, Brushes.Red, 60, points1[0].Y - 20);
                    g.DrawString(Count1[1].ToString(), font3, Brushes.Red, 138, points1[1].Y - 20);
                    g.DrawString(Count1[2].ToString(), font3, Brushes.Red, 218, points1[2].Y - 20);
                    g.DrawString(Count1[3].ToString(), font3, Brushes.Red, 298, points1[3].Y - 20);

                    g.DrawString(Count1[4].ToString(), font3, Brushes.Red, 378, points1[4].Y - 20);
                    g.DrawString(Count1[5].ToString(), font3, Brushes.Red, 458, points1[5].Y - 20);

                    g.DrawString(Count1[6].ToString(), font3, Brushes.Red, 538, points1[6].Y - 20);
                    g.DrawString(Count1[7].ToString(), font3, Brushes.Red, 600, points1[7].Y - 20);
                    #endregion

                    #region 水温


                    //显示折线效果

                    SolidBrush mybrush1 = new SolidBrush(Color.Blue);
                    Point[] points2 = new Point[8];
                    points2[0].X = 60; points2[0].Y = 5 * (50 - Count2[0]) + 80; //从106纵坐标开始, 到(0, 0)坐标时
                    points2[1].X = 140; points2[1].Y = 5 * (50 - Count2[1]) + 80;
                    points2[2].X = 220; points2[2].Y = 5 * (50 - Count2[2]) + 80;
                    points2[3].X = 300; points2[3].Y = 5 * (50 - Count2[3]) + 80;

                    points2[4].X = 380; points2[4].Y = 5 * (50 - Count2[4]) + 80;
                    points2[5].X = 460; points2[5].Y = 5 * (50 - Count2[5]) + 80;

                    points2[6].X = 540; points2[6].Y = 5 * (50 - Count2[6]) + 80;
                    points2[7].X = 620; points2[7].Y = 5 * (50 - Count2[7]) + 80;
                    g.DrawLines(mypen3, points2); //绘制折线

                    //绘制数字
                    g.DrawString(Count2[0].ToString(), font3, Brushes.Blue, 60, points2[0].Y - 20);
                    g.DrawString(Count2[1].ToString(), font3, Brushes.Blue, 138, points2[1].Y - 20);
                    g.DrawString(Count2[2].ToString(), font3, Brushes.Blue, 218, points2[2].Y - 20);
                    g.DrawString(Count2[3].ToString(), font3, Brushes.Blue, 298, points2[3].Y - 20);

                    g.DrawString(Count2[4].ToString(), font3, Brushes.Blue, 378, points2[4].Y - 20);
                    g.DrawString(Count2[5].ToString(), font3, Brushes.Blue, 458, points2[5].Y - 20);

                    g.DrawString(Count2[6].ToString(), font3, Brushes.Blue, 538, points2[6].Y - 20);
                    g.DrawString(Count2[7].ToString(), font3, Brushes.Blue, 600, points2[7].Y - 20);
                    #endregion

                    //绘制标识
                    g.DrawRectangle(new Pen(Brushes.Blue), 180, 390, 250, 50); //绘制范围框
                    g.FillRectangle(Brushes.Red, 270, 402, 20, 10); //绘制小矩形
                    g.DrawString("温度", font2, Brushes.Red, 292, 400);

                    g.FillRectangle(Brushes.Blue, 270, 422, 20, 10);
                    g.DrawString("水温", font2, Brushes.Blue, 292, 420);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    Response.ClearContent();

                    Response.ContentType = "image/Jpeg";
                    Response.BinaryWrite(ms.ToArray());
                    #endregion
                }
                else if (data == 1)
                {
                    //y轴
                    String[] m = {  " 9", " 8", " 7", " 6", " 5", " 4", " 3",
"  2","  1"};
                    y = 100;
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawString(m[i].ToString(), font, Brushes.Blue, 10, y); //设置文字内容及输出位置
                        y = y + 26;
                    }

                    int[] Count1 = new int[8];

                    foreach (var item in source)
                    {
                        double hourspan = (item.SubTime - today).TotalHours;

                        if (hourspan >= 0 && hourspan <= 3)
                        {
                            Count1[0] = (int)item.PhValues;
                        }
                        else if (hourspan >= 4 && hourspan <= 7)
                        {
                            Count1[1] = (int)item.PhValues;
                        }
                        else if (hourspan >= 8 && hourspan <= 11)
                        {
                            Count1[2] = (int)item.PhValues;
                        }
                        else if (hourspan >= 12 && hourspan <= 13)
                        {
                            Count1[3] = (int)item.PhValues;
                        }
                        else if (hourspan >= 14 && hourspan <= 15)
                        {
                            Count1[4] = (int)item.PhValues;
                        }
                        else if (hourspan >= 16 && hourspan <= 19)
                        {
                            Count1[5] = (int)item.PhValues;
                        }
                        else if (hourspan >= 20 && hourspan <= 22)
                        {
                            Count1[6] = (int)item.PhValues;
                        }
                        else if (hourspan >= 22 && hourspan <= 23)
                        {
                            Count1[7] = (int)item.PhValues;
                        }
                    }



                    #region PH处理


                    //显示折线效果
                    Font font3 = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                    SolidBrush mybrush = new SolidBrush(Color.Red);
                    Point[] points1 = new Point[8];
                    points1[0].X = 60; points1[0].Y = 340 - 26 * Count1[0]; //从106纵坐标开始, 到(0, 0)坐标时
                    points1[1].X = 140; points1[1].Y = 340 - 26 * Count1[1];
                    points1[2].X = 220; points1[2].Y = 340 - 26 * Count1[2];
                    points1[3].X = 300; points1[3].Y = 340 - 26 * Count1[3];

                    points1[4].X = 380; points1[4].Y = 340 - 26 * Count1[4];
                    points1[5].X = 460; points1[5].Y = 340 - 26 * Count1[5];

                    points1[6].X = 540; points1[6].Y = 340 - 26 * Count1[6];
                    points1[7].X = 620; points1[7].Y = 340 - 26 * Count1[7];
                    g.DrawLines(mypen2, points1); //绘制折线

                    //绘制数字
                    g.DrawString(Count1[0].ToString(), font3, Brushes.Red, 60, points1[0].Y - 20);
                    g.DrawString(Count1[1].ToString(), font3, Brushes.Red, 138, points1[1].Y - 20);
                    g.DrawString(Count1[2].ToString(), font3, Brushes.Red, 218, points1[2].Y - 20);
                    g.DrawString(Count1[3].ToString(), font3, Brushes.Red, 298, points1[3].Y - 20);

                    g.DrawString(Count1[4].ToString(), font3, Brushes.Red, 378, points1[4].Y - 20);
                    g.DrawString(Count1[5].ToString(), font3, Brushes.Red, 458, points1[5].Y - 20);

                    g.DrawString(Count1[6].ToString(), font3, Brushes.Red, 538, points1[6].Y - 20);
                    g.DrawString(Count1[7].ToString(), font3, Brushes.Red, 600, points1[7].Y - 20);
                    #endregion


                    //绘制标识
                    g.DrawRectangle(new Pen(Brushes.Blue), 180, 390, 250, 30); //绘制范围框

                    g.FillRectangle(Brushes.Red, 270, 400, 20, 10);
                    g.DrawString("PH", font2, Brushes.Red, 292, 400);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    string path = Server.MapPath("1.jpg");
                    path = path.Replace("Home\\", "");

                    System.IO.File.WriteAllBytes(path, ms.ToArray());
                    Response.ClearContent();

                    Response.ContentType = "image/Jpeg";
                    Response.BinaryWrite(ms.ToArray());


                }
                else if (data == 2)
                {
                    //y轴
                    String[] m = {  " 9", " 8", " 7", " 6", " 5", " 4", " 3",
"  2","  1"};
                    y = 100;
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawString(m[i].ToString(), font, Brushes.Blue, 10, y); //设置文字内容及输出位置
                        y = y + 26;
                    }

                    int[] Count1 = new int[8];

                    foreach (var item in source)
                    {
                        double hourspan = (item.SubTime - today).TotalHours;

                        if (hourspan >= 0 && hourspan <= 3)
                        {
                            Count1[0] = (int)item.Oxygen;
                        }
                        else if (hourspan >= 4 && hourspan <= 7)
                        {
                            Count1[1] = (int)item.Oxygen;
                        }
                        else if (hourspan >= 8 && hourspan <= 11)
                        {
                            Count1[2] = (int)item.Oxygen;
                        }
                        else if (hourspan >= 12 && hourspan <= 13)
                        {
                            Count1[3] = (int)item.Oxygen;
                        }
                        else if (hourspan >= 14 && hourspan <= 15)
                        {
                            Count1[4] = (int)item.Oxygen;
                        }
                        else if (hourspan >= 16 && hourspan <= 19)
                        {
                            Count1[5] = (int)item.Oxygen;
                        }
                        else if (hourspan >= 20 && hourspan <= 22)
                        {
                            Count1[6] = (int)item.Oxygen;
                        }
                        else if (hourspan >= 22 && hourspan <= 23)
                        {
                            Count1[7] = (int)item.Oxygen;
                        }
                    }



                    #region 溶解氧处理


                    //显示折线效果
                    Font font3 = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                    SolidBrush mybrush = new SolidBrush(Color.Red);
                    Point[] points1 = new Point[8];
                    points1[0].X = 60; points1[0].Y = 340 - 26 * Count1[0]; //从106纵坐标开始, 到(0, 0)坐标时
                    points1[1].X = 140; points1[1].Y = 340 - 26 * Count1[1];
                    points1[2].X = 220; points1[2].Y = 340 - 26 * Count1[2];
                    points1[3].X = 300; points1[3].Y = 340 - 26 * Count1[3];

                    points1[4].X = 380; points1[4].Y = 340 - 26 * Count1[4];
                    points1[5].X = 460; points1[5].Y = 340 - 26 * Count1[5];

                    points1[6].X = 540; points1[6].Y = 340 - 26 * Count1[6];
                    points1[7].X = 620; points1[7].Y = 340 - 26 * Count1[7];
                    g.DrawLines(mypen2, points1); //绘制折线

                    //绘制数字
                    g.DrawString(Count1[0].ToString(), font3, Brushes.Red, 60, points1[0].Y - 20);
                    g.DrawString(Count1[1].ToString(), font3, Brushes.Red, 138, points1[1].Y - 20);
                    g.DrawString(Count1[2].ToString(), font3, Brushes.Red, 218, points1[2].Y - 20);
                    g.DrawString(Count1[3].ToString(), font3, Brushes.Red, 298, points1[3].Y - 20);

                    g.DrawString(Count1[4].ToString(), font3, Brushes.Red, 378, points1[4].Y - 20);
                    g.DrawString(Count1[5].ToString(), font3, Brushes.Red, 458, points1[5].Y - 20);

                    g.DrawString(Count1[6].ToString(), font3, Brushes.Red, 538, points1[6].Y - 20);
                    g.DrawString(Count1[7].ToString(), font3, Brushes.Red, 600, points1[7].Y - 20);
                    #endregion


                    //绘制标识
                    g.DrawRectangle(new Pen(Brushes.Blue), 180, 390, 250, 30); //绘制范围框

                    g.FillRectangle(Brushes.Red, 270, 400, 20, 10);
                    g.DrawString("溶解氧", font2, Brushes.Red, 292, 400);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    string path = Server.MapPath("1.jpg");
                    path = path.Replace("Home\\", "");

                    System.IO.File.WriteAllBytes(path, ms.ToArray());
                    Response.ClearContent();

                    Response.ContentType = "image/Jpeg";
                    Response.BinaryWrite(ms.ToArray());


                }
                else if (data == 3)
                {
                    #region 光照的折线图



                    //y轴
                    String[] m = { " 450", " 400", " 350", " 300", " 250", " 200", " 150", " 100",
"  50"};
                    y = 100;
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawString(m[i].ToString(), font, Brushes.Blue, 10, y); //设置文字内容及输出位置
                        y = y + 26;
                    }
                    //温度
                    int[] Count1 = new int[8];


                    #region 数据处理

                    foreach (var item in source)
                    {
                        double hourspan = (item.SubTime - today).TotalHours;

                        if (hourspan >= 0 && hourspan <= 3)
                        {
                            if (jiedian == 1)
                            {
                                Count1[0] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[0] = (int)item.SunValue2;

                            }

                        }
                        else if (hourspan >= 4 && hourspan <= 7)
                        {
                            if (jiedian == 1)
                            {
                                Count1[1] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[1] = (int)item.SunValue2;

                            }
                        }
                        else if (hourspan >= 8 && hourspan <= 11)
                        {

                            if (jiedian == 1)
                            {
                                Count1[2] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[2] = (int)item.SunValue2;

                            }
                        }
                        else if (hourspan >= 12 && hourspan <= 13)
                        {
                            if (jiedian == 1)
                            {
                                Count1[3] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[3] = (int)item.SunValue2;

                            }
                        }
                        else if (hourspan >= 14 && hourspan <= 15)
                        {
                            if (jiedian == 1)
                            {
                                Count1[4] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[4] = (int)item.SunValue2;

                            }
                        }
                        else if (hourspan >= 16 && hourspan <= 19)
                        {
                            if (jiedian == 1)
                            {
                                Count1[5] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[5] = (int)item.SunValue2;

                            }
                        }
                        else if (hourspan >= 20 && hourspan <= 22)
                        {
                            if (jiedian == 1)
                            {
                                Count1[6] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[6] = (int)item.SunValue2;

                            }
                        }
                        else if (hourspan >= 22 && hourspan <= 23)
                        {
                            if (jiedian == 1)
                            {
                                Count1[7] = (int)item.SunValue;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[7] = (int)item.SunValue2;

                            }
                        }
                    }
                    #endregion

                    #region 温度


                    //显示折线效果
                    Font font3 = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                    SolidBrush mybrush = new SolidBrush(Color.Red);
                    Point[] points1 = new Point[8];
                    points1[0].X = 60; points1[0].Y = 340 - Count1[0] / 2; //从106纵坐标开始, 到(0, 0)坐标时
                    points1[1].X = 140; points1[1].Y = 340 - Count1[1] / 2;
                    points1[2].X = 220; points1[2].Y = 340 - Count1[2] / 2;
                    points1[3].X = 300; points1[3].Y = 340 - Count1[3] / 2;

                    points1[4].X = 380; points1[4].Y = 340 - Count1[4] / 2;
                    points1[5].X = 460; points1[5].Y = 340 - Count1[5] / 2;

                    points1[6].X = 540; points1[6].Y = 340 - Count1[6] / 2;
                    points1[7].X = 620; points1[7].Y = 340 - Count1[7] / 2;
                    g.DrawLines(mypen2, points1); //绘制折线

                    //绘制数字
                    g.DrawString(Count1[0].ToString(), font3, Brushes.Red, 60, points1[0].Y - 20);
                    g.DrawString(Count1[1].ToString(), font3, Brushes.Red, 138, points1[1].Y - 20);
                    g.DrawString(Count1[2].ToString(), font3, Brushes.Red, 218, points1[2].Y - 20);
                    g.DrawString(Count1[3].ToString(), font3, Brushes.Red, 298, points1[3].Y - 20);

                    g.DrawString(Count1[4].ToString(), font3, Brushes.Red, 378, points1[4].Y - 20);
                    g.DrawString(Count1[5].ToString(), font3, Brushes.Red, 458, points1[5].Y - 20);

                    g.DrawString(Count1[6].ToString(), font3, Brushes.Red, 538, points1[6].Y - 20);
                    g.DrawString(Count1[7].ToString(), font3, Brushes.Red, 600, points1[7].Y - 20);
                    #endregion

                    //绘制标识
                    g.DrawRectangle(new Pen(Brushes.Blue), 180, 390, 250, 40); //绘制范围框
                    g.FillRectangle(Brushes.Red, 270, 402, 20, 10); //绘制小矩形
                    g.DrawString("光照", font2, Brushes.Red, 292, 400);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    string path = Server.MapPath("1.jpg");
                    path = path.Replace("Home\\", "");

                    System.IO.File.WriteAllBytes(path, ms.ToArray());

                    Response.ClearContent();

                    Response.ContentType = "image/Jpeg";
                    Response.BinaryWrite(ms.ToArray());
                    #endregion
                }
                else if (data == 4)
                {
                    #region 环境湿度


                    String[] m = { " 90", " 80", " 70", " 60", " 50", " 40", " 30", " 20",
"  10"};
                    y = 100;
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawString(m[i].ToString(), font, Brushes.Blue, 10, y); //设置文字内容及输出位置
                        y = y + 26;
                    }
                    //温度
                    int[] Count1 = new int[8];


                    #region 数据处理

                    foreach (var item in source)
                    {
                        double hourspan = (item.SubTime - today).TotalHours;

                        if (hourspan >= 0 && hourspan <= 3)
                        {
                            if (jiedian == 1)
                            {
                                Count1[0] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[0] = (int)item.EnviroHumidity2;

                            }

                        }
                        else if (hourspan >= 4 && hourspan <= 7)
                        {
                            if (jiedian == 1)
                            {
                                Count1[1] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[1] = (int)item.EnviroHumidity2;

                            }
                        }
                        else if (hourspan >= 8 && hourspan <= 11)
                        {

                            if (jiedian == 1)
                            {
                                Count1[2] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[2] = (int)item.EnviroHumidity2;

                            }
                        }
                        else if (hourspan >= 12 && hourspan <= 13)
                        {
                            if (jiedian == 1)
                            {
                                Count1[3] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[3] = (int)item.EnviroHumidity2;

                            }
                        }
                        else if (hourspan >= 14 && hourspan <= 15)
                        {
                            if (jiedian == 1)
                            {
                                Count1[4] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[4] = (int)item.EnviroHumidity2;

                            }
                        }
                        else if (hourspan >= 16 && hourspan <= 19)
                        {
                            if (jiedian == 1)
                            {
                                Count1[5] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[5] = (int)item.EnviroHumidity2;

                            }
                        }
                        else if (hourspan >= 20 && hourspan <= 22)
                        {
                            if (jiedian == 1)
                            {
                                Count1[6] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[6] = (int)item.EnviroHumidity2;

                            }
                        }
                        else if (hourspan >= 22 && hourspan <= 23)
                        {
                            if (jiedian == 1)
                            {
                                Count1[7] = (int)item.EnviroHumidity;

                            }
                            else if (jiedian == 2)
                            {
                                Count1[7] = (int)item.EnviroHumidity2;

                            }
                        }
                    }
                    #endregion

                    #region 湿度


                    //显示折线效果
                    Font font3 = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                    SolidBrush mybrush = new SolidBrush(Color.Red);
                    Point[] points1 = new Point[8];
                    points1[0].X = 60; points1[0].Y = (int)(340 - 2.6 * Count1[0]); //从106纵坐标开始, 到(0, 0)坐标时
                    points1[1].X = 140; points1[1].Y = (int)(340 - 2.6 * Count1[1]);
                    points1[2].X = 220; points1[2].Y = (int)(340 - 2.6 * Count1[2]);
                    points1[3].X = 300; points1[3].Y = (int)(340 - 2.6 * Count1[3]);
                    points1[4].X = 380; points1[4].Y = (int)(340 - 2.6 * Count1[4]);
                    points1[5].X = 460; points1[5].Y = (int)(340 - 2.6 * Count1[5]);
                    points1[6].X = 540; points1[6].Y = (int)(340 - 2.6 * Count1[6]);
                    points1[7].X = 620; points1[7].Y = (int)(340 - 2.6 * Count1[7]);
                    g.DrawLines(mypen2, points1); //绘制折线

                    //绘制数字
                    g.DrawString(Count1[0].ToString(), font3, Brushes.Red, 60, points1[0].Y - 20);
                    g.DrawString(Count1[1].ToString(), font3, Brushes.Red, 138, points1[1].Y - 20);
                    g.DrawString(Count1[2].ToString(), font3, Brushes.Red, 218, points1[2].Y - 20);
                    g.DrawString(Count1[3].ToString(), font3, Brushes.Red, 298, points1[3].Y - 20);

                    g.DrawString(Count1[4].ToString(), font3, Brushes.Red, 378, points1[4].Y - 20);
                    g.DrawString(Count1[5].ToString(), font3, Brushes.Red, 458, points1[5].Y - 20);

                    g.DrawString(Count1[6].ToString(), font3, Brushes.Red, 538, points1[6].Y - 20);
                    g.DrawString(Count1[7].ToString(), font3, Brushes.Red, 600, points1[7].Y - 20);
                    #endregion

                    //绘制标识
                    g.DrawRectangle(new Pen(Brushes.Blue), 180, 390, 250, 40); //绘制范围框
                    g.FillRectangle(Brushes.Red, 270, 402, 20, 10); //绘制小矩形
                    g.DrawString("湿度", font2, Brushes.Red, 292, 400);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    string path = Server.MapPath("1.jpg");
                    path = path.Replace("Home\\", "");

                    System.IO.File.WriteAllBytes(path, ms.ToArray());

                    Response.ClearContent();

                    Response.ContentType = "image/Jpeg";
                    Response.BinaryWrite(ms.ToArray());
                    #endregion
                }

                return View();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }

        }

        //导出excel
        public ActionResult GetExcel(int jiedian, int data, DateTime today)
        {
            string title = today.ToString("yyyy-MM-dd ") + jiedian + "号节点" + "数据统计图";

            title = title + Guid.NewGuid().ToString();

            IWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("智能水产数据");

            string path = Server.MapPath("1.jpg");
            path = path.Replace("Home\\", "");

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            int pictureIdx = hssfworkbook.AddPicture(bytes, PictureType.JPEG);

            HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 1023, 0, 0, 0, 10, 20);
            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);

            string writepath = Server.MapPath("upload/" + title+".xls");
            writepath = writepath.Replace("Home\\", "");

            using (FileStream write = System.IO.File.OpenWrite(writepath))
            {
                hssfworkbook.Write(write);
            }
            return Content("/upload/" + title + ".xls");
        }

        public ActionResult OpenLight(int num,int type)
        {
            byte[] sendbyte = new byte[]{};
            if (num == 1)
            {
                if (type == 1)
                {
                    //打开一号灯
                    sendbyte = new byte[] { 0x12, 0xB1, 0x11 };
                }
                else if (type == 0)
                {
                    //关闭一号灯
                    sendbyte = new byte[] { 0x12, 0xB1, 0x12 };
                }
            }
            else
            {
                if (type == 1)
                {
                    //打开二号灯
                    sendbyte = new byte[] { 0x12, 0xB1, 0x21 };
                }
                else if (type == 0)
                {
                    //关闭二号灯
                    sendbyte = new byte[] { 0x12, 0xB1, 0x22 };
                }
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
               
                socket.Connect("192.168.0.15", 8080);

                socket.Send(sendbyte);
                return Content("操作成功");
            }
            catch (Exception)
            {
                return Content("设备连接失败");
            }
            finally
            {
                socket.Close();
            }
        }

    }
}

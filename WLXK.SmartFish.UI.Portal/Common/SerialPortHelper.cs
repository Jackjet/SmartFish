using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using WLXK.SmartFish.Bll;
using WLXK.SmartFish.IBll;
using WLXK.SmartFish.Model;

namespace WLXK.SmartFish.UI.Portal.Common
{
    public static class SerialPortHelper
    {
        private static string serverIp = ConfigurationManager.AppSettings["ServerIp"].ToString();
        private static string serverPort = ConfigurationManager.AppSettings["ServerPort"].ToString();

        public static IReceiveDatasServices ReceiveDatasServices = new ReceiveDatasServices();
        public static IControlDataServices ControlDataServices = new ControlDataServices();

        public static void GetData()
        {
            //接收数据的线程
            Thread th = new Thread(StartGet);
            th.IsBackground = true;
            th.Start();

            // 自动控制的线程
            Thread contTh = new Thread(CompareData);
            contTh.IsBackground = true;
            contTh.Start();

        }

        private static void StartSendGet(object obj)
        {
            try
            {
                while (true)
                {
                    if (socket != null && socket.Connected)
                    {
                        //发送数据，获取溶解氧
                        byte[] a = new byte[] { 0xFC, 0xAB, 0x01, 0x03, 0x00, 0x00, 0x00, 0x0C, 0x45, 0xCF };
                        socket.Send(a);

                        Thread.Sleep(100);

                        //发送数据，获取PH
                        byte[] b = new byte[] { 0xFC, 0xAB, 0x02, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x39 };
                        socket.Send(b);
                        Thread.Sleep(20000);
                    }

                }
            }
            catch (Exception)
            {
                StartGet();
            }
          

        }

        private static StringBuilder builder = new StringBuilder();//存储数据

        static Socket socket;

        private static void StartGet()
        {

            Thread thget = new Thread(DataReceived);
            Thread th2 = new Thread(StartSendGet);
            try
            {
                //连接服务器
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverIp, int.Parse(serverPort));

                thget.IsBackground = true;
                thget.Start();

                th2.IsBackground = true;
                th2.Start();

            }
            catch
            {
                thget.Abort();
                th2.Abort();

                //throw new Exception("服务器连接失败");
                Thread.Sleep(10000);
                StartGet();
            }
        }
        static int sum = 0;
        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void DataReceived()
        {
            try
            {
                while (true)
                {
                    byte[] bytes = new byte[1024 * 1024 * 3];

                    int r = socket.Receive(bytes);

                    string content = getStringFromBytes(bytes, r);

                    builder.Append(content + " ");//添加到数据缓存中

                    sum = builder.Length;

                    if (sum > 100)
                    {
                        string text = builder.ToString();

                        #region 一号节点的数据
                        Match match = Regex.Match(text, "FC A1 ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2})");


                        //温度
                        int value1 = GetHexadecimalValue(match.Groups[1].Value);
                        //湿度
                        int value2 = GetHexadecimalValue(match.Groups[2].Value);
                        //水温
                        int value3 = GetHexadecimalValue(match.Groups[3].Value);
                        //光照
                        int value4 = GetHexadecimalValue(match.Groups[4].Value);
                        #endregion

                        #region 二号节点的数据
                        Match matchtwo = Regex.Match(text, "FC A2 ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2})");


                        //温度
                        int value21 = GetHexadecimalValue(matchtwo.Groups[1].Value);
                        //湿度
                        int value22 = GetHexadecimalValue(matchtwo.Groups[2].Value);
                        //水温
                        int value23 = GetHexadecimalValue(matchtwo.Groups[3].Value);
                        //光照
                        int value24 = GetHexadecimalValue(matchtwo.Groups[4].Value);
                        #endregion


                        //溶解氧的数据
                        Match match2 = Regex.Match(text, "FC AB 01 03 0C ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2})");
                        string v1 = match2.Groups[1].Value;
                        string v2 = match2.Groups[2].Value;
                        int shuju = GetHexadecimalValue(v1 + v2);
                        double all = ((double)shuju) / 1000;

                        //PH值
                        Match match3 = Regex.Match(text, "FC AB 02 03 02 ([a-fA-F0-9]{2}) ([a-fA-F0-9]{2})");
                        string s1 = match3.Groups[1].Value;
                        string s2 = match3.Groups[2].Value;
                        int phvalue = GetHexadecimalValue(s1 + s2);
                        double phv = phvalue / 100;

                        if (value1 != 0 && value2 != 0 && value3 != 0 && all != 0 && phvalue != 0)
                        {
                            ReceiveDatas data = new ReceiveDatas();
                            data.EnviroTemperate = value1;
                            data.EnviroHumidity = value2;
                            data.FishTemperate = value3;
                            data.SunValue = value4;
                            data.Oxygen = all;
                            data.PhValues = phv;

                            data.EnviroTemperate2 = value21;
                            data.EnviroHumidity2 = value22;
                            data.FishTemperate2 = value23;
                            data.SunValue2 = value24;

                            data.SubTime = DateTime.Now;


                            ReceiveDatasServices.Add(data);


                            //Thread.Sleep(0000);
                        }
                        sum = 0;

                        //清除接收数据
                        builder.Clear();
                    }
                }
            }
            catch
            {
                Thread.Sleep(1000);
                socket.Close();

                StartGet();
            }
            
        }
        /// <summary>
        /// 十六进制换算为十进制
        /// </summary>
        /// <param name="strColorValue"></param>
        /// <returns></returns>
        public static int GetHexadecimalValue(String strColorValue)
        {
            char[] nums = strColorValue.ToCharArray();
            int total = 0;
            try
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    String strNum = nums[i].ToString().ToUpper();
                    switch (strNum)
                    {
                        case "A":
                            strNum = "10";
                            break;
                        case "B":
                            strNum = "11";
                            break;
                        case "C":
                            strNum = "12";
                            break;
                        case "D":
                            strNum = "13";
                            break;
                        case "E":
                            strNum = "14";
                            break;
                        case "F":
                            strNum = "15";
                            break;
                        default:
                            break;
                    }
                    double power = Math.Pow(16, Convert.ToDouble(nums.Length - i - 1));
                    total += Convert.ToInt32(strNum) * Convert.ToInt32(power);
                }

            }
            catch (System.Exception ex)
            {
                String strErorr = ex.ToString();
                return 0;
            }


            return total;
        }
        /// <summary>
        /// 把字节数组转换为十六进制格式的字符串。
        /// </summary>
        /// <param name="pByte">要转换的字节数组。</param>
        /// <returns>返回十六进制格式的字符串。</returns>
        public static string getStringFromBytes(byte[] pByte, int r)
        {
            string str = "";     //定义字符串类型临时变量。
            //遍历字节数组，把每个字节转换成十六进制字符串，不足两位前面添“0”，以空格分隔累加到字符串变量里。
            for (int i = 0; i < r; i++)
                str += (pByte[i].ToString("X").PadLeft(2, '0') + " ");
            str = str.TrimEnd(' ');     //去掉字符串末尾的空格。
            return str;     //返回字符串临时变量。
        }

        /// <summary>
        /// 发送数据到指定串口
        /// </summary>
        /// <param name="isHex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SendData(bool isHex, byte[] data)
        {
          
                try
                {
                    //16进制发送   
                    if (isHex)
                    {
                        //转换列表为数组后发送   
                        socket.Send(data);
                    }
                    else
                    {
                        socket.Send(data);
                    }

                    return true;
                }
                catch (Exception)
                {
                    

                    socket.Close();

                    StartGet();

                    return false;


                }

        }
        /// <summary>
        /// 自动控制
        /// </summary>
        public static void CompareData()
        {
            try
            {
                while (true)
                {

                    ReceiveDatas receive = ReceiveDatasServices.LoadEntities(u => true).LastOrDefault();
                    ControlData control = ControlDataServices.LoadEntities(u => true).LastOrDefault();

                    if (receive != null && control != null)
                    {
                        //控制温度
                        if (receive.EnviroTemperate < control.MinTemperate)
                        {
                            Thread.Sleep(500);
                            //如果温度过低提高温度
                            SendData(true, new byte[] { 0x13 });
                            Thread.Sleep(500);
                            //如果温度过低提高温度
                            SendData(true, new byte[] { 0x13 });
                            Thread.Sleep(500);
                            //如果温度过低提高温度
                            SendData(true, new byte[] { 0x13 });
                        }
                        else if (receive.EnviroTemperate > control.MaxTemperate)
                        {
                            Thread.Sleep(500);
                            //如果温度过高停止升温
                            SendData(true, new byte[] { 0x14 });
                            Thread.Sleep(500);
                            //如果温度过高停止升温
                            SendData(true, new byte[] { 0x14 });
                            Thread.Sleep(500);
                            //如果温度过高停止升温
                            SendData(true, new byte[] { 0x14 });
                        }
                        else
                        {
                            //正常状态

                            Thread.Sleep(500);
                            //如果温度过高停止升温
                            SendData(true, new byte[] { 0x14 });
                            Thread.Sleep(500);
                            //如果温度过高停止升温
                            SendData(true, new byte[] { 0x14 });
                            Thread.Sleep(500);
                            //如果温度过高停止升温
                            SendData(true, new byte[] { 0x14 });

                        }

                        //控制氧气
                        if (receive.Oxygen < control.Oxygen)
                        {
                            Thread.Sleep(500);
                            //开始增氧
                            SendData(true, new byte[] { 0x21 });
                            Thread.Sleep(500);
                            //开始增氧
                            SendData(true, new byte[] { 0x21 });
                            Thread.Sleep(500);
                            //开始增氧
                            SendData(true, new byte[] { 0x21 });
                        }
                        else
                        {
                            Thread.Sleep(500);
                            //正常状态，停止增氧
                            SendData(true, new byte[] { 0x22 });
                            Thread.Sleep(500);
                            //正常状态，停止增氧
                            SendData(true, new byte[] { 0x22 });
                            Thread.Sleep(500);
                            //正常状态，停止增氧
                            SendData(true, new byte[] { 0x22 });
                        }

                        //控制排水
                        if (receive.PhValues > control.PhValues)
                        {
                            Thread.Sleep(500);
                            //开始排水
                            SerialPortHelper.SendData(true, new byte[] { 0x11 });
                            Thread.Sleep(500);
                            //开始排水
                            SerialPortHelper.SendData(true, new byte[] { 0x11 }); Thread.Sleep(500);
                            //开始排水
                            SerialPortHelper.SendData(true, new byte[] { 0x11 });
                        }
                        else
                        {
                            Thread.Sleep(500);
                            //停止排水
                            SendData(true, new byte[] { 0x12 });
                            Thread.Sleep(500);
                            //停止排水
                            SendData(true, new byte[] { 0x12 });
                            Thread.Sleep(500);
                            //停止排水
                            SendData(true, new byte[] { 0x12 });
                        }


                    }
                    Thread.Sleep(10000);
                }
            }
            catch
            {
            }

            
        }

        public static void CloseSocket()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

    }
}
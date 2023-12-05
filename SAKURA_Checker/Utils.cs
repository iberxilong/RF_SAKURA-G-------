using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SAKURA
{
    class Utils
    {
        public static string formHexlString(string str)
        {
            Regex reg = new Regex("(^ +)|( +$)");           //定位字符串开头或结尾的空字符
            string strTrim = reg.Replace(str, "");          //将空字符去掉
            string[] strSplit = Regex.Split(strTrim, " +"); //字符串每个字母之间可能有多个空格，只保留一个

            //用于检测明文、密文等输入的正确性
            foreach (string s in strSplit)
            {
                if (s.Length > 2)
                    throw new SystemException("Error: 输入16进制数超过3位");
            }
            if (strSplit.Length > 16)
                throw new SystemException("Error: 输入超过16组，请重新输入");
            else if (strSplit.Length < 16)
                throw new SystemException("输入不足16组，请重新输入");
            return String.Join(" ", strSplit);
        }

        public static string byteArrayToString(byte[] data)
        {
            if (data == null)
            {
                return "";
            }

            string str = "";
            if (data.Length > 0)
            {
                for (int i = 0; i < data.Length - 1; i++)
                {
                    str += string.Format("{0:X2} ", data[i]);
                }
                str += string.Format("{0:X2}", data[data.Length - 1]);//注意与for里面不一样的空格
            }
            return str;
        }

        public static byte[] stringToByteArray(string hexString)
        {
            string[] strSplit = hexString.Split(' ');
            if (strSplit[0] == "")
            {
                return new byte[0];
            }
            var bytesList = new List<byte>();
            foreach (string hex in strSplit)
            {
                bytesList.Add((byte)Convert.ToByte(hex, 16));       //把字符串转化为16进制，若出现*等，cantch
            }                                                       //会捕捉错误
            return bytesList.ToArray();
        }
        public static int[] stringToIntArray(string hexString)
        {
            string[] strSplit = hexString.Split(' ');
            if (strSplit[0] == "")
            {
                return new int[0];
            }
            var intsList = new List<int>();
            foreach (string hex in strSplit)
            {
                intsList.Add((int)Convert.ToInt32(hex, 2000));
            }
            return intsList.ToArray();
        }

        public static bool differenceByteArray(ref byte[] difference, byte[] input1, byte[] input2)
        {
            bool diff = false;
            difference = new byte[16];

            for (int i = 0; i < difference.Length; i++)
            {
                difference[i] = (byte)(input1[i] ^ input2[i]);//异或操作
                if (difference[i] != 0)
                {
                    diff = true;
                }
            }
            return diff;
        }
    }
}

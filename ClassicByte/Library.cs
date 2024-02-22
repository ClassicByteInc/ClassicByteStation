using Microsoft.Win32;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ClassicByte
{
    /// <summary>
    /// 所有在ClassicByte解决方案下的自定义类的基类，规定了当前系统环境变量和某些属性
    /// </summary>
    public abstract class Station
    {
        /// <summary>
        /// 当前系统的ClassicByte工作区的目录
        /// </summary>
        protected static String Workspace
        {
            get
            {
                string value = Environment.GetEnvironmentVariable("CLASSICBYtEWORKSPACE");
                if (value == null)
                {
                    throw new WorkspaceNotFound();
                }
                else
                {
                    return value;
                }
            }
        }


    }

    namespace Library
    {
        namespace Util
        {
            /// <summary>
            /// 为ClassicByte项目提供常用功能
            /// </summary>
            public static class Functions
            {
                /// <summary>
                /// AES加密，并且有向量
                /// </summary>
                /// <param name="encrypteStr">需要加密的明文</param>
                /// <param name="key">秘钥</param>
                /// <param name="vector">向量</param>
                /// <returns>密文</returns>
                public static string AESEncryptedString(this string encrypteStr, string key, string vector)
                {
                    byte[] aesBytes = Encoding.UTF8.GetBytes(encrypteStr);

                    byte[] aesKey = new byte[32];
                    //直接转
                    Array.Copy(Convert.FromBase64String(key), aesKey, aesKey.Length);
                    byte[] aesVector = new byte[16];
                    //直接转
                    Array.Copy(Convert.FromBase64String(vector), aesVector, aesVector.Length);

                    Rijndael Aes = Rijndael.Create();
                    //或者采用下方生成Aes
                    //RijndaelManaged Aes = new();

                    // 开辟一块内存流  
                    using MemoryStream memoryStream = new();
                    // 把内存流对象包装成加密流对象  
                    using CryptoStream cryptoStream = new(memoryStream, Aes.CreateEncryptor(aesKey, aesVector), CryptoStreamMode.Write);
                    // 明文数据写入加密流  
                    cryptoStream.Write(aesBytes, 0, aesBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    string result = Convert.ToBase64String(memoryStream.ToArray());
                    return result;
                }

                /// <summary>
                /// AES解密，并且有向量
                /// </summary>
                /// <param name="decryptStr">被加密的明文</param>
                /// <param name="key">秘钥</param>
                /// <param name="vector">向量</param>
                /// <returns>明文</returns>
                public static string AESDecryptString(this string decryptStr, string key, string vector)
                {
                    byte[] aesBytes = Convert.FromBase64String(decryptStr);
                    byte[] aesKey = new byte[32];
                    //直接转，可采用不同的方法，但是需与加密方法一致
                    Array.Copy(Convert.FromBase64String(key), aesKey, aesKey.Length);
                    byte[] aesVector = new byte[16];
                    //直接转，可采用不同的方法，但是需与加密方法一致
                    Array.Copy(Convert.FromBase64String(vector), aesVector, aesVector.Length);
                    Rijndael Aes = Rijndael.Create();
                    //或者采用下方生成Aes
                    //RijndaelManaged Aes = new();

                    // 开辟一块内存流，存储密文  
                    using MemoryStream memoryStream = new(aesBytes);
                    // 把内存流对象包装成加密流对象  
                    using CryptoStream Decryptor = new(memoryStream, Aes.CreateDecryptor(aesKey, aesVector), CryptoStreamMode.Read);
                    // 明文存储区  
                    using MemoryStream originalMemory = new();
                    byte[] Buffer = new byte[1024];
                    int readBytes = 0;
                    while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                    {
                        originalMemory.Write(Buffer, 0, readBytes);
                    }

                    byte[] original = originalMemory.ToArray();
                    string result = Convert.ToBase64String(originalMemory.ToArray());
                    return result;
                }

                /// <summary>  
                /// AES加密(无向量)  
                /// </summary>  
                /// <param name="encrypteStr">需要加密的明文</param>  
                /// <param name="key">密钥</param>  
                /// <returns>密文</returns>  
                public static string AESEncryptedString(this string encrypteStr, string key)
                {
                    byte[] aesBytes = Encoding.UTF8.GetBytes(encrypteStr);
                    byte[] aesKey = new byte[32];
                    //直接转
                    //Array.Copy(Convert.FromBase64String(key), aesKey, aesKey.Length);
                    //当长度不够时，右侧添加空格
                    Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(aesKey.Length)), aesKey, aesKey.Length);

                    using MemoryStream memoryStream = new();
                    Rijndael Aes = Rijndael.Create();
                    //或者采用下方生成Aes
                    //RijndaelManaged Aes = new();

                    Aes.Mode = CipherMode.ECB;
                    Aes.Padding = PaddingMode.PKCS7;
                    Aes.KeySize = 128;
                    Aes.Key = aesKey;
                    using CryptoStream cryptoStream = new(memoryStream, Aes.CreateEncryptor(), CryptoStreamMode.Write);
                    cryptoStream.Write(aesBytes, 0, aesBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    Aes.Clear();
                    return Convert.ToBase64String(memoryStream.ToArray());
                }


                /// <summary>
                /// AES解密(无向量)  
                /// </summary>  
                /// <param name="decryptStr">被加密的明文</param>  
                /// <param name="key">密钥</param>  
                /// <returns>明文</returns>  
                public static string AESDecryptString(this string decryptStr, string key)
                {
                    byte[] aesBytes = Convert.FromBase64String(decryptStr);
                    byte[] aesKey = new byte[32];
                    //需要跟加密一致
                    //直接转
                    //Array.Copy(Convert.FromBase64String(key), aesKey, aesKey.Length);
                    //当长度不够时，右侧添加空格
                    Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(aesKey.Length)), aesKey, aesKey.Length);

                    using MemoryStream memoryStream = new(aesBytes);
                    Rijndael Aes = Rijndael.Create();
                    //或者采用下方生成Aes
                    //RijndaelManaged Aes = new();

                    Aes.Mode = CipherMode.ECB;//需与加密方法一致
                    Aes.Padding = PaddingMode.PKCS7;//需与加密方法一致
                    Aes.KeySize = 128;
                    Aes.Key = aesKey;
                    using CryptoStream cryptoStream = new(memoryStream, Aes.CreateDecryptor(), CryptoStreamMode.Read);

                    byte[] temp = new byte[aesBytes.Length + 32];
                    int len = cryptoStream.Read(temp, 0, aesBytes.Length + 32);
                    byte[] ret = new byte[len];
                    Array.Copy(temp, 0, ret, 0, len);
                    Aes.Clear();
                    string result = Encoding.UTF8.GetString(ret);
                    return result;
                }

                /// <summary>
                /// 将目标转换为base64密文
                /// </summary>
                /// <param name="target">转换目标</param>
                /// <returns>base64密文</returns>
                public static string ToBase64(string target)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(target);
                    return Convert.ToBase64String(bytes);
                }

                /// <summary>
                /// 将目标base64密文转换为明文
                /// </summary>
                /// <param name="target">base64密文</param>
                /// <returns>明文</returns>
                public static string DeBase64(string target)
                {
                    //target = target.Replace("+","");
                    byte[] outputb = Convert.FromBase64String(target);
                    return Encoding.Default.GetString(outputb);
                }

                /// <summary>
                /// 通过AES加密文件
                /// </summary>
                /// <param name="path">文件路径</param>
                /// <param name="key">加密密钥</param>
                public static void FileCoding(string path, string key)
                {
                    //try
                    //{
                    string text = File.ReadAllText(path);
                    string code = AESEncryptedString(text, key);
                    File.WriteAllText(path, code);
                    //}
                    //catch (System.ArgumentException) {
                    //    MessageBox.Show("","",MessageBoxButtons.OKCancel,MessageBoxIcon.Stop);
                    //}
                }

                /// <summary>
                /// 通过AES解密文件
                /// </summary>
                /// <param name="path">文件路径</param>
                /// <param name="key">加密密钥</param>
                public static void FileDeCoding(string path, string key)
                {
                    string code = File.ReadAllText(path);
                    string text = AESDecryptString(code, key);
                    File.WriteAllText(path, text);
                }


                /// <summary>
                /// 解析ClassicByte协议
                /// </summary>
                /// <param name="protocol_mode">协议模式</param>
                /// <param name="target">对象字符串</param>
                /// <returns>协议</returns>
                public static string[] ClassicByteProtocolParser(ClassicByteProtocolMode protocol_mode, string target)
                {
                    switch (protocol_mode)
                    {
                        case ClassicByteProtocolMode.A:
                            {
                                return target.Split(':');

                            }
                        case ClassicByteProtocolMode.B:
                            {
                                //return target.Split("//", StringSplitOptions.RemoveEmptyEntries);
                                break;
                            }
                    }
                    return null;
                }

                /// <summary>
                /// ClassicByte协议
                /// </summary>
                public enum ClassicByteProtocolMode
                {
                    A, B, C, D, E, F, G
                };


                /// <summary>
                /// 三角函数
                /// </summary>
                public static void MathSine()
                {
                    Console.WriteLine("开始输出....");
                    Double s0 = Math.Sin(0.00029);
                    double c0 = Math.Sqrt(1 - Math.Pow(s0, 2));
                    Console.WriteLine(s0);
                    double s = s0;
                    double c = c0;
                    int n = 2;
                    while (!(n > 5400))
                    {
                        s = s * c0 + c * s0;
                        c = Math.Sqrt(1 - Math.Pow(s, 2));
                        Console.WriteLine(s);
                        n++;
                    }
                }

                /// <summary>
                /// 在文件后写入内容
                /// </summary>
                /// <param name="value">写入内容</param>
                /// <param name="path">文件路径</param>
                public static void LogOut(string value, string path)
                {


                    // 打开文件并创建StreamWriter对象
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        // 写入文件
                        writer.WriteLine("[{0}]{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), value);
                    }
                }

                /// <summary>
                /// 新建环境变量
                /// </summary>
                /// <param name="root">环境变量名</param>
                /// <param name="value">环境变量值</param>
                public static void SetEnvironmentVariable(string root,string value) {

                    Environment.SetEnvironmentVariable(root, value, EnvironmentVariableTarget.Machine);

                }
            }
        }
        namespace Language {
            /// <summary>
            /// 使ClassicByte支持不同的语言
            /// </summary>
            public partial class RunTimeLanguage:Station 
            {
                public String start;
                public RunTimeLanguage() { }
            }
        }

    }
}

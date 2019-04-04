﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UniversalAPP.Tools
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 目录分隔符 windows 下是 "\"， Mac OS and Linux 下是 "/"
        /// </summary>
        private static string DirectorySeparatorChar = Path.DirectorySeparatorChar.ToString();

        /// <summary>
        /// 环境根目录
        /// </summary>
        private static string _WebRootPath = "";

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="WebRootPath">IHostingEnvironment.WebRootPath</param>
        public FileHelper(string WebRootPath)
        {
            _WebRootPath = WebRootPath;
        }

        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public string GetMD5HashFromFile(string filePath)
        {
            try
            {
                FileStream file = new FileStream(IsAbsolute(filePath) ? filePath : MapPath(filePath), FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取文件绝对路径
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            var result = IsAbsolute(path) ? path : Path.Combine(_WebRootPath, path.TrimStart('~', '/').Replace("/", DirectorySeparatorChar));
            return result;
        }

        /// <summary>
        /// 是否是绝对路径
        /// windows下判断 路径是否包含 ":"
        /// Mac OS、Linux下判断 路径是否包含 "\"
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool IsAbsolute(string path)
        {
            return Path.VolumeSeparatorChar == ':' ? path.IndexOf(Path.VolumeSeparatorChar) > 0 : path.IndexOf('\\') > 0;
        }

        /// <summary>
        /// 检测指定路径是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isDirectory">是否是目录</param>
        /// <returns></returns>
        public bool IsExist(string path, bool isDirectory)
        {
            return isDirectory ? Directory.Exists(IsAbsolute(path) ? path : MapPath(path)) : File.Exists(IsAbsolute(path) ? path : MapPath(path));
        }

        /// <summary>
        /// 检测目录是否为空
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool IsEmptyDirectory(string path)
        {
            return Directory.GetFiles(IsAbsolute(path) ? path : MapPath(path)).Length <= 0 && Directory.GetDirectories(IsAbsolute(path) ? path : MapPath(path)).Length <= 0;
        }

        /// <summary>
        /// 创建目录或文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isDirectory">是否是目录</param>
        public void CreateFiles(string path, bool isDirectory)
        {
            try
            {
                if (!IsExist(path, isDirectory))
                {
                    if (isDirectory)
                        Directory.CreateDirectory(IsAbsolute(path) ? path : MapPath(path));
                    else
                    {
                        FileInfo file = new FileInfo(IsAbsolute(path) ? path : MapPath(path));
                        FileStream fs = file.Create();
                        fs.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除目录或文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isDirectory">是否是目录</param>
        public void DeleteFiles(string path, bool isDirectory)
        {
            try
            {
                if (IsExist(path, isDirectory))
                {
                    if (isDirectory)
                        Directory.Delete(IsAbsolute(path) ? path : MapPath(path));
                    else
                        File.Delete(IsAbsolute(path) ? path : MapPath(path));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 清空目录下所有文件及子目录，依然保留该目录
        /// </summary>
        /// <param name="path"></param>
        public void ClearDirectory(string path)
        {
            if (IsExist(path, true))
            {
                //目录下所有文件
                string[] files = Directory.GetFiles(IsAbsolute(path) ? path : MapPath(path));
                foreach (var file in files)
                {
                    DeleteFiles(file, false);
                }
                //目录下所有子目录
                string[] directorys = Directory.GetDirectories(IsAbsolute(path) ? path : MapPath(path));
                foreach (var dir in directorys)
                {
                    DeleteFiles(dir, true);
                }
            }
        }
        
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <returns></returns>
        public long GetFileSize(string path)
        {
            if (!IsExist(path,false)) return 0;
            FileInfo fileInfo = new FileInfo(IsAbsolute(path) ? path : MapPath(path));
            return fileInfo.Length;            
        }

        /// <summary>
        /// 复制文件内容到目标文件夹
        /// </summary>
        /// <param name="sourcePath">源文件</param>
        /// <param name="targetPath">目标文件夹</param>
        /// <param name="isOverWrite">是否可以覆盖</param>
        public void Copy(string sourcePath, string targetPath, bool isOverWrite = true)
        {
            File.Copy(IsAbsolute(sourcePath) ? sourcePath : MapPath(sourcePath), (IsAbsolute(targetPath) ? targetPath : MapPath(targetPath)) + GetFileName(sourcePath), isOverWrite);
        }

        /// <summary>
        /// 移动文件到目标目录-指定新文件名
        /// </summary>
        /// <param name="sourcePath">源文件</param>
        /// <param name="targetPath">目标目录</param>
        /// <param name="newName">新文件名</param>
        public void Move(string sourcePath, string targetPath,string newName)
        {
            //如果目标目录不存在则创建
            if (!IsExist(targetPath, true))
            {
                CreateFiles(targetPath, true);
            }
            else
            {
                //如果目标目录存在同名文件则删除
                if (IsExist(Path.Combine(IsAbsolute(targetPath) ? targetPath : MapPath(targetPath), newName), false))
                {
                    DeleteFiles(Path.Combine(IsAbsolute(targetPath) ? targetPath : MapPath(targetPath), newName), false);
                }
            }

            File.Move(IsAbsolute(sourcePath) ? sourcePath : MapPath(sourcePath), Path.Combine(IsAbsolute(targetPath) ? targetPath : MapPath(targetPath), newName));
        }

        /// <summary>
        /// 移动文件到目标目录
        /// </summary>
        /// <param name="sourcePath">源文件</param>
        /// <param name="targetPath">目标目录(不包含文件名)</param>
        public void Move(string sourcePath, string targetPath)
        {
            string sourceFileName = GetFileName(sourcePath);
            //如果目标目录不存在则创建
            if (!IsExist(targetPath, true))
            {
                CreateFiles(targetPath, true);
            }
            else
            {
                //如果目标目录存在同名文件则删除
                if (IsExist(Path.Combine(IsAbsolute(targetPath) ? targetPath : MapPath(targetPath), sourceFileName), false))
                {
                    DeleteFiles(Path.Combine(IsAbsolute(targetPath) ? targetPath : MapPath(targetPath), sourceFileName), true);
                }
            }
            
            File.Move(IsAbsolute(sourcePath) ? sourcePath : MapPath(sourcePath), Path.Combine(IsAbsolute(targetPath) ? targetPath : MapPath(targetPath), sourceFileName));
        }

        /// <summary>
        /// 获取文件名和扩展名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public string GetFileName(string path)
        {
            return Path.GetFileName(IsAbsolute(path) ? path : MapPath(path));
        }

        /// <summary>
        /// 获取文件名不带扩展名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public string GetFileNameWithOutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(IsAbsolute(path) ? path : MapPath(path));
        }

        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public string GetFileExtension(string path)
        {
            return Path.GetExtension(IsAbsolute(path) ? path : MapPath(path));
        }

        /// <summary>
        /// 返回文件扩展名，不含“.” 截取文件名方式
        /// </summary>
        /// <param name="_filepath">文件全名称</param>
        /// <returns>string</returns>
        public string GetFileExtNoFile(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return "";
            }
            if (_filepath.LastIndexOf(".") > 0)
            {
                return _filepath.Substring(_filepath.LastIndexOf(".") + 1); //文件扩展名，不含“.”
            }
            return "";
        }

    }
}

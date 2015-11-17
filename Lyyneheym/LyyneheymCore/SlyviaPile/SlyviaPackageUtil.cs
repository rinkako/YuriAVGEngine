using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 包管理类：负责资源打包和解包的类
    /// 她是一个静态类
    /// </summary>
    public static class SlyviaPackageUtil
    {
        /// <summary>
        /// 把指定资源封装为一个包
        /// 她是一个静态方法
        /// </summary>
        /// <param name="fileList">一个装有待打包数据路径的向量</param>
        /// <param name="savePath">打包文件的保存路径</param>
        /// <returns>操作成功与否</returns>
        public static bool pack(List<string> fileList, string savePath)
        {
            if (fileList == null)
            {
                return false;
            }
            // 开启输出流
            StreamWriter synWriter = new StreamWriter(savePath + ".pst");
            FileStream pakFs = new FileStream(savePath, FileMode.Create);
            BinaryWriter pakBw = new BinaryWriter(pakFs);
            // 获取文件长度
            int fileEncounter = fileList.Count;
            synWriter.WriteLine(String.Format("___SlyviaLyyneheym@{0}", fileEncounter));
            // 打包文件
            FileStream fs;
            BinaryReader fbr;
            long accCounter = 0;
            //保存每张Image为byte[]
            for (int i = 0; i < fileEncounter; i++)
            {
                fs = new FileStream(fileList[i], FileMode.Open);
                fbr = new BinaryReader(fs);
                long fcount = 0;
                while (fs.Position != fs.Length)
                {
                    pakBw.Write(fbr.ReadByte());
                    fcount++;
                }

                string[] nameSplitItem = fileList[i].Split('\\');
                synWriter.WriteLine(String.Format("{0}:{1}:{2}", nameSplitItem[nameSplitItem.Length - 1], accCounter, fcount));
                accCounter += fcount;
                fs.Close();
                fs.Dispose();
            }
            synWriter.WriteLine("___SlyviaLyyneheymEOF");
            synWriter.Close();
            pakBw.Close();
            pakFs.Close();
            return true;
        }

        /// <summary>
        /// 把包中的某个资源解压出来
        /// 她是一个静态方法
        /// </summary>
        /// <param name="packFile">包路径</param>
        /// <param name="getFile">资源名称</param>
        /// <param name="saveFile">解压目标路径</param>
        /// <returns>操作成功与否</returns>
        public static bool unpack(string packFile, string getFile, string saveFile)
        {
            StreamReader synReader = new StreamReader(packFile + ".pst");
            FileStream pakFs = new FileStream(packFile, FileMode.Open);
            BinaryReader pakBr = new BinaryReader(pakFs);
            FileStream extFs = new FileStream(saveFile, FileMode.Create);
            BinaryWriter extBr = new BinaryWriter(extFs);
            string[] synHeadSplitItem = synReader.ReadLine().Split('@');
            if (synHeadSplitItem[0] != "___SlyviaLyyneheym")
            {
                return false;
            }
            int fileEncounter = Convert.ToInt32(synHeadSplitItem[1]);
            long filePointer = -1;
            long fileSize = -1;
            for (int t = 0; t < fileEncounter; t++)
            {
                string[] lineitem = synReader.ReadLine().Split(':');
                if (lineitem[0] == getFile)
                {
                    filePointer = Convert.ToInt64(lineitem[1]);
                    fileSize = Convert.ToInt64(lineitem[2]);
                    break;
                }
            }
            if (filePointer == -1 || fileSize == -1)
            {
                return false;
            }
            pakFs.Seek(filePointer, SeekOrigin.Begin);
            for (int t = 0; t < fileSize; t++)
            {
                extBr.Write(pakBr.ReadByte());
            }
            pakBr.Close();
            pakFs.Close();
            extBr.Close();
            extFs.Close();
            return true;
        }

        
    }
}

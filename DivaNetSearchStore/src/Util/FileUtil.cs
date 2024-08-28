using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DivaNetSearchStore
{
    // ファイルユーティリティクラス
    public static class FileUtil
    {
        // ファイル文字コード
        public static readonly Encoding FILE_ENCODING = Encoding.UTF8;

        /*
         * フォルダを作成
         */
        public static void createFolder(string path)
        {
            // フォルダを新規作成する
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /*
         * ファイルに書き込む
         * 
         * addFlg   true    追記
         *          false   上書き
         */
        public static void writeFile(string str, string path, Boolean addFlg)
        {
            using (StreamWriter writer = new StreamWriter(
                path,
                addFlg,             // true：追記、false：上書き
                FILE_ENCODING
            )){
                // 楽曲情報を書き込む
                writer.Write(str);
                writer.Close();
            }
        }

        /*
         * ファイルを読み込んで全文字列を返す
         * ※ ローカルのHTMLファイル用
         * 
         * addFlg   true    追記
         *          false   上書き
         */
        public static string readFile(string path)
        {
            string ret = "";

            using (StreamReader sr = new StreamReader(
                path,
                FILE_ENCODING
            ))
            {
                // 文字列を設定
                ret = sr.ReadToEnd();
                sr.Close();
            }

            return ret;
        }
    }
}

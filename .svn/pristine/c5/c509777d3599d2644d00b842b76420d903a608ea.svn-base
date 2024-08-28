using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DivaNetSearchStore
{
    // 設定情報クラス
    public class Setting
    {
        // 区切り文字
        private readonly string SEPALATOR = "\n";

        // アクセスコード
        public string accessCode { get; private set; }

        // パスワード
        public string password { get; private set; }

        // Post通信用文字列
        public Dictionary<string, string> postDatas { get; private set; }

        public string postDataStr
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in postDatas.Keys)
                {
                    sb.Append(string.Format("{0}={1}", key, postDatas[key]));
                    sb.Append("&");
                }

                string ret = sb.ToString();

                return ret.Substring(0, ret.Length - 1);
            }
        }

        public byte[] postData {
            get
            {
                return Encoding.ASCII.GetBytes(postDataStr);
            }
        }

        // クッキー情報
        public CookieContainer cc { get; set; }

        /*
         * コンストラクタ＠DIVA.NET接続時
         */
        public Setting()
        {
            this.postDatas = new Dictionary<string, string>();
            this.cc = new CookieContainer();
        }

        /*
         * コンストラクタ＠DIVA.NET接続時
         */
        public Setting(string accessCode, string password)
        {
            this.postDatas = new Dictionary<string, string>();
            this.cc = new CookieContainer();

            this.accessCode = accessCode;
            this.password = password;

            this.postDatas.Add("accessCode", accessCode);
            this.postDatas.Add("password", password);
        }

        /*
         * コンストラクタ＠ファイル読み込み時
         */
        public Setting(string[] lines)
        {
            this.postDatas = new Dictionary<string, string>();
            this.cc = new CookieContainer();

            this.accessCode = lines[0];
            this.password = lines[1];
        }

        /*
         * プレイヤー情報書き込み用
         */
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();

            ret.Append(accessCode + SEPALATOR);
            ret.Append(password + SEPALATOR);

            return ret.ToString();
        }

        /*
         * キー情報をクリアする
         */
        public void clearKey()
        {
            // 元のキー情報をクリアする
            postDatas.Clear();

            this.postDatas.Add("accessCode", accessCode);
            this.postDatas.Add("password", password);
        }

        /*
         * キー情報を設定する
         */
        public void addKey(string key, string value)
        {
            this.postDatas.Add(key, value);
        }
    }
}

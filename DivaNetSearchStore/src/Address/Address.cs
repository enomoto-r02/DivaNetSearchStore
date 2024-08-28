using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DivaNetSearchStore
{
    // 住所情報クラス
    public class Address
    {
        // 区切り文字
        private readonly string SEPALATOR = "\n";

        // 置換情報
        public Dictionary<string, string> rep { get; private set; }

        /*
         * コンストラクタ
         */
        public Address()
        {
            this.rep = new Dictionary<string, string>();
        }

        /*
         * 追加
         */
        public void Add(string tempo, string address)
        {
            rep.Add(tempo, address);
        }

        /*
         * 置換
         */
        public string Replace(string tempoName)
        {
            string ret = "";

            if(rep.ContainsKey(tempoName))
            {
                ret = rep[tempoName];
            }

            return ret;
        }
    }
}

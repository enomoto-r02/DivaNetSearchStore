﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DivaNetSearchStore
{
    public static class AddressManager
    {
        static readonly string FILE_ADDRESS = "address.txt";

        /*
         * 置き換え
         */
        public static void ReplaceAddress(Address address, ref Dictionary<int, Store> stores)
        {
            foreach(int no in stores.Keys)
            {
                Store store = stores[no];

                string repAddress = address.Replace(store.name);

                if (!string.IsNullOrEmpty(repAddress))
                {
                    store.address = repAddress;
                }
            }
        }

        /*
         * 読み込み
         */
        public static Address ReadAddress()
        {
            // 設定情報生成
            Address address = new Address();

            // ファイルを開く
            using (StreamReader sr = new StreamReader(
                FILE_ADDRESS,
                FileUtil.FILE_ENCODING
            ))
            {
                string line;

                // ファイルの末尾まで
                while ((line = sr.ReadLine()) != null)
                {
                    string[] tempo = line.Split(',');

                    address.Add(tempo[0], tempo[1]);
                }
            }

            return address;
        }
    }
}

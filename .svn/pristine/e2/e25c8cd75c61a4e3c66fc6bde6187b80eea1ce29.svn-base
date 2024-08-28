using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DivaNetSearchStore
{
    public static class SettingManager
    {
        static readonly string FILE_SETTING = "Setting.txt";

        /*
         * 読み込み
         */
        public static Setting ReadSetting()
        {

            // 設定情報生成
            Setting setting;

            // ファイルを開く
            using (StreamReader sr = new StreamReader(
                FILE_SETTING,
                FileUtil.FILE_ENCODING
            ))
            {
                // ファイルをすべて読み込む
                string fileStr = sr.ReadToEnd().Replace("\r", "").Replace("\n", "");;

                // 設定情報設定
                setting = new Setting(fileStr.Split(','));
            }

            return setting;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DivaNetSearchStore
{
    class Program
    {
        static readonly string WINDOW_TITLE = "DivaNetSearchStore";

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = WINDOW_TITLE;

            DateTime dt = DateTime.Now;

            // 設定情報読み込み
            Setting setting = SettingManager.ReadSetting();

            // 置換情報読み込み
            Address address = AddressManager.ReadAddress();

            // 店舗情報取得
            Dictionary<int, Store> stores = StoreManager.GetStore(setting, dt);

            // 店舗情報置換
            AddressManager.ReplaceAddress(address, ref stores);

            // 店舗情報出力
            StoreManager.WriteStore(stores, dt, "");

            // フォトスタジオ有のみ抽出
            Dictionary<int, Store> photoStudioStores = StoreManager.GetStorePhotoStudio(stores);

            // 店舗情報出力
            StoreManager.WriteStore(photoStudioStores, dt, "_PhotoStudio");
        }
    }
}

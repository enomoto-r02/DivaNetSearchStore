using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DivaNetSearchStore
{
    public static class WebUtil
    {
        // 置換用URL
        public static string AFTER_URL = "http://project-diva-ac.net";

        // Webページ文字コード
        private static readonly Encoding WEB_ENCODING = Encoding.UTF8;
        private static readonly string TOKEN_KEY = "org.apache.struts.taglib.html.TOKEN";

        // ログインページ
        private static readonly string URL_LOGIN = "https://project-diva-ac.net/divanet/login/";
        private static readonly string URL_NEXT_PAGE_STR = "次へ[#]";
        private static readonly string URL_BACK_PAGE_STR = "[*]前へ";
        private static readonly string URL_BACK_STR = "戻る";

        private static readonly int WAIT_TIME = 300;
        private static readonly int MAX_ERROR_CNT = 3;

        /*
         *  GET通信を行う
         * 
         * 引数       url
         * 戻り値     String配列(0:POST後のURL、1:POST後のソース文字列) 
         */
        public static string[] HttpGet(string url, Setting setting)
        {
            int errCnt = 0;

            string[] result = null;

            while(true)
            {
                try
                {
                    System.Threading.Thread.Sleep(WAIT_TIME);

                    // リクエストの作成
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse res = req.GetResponse();

                    // レスポンスの読み取り
                    Stream resStream = res.GetResponseStream();
                    StreamReader sr = new StreamReader(resStream, WEB_ENCODING);

                    result = new string[] { res.ResponseUri.ToString(), sr.ReadToEnd() };

                    sr.Close();
                    resStream.Close();

                    // 正常終了
                    break;
                }
                catch(WebException e)
                {
                    errCnt++;

                    // リトライ回数を超えた
                    if (errCnt > MAX_ERROR_CNT)
                    {
                        throw e;
                    }

                    // DIVA.NET関連ページなら
                    if (url.Contains("project-diva-ac.net/divanet"))
                    {
                        // cookieの再取得を行う
                        WebUtil.UpdateCookie(setting);
                    }

                    // リトライ
                }
            }

            return result;
        }

        /*
         *  POST通信を行う
         * 
         * 引数       url
         * 戻り値     String配列(0:POST後のURL、1:POST後のソース文字列) 
         */
        public static string[] HttpPost(string url, Setting setting)
        {
            System.Threading.Thread.Sleep(WAIT_TIME);

            // postDataのTOKENが無ければCookie無しとし、Cookieを取得する
            if (!setting.postDatas.ContainsKey(TOKEN_KEY))
            {
                WebUtil.UpdateCookie(setting);
            }

            int errCnt = 0;

            string[] result = null;

            while(true)
            {
                try
                {
                    // リクエストの作成
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = setting.postData.Length;
                    req.CookieContainer = setting.cc;

                    // ポスト・データの書き込み
                    Stream reqStream = req.GetRequestStream();
                    reqStream.Write(setting.postData, 0, setting.postData.Length);
                    reqStream.Close();

                    WebResponse res = req.GetResponse();

                    // レスポンスの読み取り
                    Stream resStream = res.GetResponseStream();
                    StreamReader sr = new StreamReader(resStream, WEB_ENCODING);

                    // 結果を設定
                    result = new string[] { res.ResponseUri.ToString(), sr.ReadToEnd() };

                    sr.Close();
                    resStream.Close();

                    if (result[1].Contains("DIVA.NETサーバへの接続を終了します。"))
                    {
                        errCnt++;

                        // リトライ回数を超えた
                        if (errCnt > MAX_ERROR_CNT)
                        {
                            throw new Exception("接続が確立できませんでした。");
                        }

                        // DIVA.NET関連ページなら
                        if (url.Contains("project-diva-ac.net/divanet"))
                        {
                            // cookieの再取得を行う
                            WebUtil.UpdateCookie(setting);
                        }

                        // リトライ
                    }
                    else
                    {
                        // 終了
                        break;
                    }
                }
                catch(WebException e)
                {
                    errCnt++;

                    // リトライ回数を超えた
                    if (errCnt > MAX_ERROR_CNT)
                    {
                        throw e;
                    }

                    // DIVA.NET関連ページなら
                    if (url.Contains("project-diva-ac.net/divanet"))
                    {
                        // cookieの再取得を行う
                        WebUtil.UpdateCookie(setting);
                    }

                    // リトライ
                }
            }

            return result;
        }

        // メンバのDIVA.NETのCookieを更新する
        public static string[] UpdateCookie(Setting setting)
        {
            System.Threading.Thread.Sleep(WAIT_TIME);

            // アクセスコード、パスワード以外のパラメータをクリアする
            setting.clearKey();
            string[] result = null;

            // TOKEN情報を除いたパラメータでログインする
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL_LOGIN);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = setting.postData.Length;
            req.CookieContainer = setting.cc;

            // ポスト・データの書き込み
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(setting.postData, 0, setting.postData.Length);
            reqStream.Close();

            WebResponse res = req.GetResponse();

            Stream resStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, WEB_ENCODING);

            // 結果を設定
            result = new string[] { res.ResponseUri.ToString(), sr.ReadToEnd() };

            sr.Close();
            resStream.Close();

            // hidden項目をパラメータに加える
            KeyValuePair<string, string> param = getHiddenParameter(result[1], "loginActionForm");
            setting.addKey(param.Key, param.Value);

            System.Threading.Thread.Sleep(WAIT_TIME);

            // リクエストの作成
            req = (HttpWebRequest)WebRequest.Create(URL_LOGIN);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = setting.postData.Length;
            req.CookieContainer = setting.cc;

            reqStream = req.GetRequestStream();
            reqStream.Write(setting.postData, 0, setting.postData.Length);
            reqStream.Close();

            res = req.GetResponse();

            // レスポンスの読み取り
            resStream = res.GetResponseStream();
            sr = new StreamReader(resStream, WEB_ENCODING);

            // 結果を設定
            result = new string[] { res.ResponseUri.ToString(), sr.ReadToEnd() };

            return result;
        }

        // formタグ内のhiddenパラメータを追加する取得する
        private static KeyValuePair<string, string> getHiddenParameter(string source, string formTagName)
        {
            KeyValuePair<string, string> ret = new KeyValuePair<string, string>("", "");

            HtmlDocument html = WebUtil.getHtmlDocument(source);

            // ページ内のtbodyタグ(各難易度)を全て取得する
            HtmlElementCollection forms = html.GetElementsByTagName("form");

            // tbodyタグ全検索
            foreach (HtmlElement form in forms)
            {
                if (form.GetAttribute("name").Equals(formTagName))
                {
                    // tbodyタグ全検索
                    foreach (HtmlElement input in form.GetElementsByTagName("input"))
                    {
                        // tbodyタグ全検索
                        if (input.GetAttribute("type").Equals("hidden"))
                        {
                            string name = input.GetAttribute("name");
                            string value = input.GetAttribute("value");

                            ret = new KeyValuePair<string, string>(name, value);
                        }
                    }
                }
            }

            return ret;
        }



        /*
         * ソース文字列からHTMLDocumentを取得する
         */
        public static HtmlDocument getHtmlDocument(string source)
        {
            HtmlDocument ret;

            // WebBrowserコントロールを作成
            WebBrowser browser = new WebBrowser();
            browser.Dock = DockStyle.Fill;
            browser.Name = "webBrowser1";
            browser.ScriptErrorsSuppressed = true;     // スクリプトエラーの警告を無効にする＠Wiki対策

            browser.Navigate("about:blank");    // 空白ページを開く
            browser.Document.OpenNew(true);     // クリア
            browser.Document.Write(source);     // 書き込み

            try
            {
                // サーバーへアクセスするわけではないので、ウェイトは控えめ
                int waitTime = 20;

                // 初回強制ウェイト
                System.Threading.Thread.Sleep(waitTime);

                // 読み込みが完了していない
                while (browser.IsBusy)
                {
                    // 再度ウェイト
                    System.Threading.Thread.Sleep(waitTime);

                    browser.Refresh();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("HTMLの読み込みに失敗しました。", MessageConst.E_MSG_ERROR_T);

                throw;
            }

            ret = browser.Document;

            // 解放
            browser.Dispose();

            return ret;
        }

        /*
         * HtmlDocumentで読み込んだURLを修正する
         */
        public static string convUrlToHtml(string url)
        {
            if (url.StartsWith("about:blank"))
            {
                return url.Replace("about:blank", AFTER_URL);
            }
            else if (url.StartsWith("about:"))
            {
                return url.Replace("about:", AFTER_URL);
            }

            // 外部リンク等は変換されない？ そのまま返す
            return url;
        }

        /*
         * ページ内のリンクのURLと文字列を取得
         */
        public static void getLinkUrlStrOnePage(ref Dictionary<string, string> ret, string source, string searchUrl)
        {
            HtmlDocument html = WebUtil.getHtmlDocument(source);

            // リンクを全て取得する
            for (int i = 0; i < html.Links.Count; i++)
            {
                // URL形式変更
                string url = WebUtil.convUrlToHtml(html.Links[i].GetAttribute("href"));
                string value = html.Links[i].InnerText;

                // 詳細ページを取得
                if (url.StartsWith(searchUrl))
                {
                    // 文字列がページ遷移のURLは取得しない("次へ"リンクとの重複対策)
                    MatchCollection mc = Regex.Matches(html.Links[i].InnerText, @"^[0-9]+$");

                    if(1 > mc.Count)
                    {
                        ret.Add(url, value);
                    }
                }
            }
        }

        /*
         * 改ページを含むリンクのURLと文字列を取得
         */
        public static void getLinkUrlStrAllPage(Setting setting, ref Dictionary<string, string> ret, string source, string searchUrl)
        {
            HtmlDocument html = WebUtil.getHtmlDocument(source);

            // リンクを全て取得する
            for (int i = 0; i < html.Links.Count; i++)
            {
                // URL形式変更
                string url = WebUtil.convUrlToHtml(html.Links[i].GetAttribute("href"));
                string value = html.Links[i].InnerText;

                // 詳細ページを取得(戻るページなどは含まない)
                if (url.StartsWith(searchUrl) && !value.Contains(URL_NEXT_PAGE_STR) 
                    && !value.Contains(URL_BACK_PAGE_STR) && !value.Contains(URL_BACK_STR))
                {
                    // 文字列がページ遷移のURLは取得しない("次へ"リンクとの重複対策)
                    MatchCollection mc = Regex.Matches(html.Links[i].InnerText, @"^[0-9]+$");

                    if (1 > mc.Count)
                    {
                        ret.Add(url, value);
                    }
                }
            }

            // リンクを全て取得する
            for (int i = 0; i < html.Links.Count; i++)
            {
                // URL形式変更
                string url = WebUtil.convUrlToHtml(html.Links[i].GetAttribute("href"));
                string value = html.Links[i].InnerText;

                // 次へリンクを取得
                if (value.StartsWith(URL_NEXT_PAGE_STR))
                {
                    string[] page = HttpPost(url, setting);
                    getLinkUrlStrAllPage(setting, ref ret, page[1], searchUrl);
                }
            }
        }

        /*
         * innterTextの改行、トリム処理を行う
         */
        public static string[] getInnerTextReplace(string innerText)
        {
            string[] tmpArray = innerText.Replace("\r\n", "\n").Split('\n');

            string[] retArray = new string[tmpArray.Length];

            for (int i = 0; i < tmpArray.Length; i++)
            {
                retArray[i] = tmpArray[i].Trim();
            }

            return retArray;
        }

        /*
         * 正規表現に最初にマッチする文字列を取得する
         */
        public static string getMache(string target, string pattern)
        {
            string ret = "";

            // null回避
            if (target == null)
            {
                return ret;
            }

            MatchCollection mc = Regex.Matches(target, pattern);

            // ヒットした
            if (mc.Count > 0)
            {
                ret = mc[0].Value;
            }

            return ret;
        }

        /*
         * DIVA.NET用の汎用メソッド
         * 　DIVA.NETのInnerTextから
         * 　"[xxx]"で始まる要素ごとに振り分ける
         */
        public static Dictionary<string, List<string>> getDivaNetKomoku(string[] innerTexts)
        {
            // 要素ごとに振り分け
            Dictionary<string, List<string>> innerElems = new Dictionary<string, List<string>>();

            bool bSearchEnd = false;
            List<string> elems = new List<string>();
            string title = "";

            for (int i = 0; i < innerTexts.Length; i++)
            {
                string titleElement = WebUtil.getMache(innerTexts[i].Trim(), "^\\[.*?\\].*");

                // "[(文字列)]"を含む行
                if (!string.IsNullOrEmpty(titleElement))
                {
                    if (bSearchEnd)
                    {
                        // 検索終了
                        bSearchEnd = false;

                        // Listに追加
                        innerElems.Add(title, elems);

                        // 初期化
                        elems = new List<string>();
                        title = "";
                    }
                    if (!bSearchEnd)
                    {
                        // 検索開始
                        bSearchEnd = true;

                        title = titleElement;
                        elems.Add(innerTexts[i]);
                    }
                }
                else
                {
                    if (bSearchEnd)
                    {
                        // 要素を追加
                        if (!string.IsNullOrEmpty(innerTexts[i].Trim()))
                        {
                            elems.Add(innerTexts[i]);
                        }

                    }
                }
            }

            return innerElems;
        }

        /*
         * UTF-8→SJISの文字化け対策＠暫定;
         */
        public static string replaceUtf8(string str)
        {
            string ret = str;

            ret = ret.Replace("〜", "～");
            ret = ret.Replace("−", "-");
            ret = ret.Replace("−", "-");
            
            return ret;
        }

        /*
         * UTF-8→SJISの文字化け対策＠暫定;
         */
        public static string replaceUtf8Address(string str)
        {
            string ret = replaceUtf8(str);

            ret = ret.Replace("０", "0");
            ret = ret.Replace("１", "1");
            ret = ret.Replace("２", "2");
            ret = ret.Replace("３", "3");
            ret = ret.Replace("４", "4");
            ret = ret.Replace("５", "5");
            ret = ret.Replace("６", "6");
            ret = ret.Replace("７", "7");
            ret = ret.Replace("８", "8");
            ret = ret.Replace("９", "9");

            return ret;
        }
    }
}
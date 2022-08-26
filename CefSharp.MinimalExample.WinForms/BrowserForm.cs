// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp.DevTools.IO;
using CefSharp.MinimalExample.WinForms.Controls;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CefSharp.MinimalExample.WinForms
{
    public partial class BrowserForm : Form
    {
#if DEBUG
        private const string Build = "Debug";
#else
        private const string Build = "Release";
#endif
        private readonly string title = "CefSharp.MinimalExample.WinForms (" + Build + ")";
        private readonly ChromiumWebBrowser browser;

        public BrowserForm()
        {
            InitializeComponent();

            Text = title;
            WindowState = FormWindowState.Maximized;

            browser = new ChromiumWebBrowser("www.ic.net.cn");
            browser.MenuHandler = new MenuHandler();
            toolStripContainer.ContentPanel.Controls.Add(browser);

            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            browser.LoadingStateChanged += OnLoadingStateChanged;
            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;
            browser.LoadError += OnBrowserLoadError;

            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
               Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

#if NETCOREAPP
            // .NET Core
            var environment = string.Format("Environment: {0}, Runtime: {1}",
                System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
                System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
#else
            // .NET Framework
            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var environment = String.Format("Environment: {0}", bitness);
#endif

            DisplayOutput(string.Format("{0}, {1}", version, environment));
        }

        private void OnBrowserLoadError(object sender, LoadErrorEventArgs e)
        {
            //Actions that trigger a download will raise an aborted error.
            //Aborted is generally safe to ignore
            if (e.ErrorCode == CefErrorCode.Aborted)
            {
                return;
            }

            var errorHtml = string.Format("<html><body><h2>Failed to load URL {0} with error {1} ({2}).</h2></body></html>",
                                              e.FailedUrl, e.ErrorText, e.ErrorCode);

            _ = e.Browser.SetMainFrameDocumentContentAsync(errorHtml);

            //AddressChanged isn't called for failed Urls so we need to manually update the Url TextBox
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = e.FailedUrl);
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs e)
        {
            var b = ((ChromiumWebBrowser)sender);

            this.InvokeOnUiThreadIfRequired(() => b.Focus());
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = title + " - " + args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";
            goButton.Image = isLoading ?
                Properties.Resources.nav_plain_red :
                Properties.Resources.nav_plain_green;

            HandleToolStripLayout();
        }

        public void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }

        private void BrowserForm_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var js = @"
//将获取的独立的数据项组装成数组的一条记录返回
function GetData(result_supply, product_number, result_factory, result_batchNumber, result_totalNumber, result_pakaging, result_prompt, result_date, qqs, icon_xianHuo, vip) {
    var Q1 = '', Q2 = '';
    if (qqs && qqs.length > 0) {
        Q1 = qqs[0];
    }
    if (qqs && qqs.length > 1) {
        Q2 = qqs[1];
    }
    return {
        'resultSupply': result_supply,
        'productNumber': product_number,
        'resultFactory': result_factory,
        'resultBatchNumber': result_batchNumber,
        'resultTotalNumber': result_totalNumber,
        'resultPakaging': result_pakaging,
        'resultPrompt':result_prompt,
        'resultDate': result_date,
        'QQ1': Q1,
        'QQ2': Q2,
        'iconXianHuo': icon_xianHuo,
        'VIP': vip
    };
}

//获取一页查询网页所需信息，通过函数GetData()组装成数组，并转换为JSON数据返回给C#
function ic_net_cn() {

    //定义数组arrDataResults,保存从每一页中截取的信息
    var arrDataResults = [];
    //获取'#resultList'层下所有li列表内容，存入resultList数组
    var resultList = document.querySelectorAll('#resultList > li');
    if(resultList.length==0)
    {
        //提示：先（注册）登录、进入产品搜索结果页面，然后点击按钮“收集信息”
        alert('请（注册）登录ic.cn.net网站，然后搜索指定型号产品，在搜索结果页面运行“收集信息”，否则，可能会出错！');
        return '';
    }
 
    //遍历resultList数组
    for (var i = 0; i < resultList.length; i++) {
        try {
            //根据每个列表项前是否存在'input.result_ck',判断是否为有用项
            if (!resultList[i].querySelector('input.result_ckb')) {
                continue;
            }

            //读取每个列表项中，需要信息项
            var result_supply = resultList[i].querySelector('div.result_supply').innerText;              //1供货商
            var product_number = resultList[i].querySelector('span.product_number').innerText;           //2型号
            var result_factory = resultList[i].querySelector('div.result_factory').innerText;            //3厂商
            var result_batchNumber = resultList[i].querySelector('div.result_batchNumber').innerText;    //4批号

            var result_totalNumbers = resultList[i].querySelectorAll('div.result_totalNumber') //数量集数组
            //遍历数量集数组result_totalNumbers
            for (var j = 0; j < result_totalNumbers.length; j++) {
                if (result_totalNumbers[j].offsetParent) {
                    var result_totalNumber = result_totalNumbers[j].innerText;                           //5数量
                    break;
                }
            }
            var result_pakaging = resultList[i].querySelector('div.result_pakaging').innerText;          //6封装

            var result_prompt = resultList[i].querySelector('div.result_prompt').innerText.replace('\n', ' '); //7说明

            var result_date = resultList[i].querySelector('div.result_date').innerText;                  //8日期

            var result_askPrices = resultList[i].querySelectorAll('div.result_askPrice > a.qqicon');    
            var qqs = [];                                                                               //9QQ数组
            for (var x = 0; x < result_askPrices.length; x++) {
                if (result_askPrices[x].offsetParent) {
                    qqs.push(result_askPrices[x].title);
                }
            }
			var icon_xianHuo='';
            var icon_xianHuoX = resultList[i].querySelector('div.result_id > a > span.icon_xianHuo');
			if(icon_xianHuoX&&icon_xianHuoX.title) icon_xianHuo=icon_xianHuoX.title;    //10现货排名

            var vip = '';                                                                                     //11vip
            if (resultList[i].querySelector('p.result_icons>a.sscp')) {
                vip = 'sscp';
            }

            arrDataResults.push(GetData(result_supply, product_number, result_factory, result_batchNumber, result_totalNumber, result_pakaging, result_prompt, result_date, qqs, icon_xianHuo, vip));
        }
        catch (e)
        {
        }
    }


    //切换到下一页（获取前3页信息就可以）
    var pageNo = document.querySelector('#searchForm > div > div.right_results > div:nth-child(1) > div.pagepicker > li.current > a').innerText;
    if (parseInt(pageNo) < 3) {
        pageTo(parseInt(pageNo) + 1);
    }

    //将数组arrDataResults格式化成JSON串后返回
    return JSON.stringify(arrDataResults);
}

//执行函数ic_net_cn()
ic_net_cn();

";
                //调用onJs()执行js代码，获取字符串
                //var s = browser.onJs(js).Result;

                //调用evaluateScriptSync()方法,执行js代码，获取网页信息
                var task = browser.EvaluateScriptAsync(js);
                task.ContinueWith((respA) =>
                {
                    if (!respA.IsFaulted)
                    {
                        string s = task.Result.Result?.ToString();
                        MessageBox.Show(s);
                    }
                    return respA.Result.Result;
                }).Wait();

                var result = "";
                int count = browser.GetBrowser().GetFrameNames().Count;
                for (int i = 0; i <count; i++)
                {
                    var frm = browser.GetBrowser().GetFrame(browser.GetBrowser().GetFrameNames()[i]);

                    var task01 =frm.EvaluateScriptAsync(js);
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    if (response?.Result != null && response.Result.GetType().Name == "ExpandoObject")
                                    {
                                        result = Newtonsoft.Json.JsonConvert.SerializeObject(response.Result);
                                        ;
                                    }
                                    else
                                    {
                                        string resultStr = response.Result.ToString();
                                        result = resultStr;
                                    }
                                }
                            }
                             
                        }
                    }).Wait();
                    if (string.IsNullOrEmpty(result) == false)
                    {
                        break;
                    }
                }
                //



                //tb_log.AppendText(s);
            }
            catch (Exception)
            {

            }

        }

        private void dontDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
             
        }
    }

}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Html.Parser;
using LowUtilities;

namespace MintManger_NET
{
    public partial class Form1 : Form
    {
        Thread thread;
        bool isAsync = true;
        public bool quit = false;
        List<string> _letters = new List<string>()
        {
            "%22",
            "%23",
            "%28",
            "%2B",
            "-",
            ".",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "%3A",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "%5B",
            "%C2%AB",
            "%C2%BF",
            "А"
        };
        Net net = new Net("https://mintmanga.live/list?sortType=name");
        public Form1()
        {
            InitializeComponent();
            Multithreading.SetMaxTaskCount(10);
            net.Send("");
        }
        private void GetAllManga()
        {
            foreach (string letter in _letters)
            {
                net.ChangeURL("https://mintmanga.live/list?letter=" + letter + "&sortType=name");
                bool tmp_b = false;
                while (!tmp_b && !quit)
                {
                    tmp_b = net.Send("");
                    if(!tmp_b)
                    {
                        net.ChangeURL("https://mintmanga.live/list?letter=" + letter + "&sortType=name");
                    }
                    Thread.Sleep(100);
                }
                if (quit) return;
                MangaParser();
            }
        }
        private void SearchManga(string search)
        {
            net = new Net("https://mintmanga.live/search", Net.HTTPMethod.POST);
            bool tmp_b = false;
            while (!tmp_b && !quit)
            {
                tmp_b = net.Send("q=" + net.CyrillicToURL(search));
                if (!tmp_b)
                {
                    net.ChangeURL("https://mintmanga.live/search", Net.HTTPMethod.POST);
                }
            }
            if (quit) return;
            MangaParser();
        }
        private void reloadBtn_Click(object sender, EventArgs e)
        {
            if (thread != null)
            {
                if (thread.IsAlive)
                {
                    quit = true;
                    thread.Join();
                    quit = false;
                }
            }
            reloadBtn.Enabled = false; 
            progressBar1.Visible = true;
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            flowLayoutPanel1.Controls.Clear();
            thread = new Thread(() => GetAllManga());
            thread.Start();
        }

        public void MangaParser()
        {
            if (isAsync)
            {
                string tmp_string = net.GetResponse();
                HtmlParser html = new HtmlParser();
                var doc = html.ParseDocument(tmp_string);
                var collection = doc.QuerySelectorAll("div.tile.col-sm-6");
                CheckInvoke(() =>
                {
                    progressBar1.Maximum = collection.Count();
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;
                });
                foreach (var element in collection)
                {
                    var img = element.QuerySelector("img");
                    string tmp_imgPath = "";
                    if (img != null) tmp_imgPath = img.GetAttribute("data-original");
                    else tmp_imgPath = null;
                    var desc = element.QuerySelector("div.desc");
                    var title = desc.QuerySelector("h4");
                    var origTitleH3 = desc.QuerySelector("h3");
                    var origTitle = origTitleH3.QuerySelector("a");
                    string origTitleText = "";
                    if (origTitle == null) origTitleText = "";
                    else origTitleText = origTitle.TextContent;
                    string titleText = "";
                    if (title == null) titleText = "";
                    else titleText = title.TextContent;
                    var titles = origTitleText + " - " + titleText;
                    MangaControl mc = new MangaControl(tmp_imgPath, titles/*GetAttribute("title")*/, element.QuerySelector("a.non-hover").GetAttribute("href"));
                    mc.Tag = element.QuerySelector("a").GetAttribute("href");
                    /*if(InvokeRequired)
                    {
                        Invoke(new MethodInvoker(() => { panel.Controls.Add(mc); }));
                    } else
                    {
                        panel.Controls.Add(mc);
                    }
                    progressBar1.Value++;*/
                    if (quit) return;
                    CheckInvoke(() =>
                    {
                        mc.DoubleClick += MangaDoubleClick;
                        flowLayoutPanel1.Controls.Add(mc);
                        if (progressBar1.Value > progressBar1.Maximum)
                            progressBar1.Value = progressBar1.Maximum;
                        else
                            progressBar1.Value++;
                    });
                    Thread.Sleep(0);
                }
                CheckInvoke(() =>
                {
                    reloadBtn.Enabled = true;
                    progressBar1.Visible = false;
                });
            }
            else
            {
                string tmp_string = net.GetResponse();
                HtmlParser html = new HtmlParser();
                var doc = html.ParseDocument(tmp_string);
                var collection = doc.QuerySelectorAll("div.tile.col-sm-6");
                CheckInvoke(() =>
                {
                    progressBar1.Maximum = collection.Count();
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;
                });
                foreach (var element in collection)
                {
                    var img = element.QuerySelector("img");
                    string tmp_imgPath = "";
                    if (img != null) tmp_imgPath = img.GetAttribute("data-original");
                    else tmp_imgPath = null;
                    Image tmp_img = null;
                    if (tmp_imgPath != null)
                    {
                        tmp_img = net.DownloadImage(tmp_imgPath);
                        net.GetFolder(tmp_imgPath);
                    }
                    else
                    {
                        tmp_img = new Bitmap(1, 1);
                    }
                    var desc = element.QuerySelector("div.desc");
                    var title = desc.QuerySelector("h4");
                    var origTitleH3 = desc.QuerySelector("h3");
                    var origTitle = origTitleH3.QuerySelector("a");
                    string origTitleText = "";
                    if (origTitle == null) origTitleText = "";
                    else origTitleText = origTitle.TextContent;
                    string titleText = "";
                    if (title == null) titleText = "";
                    else titleText = title.TextContent;
                    var titles = origTitleText + " - " + titleText;
                    MangaControl mc = new MangaControl(tmp_img, titles/*GetAttribute("title")*/, element.QuerySelector("a.non-hover").GetAttribute("href"));
                    mc.Tag = element.QuerySelector("a").GetAttribute("href");
                    /*if(InvokeRequired)
                    {
                        Invoke(new MethodInvoker(() => { panel.Controls.Add(mc); }));
                    } else
                    {
                        panel.Controls.Add(mc);
                    }
                    progressBar1.Value++;*/
                    if (quit) return;
                    CheckInvoke(() =>
                    {
                        mc.DoubleClick += MangaDoubleClick;
                        flowLayoutPanel1.Controls.Add(mc);
                        if (progressBar1.Value > progressBar1.Maximum)
                            progressBar1.Value = progressBar1.Maximum;
                        else
                            progressBar1.Value++;
                    });
                }
                CheckInvoke(() =>
                {
                    reloadBtn.Enabled = true;
                    progressBar1.Visible = false;
                });
            }
        }
        private void GetChapters(string mangaPath)
        {
            flowLayoutPanel1.Controls.Clear();
            bool tmp_b = false;
            while (!tmp_b)
            {
                net = new Net(mangaPath);
                tmp_b = net.Send("");
            }
            string tmp_string = net.GetResponse();
            Debug.WriteLine(tmp_string);
            HtmlParser html = new HtmlParser();
            var doc = html.ParseDocument(tmp_string);
            var table = doc.QuerySelector("table.table.table-hover");
            var collection = table.QuerySelector("tbody").QuerySelectorAll("tr");
            CheckInvoke(() => {
                progressBar1.Maximum = collection.Count();
                progressBar1.Value = 0;
                progressBar1.Visible = true;
            });
            foreach (var element in collection)
            {
                var link = new LinkLabel();
                link.Text = element.QuerySelector("a").GetAttribute("href");
                link.Tag = "https://mintmanga.live" + element.QuerySelector("a").GetAttribute("href") + "?mtr=1";
                link.Click += (object sender, EventArgs e) => { GetChapter((sender as Control).Tag.ToString()); };
                link.AutoSize = true;
                CheckInvoke(() => {
                    flowLayoutPanel1.Controls.Add(link);
                    progressBar1.Value++;
                    progressBar1.Visible = true;
                });
            }
            CheckInvoke(() => {
                reloadBtn.Visible = true;
                progressBar1.Visible = false;
            });
        }
        private void GetChapter(string volumePath)
        {
            flowLayoutPanel1.Controls.Clear();
            bool tmp_b = false;
            while (!tmp_b)
            {
                net = new Net(volumePath);
                tmp_b = net.Send("");
            }
            string tmp_string = net.GetResponse();
            File.WriteAllText("html.txt", tmp_string);
            HtmlParser html = new HtmlParser();
            var doc = html.ParseDocument(tmp_string);
            var scripts = doc.Scripts;
            CheckInvoke(() => {
                progressBar1.Maximum = scripts.Count();
                progressBar1.Value = 0;
                progressBar1.Visible = true;
            });
            foreach (var script in scripts)
            {
                if (script.TextContent.Contains("rm_h.init"))
                {
                    var list = GetChapterImagesToDownload(script.TextContent);
                    foreach(string url in list)
                    {
                        PictureBox pb = new PictureBox();
                        pb.SizeMode = PictureBoxSizeMode.AutoSize;
                        Net nets = new Net(url);
                        nets.OnDownloadComplete += (image) => { 
                            if(image != null) CheckInvoke(() => { pb.Image = (Image)image; });
                                };
                        nets.DownloadImageAsync(url);
                        flowLayoutPanel1.Controls.Add(pb);
                    }
                }
            }
            CheckInvoke(() => {
                reloadBtn.Visible = true;
                progressBar1.Visible = false;
            });
        }
        private void MangaDoubleClick(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control.Tag != null)
            {
                flowLayoutPanel1.Controls.Clear();
                /*var pic = new PictureBox();
                //pic.Image = ;
                pic.Dock = DockStyle.Fill;
                pic.SizeMode = PictureBoxSizeMode.Zoom;*/
                GetChapters("https://mintmanga.live/" + control.Tag.ToString());
            }
        }
        private List<string> GetChapterImagesToDownload(string script)
        {
            try
            {
                var tmp_i1 = script.IndexOf("rm_h.init");
                var tmp_string = script.Substring(tmp_i1);
                tmp_i1 = tmp_string.IndexOf("[[") + 2;
                var tmp_i2 = tmp_string.IndexOf("]]");
                tmp_string = tmp_string.Substring(tmp_i1, tmp_i2 - tmp_i1);
                var tmp_strings = tmp_string.Split("],[");
                var tmp_stringsf = new List<string>();
                foreach (var tmp_s in tmp_strings)
                {
                    var strings = tmp_s.Split(',');
                    strings[0] = strings[0].Replace("\'", "");
                    strings[0] = strings[0].Replace("\"", "");
                    strings[2] = strings[2].Replace("\'", "");
                    strings[2] = strings[2].Replace("\"", "");
                    tmp_stringsf.Add(strings[0] + strings[2]);
                }
                return tmp_stringsf;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        private void CheckInvoke(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => { action(); }));
            }
            else
            {
                action();
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (thread != null)
                {
                    if (thread.IsAlive)
                    {
                        quit = true;
                        thread.Join();
                    }
                }
                quit = false;
                reloadBtn.Enabled = false;
                progressBar1.Visible = true;
                progressBar1.Maximum = 0;
                progressBar1.Value = 0;
                flowLayoutPanel1.Controls.Clear();
                thread = new Thread(() => SearchManga(searchBox.Text));
                thread.Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            quit = true;
            if(thread != null) if(thread.IsAlive) thread.Join();
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MintManger_NET
{
    public partial class MangaControl : UserControl
    {
        public Image image { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public MangaControl()
        {
            image = null;
            Title = "Cat is fine too desu~";
            InitializeComponent();
        }
        public MangaControl(Image img, string title, string link)
        {
            InitializeComponent();
            image = img;
            Title = title;
            Link = link;
        }

        public MangaControl(string imgUrl, string title, string link)
        {
            InitializeComponent();
            image = null;
            Title = title;
            Link = link;
        }

        private void MangaControl_Paint(object sender, PaintEventArgs e)
        {
            Size = new Size(Parent.ClientSize.Width - 5, Size.Height);
            if (image != null) imageBox.Image = image;
            titleLabel.Text = Title;
        }
    }
}

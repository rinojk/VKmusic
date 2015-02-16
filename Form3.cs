using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VKmusic
{
	public partial class Form3 : Form
	{
		public Form3()
		{
			InitializeComponent();
		}

		private void Form3_Load(object sender, EventArgs e)
		{
			webBrowser1.Navigate("https://oauth.vk.com/authorize?client_id=4713732&scope=audio,offline&redirect_uri=https://oauth.vk.com/blank.html&display=popup&v=5.27&response_type=token");

		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			statusStrip1.Text = "Completed";
			try
			{
				string url = webBrowser1.Url.ToString();
				string str = url.Split('#')[1];
				if (str[0] == 'a')
				{
					Settings1.Default.token = url.Split('&')[0].Split('=')[1];
					Settings1.Default.id = url.Split('=')[3];
					Settings1.Default.auth = true;
					//MessageBox.Show(Settings1.Default.token + "  " + Settings1.Default.id + "  " + Settings1.Default.auth);
					this.Close();
				}
			}
			catch (Exception ex)
			{

			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;


namespace VKmusic
{
	public partial class mainForm : Form
	{
		public mainForm()
		{
			InitializeComponent();
		}

		string answer;

		public List<ToM3U> song = new List<ToM3U>();

		private void Form1_Load(object sender, EventArgs e)
		{
			if(Settings1.Default.token==string.Empty)new Form3().Show();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			 Search();
		}

		private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			try
			{
				axWindowsMediaPlayer1.URL = @listBox1.SelectedValue.ToString();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Oops! Error occured...\nError message: " + ex.Message);
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				song.Add(new ToM3U()
				         {
					         Name = listBox1.Text,
					         Url = listBox1.SelectedValue.ToString()
				         });
				listBox2.Items.Clear();
				foreach (var m in song)
				{
					listBox2.Items.Add(m.Name);
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show("Oops! Error occured...\nError message: " + ex.Message);
			}


		}

		private string stringParse(string source, string i)
		{
			try
			{
				string result = string.Empty;
				int pos = 0;
				while (i[pos] != ',')
				{
					if (pos == 0)
						pos = i.IndexOf(source) + source.Length + 1;
					result += i[pos];
					pos++;
					if (pos == i.Length)
						break;
				}

				return result;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Oops! Error occured...\nError message: " + ex.Message);
				return string.Empty;
			}
		}

        WebClient web = new WebClient();

		private void Search()
		{
			try
			{

				

				string url = "https://api.vk.com/method/audio.search?q=" + textBox1.Text + "&v=5.27&count=300&access_token=" +
							 Settings1.Default.token;


                try
                {
                    answer = web.DownloadString(url);
                }
                finally
                {
                    if (web != null)
                    {
                        web.Dispose();
                    }
                }

				byte[] bytes = Encoding.Default.GetBytes(answer);
				answer = Encoding.UTF8.GetString(bytes);
				answer = answer.Replace("\"", "");

				if (answer.IndexOf("count:0") != -1)
				{
					MessageBox.Show("No results found");
				} 
				else 
				{ 
					List<Audio> songs = new List<Audio>();

					
					answer = answer.Replace("]}}", "");
					answer = answer.Remove(0, answer.IndexOf("{id:"));
					answer = answer.Remove(0, 1);
					answer = answer.Remove(answer.Length - 1, 1);
					string[] strSeparator = new string[]
					                        {
						                        "},{"
					                        };
					string[] str = answer.Split(strSeparator, StringSplitOptions.RemoveEmptyEntries);
					foreach (string i in str)
					{
						songs.Add(new Audio()
						{
							Name = stringParse("artist", i) + " - " + stringParse("title", i),
							Url = stringParse("url", i).Replace(@"\", "")
						});
					}

					listBox1.DataSource = songs;
					listBox1.DisplayMember = "Name";
					listBox1.ValueMember = "Url";

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Oops! Error occured...\nError message: " + ex.Message);
			}
		}


		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Return)
			{
				Search();
			}
        }

	   
        public void Download_and_Create_Click(object sender, EventArgs e)
        {
            //WebClient client = new WebClient();
            StreamWriter writeText;
            string playlistName = textBox2.Text + @"\" + "VK" + DateTime.Now.Year.ToString() + "_" +
                              DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" +
                              DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" +
                              DateTime.Now.Second.ToString() + ".m3u";
            FileInfo file = new FileInfo(@playlistName);
            writeText = file.AppendText();
            foreach (var i in song)
            {
                toolStripStatusLabel1.Text = "Downloading " + i.Name;
                string songName = @"temp\" + i.Name + ".mp3";

                /*web.Dispose();
                web.DownloadFile(i.Url, songName);//DownloadFile(i.Url, );
                writeText.WriteLine(System.Reflection.Assembly.GetEntryAssembly().Location + @"\temp" + i.Name + ".mp3");*/
                //WebClient webClient;
                /*using (webClient = new WebClient())
                {
                    webClient.DownloadFile(i.Url, songName);
                }*/

                byte[] bytes = Encoding.UTF8.GetBytes(Directory.GetCurrentDirectory() + @"\temp\" + i.Name + ".mp3");
                writeText.WriteLine(Encoding.UTF8.GetString(bytes));
                WebClient webClient = new WebClient();
                try
                {
                    webClient.DownloadFile(i.Url, songName);
                }
                finally
                {
                    if (webClient != null)
                    {
                        webClient.Dispose();
                    }
                }
                
            }
            writeText.Close();
            toolStripStatusLabel1.Text = "Completed!";
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = folderBrowserDialog1.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops! Error occured...\nError message: " + ex.Message);
            }
        }
	}

	class Audio
	{
		public string Name
		{
			get;
			set;
		}
		public string Url
		{
			get;
			set;
		}

	}

    public class ToM3U
	{
		public string Name
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}
	}
}

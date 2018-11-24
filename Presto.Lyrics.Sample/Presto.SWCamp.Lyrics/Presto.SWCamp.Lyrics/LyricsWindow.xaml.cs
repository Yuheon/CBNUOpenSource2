﻿using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {
        string[] lines; /*= File.ReadAllLines(@"C:\Users\syh04\Desktop\Presto.Lyrics.Sample\Musics\볼빨간사춘기 - 여행.lrc");*/
        string[] lyrics;//출력할 가사
        double[] times;//가사 시간
        public LyricsWindow()
        {
            InitializeComponent();
            lines = File.ReadAllLines(@"C:\Users\syh04\Desktop\Presto.Lyrics.Sample\Musics\01. サイレントマジョリティー.lrc");
            lyrics = new string[lines.Length];
            len.Text = lines.Length.ToString();
            times = new double[lines.Length];
            for (int i = 3; i < lines.Length; i++)
            {
                var splitData = lines[i].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(),
                   @"mm\:ss\.ff", CultureInfo.InvariantCulture);//시간
                times[i] = time.TotalMilliseconds;
                             
                lyrics[i] = splitData[splitData.Length-1];//]처리

            }

            //for (int i = 3; i < lines.Length; i++)
            //{
            //    var splitData = lines[i].Split(']');
            //    var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(),
            //       @"mm\:ss\.ff", CultureInfo.InvariantCulture);
            //    //textLyrics.Text = time.ToString();
            //    MessageBox.Show(time.TotalMilliseconds.ToString());


            //}
            //foreach (var line in lines)
            //{
            //    var datas = line.Split(']');

            //}
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        
        private int Trans(TimeSpan a)
        {
            int result = 0;
            result += a.Seconds * 1000;
            result += a.Milliseconds;
            result += a.Minutes * 60 * 1000;
            return result;
        }
        private int Searchbin(double[] lintime, double ltime)//가사들 시간, 현재 시간 인자로 받음, 이진탐색
        {
            int left, mid, right;
            left = 3; right = lintime.Length - 1; mid = (left + right) / 2;//0,1,2는 곡정보라서 3~lin.Length-1
            while (left < right)
            {
                if (ltime < lintime[mid])
                {
                    right = mid - 1;
                    mid = (left + right) / 2;
                }
                else if (ltime > lintime[mid])
                {
                    left = mid + 1;
                    mid = (left + right) / 2;
                }
                else
                {
                    return mid;
                }
            }

            return left - 1;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            TimeSpan a = TimeSpan.FromMilliseconds(PrestoSDK.PrestoService.Player.Position);

            int i = Searchbin(times, Trans(a));
            
            tt.Text = times[i].ToString();
            TimeSpan b = TimeSpan.FromMilliseconds(times[i]);
            min.Text = b.Minutes.ToString();
            sec.Text = b.Seconds.ToString();
            mili.Text = b.Milliseconds.ToString();
            t.Text = PrestoSDK.PrestoService.Player.Position.ToString();
            //int i=Searchbin(times, PrestoSDK.PrestoService.Player.Position);


            string str="";
            int j = i - 5;
            int max = i + 5;
            if (j - 5 < 0)
                j = 0;
            if (max >= lines.Length)
                max = lines.Length - 1;
            for(;j<max; j++)
                {
                    if(times[j]==times[i])
                    {
                        str += lyrics[j]+"\r\n";
                    }
                }
            textLyrics.Text = str;
            //while (true)//시간 같은 것들 출력
            //{

            //    if (times[i] == times[++i])
            //        textLyrics.Text += lyrics[i] + "\r\n";
            //    else
            //        break;
            //}
        }
    }
}
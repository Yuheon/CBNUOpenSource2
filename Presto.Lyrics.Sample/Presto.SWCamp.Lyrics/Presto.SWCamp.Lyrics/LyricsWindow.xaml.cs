using Presto.SDK;
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
        string[] lines;//가사저장
        string[] lyrics;//출력할 가사
        double[] times;//가사 시간
        public LyricsWindow()
        {
            InitializeComponent();
            lines = File.ReadAllLines(@"C:\Users\syh04\Desktop\Presto.Lyrics.Sample\Musics\숀 (SHAUN) - Way Back Home.lrc");
            for(int i=0;i<2;i++)
            {
                var splitData = lines[i].Split(']',':');
                lines[i] = splitData[1];
            }
            title_Singer_info.Text = lines[1] + "\r\n-" + lines[0];//title singer 출력
            lyrics = new string[lines.Length];
            times = new double[lines.Length];
            for (int i = 3; i < lines.Length; i++)
            {
                var splitData = lines[i].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(),
                   @"mm\:ss\.ff", CultureInfo.InvariantCulture);//시간
                times[i] = time.TotalMilliseconds;

                lyrics[i] = splitData[splitData.Length - 1];//]처리

            }

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        //private int Trans(TimeSpan a)//time double형으로 변환->TotalMilliseconds로 대체
        //{
        //    int result = 0;
        //    result += a.Seconds * 1000;
        //    result += a.Milliseconds;
        //    result += a.Minutes * 60 * 1000;
        //    return result;
        //}
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
                    return mid + 1;
                }
            }

            //ltime이 정확히 lintime 에 있는 것이아니라
            //ltime좌, 우에 있기 때문에 조건 달아줌.
            if (ltime < lintime[left])
                return left - 1;
            else
                return left;
            
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            TimeSpan a = TimeSpan.FromMilliseconds(PrestoSDK.PrestoService.Player.Position);

            int i = Searchbin(times, a.TotalMilliseconds);//출력할 가사 찾음

            //시간 같은 것들 한번에 출력
            string str = "\r\n";
            int j = i - 5;
            int count = 0;
            int max = i + 5;
            if (j - 5 < 0)
                j = 0;
            if (max >= lines.Length)
                max = lines.Length - 1;
            for (; j < max; j++)
            {
                if (times[j] == times[i])
                {
                    str += lyrics[j] + "\r\n";
                    count++;
                }
                if (count != 0 && times[j] != times[i])
                    break;
            }
            textLyrics.Text = str;
            str = "";
            if (j + count < lines.Length)
                j += count;
            for (; count > 0; count--)
                str += lyrics[j - count]+"\r\n";
            nextLyr.Text = str;
        }
    }
}
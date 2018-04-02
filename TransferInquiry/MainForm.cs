using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransferInquiry.Command;
using TransferInquiry.Entitys;

namespace TransferInquiry
{
    public partial class MainForm : Form
    {
        StationEntity[] stations = null;
        bool isQeury = false;

        public MainForm()
        {
            InitializeComponent();
        }

        #region 下拉框事件

        /// <summary>
        /// 初始化下拉框属性
        /// </summary>
        /// <param name="cb"></param>
        private void InitComboBox(ComboBox cb)
        {
            cb.DisplayMember = "FN";
            cb.DropDownStyle = ComboBoxStyle.Simple;
            cb.Height = 20;
        }

        /// <summary>
        /// 初始化下拉框事件
        /// </summary>
        /// <param name="cb"></param>
        private void InitComboBoxEvent(ComboBox cb)
        {
            cb.TextChanged += ComboBox_TextChanged;
            cb.Leave += ComboBox_Leave;
            cb.KeyDown += ComboBox_KeyDown;
            cb.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
        }

        /// <summary>
        /// 下拉框值改变事件
        /// TextChanged先于SelectedIndexChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            if (isQeury)
            {
                var cb = (ComboBox)sender;
                cb.Height = 150;
                if (stations != null)
                {
                    var c = cb.Text;
                    if (c.Trim() == "")
                    {
                        cb.Items.Clear();
                    }
                    else
                    {
                        var s = stations.Where(p => p.OP.StartsWith(c) || p.FP.StartsWith(c) || p.JP.StartsWith(c) || p.FN.StartsWith(c)).ToArray();
                        //var s = stations.Where(p => p.StationData[0].StartsWith(c) || p.StationData[1].StartsWith(c) || p.StationData[3].StartsWith(c)).ToArray();
                        cb.Items.Clear();
                        cb.Items.AddRange(s);
                        cb.SelectionStart = c.Length;
                    }
                }
                isQeury = false;
            }
        }

        /// <summary>
        /// 下拉框失去焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Leave(object sender, EventArgs e)
        {
            isQeury = false;
            ((ComboBox)sender).Height = 20;
        }

        /// <summary>
        /// 下拉框按下按键事件
        /// KeyDown先于TextChanged，也先于SelectedIndexChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            isQeury = true;
        }

        /// <summary>
        /// 下拉框选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((ComboBox)sender).Height = 20;
        }

        #endregion 下拉框事件


        private void MainForm_Load(object sender, EventArgs e)
        {
            InitComboBox(cbStartStation);
            InitComboBoxEvent(cbStartStation);

            InitComboBox(cbStopStation);
            InitComboBoxEvent(cbStopStation);
        }

        /// <summary>
        /// 加载站点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnLoadStanName_Click(object sender, EventArgs e)
        {
            label3.Text = "正在加载";

            //主线程遇到await就暂时结束，当执行完毕时，由支线程通知主线程继续执行后面的操作
            stations = await LoadStationNames();
            //stations = await Task.Run(() => { return StationHelp.LoadStationNames(); });

            label3.Text = "加载结束";
        }

        /// <summary>
        /// 加载站点
        /// </summary>
        /// <returns></returns>
        private async Task<StationEntity[]> LoadStationNames()
        {
            //这里会创建一个线程
            var result = await Task.Run(() => { return StationHelp.LoadStationNames(); });
            return result;
        }
        
        /// <summary>
        /// 保存所有站点到数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveStation_Click(object sender, EventArgs e)
        {
            if (stations != null)
            {
                StationHelp.SaveStationNames(stations);
            }
        }



        /// <summary>
        /// 加载指定地点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var date = "2018-02-14";
            var start = cbStartStation.SelectedItem as StationEntity;
            var end = cbStopStation.SelectedItem as StationEntity;
            if (start == null || end == null)
            {
                MessageBox.Show("起止点不正确！");
                return;
            }
            StationHelp.LoadTrain(start.SC, end.SC, date, null);
            label3.Text = "OK";
        }

        /// <summary>
        /// 转车查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            //乘车数量
            var count = 1;
            var start = cbStartStation.SelectedItem as StationEntity;
            var end = cbStopStation.SelectedItem as StationEntity;
            if (start == null || end == null)
            {
                MessageBox.Show("起止点不正确！");
                return;
            }

            //StationHelp.TransferQeury(start.FN, end.FN, count);
            var result = StationHelp.TransferOne(start.FN, end.FN);
            listBox1.DataSource = result;

            var result2 = StationHelp.TransferTwo(start.FN, end.FN);
            listBox2.DataSource = result2;

            label6.Text = "开始加载换乘两次的列车";
            //var result3 = StationHelp.TransferThree(start.FN, end.FN);
            //listBox3.DataSource = result3;

            await TransferThree(start.FN, end.FN);

            label6.Text = "结束加载换乘两次的列车！";
        }

        private async Task<List<string>> TransferThree(string startName, string endName)
        {
            return await Task.Run(()=> { return StationHelp.TransferThree(startName, endName, ShowThree); });
        }

        private void ShowThree(string trainStr)
        {
            this.BeginInvoke(new Action(()=> 
            {
                listBox3.Items.Add(trainStr);
            }));
        }




        /// <summary>
        /// 结束自动获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            StationHelp.IsContinue = false;
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnLoadAllTrain_Click(object sender, EventArgs e)
        {
            label3.Text = "开始加载所有列车";
            var date = "2018-02-14";

            var t = await Task.Run(() => { return LoadAllTrain(date); });

            label3.Text = "所有列车加载完毕！！！！";
        }

        private async Task<bool> LoadAllTrain(string date)
        {
            StationHelp.IsContinue = true;
            return await Task.Run(() => { return StationHelp.LoadTheAllTrain(date, ShowProgess, ShowMsg); });
        }

        private void ShowProgess(int i, int t)
        {
            Invoke(new Action(()=> 
            {
                label4.Text = i.ToString();
                label5.Text = t.ToString();
            }));
        }

        private void ShowMsg(string msg)
        {
            this.Invoke(new Action(()=> 
            {
                richTextBox1.AppendText(msg + "\r\n");
            }));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Aspose.Cells;

namespace K12StudentTagReport
{
    public partial class StudTagReportForm : BaseForm
    {
        BackgroundWorker _bgWorker;
        DataTable _dt;
        List<string> _StudIDList;        
        public StudTagReportForm(List<string> StudIDList)
        {
            InitializeComponent();
            _StudIDList = StudIDList;            
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += new DoWorkEventHandler(_bgWorker_DoWork);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);
            lvData.Items.Clear();
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblMsg.Visible = false;
            // 放入可選
            foreach (DataColumn dc in _dt.Columns)
                lvData.Items.Add(dc.Caption);

            foreach (ListViewItem lvi in lvData.Items)
                lvi.Checked = true;

            btnPrint.Enabled = true;
            lvData.Enabled = true;
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 讀取資料
            _dt = DAO.QueryData.GetStudentTagData(_StudIDList);
         
        }

        private void StudTagForm_Load(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            lvData.Enabled = false;
            lblMsg.Visible = true;
            _bgWorker.RunWorkerAsync();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // 檢查是否有勾選
            if (lvData.CheckedItems.Count == 0)
            {
                MsgBox.Show("請勾選產生欄位");
                return;
            }
            btnPrint.Enabled = false;
            // 取得勾選欄位，移除不必要欄位
            List<string> rmStringList = new List<string>();
            foreach (DataColumn dc in _dt.Columns)
                rmStringList.Add(dc.Caption);
            // 有選
            foreach (ListViewItem lvi in lvData.CheckedItems)
                rmStringList.Remove(lvi.Text);    
            
            //移除沒選
            foreach (string str in rmStringList)
                _dt.Columns.Remove(str);

            Workbook wb = new Workbook();
            Utility.CompletedXls("學生類別報表", _dt, wb);
            btnPrint.Enabled = true;
        }

        private void chkSelAll_CheckedChanged(object sender, EventArgs e)
        {
                foreach (ListViewItem lvi in lvData.Items)
                    lvi.Checked = chkSelAll.Checked;            
        }    
    }
}

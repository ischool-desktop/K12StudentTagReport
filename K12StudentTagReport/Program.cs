using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12StudentTagReport
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            string regCode = "K12.Student.K12StudentTagReport";
            RibbonBarItem rbItem1 = MotherForm.RibbonBarItems["學生", "資料統計"];
            rbItem1["報表"]["學籍相關報表"]["學生類別報表"].Enable = UserAcl.Current[regCode].Executable;
            rbItem1["報表"]["學籍相關報表"]["學生類別報表"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0)
                {
                    StudTagReportForm strf = new StudTagReportForm(K12.Presentation.NLDPanels.Student.SelectedSource);
                    strf.ShowDialog();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇選學生");
                    return;
                }
            };
      
            // 學生類別報表
            Catalog catalog1a = RoleAclSource.Instance["學生"]["功能按鈕"];
            catalog1a.Add(new RibbonFeature(regCode, "學生類別報表"));

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12StudentTagReport.DAO
{
    /// <summary>
    /// 學生類別資料
    /// </summary>
    public class StudTagData
    {
        public string StudentID { get; set; }

        /// <summary>
        /// 學生姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// 年級
        /// </summary>
        public string GradeYear { get; set; }

        /// <summary>
        /// 科別名稱
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string SetaNo { get; set; }

        /// <summary>
        /// 學生類別
        /// </summary>
        public Dictionary<string, List<string>> TagDict = new Dictionary<string, List<string>>();

        /// <summary>
        /// 身分證號
        /// </summary>
        public string IDNumber { get; set; }

    }
}

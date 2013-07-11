using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.Data;

namespace K12StudentTagReport.DAO
{
    public class QueryData
    {
        /// <summary>
        /// 透過學生ID 取得學生類別相關資料
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <returns></returns>
        public static DataTable GetStudentTagData(List<string> StudentIDList)
        {
            DataTable dt = new DataTable();
            if (StudentIDList.Count > 0)
            { 
                // 檢查是高中或國中
                QueryHelper qh1 = new QueryHelper();
                string query1 = "select count(ref_dept_id) from class;";
                DataTable dtq1 = qh1.Select(query1);
                bool hasDept=false;
                if (dtq1.Rows.Count > 0)
                {
                    int i = int.Parse(dtq1.Rows[0][0].ToString());
                    if (i > 0)
                        hasDept = true;
                }
                string query2 = "";

                if (hasDept)
                {
                    dt.Columns.Add("科別");
                    // 高中
                    query2 = "select student.id,class.grade_year,dept.name as dept_name,class_name,student.name as student_name,student.seat_no,student.student_number,student.id_number,tag.prefix as tag_prefix,tag.name as tag_name from student left join tag_student on student.id=tag_student.ref_student_id left join tag on tag_student.ref_tag_id=tag.id  left join class on student.ref_class_id=class.id left join dept on class.ref_dept_id=dept.id where student.id in(" + string.Join(",", StudentIDList.ToArray()) + ") order by grade_year,student.student_number,tag_prefix,tag_name;";

                }
                else
                { 
                    // 國中
                    query2 = "select student.id,class.grade_year,class_name,student.name as student_name,student.seat_no,student.student_number,student.id_number,tag.prefix as tag_prefix,tag.name as tag_name from student left join tag_student on student.id=tag_student.ref_student_id left join tag on tag_student.ref_tag_id=tag.id  left join class on student.ref_class_id=class.id left join dept on class.ref_dept_id=dept.id where student.id in(" + string.Join(",", StudentIDList.ToArray()) + ") order by grade_year,class_name,seat_no,tag_prefix,tag_name";                
                }
                dt.Columns.Add("學號");
                dt.Columns.Add("年級");
                dt.Columns.Add("班級");
                dt.Columns.Add("座號");
                dt.Columns.Add("姓名");
                dt.Columns.Add("身分證號");

                QueryHelper qh2 = new QueryHelper();
                DataTable dtq2 = qh2.Select(query2);

                // 整理資料
                Dictionary<string, StudTagData> studTagDict = new Dictionary<string, StudTagData>();
                foreach (DataRow dr in dtq2.Rows)
                {
                    // id,dept_name,class_name,student_name,seat_no,student_number,id_number,tag_prefix,tag_name
                    string id = dr["id"].ToString();
                    if(!studTagDict.ContainsKey(id))
                        studTagDict.Add(id,new StudTagData());

                    studTagDict[id].StudentID = id;
                    if (hasDept)
                        studTagDict[id].DeptName = dr["dept_name"].ToString();
                    else
                        studTagDict[id].DeptName = "";

                    studTagDict[id].GradeYear = dr["grade_year"].ToString();

                    studTagDict[id].ClassName = dr["class_name"].ToString();

                    studTagDict[id].StudentNumber = dr["student_number"].ToString();

                    studTagDict[id].SetaNo = dr["seat_no"].ToString();

                    studTagDict[id].Name = dr["student_name"].ToString();

                    studTagDict[id].IDNumber = dr["id_number"].ToString();

                    // 類別處理原則：群組當欄位名稱，如果沒有群組，欄位名稱用[類別名稱]，內容值是類別名稱，如果有2個以上用,隔開。
                    string tname = dr["tag_name"].ToString();
                    string pname = dr["tag_prefix"].ToString();                    
                    string tkey = pname;
                    if (!string.IsNullOrEmpty(tname))
                    {
                        if (string.IsNullOrEmpty(pname))
                            tkey = "[" + tname + "]";

                        if (!studTagDict[id].TagDict.ContainsKey(tkey))
                            studTagDict[id].TagDict.Add(tkey, new List<string>());

                        studTagDict[id].TagDict[tkey].Add(tname);
                    }
                }
                
                // 處理DataTable 新增欄位
                List<string> addColumnsList = new List<string>();
                foreach (StudTagData std in studTagDict.Values)
                {
                    foreach (string name in std.TagDict.Keys)
                        if (!addColumnsList.Contains(name))
                            addColumnsList.Add(name);
                }
                addColumnsList.Sort();
                foreach (string name in addColumnsList)
                    dt.Columns.Add(name);

                // 資料填入 DataTable
                foreach (StudTagData std in studTagDict.Values)
                {
                    DataRow dr = dt.NewRow();
                    
                    if(hasDept)
                        dr["科別"]=std.DeptName;

                    dr["學號"]=std.StudentNumber;
                    dr["班級"]=std.ClassName;
                    dr["座號"]=std.SetaNo;
                    dr["姓名"] = std.Name;
                    dr["年級"] = std.GradeYear;
                    dr["身分證號"] = std.IDNumber;
                    // 填入Tag
                    foreach (KeyValuePair<String, List<string>> data in std.TagDict)
                        dr[data.Key] = string.Join(",",data.Value.ToArray());

                    dt.Rows.Add(dr);
                }            
            }            
            return dt;
        }
    }
}

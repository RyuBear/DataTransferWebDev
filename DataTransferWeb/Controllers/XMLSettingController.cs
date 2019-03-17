using DataTransferWeb.Comparer;
using DataTransferWeb.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Transfer.Models;
using Transfer.Models.Repository;

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class XMLSettingController : BaseController
    {
        /// <summary>
        /// XMLMappings 暫存 XML設定資料
        /// setedColumns 暫存 已設定之欄位
        /// </summary>
        XMLSettingVM model = new XMLSettingVM();

        [HttpGet]
        public ActionResult Index()
        {
            XMLSettingListVM vm = new XMLSettingListVM();

            using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
            {
                vm.settings = rep.get(vm.XMLName, vm.SQLName, vm.CustomerName); ;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(XMLSettingListVM vm)
        {

            using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
            {
                vm.settings = rep.get(vm.XMLName, vm.SQLName, vm.CustomerName);
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult Load(string id)
        {
            Cache.DelCache("setedColumns");
            Cache.DelCache("XMLMappings");

            if (string.IsNullOrEmpty(id))
            {
                return View("Edit", model);
            }

            model.ViewStatus = "E";
            string XMLName = id;
            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                model.UnsetColumns = rep.getAllColumns(model.SQLName).ToList();
                var options = new StringBuilder();
                options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");

                using (tblXMLSettingRepository setRep = new tblXMLSettingRepository())
                using (tblXMLMappingRepository xmlRep = new tblXMLMappingRepository())
                {
                    tblXMLSetting setting = setRep.get(XMLName);
                    IEnumerable<tblXMLMapping> mapping = xmlRep.get(XMLName);

                    model.XMLName = setting.XMLName;
                    model.SQLName = setting.SQLName;
                    model.CustomerName = setting.CustomerName;
                    model.FileName = setting.FileName;
                    model.FileNameDateFormat = setting.FileNameDateFormat;
                    model.UserID = setting.UserId;

                    foreach (var m in mapping)
                    {
                        model.XMLMappingDataRow.Add(new tblXMLMapping()
                        {
                            TagName = m.TagName,
                            FieldName = m.FieldName,
                            DefaultValue = m.DefaultValue,
                            FatherTag = m.FatherTag,
                            Idx = m.Idx,
                            CanRepeat = m.CanRepeat
                        });
                    }

                    using (tblSQLColumnsRepository colRep = new tblSQLColumnsRepository())
                    {
                        List<tblSQLColumns> allColumns = colRep.getAllColumns(model.SQLName).ToList();
                        List<tblSQLColumns> setedColumns = new List<tblSQLColumns>(); ;
                        foreach (var c in mapping)
                        {
                            setedColumns.Add(new tblSQLColumns()
                            {
                                SQLName = model.SQLName,
                                ColumnName = c.FieldName
                            });
                        }
                        model.SetedColumns = setedColumns;

                        IEnumerable<tblSQLColumns> unsetColumns = allColumns.Except(setedColumns, new ColumnComparer());
                        model.UnsetColumns = unsetColumns.ToList();
                    }
                }
            }

            Cache.SetLimitedCache("XMLMappings", model.XMLMappingDataRow);
            Cache.SetLimitedCache("setedColumns", model.SetedColumns);
            return View("Edit", model);
        }

        /// <summary>
        /// 取得 Columns 清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxGetColumns")]
        public ActionResult GetColumns(XMLSettingVM vm)
        {
            Cache.DelCache("setedColumns");
            Cache.DelCache("XMLMappings");

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");

            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                List<tblSQLColumns> allColumns = rep.getAllColumns(vm.SQLName).ToList();
                foreach (var c in allColumns)
                {
                    options.AppendFormat("<option value='{0}'>{1}</option>", c.ColumnName, c.ColumnName);
                }
            }

            // Father Tag Option
            var fatherTag = new StringBuilder();
            fatherTag.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");

            var jsonData = new
            {
                Options = options.ToString(),
                FatherTagOptions = fatherTag.ToString()
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 新增 Mapping
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxInsertMapping")]
        public ActionResult InsertMapping(XMLSettingVM vm)
        {
            List<tblXMLMapping> xmlMappings = new List<tblXMLMapping>();
            string ColumnName = string.Empty;
            int idx = -1;

            #region 寫入暫存 Session XMLMappings
            try
            {
                xmlMappings = JsonConvert.DeserializeObject<List<tblXMLMapping>>(Cache.GetCache("XMLMappings"));
            }
            catch { }

            // 判斷 Tag Name 是否有重複
            if (!string.IsNullOrEmpty(vm.TagName) && !vm.TagName.Equals(vm.newTagName, StringComparison.OrdinalIgnoreCase))
            {
                tblXMLMapping map = xmlMappings.Find(x => x.TagName.Equals(vm.newTagName, StringComparison.OrdinalIgnoreCase));
                if (map != null)
                {
                    var result = new
                    {
                        status = "Tag Name 已存在！",
                    };
                    return Json(result);
                }
            }
            // 更新時 將舊暫存資料刪除
            tblXMLMapping mapping = xmlMappings.Find(x => x.TagName.Equals(vm.TagName, StringComparison.OrdinalIgnoreCase));
            if (mapping != null)
            {
                ColumnName = mapping.FieldName;
                idx = mapping.Idx;
                xmlMappings.Remove(mapping);
            }

            // 將新的設定值寫入
            mapping = new tblXMLMapping()
            {
                TagName = vm.newTagName,
                FieldName = vm.FieldName ?? string.Empty,
                DefaultValue = vm.DefaultValue,
                FatherTag = vm.FatherTag ?? string.Empty,
                Idx = (idx < 0) ? xmlMappings.Count + 1 : idx,
                CanRepeat = vm.CanRepeat
            };
            xmlMappings.Add(mapping);
            Cache.SetLimitedCache("XMLMappings", xmlMappings);

            // Father Tag Option
            var fatherTag = new StringBuilder();
            fatherTag.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");
            foreach (var m in xmlMappings)
            {
                fatherTag.AppendFormat("<option value='{0}'>{1}</option>", m.TagName, m.TagName);
            }
            #endregion

            #region 寫入暫存 Session setedColumns
            List<tblSQLColumns> setedColumns = new List<tblSQLColumns>(); ;
            try
            {
                setedColumns = JsonConvert.DeserializeObject<List<tblSQLColumns>>(Cache.GetCache("setedColumns"));
            }
            catch { }

            // 更換指定欄位時
            if (!ColumnName.Equals(vm.FieldName))
            {
                // 更新時 將原指定的欄位刪除
                tblSQLColumns column = setedColumns.Find(x => x.ColumnName.Equals(ColumnName, StringComparison.OrdinalIgnoreCase));
                if (column != null)
                    setedColumns.Remove(column);

                // 記錄 新指定的欄位
                if (!string.IsNullOrEmpty(vm.FieldName))
                {
                    tblSQLColumns setedColumn = new tblSQLColumns()
                    {
                        SQLName = vm.SQLName,
                        ColumnName = vm.FieldName
                    };
                    setedColumns.Add(setedColumn);
                }
            }

            Cache.SetLimitedCache("setedColumns", setedColumns);
            #endregion

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");
            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                List<tblSQLColumns> allColumns = rep.getAllColumns(vm.SQLName).ToList();
                IEnumerable<tblSQLColumns> unsetColumns = allColumns.Except(setedColumns, new ColumnComparer());
                foreach (var c in unsetColumns)
                {
                    options.AppendFormat("<option value='{0}'>{1}</option>", c.ColumnName, c.ColumnName);
                }
            }

            var jsonData = new
            {
                status = "ok",
                Options = options.ToString(),
                FatherTagOptions = fatherTag.ToString()
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 刪除 Mapping
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxDeleteMapping")]
        public ActionResult DeleteMapping(XMLSettingVM vm)
        {
            #region 從暫存 Session XMLMappings 刪除
            List<tblXMLMapping> xmlMappings = new List<tblXMLMapping>(); ;
            string ColumnName = string.Empty;

            try
            {
                xmlMappings = JsonConvert.DeserializeObject<List<tblXMLMapping>>(Cache.GetCache("XMLMappings"));
            }
            catch { }
            tblXMLMapping mapping = xmlMappings.Find(x => x.TagName.Equals(vm.TagName, StringComparison.OrdinalIgnoreCase));
            if (mapping != null)
            {
                ColumnName = mapping.FieldName;
                xmlMappings.Remove(mapping);
            }

            Cache.SetLimitedCache("XMLMappings", xmlMappings);

            // Father Tag Option
            var fatherTag = new StringBuilder();
            fatherTag.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");
            foreach (var m in xmlMappings)
            {
                fatherTag.AppendFormat("<option value='{0}'>{1}</option>", m.TagName, m.TagName);
            }
            #endregion

            #region 從暫存 Session setedColumns 刪除
            List<tblSQLColumns> setedColumns = new List<tblSQLColumns>(); ;
            try
            {
                setedColumns = JsonConvert.DeserializeObject<List<tblSQLColumns>>(Cache.GetCache("setedColumns"));
            }
            catch { }
            tblSQLColumns column = setedColumns.Find(x => x.ColumnName.Equals(ColumnName, StringComparison.OrdinalIgnoreCase));
            if (column != null)
                setedColumns.Remove(column);

            Cache.SetLimitedCache("setedColumns", setedColumns);
            #endregion

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");
            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                List<tblSQLColumns> allColumns = rep.getAllColumns(vm.SQLName).ToList();
                IEnumerable<tblSQLColumns> unsetColumns = allColumns.Except(setedColumns, new ColumnComparer());
                foreach (var c in unsetColumns)
                {
                    options.AppendFormat("<option value='{0}'>{1}</option>", c.ColumnName, c.ColumnName);
                }
            }

            var jsonData = new
            {
                Options = options.ToString(),
                FatherTagOptions = fatherTag.ToString()
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 顯示已設定的XML設定
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxShowMappingData")]
        [OutputCache(NoStore = true, Duration = 0)] // 以防Server取得的是Cache，必須即時更新
        public ActionResult ShowMappingData(XMLSettingVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                return Content("Fail");
            }

            #region 讀入暫存 Session XMLMappings
            List<tblXMLMapping> xmlMappings = new List<tblXMLMapping>(); ;
            try
            {
                xmlMappings = JsonConvert.DeserializeObject<List<tblXMLMapping>>(Cache.GetCache("XMLMappings"));
            }
            catch { }
            #endregion

            model.XMLMappingDataRow = xmlMappings.OrderBy(x => x.Idx).ToList(); ;

            return PartialView("_XMLMappingRow", model);
        }

        /// <summary>
        /// 預覽結果
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxShowXML")]
        [OutputCache(NoStore = true, Duration = 0)] // 以防Server取得的是Cache，必須即時更新
        public ActionResult ShowXML(XMLSettingVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                return Content("Fail");
            }

            #region 取得暫存 Session XMLMappings
            List<tblXMLMapping> xmlMappings = new List<tblXMLMapping>();
            string ColumnName = string.Empty;

            try
            {
                xmlMappings = JsonConvert.DeserializeObject<List<tblXMLMapping>>(Cache.GetCache("XMLMappings"));
            }
            catch { }
            #endregion

            #region 依指定SQL語句抓取資料
            using (tblSQLSettingRepository setting = new tblSQLSettingRepository())
            {
                tblSQLSetting SQLSetting = setting.select(vm.SQLName);
                if (SQLSetting != null)
                {
                    using (DataAccess da = new DataAccess())
                    {
                        // 將 top(n) 帶入 SQL語句
                        //int index = SQLSetting.SQLStatement.IndexOf("Select", StringComparison.OrdinalIgnoreCase);  // 找出第一個select的位置
                        //string sql = "SELECT TOP (" + SQLSetting.DataRow + ") " + SQLSetting.SQLStatement.Remove(index, 6);

                        string sql = Func.SQLTop(SQLSetting.SQLStatement, SQLSetting.DataRow);
                        Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sql);
                        DataTable dt = result.Item2;

                        XmlDocument xmlDoc = new XmlDocument();
                        //根節點 只有1個
                        var rootTag = xmlMappings.Where(x => string.IsNullOrEmpty(x.FatherTag)).First();

                        XmlElement root = xmlDoc.CreateElement(rootTag.TagName);
                        for (int i = 0; i < dt.Rows.Count; i++)
                            appendXmlByRow(dt.Rows[i], xmlMappings, xmlDoc, root, rootTag.TagName, 1);

                        //addSubElement(xmlMappings, xmlDoc, root, rootTag.TagName, dt);
                        xmlDoc.AppendChild(root);

                        model.XMLView = Server.HtmlEncode(Func.BeautifyXML(xmlDoc));

                    }
                }
                else
                {
                    model.XMLView = "找不到指定的 SQL 設定!";
                }
            }
            #endregion

            return PartialView("_XMLView", model);
        }

        void addSubElement(List<tblXMLMapping> mappings, XmlDocument xmlDoc, XmlElement root, string FatherTag, DataTable dt)
        {
            var subs = mappings.Where(x => x.FatherTag.Equals(FatherTag, StringComparison.OrdinalIgnoreCase));
            foreach (tblXMLMapping s in subs)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string value = string.Empty;
                    if (!string.IsNullOrEmpty(s.FieldName) || !string.IsNullOrEmpty(s.DefaultValue))
                    {
                        if (string.IsNullOrEmpty(s.FieldName))
                            value = s.DefaultValue;
                        else
                            value = (string.IsNullOrEmpty(dt.Rows[i][s.FieldName].ToString())) ? s.DefaultValue : dt.Rows[i][s.FieldName].ToString();
                    }

                    XmlNodeList nodes = root.GetElementsByTagName(s.TagName);
                    int nodeCount = nodes.Cast<XmlNode>().Where(n => n.InnerText == value).Count();

                    if (nodeCount == 0)
                    {
                        XmlElement element = xmlDoc.CreateElement(s.TagName);
                        element.InnerText = value;
                        addSubElement(mappings, xmlDoc, element, s.TagName, dt);
                        root.AppendChild(element);
                    }
                }
            }
        }

        void appendXmlByRow(DataRow row, List<tblXMLMapping> mappings, XmlDocument xmlDoc, XmlElement Node, string FatherTag, int layer)
        {
            // 根節點
            var roots = mappings.Where(x => x.FatherTag.Equals(FatherTag, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Idx);
            foreach (var root in roots)
            {
                string value = string.Empty;
                if (!string.IsNullOrEmpty(root.FieldName) || !string.IsNullOrEmpty(root.DefaultValue))
                {
                    if (string.IsNullOrEmpty(root.FieldName))
                        value = root.DefaultValue;
                    else
                        value = (string.IsNullOrEmpty(row[root.FieldName].ToString())) ? root.DefaultValue : row[root.FieldName].ToString();
                }

                XmlNode node = Node.SelectSingleNode("/".Repeat(layer) + root.TagName);
                if (node == null)
                {
                    XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                    if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                    appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);

                    Node.AppendChild(rootNode);
                }
                else
                {
                    XmlNodeList nodes = Node.GetElementsByTagName(root.TagName);

                    //// 單純為階層節點時
                    if (string.IsNullOrEmpty(root.FieldName) && string.IsNullOrEmpty(root.DefaultValue) && root.CanRepeat)
                    {
                        XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                        if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                        appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);
                        Node.AppendChild(rootNode);
                    }
                    else
                    {
                        int nodeCount = 0;
                        if (!string.IsNullOrEmpty(root.FieldName))
                            nodeCount = nodes.Cast<XmlNode>().Where(n => n.InnerText == row[root.FieldName].ToString()).Count();
                        if ((node == null) || (nodes.Count > 0 && !string.IsNullOrEmpty(root.FieldName) && nodeCount == 0))
                        {
                            XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                            if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                            appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);
                            Node.AppendChild(rootNode);
                        }
                        else
                        {
                            XmlElement element = nodes.Cast<XmlElement>().FirstOrDefault();
                            appendXmlByRow(row, mappings, xmlDoc, element, root.TagName, 2);
                            // Node.AppendChild(element);
                        }
                    }
                }
            }

        }

        [HttpPost]
        [ActionName("AjaxIsExist")]
        public ActionResult isExist(string XMLName)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                ModelState.AddModelError("", "非 ajax 呼叫");
                //throw new Exception("非 ajax 呼叫");
            }

            using (tblXMLSettingRepository setting = new tblXMLSettingRepository())
            {
                bool isExist = setting.isExist(XMLName);
                return Json(new { Exist = isExist });
            }
        }

        [HttpPost]
        public ActionResult Save(XMLSettingVM vm, string[] DateFormats)
        {
            if (string.IsNullOrEmpty(vm.XMLName))
                vm.SaveResult += "請輸入 XML Name!\r\n";
            if (string.IsNullOrEmpty(vm.CustomerName))
                vm.SaveResult += "請輸入 Customer Name!\r\n";
            if (string.IsNullOrEmpty(vm.SQLName))
                vm.SaveResult += "請選擇 SQL Name!\r\n";

            vm.FileNameDateFormat = string.Join(",", DateFormats);

            if (!string.IsNullOrEmpty(vm.SaveResult))
            {
                return View("Edit", vm);
            }

            #region 取得暫存 Session XMLMappings
            List<tblXMLMapping> xmlMappings = new List<tblXMLMapping>();
            string ColumnName = string.Empty;

            try
            {
                xmlMappings = JsonConvert.DeserializeObject<List<tblXMLMapping>>(Cache.GetCache("XMLMappings"));
            }
            catch (Exception ex) { }
            #endregion

            using (tblXMLSettingRepository setting = new tblXMLSettingRepository())
            {
                vm.SaveResult = setting.Save(vm.XMLName, vm.CustomerName, vm.SQLName, vm.FileName, vm.FileNameDateFormat, vm.UserID, userInfo.Account, xmlMappings);
                if (vm.SaveResult.Equals("ok")) vm.SaveResult = "Save Successful!";

            }
            return RedirectToAction("Index");
        }


        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
            {
                rep.Delete(id);
            }
            return RedirectToAction("Index");
        }
    }
}
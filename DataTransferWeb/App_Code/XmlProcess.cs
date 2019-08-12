using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Transfer.Models;
using Transfer.Models.Repository;

namespace DataTransferWeb
{
    public class XmlProcess
    {
        static List<vwCodeMapping> codeMap = new List<vwCodeMapping>();

        //回傳重複的 "字串" 所組成的字串
        public static string strRepeat(string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }


        public static XmlDocument GenerateXML(DataTable dt, List<tblXMLMapping> xmlMappings)
        {
            string XMLName = xmlMappings[0].XMLName;
            // 取得代碼轉換資料
            using (vwCodeMappingRepository rep = new vwCodeMappingRepository())
            {
                codeMap = rep.query("", "EXPORT", "XML", XMLName, "");
            }

            XmlDocument xmlDoc = new XmlDocument();

            //根節點 只有1個
            var rootTag = xmlMappings.Where(x => string.IsNullOrEmpty(x.FatherTag)).First();

            XmlElement root = xmlDoc.CreateElement(rootTag.TagName);
            for (int i = 0; i < dt.Rows.Count; i++)
                appendXmlByRow(dt.Rows[i], xmlMappings, xmlDoc, root, rootTag.TagName, 1);

            //addSubElement(xmlMappings, xmlDoc, root, rootTag.TagName, dt);
            xmlDoc.AppendChild(root);

            return xmlDoc;
        }

        static void appendXmlByRow(DataRow row, List<tblXMLMapping> mappings, XmlDocument xmlDoc, XmlElement Node, string FatherTag, int layer)
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

                    // 代碼轉換
                    if (codeMap.Where(x => x.FieldName.Equals(root.TagName, StringComparison.OrdinalIgnoreCase)).Count() > 0
                      && codeMap.Where(x => x.BeforeValue.Equals(value, StringComparison.OrdinalIgnoreCase)).Count() > 0)
                    {
                        vwCodeMapping map = codeMap.Find(x => x.FieldName.Equals(root.TagName, StringComparison.OrdinalIgnoreCase) && x.BeforeValue.Equals(value, StringComparison.OrdinalIgnoreCase));
                        if (map != null)
                            value = map.AfterValue;
                    }
                }

                XmlNode node = Node.SelectSingleNode(strRepeat("/", layer) + root.TagName);
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

                    //// 單純為階層節點時、允許重複
                    if (string.IsNullOrEmpty(root.FieldName) && string.IsNullOrEmpty(root.DefaultValue) && root.CanRepeat)
                    {
                        XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                        if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                        appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);
                        // 節點完成後，判斷是否有一模一樣的節點，如果有則不寫出
                        int count = nodes.Cast<XmlNode>().Where(n => n.InnerText == rootNode.InnerText).Count();
                        if (count == 0)
                            Node.AppendChild(rootNode);
                    }
                    else
                    {
                        int nodeCount = 0;
                        if (!string.IsNullOrEmpty(root.FieldName))
                            nodeCount = nodes.Cast<XmlNode>().Where(n => n.FirstChild.InnerText == row[root.FieldName].ToString()).Count();
                        if ((node == null) || (nodes.Count > 0 && !string.IsNullOrEmpty(root.FieldName) && nodeCount == 0))
                        {
                            XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                            if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                            appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);
                            Node.AppendChild(rootNode);
                        }
                        else
                        {
                            XmlElement element;
                            if (string.IsNullOrEmpty(root.FieldName))
                                element = nodes.Cast<XmlElement>().FirstOrDefault();
                            else
                                element = nodes.Cast<XmlElement>().Where(n => n.FirstChild.InnerText == row[root.FieldName].ToString()).FirstOrDefault();
                            appendXmlByRow(row, mappings, xmlDoc, element, root.TagName, 2);
                        }
                    }
                }
            }
        }

    }
}
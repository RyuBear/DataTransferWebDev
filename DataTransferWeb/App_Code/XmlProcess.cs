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
                appendXmlByRow(dt.Rows[i], xmlMappings, xmlDoc, root, null, rootTag.TagName, 1);

            xmlDoc.AppendChild(root);

            return xmlDoc;
        }

        static bool appendXmlByRow(DataRow row, List<tblXMLMapping> mappings, XmlDocument xmlDoc, XmlElement Node, XmlElement parentNode, string FatherTag, int layer)
        {
            bool output = true;
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

                XmlNode node;

                if (parentNode == null || parentNode.ChildNodes.Count == 0)
                    node = Node.SelectSingleNode(strRepeat("/", layer) + root.TagName);
                else
                    node = parentNode.SelectSingleNode(strRepeat("/", layer + 1) + root.TagName);

                if (node == null)
                {
                    XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                    if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                    appendXmlByRow(row, mappings, xmlDoc, rootNode, Node, root.TagName, layer);

                    Node.AppendChild(rootNode);
                }
                else
                {
                    XmlNodeList nodes = node.ParentNode.SelectNodes(strRepeat("/", layer + 1) + root.TagName);

                    //// 單純為階層節點時、允許重複
                    if (string.IsNullOrEmpty(root.FieldName) && string.IsNullOrEmpty(root.DefaultValue) && root.CanRepeat)
                    {
                        XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                        if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                        if (appendXmlByRow(row, mappings, xmlDoc, rootNode, Node, root.TagName, layer))
                            Node.AppendChild(rootNode);
                    }
                    else
                    {
                        int nodeCount = 0;

                        XmlNodeList existedNode = parentNode.SelectNodes(strRepeat("/", layer + 1) + root.TagName);

                        if (!string.IsNullOrEmpty(root.FieldName))
                            nodeCount = existedNode.Cast<XmlNode>().Where(n => n.FirstChild.InnerText == row[root.FieldName].ToString()).Count();
                        if (nodes.Count > 0 && !string.IsNullOrEmpty(root.FieldName) && nodeCount == 0)
                        {
                            XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                            if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                            appendXmlByRow(row, mappings, xmlDoc, rootNode, Node, root.TagName, layer);
                            Node.AppendChild(rootNode);
                        }
                        else
                        {
                            XmlElement element;
                            if (string.IsNullOrEmpty(root.FieldName))
                                element = existedNode.Cast<XmlElement>().FirstOrDefault();
                            else
                                element = existedNode.Cast<XmlElement>().Where(n => n.FirstChild.InnerText == row[root.FieldName].ToString()).FirstOrDefault();
                            appendXmlByRow(row, mappings, xmlDoc, element, (XmlElement)element.ParentNode, root.TagName, layer);
                            output = false;
                        }
                    }
                }
            }

            return output;
        }
    }
}
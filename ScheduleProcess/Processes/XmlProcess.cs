﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Transfer.Models;

namespace ScheduleProcess.Processes
{
    public class XmlProcess
    {
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
                        }
                    }
                }
            }
        }

    }
}
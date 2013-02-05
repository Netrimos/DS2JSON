using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Xml;

namespace JSON_MDS
{
    public class Dataset2JSON
    {
        private char[] charsToTrim = { ',', ' ', '\n' };
        public string serializeJSON(DataSet setToTransform)
        {
            string json = "";

            int rowCount = 1;
            //Loop DataTables
            foreach (DataTable table in setToTransform.Tables)
            {
                json += "{\"" + table.TableName + "\":[";
                //Loop Columns and create Dynamic Array of Names
                List<string> columnNames = new List<string>();
                foreach (DataColumn col in table.Columns)
                {
                    columnNames.Add(col.ColumnName);
                }
                //Loop DataRows

                foreach (DataRow row in table.Rows)
                {
                    json += "{";
                    //Loop to add the correct column name to each item                    
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        //check for returned XML
                        if (!row[i].ToString().StartsWith("<")) //-----concern------stray "<" will break code, not a solid XML verification.
                        {
                            json += "\"" + columnNames[i] + "\":";
                            json += "\"" + row[i] + "\",";
                        }
                        //parse returned XML
                        else
                        {
                            json += serializeXMLtoJSON(row[i].ToString());
                        }
                    }
                    json = json.TrimEnd(charsToTrim);
                    json += "\n";
                    json += "},";
                    rowCount++;
                }
                json = json.TrimEnd(charsToTrim);
                json += "\n";
                json += "],";
            }
            json = json.TrimEnd(charsToTrim);
            return "" + json + "}";
        }

        private string serializeXMLtoJSON(string xmlString)
        {
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(xmlString));

            string parsedString = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        parsedString += "\"" + reader.Name + "\":{";
                        if (reader.HasAttributes)
                        {
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                parsedString += "\"" + reader.Name + "\":";
                                parsedString += "\"" + reader.Value + "\",";
                            }
                        }
                        parsedString = parsedString.TrimEnd(charsToTrim);

                        if (reader.HasValue) { parsedString += "},"; }//--looks for any attributes
                        parsedString += "\n";
                        break;
                    case XmlNodeType.Text:
                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }
            parsedString = parsedString.TrimEnd(charsToTrim);
            return parsedString;
        }
    }
}
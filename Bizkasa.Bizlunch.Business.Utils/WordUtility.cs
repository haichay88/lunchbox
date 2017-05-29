using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public class WordUtility
    {
        #region Properties
        public byte[] FileContent { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Hàm khởi tại lớp thao tác file word
        /// </summary>
        public WordUtility(byte[] fileContent)
        {
            this.FileContent = fileContent;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Thực hiện chèn một đoạn text vào book
        /// </summary>
        /// <example>
        /// </example>
        /// <param name="fileContent">mảng byte nội dung file cần chèn</param>
        /// <returns>trả về 1 instance MemoryStream sử dụng ghi ra file</returns>
        public MemoryStream InsertBookMarkText(byte[] fileContent, Dictionary<string, string> bookMarkData)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                using (MemoryStream memStr = new MemoryStream())
                {
                    // đổ mảng byte vào chuỗi
                    memStr.Write(fileContent, 0, fileContent.Length);
                    // mở 
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                    {
                        Document document = wordDoc.MainDocumentPart.Document;
                        // thực hiện lấy tất cả bookmark trong file
                        var bookMarks = FindBookmarks(document.MainDocumentPart.Document);

                        // tìm kiếm bookmark trùng với tên truyền vào
                        foreach (var element in bookMarks)
                        {
                            if (bookMarkData.ContainsKey(element.Key))
                            {
                                // thực hiện chèn 1 run với nội dung text truyền vào
                                Text textElement = new Text(bookMarkData[element.Key]);
                                textElement.Space = new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve);
                                Run runElement = new Run(new RunProperties(new Bold()));
                                runElement.Append(textElement);
                                element.Value.InsertAfterSelf(runElement);
                            }
                        }
                    }
                    return memStr;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MemoryStream InsertBookMarkSignPage(byte[] fileContent, Dictionary<string, string> bookMarkData)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                using (MemoryStream memStr = new MemoryStream())
                {
                    // đổ mảng byte vào chuỗi
                    memStr.Write(fileContent, 0, fileContent.Length);
                    // mở 
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                    {
                        Document document = wordDoc.MainDocumentPart.Document;
                        // thực hiện lấy tất cả bookmark trong file
                        var bookMarks = FindBookmarks(document.MainDocumentPart.Document);

                        // tìm kiếm bookmark trùng với tên truyền vào
                        foreach (var element in bookMarks)
                        {
                            if (bookMarkData.ContainsKey(element.Key))
                            {
                                // thực hiện chèn 1 run với nội dung text truyền vào
                                Text textElement = new Text(bookMarkData[element.Key]);
                                textElement.Space = new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve);
                                if (element.Key == "BM_DON_VI" || element.Key == "BM_CHUC_DANH")
                                {
                                    Run runElement = new Run(new RunProperties(new Bold()));
                                    runElement.Append(textElement);

                                    element.Value.InsertAfterSelf(runElement);
                                }
                                else
                                {
                                    Run runElement = new Run();
                                    runElement.Append(textElement);

                                    element.Value.InsertAfterSelf(runElement);
                                }
                            }
                        }
                    }
                    return memStr;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm chèn dữ liệu 
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="bookMarkTableName"></param>
        /// <param name="dataTables"></param>
        /// <returns></returns>
        public MemoryStream FillDataToTable(byte[] fileContent, string bookMarkTableName, DataTable dataTables)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                using (MemoryStream memStr = new MemoryStream())
                {
                    // đổ mảng byte vào chuỗi
                    memStr.Write(fileContent, 0, fileContent.Length);
                    // mở 
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                    {
                        Document document = wordDoc.MainDocumentPart.Document;
                        // thực hiện lấy tất cả bookmark trong file
                        var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                        foreach (var element in bookMarks)
                        {
                            if (element.Key == bookMarkTableName)
                            {
                                Table table = element.Value.Parent as Table;

                                if (table != null)
                                {

                                    for (int i = 0; i < dataTables.Rows.Count; i++)
                                    {
                                        // new row
                                        TableRow tableRow = new TableRow();

                                        for (int j = 0; j < dataTables.Columns.Count; j++)
                                        {
                                            // new cell
                                            TableCell tableCell = new TableCell();
                                            string _colName = dataTables.Columns[j].ColumnName;
                                            // new cell properties
                                            TableCellProperties tableCellProperties = new TableCellProperties();
                                            TableCellWidth tableCellWidth = new TableCellWidth();

                                            tableCellProperties.Append(tableCellWidth);

                                            // new paragraph for cell
                                            Paragraph paragraph = new Paragraph();

                                            ParagraphProperties paragraphProperties = new ParagraphProperties();

                                            Run run1 = new Run();
                                            Text text1 = new Text();
                                            text1.Text = dataTables.Rows[i][_colName].ToString();

                                            run1.Append(text1);

                                            paragraph.Append(paragraphProperties);
                                            paragraph.Append(run1);

                                            tableCell.Append(tableCellProperties);
                                            tableCell.Append(paragraph);

                                            tableRow.Append(tableCell);
                                        }

                                        table.Append(tableRow);
                                    }
                                }
                                break;
                            }
                        }

                    }
                    return memStr;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm chèn dữ liệu 
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="bookMarkTableName"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public MemoryStream FillDataToTable2(byte[] fileContent, string bookMarkTableName, DataTable dataTable)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                using (MemoryStream memStr = new MemoryStream())
                {
                    // đổ mảng byte vào chuỗi
                    memStr.Write(fileContent, 0, fileContent.Length);
                    // mở 
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                    {
                        Document document = wordDoc.MainDocumentPart.Document;
                        // thực hiện lấy tất cả bookmark trong file
                        var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                        foreach (var element in bookMarks)
                        {
                            if (element.Key == bookMarkTableName)
                            {
                                Table m_Table = element.Value.Parent as Table;
                                if (m_Table != null)
                                {
                                    var m_DataRows = m_Table.ChildElements.Where(e => e is TableRow).ToArray();
                                    for (int i = 0; i < dataTable.Rows.Count; i++)
                                    {
                                        // new row
                                        TableRow tableRow = new TableRow();

                                        for (int j = 0; j < dataTable.Columns.Count; j++)
                                        {
                                            // new cell
                                            TableCell tableCell = new TableCell();
                                            string _colName = dataTable.Columns[j].ColumnName;
                                            // new cell properties
                                            TableCellProperties tableCellProperties = new TableCellProperties();
                                            TableCellWidth tableCellWidth = new TableCellWidth();

                                            tableCellProperties.Append(tableCellWidth);

                                            // new paragraph for cell
                                            Paragraph paragraph = new Paragraph();

                                            ParagraphProperties paragraphProperties = new ParagraphProperties();

                                            Run run1 = new Run();
                                            Text text1 = new Text();
                                            text1.Text = dataTable.Rows[i][_colName].ToString();
                                            
                                            run1.Append(text1);

                                            paragraph.Append(paragraphProperties);
                                            paragraph.Append(run1);

                                            tableCell.Append(tableCellProperties);
                                            tableCell.Append(paragraph);

                                            tableRow.Append(tableCell);
                                        }
                                        m_Table.Append(tableRow);
                                    }
                                }
                                break;
                            }
                        }

                    }
                    return memStr;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm chèn dữ liệu 
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="bookMarkTableName"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public MemoryStream FillDataToTable3(byte[] fileContent, string bookMarkTableName, DataTable dataTable)
        {
            try
            {
                // khởi tạo memorystream dùng cho openxml đọc và ghi trên dòng byte 
                using (MemoryStream memStr = new MemoryStream())
                {
                    // đổ mảng byte vào chuỗi
                    memStr.Write(fileContent, 0, fileContent.Length);
                    // mở 
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memStr, true))
                    {
                        Document document = wordDoc.MainDocumentPart.Document;
                        // thực hiện lấy tất cả bookmark trong file
                        var bookMarks = FindBookmarks(document.MainDocumentPart.Document);
                        foreach (var element in bookMarks)
                        {
                            if (element.Key == bookMarkTableName)
                            {
                                Table m_Table = element.Value.Parent as Table;
                                if (m_Table != null)
                                {
                                    var m_DataRow = m_Table.Elements<TableRow>().Last();
                                    for (int i = 0; i < dataTable.Rows.Count; i++)
                                    {
                                        //Get Row Copy
                                        TableRow m_RowCopy = (TableRow) m_DataRow.CloneNode(true);

                                        for (int j = 0; j < dataTable.Columns.Count; j++)
                                        {
                                            var m_ColName = dataTable.Columns[j].ColumnName;
                                            var m_RunProperties = GetRunPropertyFromTableCell(m_RowCopy, j);
                                            var m_Run = new Run(new Text(dataTable.Rows[i][m_ColName].ToString()));

                                            m_Run.PrependChild<RunProperties>(m_RunProperties);

                                            m_RowCopy.Descendants<TableCell>().ElementAt(j).RemoveAllChildren<Paragraph>();
                                            m_RowCopy.Descendants<TableCell>().ElementAt(j).Append(new Paragraph(m_Run));
                                        }
                                        m_Table.Append(m_RowCopy);
                                    }
                                    m_Table.RemoveChild(m_DataRow);
                                }
                                break;
                            }
                        }

                    }
                    return memStr;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private RunProperties GetRunPropertyFromTableCell(TableRow row, int cellIndex)
        {
            var m_RunProperties = new RunProperties();

            foreach (var m_Property in row.Descendants<TableCell>().ElementAt(cellIndex).GetFirstChild<Paragraph>().GetFirstChild<ParagraphProperties>().GetFirstChild<ParagraphMarkRunProperties>())
            {
                m_RunProperties.AppendChild(m_Property.CloneNode(true));
            }

            return m_RunProperties;
        }

        /// <summary>
        /// Tìm tất cả book mark của văn bản
        /// </summary>
        /// <returns>Bộ từ điển chứa tên vào đối tượng của bookmark</returns>
        private Dictionary<string, BookmarkEnd> FindBookmarks(OpenXmlElement documentPart, Dictionary<string, BookmarkEnd> results = null, Dictionary<string, string> unmatched = null)
        {
            results = results ?? new Dictionary<string, BookmarkEnd>();
            unmatched = unmatched ?? new Dictionary<string, string>();

            foreach (var child in documentPart.Elements())
            {
                if (child is BookmarkStart)
                {
                    var bStart = child as BookmarkStart;
                    unmatched.Add(bStart.Id, bStart.Name);
                }

                if (child is BookmarkEnd)
                {
                    var bEnd = child as BookmarkEnd;
                    foreach (var orphanName in unmatched)
                    {
                        if (bEnd.Id == orphanName.Key)
                            results.Add(orphanName.Value, bEnd);
                    }
                }

                FindBookmarks(child, results, unmatched);
            }

            return results;
        }

        #endregion
    }
}

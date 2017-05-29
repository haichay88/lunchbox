using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Drawing;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public class ExcelUtility
    {
        #region Constructors

        public ExcelUtility()
        {
            this.ParameterData = new Dictionary<string, object>();
        }

        #endregion

        #region Properties

        public byte[] TemplateFileData { get; set; }
        public ConfigInfo ConfigInfo { get; set; }

        public IDictionary<string, object> ParameterData { get; set; }
        private byte[] OutputData { get; set; }

        public Action<ExcelWorksheet, ExcelCellBase, FieldInfo> AfterFillParameter { get; set; }
        public Action<ExcelWorksheet> PrepareTemplate { get; set; }
        public Action<ExcelWorksheet> AfterFillAllData { get; set; }

        #endregion

        #region Methods

        public byte[] Export<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            //1. Read Config
            this.ReadConfig();

            //2. Fill Paremter
            this.FillParameter();

            //3. Export
            this.FillData(entities);

            return this.OutputData;
        }

        private void ReadConfig()
        {
            this.ConfigInfo = new ConfigInfo();

            //1. Read data file
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                //Open Excel + Get WorkSheet
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();

                //Prepare Template
                if (PrepareTemplate != null)
                    PrepareTemplate(m_ExcelWorksheet);

                //Get Config
                var m_Dimension = m_ExcelWorksheet.Dimension;
                var m_Cells = m_ExcelWorksheet.Cells;
                for (int m_RowIndex = 1; m_RowIndex <= m_Dimension.Rows; m_RowIndex++)
                {
                    for (int m_ColumnIndex = 1; m_ColumnIndex <= m_Dimension.Columns; m_ColumnIndex++)
                    {
                        var m_Cell = m_Cells[m_RowIndex, m_ColumnIndex];
                        string m_Text = m_Cell.Text;

                        var m_FieldInfo = this.ParseConfig(m_Text);
                        if (m_FieldInfo != null)
                        {
                            m_FieldInfo.ExcelAddress = m_Cell.Address;
                            m_FieldInfo.ExcelRow = m_RowIndex;
                            m_FieldInfo.ExcelColumn = m_ColumnIndex;
                            this.ConfigInfo.Fields.Add(m_FieldInfo);
                        }
                    }
                }

                this.TemplateFileData = m_ExcelPackage.GetAsByteArray();
            }
        }

        private FieldInfo ParseConfig(string text)
        {
            FieldInfo m_FieldInfo = null;

            if (text.Contains(Key_Start) && text.Contains(Key_End))
            {
                string m_TextNoKey = text.Replace(Key_Start, string.Empty).Replace(Key_End, string.Empty);
                string[] m_TextNoKeyParts = m_TextNoKey.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (m_TextNoKeyParts.Length == 2)
                {
                    m_FieldInfo = new FieldInfo()
                    {
                        Type = m_TextNoKeyParts[0],
                        Name = m_TextNoKeyParts[1]
                    };
                }
            }
            else
                m_FieldInfo = null;

            return m_FieldInfo;
        }
        private void FillParameter()
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();

                using (var m_Cells = m_ExcelWorksheet.Cells)
                {
                    FieldInfo[] m_FieldInfos = this.ConfigInfo.Fields.Where(f => f.Type == KeyType_Parameter).ToArray();
                    foreach (var m_FieldInfo in m_FieldInfos)
                    {
                        object m_Value = string.Empty;
                        if (this.ParameterData.TryGetValue(m_FieldInfo.Name, out m_Value))
                        {
                            using (var m_Cell = m_Cells[m_FieldInfo.ExcelAddress])
                            {
                                m_Cell.Value = m_Value;

                                if (AfterFillParameter != null)
                                    AfterFillParameter(m_ExcelWorksheet, m_Cell, m_FieldInfo);
                            }
                        }
                    }
                }

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            };
        }
        private void FillData<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                using (var m_MemoryStream = new MemoryStream(this.OutputData))
                {
                    m_ExcelPackage.Load(m_MemoryStream);
                }
                ExcelWorksheet m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.First();
                using (var m_Cells = m_ExcelWorksheet.Cells)
                {
                    FieldInfo[] m_FieldInfos = this.ConfigInfo.Fields.Where(f => f.Type == KeyType_Field).ToArray();
                    if (m_FieldInfos.Length > 0)
                    {
                        int m_RowBeginIndex = m_FieldInfos.FirstOrDefault().ExcelRow;
                        //Insert Zone
                        m_ExcelWorksheet.InsertRow(m_RowBeginIndex + 1, entities.Count - 1, m_RowBeginIndex);

                        //Fill
                        int m_RowIndex = m_RowBeginIndex;
                        Type m_EntityType = typeof(TEntity);
                        foreach (var m_Entity in entities)
                        {
                            foreach (var m_FieldInfo in m_FieldInfos)
                            {
                                var m_Value = ReflectorUtility.FollowPropertyPath(m_Entity, m_FieldInfo.Name);
                                m_Cells[m_RowIndex, m_FieldInfo.ExcelColumn].Value = m_Value;
                                //PropertyInfo m_PropertyInfo = m_EntityType.GetProperty(m_FieldInfo.Name);
                                //if (m_PropertyInfo != null)
                                //{
                                //    object m_Value = m_PropertyInfo.GetValue(m_Entity);
                                //    m_Cells[m_RowIndex, m_FieldInfo.ExcelColumn].Value = m_Value;
                                //}
                            }
                            m_RowIndex++;
                        }

                        if (AfterFillAllData != null)
                            this.AfterFillAllData(m_ExcelWorksheet);
                    }
                }

                this.OutputData = m_ExcelPackage.GetAsByteArray();
            };
        }

        public object[,] ReadData(byte[] data)
        {
            object[,] m_DataOutput = null;

            //2. Import Excel
            using (var stream = new MemoryStream(data))
            {
                m_DataOutput = this.ReadData(stream, null);
            }

            return m_DataOutput;
        }
        public object[,] ReadData(byte[] data, SheetInfo sheetInfo)
        {
            object[,] m_DataOutput = null;

            //2. Import Excel
            using (var stream = new MemoryStream(data))
            {
                m_DataOutput = this.ReadData(stream, sheetInfo);
            }

            return m_DataOutput;
        }
        public object[,] ReadData(Stream stream)
        {
            return this.ReadData(stream, null);
        }
        public object[,] ReadData(Stream stream, SheetInfo sheetInfo)
        {
            object[,] m_DataOutput = null;

            //2. Import Excel
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                m_ExcelPackage.Load(stream);

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                else
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();

                if (m_ExcelWorksheet != null)
                {
                    var m_Dimension = m_ExcelWorksheet.Dimension;
                    var m_Cells = m_ExcelWorksheet.Cells;
                    m_DataOutput = new object[m_Dimension.Rows, m_Dimension.Columns];
                    for (int m_RowIndex = 0; m_RowIndex < m_Dimension.Rows; m_RowIndex++)
                    {
                        for (int m_ColumnIndex = 0; m_ColumnIndex < m_Dimension.Columns; m_ColumnIndex++)
                        {
                            m_DataOutput[m_RowIndex, m_ColumnIndex] = m_Cells[m_RowIndex + 1, m_ColumnIndex + 1].Value;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
            }

            return m_DataOutput;
        }
        public Dictionary<string, object> ReadCellData(byte[] data)
        {
            return this.ReadCellData(data, null);
        }
        public Dictionary<string, object> ReadCellData(byte[] data, SheetInfo sheetInfo)
        {
            Dictionary<string, object> m_DataOutput = new Dictionary<string, object>();
            using (var stream = new MemoryStream(data))
            {
                m_DataOutput = this.ReadCellData(data, sheetInfo);
            }
            return m_DataOutput;
        }
        public Dictionary<string, object> ReadCellData(Stream stream)
        {
            return this.ReadCellData(stream, null);
        }
        public Dictionary<string, object> ReadCellData(Stream stream, SheetInfo sheetInfo)
        {
            Dictionary<string, object> m_DataOutput = new Dictionary<string, object>();
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                m_ExcelPackage.Load(stream);

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                else
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();

                if (m_ExcelWorksheet != null)
                {
                    //Get Define Name
                    foreach (var DefineName in m_ExcelPackage.Workbook.Names)
                    {
                        m_DataOutput.Add(DefineName.Name, m_ExcelWorksheet.Cells[DefineName.Start.Row, DefineName.Start.Column].Text);
                        DefineName.Dispose();
                    }

                    //Get Cells
                    foreach (var cell in m_ExcelWorksheet.Cells)
                        m_DataOutput.Add(cell.Address, cell.Value);
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }
            }
            return m_DataOutput;
        }

        public byte[] Export(Dictionary<string, string> dicEntities)
        {
            return Export(dicEntities, null);
        }
        public byte[] Export(Dictionary<string, string> dicEntities, SheetInfo sheetInfo)
        {
            byte[] m_OutputData = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                else
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();

                if (m_ExcelWorksheet != null)
                {
                    using (var m_Cells = m_ExcelWorksheet.Cells)
                    {
                        foreach (var m_dicEntitie in dicEntities)
                        {
                            try
                            {
                                using (var DefineName = m_ExcelPackage.Workbook.Names[m_dicEntitie.Key])
                                {
                                    if (DefineName != null)
                                    {
                                        m_Cells[DefineName.Start.Row, DefineName.Start.Column].Value = m_dicEntitie.Value;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }

                m_OutputData = m_ExcelPackage.GetAsByteArray();
            }
            return m_OutputData;
        }

        public byte[] AddImage(string imageName, string address, int height, int width, byte[] imageBytes)
        {
            return AddImage(imageName, null, address, height, width, imageBytes);
        }
        public byte[] AddImage(string imageName, SheetInfo sheetInfo, string address, int height, int width, byte[] imageBytes)
        {
            byte[] m_DataOutput = null;
            using (ExcelPackage m_ExcelPackage = new ExcelPackage())
            {
                // Open the Excel file and load it to the ExcelPackage
                using (var stream = new MemoryStream(this.TemplateFileData))
                {
                    m_ExcelPackage.Load(stream);
                }

                ExcelWorksheet m_ExcelWorksheet = null;
                if (sheetInfo != null)
                    m_ExcelWorksheet = sheetInfo.SheetIndex > 0 ? m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetIndex] : m_ExcelPackage.Workbook.Worksheets[sheetInfo.SheetName];
                else
                    m_ExcelWorksheet = m_ExcelPackage.Workbook.Worksheets.FirstOrDefault();

                if (m_ExcelWorksheet != null)
                {
                    Bitmap image;
                    using (var img = new MemoryStream(imageBytes))
                    {
                        image = new Bitmap(img);
                        ExcelPicture excelImage = null;
                        if (image != null)
                        {
                            using (var DefineName = m_ExcelPackage.Workbook.Names[address])
                            {
                                if (DefineName != null)
                                {
                                    excelImage = m_ExcelWorksheet.Drawings.AddPicture(imageName, image);
                                    excelImage.From.Column = DefineName.Start.Column;
                                    excelImage.From.Row = DefineName.Start.Row;
                                    excelImage.SetSize(width, height);
                                    // 2x2 px space for better alignment
                                    excelImage.From.ColumnOff = Pixel2MTU(2);
                                    excelImage.From.RowOff = Pixel2MTU(2);
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Không tìm thấy Sheet tương ứng");
                }

                m_DataOutput = m_ExcelPackage.GetAsByteArray();
            }
            return m_DataOutput;
        }

        #endregion

        #region Utilities
        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }
        #endregion

        #region Constants

        public const string Key_Start = "[[%";
        public const string Key_End = "%]]";
        public const string Key_Seperator = ":";

        public const string KeyType_Parameter = "Parameter";
        public const string KeyType_Field = "Field";

        #endregion
    }

    public class ConfigInfo
    {
        public ConfigInfo()
        {
            this.Fields = new List<FieldInfo>();
        }
        public IList<FieldInfo> Fields { get; set; }
    }

    public class FieldInfo
    {
        public string Name { get; set; }
        public string ExcelAddress { get; set; }
        public int ExcelRow { get; set; }
        public int ExcelColumn { get; set; }
        public string Type { get; set; }
    }

    public class SheetInfo
    {
        public string SheetName { get; set; }
        public int SheetIndex { get; set; }
    }
}
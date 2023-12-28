using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace SQL_Generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Column Name";
            dataGridView1.Columns[1].Name = "Column Name CN";
            dataGridView1.Columns[2].Name = "Column Type";
            dataGridView1.Columns[3].Name = "PrimaryKey";             
            dataGridView1.Columns[4].Name = "Comment";

            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.ReadOnly = false;

            this.dataGridView1.Rows.Add(1);
            dataGridView1.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dataGridView1_RowPostPaint);
            
            dataGridView(dataGridView1);

            LoadHighlightingProvider();
            textEditorControl1.Visible = false;
            BtnCloseSqlWin.Visible = false;

        }

        private void LoadHighlightingProvider()
        {
            FileSyntaxModeProvider syntaxModeFileProvider = new FileSyntaxModeProvider(Path.GetDirectoryName(base.GetType().Assembly.Location));
            HighlightingManager.Manager.AddSyntaxModeFileProvider(syntaxModeFileProvider);
            textEditorControl1.SetHighlighting("Java");
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // 绘制行前的删除按钮，需要自定义绘制逻辑，这里仅作示例  
            //e.DrawSeparator(new Rectangle(e.RowBounds.Left, e.RowBounds.Bottom - 20, 20, 20), Color.Black);
            //e.DrawText(new Rectangle(e.RowBounds.Left + 20, e.RowBounds.Bottom - 15, e.RowBounds.Width - 40, 20), "删除", new Font("Arial", 10), Color.Red, HorizontalAlignment.Center);

        }

        private void copyClipboardTexttoGrid(string data)
        {
            string[] rows = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string[] cols;
            int rowStart = 0, columnStart = 0, i = 0, j = 0;
            if (this.dataGridView1.SelectedCells.Count > 0)
            {
                rowStart = this.dataGridView1.SelectedCells[0].RowIndex;
                columnStart = this.dataGridView1.SelectedCells[0].ColumnIndex;
            }
            int count = rowStart + rows.Length - this.dataGridView1.RowCount;
            if (count >= 0)
            {
                this.dataGridView1.Rows.Add(count + 1);
            }
            for (i = 0; i < rows.Length && i + rowStart < this.dataGridView1.RowCount; i++)
            {
                cols = rows[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                for (j = 0; j < cols.Length && j + columnStart < this.dataGridView1.ColumnCount; j++)
                {
                    this.dataGridView1.Rows[i + rowStart].Cells[j + columnStart].Value = cols[j];
                }
            }
            this.dataGridView1.ClearSelection();

            this.dataGridView1.Rows[i + rowStart - 1].Cells[j + columnStart - 1].Selected = true;
        }

        private void dataGridView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V) // 判断是否按下ctrl+v
            {
                //Paste(dataGridView1, "", 0, false);//粘贴代码

                // 获取剪贴板中的数据  
                string clipboardData = Clipboard.GetText();
                copyClipboardTexttoGrid(clipboardData); 



            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            //判断是否有数据
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("请输入字段");
            }

            String tableName = "TableName";
            Boolean nodata_flag = false;
            String primaryKey = "";
            StringBuilder sb = new StringBuilder();
            StringBuilder sbline = new StringBuilder();
            sb.AppendLine("create table "+ tableName+"(");
            for (int i = 0; i< dataGridView1.Rows.Count; i++)
            {
                Boolean notNullRowFlag = false;
                for (int j = 0; j<dataGridView1.Columns.Count; j++)
                {
                    
                    if ( dataGridView1.Rows[i].Cells[j].Value == null)
                    {
                        notNullRowFlag = true;
                    }                       
                }

                //如果为空则不处理
                if ( !notNullRowFlag)
                {
                    String columnName = dataGridView1.Rows[i].Cells[0].Value.ToString().ToUpper();
                    String columnNameCn = dataGridView1.Rows[i].Cells[1].Value.ToString() ;
                    String columnType = dataGridView1.Rows[i].Cells[2].Value.ToString().ToUpper();
                    //类型转换
                    columnType = columnType.Replace("VARCHAR2", "VARCHAR").Replace("NUMBER", "NUMERIC").Replace("CHAR2", "CHAR");
                    sb.AppendLine("\t" + columnName + "\t" + columnType + ",    --" + columnNameCn);

                    if ("Y".Equals(dataGridView1.Rows[i].Cells[3].Value.ToString()))
                    {
                        primaryKey = primaryKey + "," + columnName;
                    }

                    sbline.AppendLine("comment on column " + tableName + "." + columnName + " is '" + columnNameCn + "';");
                }


            }
            //生成主键
            if (!String.IsNullOrEmpty(primaryKey)) 
            {
                sb.AppendLine("\t constraint PK_"+ tableName + " primary key("+ primaryKey.Substring(1)+")");
            }
            sb.AppendLine(");");

            //生成注释
            sb.AppendLine("comment on table "+ tableName + " is 'xx表 ';");
            sb.AppendLine(sbline.ToString());


            textEditorControl1.Text = sb.ToString();
            textEditorControl1.Visible = true;
            BtnCloseSqlWin.Visible = true;
            BtnCloseSqlWin.Location = new System.Drawing.Point(textEditorControl1.Width - 200 , textEditorControl1.Location.Y + 10);

        }

        private void dataGridView(DataGridView dataGridView)
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridView.AllowUserToAddRows = true;
            dataGridView.AllowUserToDeleteRows = true;
            
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
            dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;//211, 223, 240dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
             
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            //dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.RowTemplate.Height = 23;
            //dataGridView.RowTemplate.ReadOnly = true;
           // dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                // 设置列标题背景色为黑色  
                column.HeaderCell.Style.BackColor = Color.Black;
                // 设置列标题文字颜色为白色  
                column.HeaderCell.Style.ForeColor = Color.White;
                //column.HeaderCell.Style.Font = new Font(column.HeaderCell.Style.Font.FontFamily, 16, FontStyle.Regular);
                
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnCloseSqlWin_Click(object sender, EventArgs e)
        {
            textEditorControl1.Visible = false;
            BtnCloseSqlWin.Visible = false;
        }

        private void textEditorControl1_SizeChanged(object sender, EventArgs e)
        {
            BtnCloseSqlWin.Location = new System.Drawing.Point(textEditorControl1.Width - 200, textEditorControl1.Location.Y + 10);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Generator
{
    public partial class FrmDialogTableInfo : Form
    {
        String tableName;
        String tabledesc;
        String tableSchema;


        public FrmDialogTableInfo()
        {
            InitializeComponent();
        }

        public string TableName { get => tableName; set => tableName = value; }
        public string Tabledesc { get => tabledesc; set => tabledesc = value; }
        public string TableSchema { get => tableSchema; set => tableSchema = value; }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            //关闭form
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            //检查三个text是否为空
            if (string.IsNullOrEmpty(textTableName.Text))
            {
                MessageBox.Show("请输入表名");
                return;
            }
            TableName = textTableName.Text;
            tabledesc = textTableDesc.Text;
            tableSchema = textTableSchema.Text;
            this.DialogResult = DialogResult.OK;
            this.Close ();
        }

        private void FrmDialogTableInfo_Load(object sender, EventArgs e)
        {

        }
    }
}

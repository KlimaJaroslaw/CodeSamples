using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MyComponents.DynamicButtonForm;

namespace MyComponents.DynamicButtonForm
{
    public partial class DynamicButtonForm : DevExpress.XtraEditors.XtraForm
    {
        private object choosen;        

        #region Construction
        public DynamicButtonForm()
        {
            InitializeComponent();
        }

        public void Initilize<T>(List<T> sourceList_, string propertyName_, string caption_, 
                                 int itemWidth_ = 200, int itemHeight_ = 80)
        {
            this.LoadSource<T>(sourceList_, propertyName_);
            this.lbCaption.Text = caption_;
            this.viewMain.OptionsTiles.ItemSize = new Size(itemWidth_, itemHeight_);            
        }        
        #endregion

        #region Buttons
        private void viewMain_DoubleClick(object sender, EventArgs e)
        {
            Choose();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btChoose_Click(object sender, EventArgs e)
        {
            Choose();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region Methods
        #region Public        
        public void Select<T>(T instance, string display=default)
        {
            if (instance == null && display == default)
                return;

            List<ASButtonDisplayer<T>> list = this.gridMain.DataSource as List<ASButtonDisplayer<T>>;
            int index = list.FindIndex(x => EqualityComparer<T>.Default.Equals(x.ObjectData, instance) || x.PropertyToDisplay==display);
            this.Select(index);
        }

        public void Select(int index)
        {     
            viewMain.FocusedRowHandle = index;
        }

        public T GetChoosen<T>()
        {
            ASButtonDisplayer<T> s = this.choosen as ASButtonDisplayer<T>;
            if (s == null)
                return default;
            else
                return s.ObjectData;
        }
        #endregion

        #region Private
        protected void LoadSource<T>(List<T> feed, string propertyName)
        {
            List<ASButtonDisplayer<T>> source = new List<ASButtonDisplayer<T>>();
            for (int i = 0; i < feed.Count; i++)
            {
                ASButtonDisplayer<T> item = new ASButtonDisplayer<T>(feed[i], propertyName);
                source.Add(item);
            }
            this.gridMain.DataSource = source;
        }        
        private void Choose()
        {
            this.choosen = viewMain.GetRow(viewMain.FocusedRowHandle);
        }        
        #endregion
        #endregion
    }
}
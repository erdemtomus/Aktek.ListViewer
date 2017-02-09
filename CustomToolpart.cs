using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;


namespace Aktek.List
{
    class CustomToolpart : ToolPart
    {
        private DropDownList ddlRootSiteLists;
        private DropDownList ddlSiteLists;
        private DropDownList ddlConfigListItems;
        //private TextBox txtTemplateRepeatingPart;
        //private TextBox txtTemplateRepeatingPartAlternative;
        //private TextBox txtNonRepeatingPart;
        //private TextBox txtColumnCount;
        //private TextBox txtFilters;
        //private TextBox txtMaxItems;

        protected string topListName;
        

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CreateControls();
        }

        public override void ApplyChanges()
        {
            EnsureChildControls();
            SendDataToWebPart();
        }

        private void SendDataToWebPart()
        {
            EnsureChildControls();
            ListWP.ListWP customWebPart = (ListWP.ListWP)this.ParentToolPane.SelectedWebPart;
            

            // Send the custom text to the Web Part.
            if (ddlRootSiteLists != null)
            {
                customWebPart.PconfigListName = ddlRootSiteLists.SelectedValue;
                topListName = customWebPart.PconfigListName;
            }
            if (ddlSiteLists != null)
                customWebPart.PcontentListName = ddlSiteLists.SelectedValue;
            if (ddlConfigListItems != null)
                customWebPart.PconfigListItems = ddlConfigListItems.SelectedValue;

            //customWebPart.PcolumnCount = "2";


        }

        public override void SyncChanges()
        {
            base.SyncChanges();

        }

        private void SetValues()
        {
            
            ListItem item = null;
            ListWP.ListWP customWebPart = (ListWP.ListWP)this.ParentToolPane.SelectedWebPart;

            if (!string.IsNullOrEmpty(customWebPart.PconfigListName))
            {
                item = ddlRootSiteLists.Items.FindByValue(customWebPart.PconfigListName);
                if (item != null)
                {
                    ddlRootSiteLists.SelectedValue = item.Value;
                    topListName = item.Value;
                }

            }

            if (!string.IsNullOrEmpty(customWebPart.PcontentListName))
            {
                item = ddlSiteLists.Items.FindByValue(customWebPart.PcontentListName);
                if (item != null)
                {
                    ddlSiteLists.SelectedValue = item.Value;
                }
            }

            if (!string.IsNullOrEmpty(customWebPart.PconfigListItems))
            {
                item = ddlConfigListItems.Items.FindByValue(customWebPart.PconfigListItems);
                if (item != null)
                {
                    if (ddlConfigListItems.Items != null && ddlConfigListItems.Items.Count > 0)
                        ddlConfigListItems.SelectedValue = item.Value;
                   
                }
                else
                {
                    FillSubComboOfRootItems(topListName);
                    ddlConfigListItems.SelectedValue = customWebPart.PconfigListItems;
                }
            }
            


        }

        public override void CancelChanges()
        {
            base.CancelChanges();
        }


        protected override void RenderToolPart(System.Web.UI.HtmlTextWriter output)
        {
            base.RenderToolPart(output);
        }



        public void CreateControls()
        {

            ddlRootSiteLists = new DropDownList { AutoPostBack = true };

            ddlRootSiteLists.SelectedIndexChanged += new EventHandler(ddlRootSiteLists_SelectedIndexChanged);          
            SPListCollection lists = SPContext.Current.Site.RootWeb.Lists;
            foreach (SPList list in lists)
                ddlRootSiteLists.Items.Add(list.Title);

            

            ddlSiteLists = new DropDownList();
            SPListCollection listsC = SPContext.Current.Web.Lists;
            foreach (SPList lst in listsC)
            {
                ddlSiteLists.Items.Add(lst.Title);
            }

            ddlConfigListItems = new DropDownList();
            FillSubComboOfRootItems(topListName);
            
            AddControls();
            SetValues();

        }


        

        void ddlRootSiteLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlis = (DropDownList)sender;
            FillSubComboOfRootItems(ddlis.SelectedValue.ToString());

        }

        private void FillSubComboOfRootItems(string mainList)
        {
            try
            {
                ddlConfigListItems.Items.Clear();
                string selected = mainList;
                SPListItemCollection litC = SPContext.Current.Site.RootWeb.Lists[selected].Items;

                foreach (SPListItem it in litC)
                {
                    ddlConfigListItems.Items.Add(it.Title);
                }
            }
            catch (Exception)
            {
                ddlConfigListItems.Items.Add("Error On SelectedIndexChanged");
            }
        }

        private void AddControls()
        {
            Table table = new Table();
            TableRow tr = new TableRow();
            TableCell td = new TableCell();
            Literal ltStatic = new Literal();
            
            #region Add a Control
            ltStatic.Text = "General Config Data From";
            td.Controls.Add(ltStatic);
            tr.Cells.Add(td);
            table.Rows.Add(tr);
            tr = new TableRow();
            td = new TableCell();
            td.Controls.Add(ddlRootSiteLists);
            tr.Cells.Add(td);
            table.Rows.Add(tr);
            #endregion


            #region Add a Control
            Literal ltStatic3 = new Literal();
            ltStatic3.Text = "</br>Load Config Item From";
            td.Controls.Add(ltStatic3);
            tr.Cells.Add(td);
            table.Rows.Add(tr);
            tr = new TableRow();
            td = new TableCell();
            td.Controls.Add(ddlConfigListItems);
            tr.Cells.Add(td);
            table.Rows.Add(tr);
            #endregion


            #region Add a Control
            Literal ltStatic2 = new Literal();
            ltStatic2.Text = "</br>Load Content Data From";
            td.Controls.Add(ltStatic2);
            tr.Cells.Add(td);
            table.Rows.Add(tr);
            tr = new TableRow();
            td = new TableCell();
            td.Controls.Add(ddlSiteLists);
            tr.Cells.Add(td);
            table.Rows.Add(tr);
            #endregion

            

            String sTitle = "Aktek's Configuration";
            this.Controls.Add(this.GetPropertyPanel(table, sTitle));
        }


        


    }
}

using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Aktek.List.ListWP
{
    [ToolboxItemAttribute(false)]
    public class ListWP : Microsoft.SharePoint.WebPartPages.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Aktek.List/ListWP/ListWPUserControl.ascx";

        string _clistName; // PconfigListName temel ayarları alacağı yer
        public string PconfigListName
        {
            get
            {
                return _clistName;
            }
            set
            {
                _clistName = value;
            }
        }

        string _clistItems; // PconfigListItems temel ayarlardaki itemlar geliyor
        public string PconfigListItems
        {
            get
            {
                return _clistItems;
            }
            set
            {
                _clistItems = value;
            }
        }


        string _cContentlistName; // PconfigListName temel ayarları alacağı yer
        public string PcontentListName
        {
            get
            {
                return _cContentlistName;
            }
            set
            {
                _cContentlistName = value;
            }
        }

        #region not needed now...
        //string _txtTemplateRepeatingPart;
        //string _txtTemplateRepeatingPartAlternative;
        //string _txtNonRepeatingPart;
        //string _txtColumnCount;
        //string _txtFilters;
        //string _txtMaxItems;

        //public string PtemplateRepeatingPart
        //{
        //    get
        //    {
        //        return _txtTemplateRepeatingPart;
        //    }
        //    set
        //    {
        //        _txtTemplateRepeatingPart = value;
        //    }
        //}

        //public string PtemplateRepeatingPartAlternative
        //{
        //    get
        //    {
        //        return _txtTemplateRepeatingPartAlternative;
        //    }
        //    set
        //    {
        //        _txtTemplateRepeatingPartAlternative = value;
        //    }
        //}

        //public string PnonRepeatingPart
        //{
        //    get
        //    {
        //        return _txtNonRepeatingPart;
        //    }
        //    set
        //    {
        //        _txtNonRepeatingPart = value;
        //    }
        //}

        //public string PcolumnCount
        //{
        //    get
        //    {
        //        return _txtColumnCount;
        //    }
        //    set
        //    {
        //        _txtColumnCount = value;
        //    }
        //}

        //public string Pfilters
        //{
        //    get
        //    {
        //        return _txtFilters;
        //    }
        //    set
        //    {
        //        _txtFilters = value;
        //    }
        //}

        // public string PmaxItems
        //{
        //    get
        //    {
        //        return _txtMaxItems;
        //    }
        //    set
        //    {
        //        _txtMaxItems = value;
        //    }
        //}
        
        #endregion


        protected override void CreateChildControls()
        {
            Control control = this.Page.LoadControl(_ascxPath);
            
            ListWPUserControl pwc = control as ListWPUserControl;
            this.Controls.Add(pwc);

            pwc.ListItems(PconfigListName, PconfigListItems, PcontentListName);
            

        }

        public override ToolPart[] GetToolParts()
        {
            ToolPart[] allToolParts = new ToolPart[3];
            WebPartToolPart standardToolParts = new WebPartToolPart();
            CustomPropertyToolPart customToolParts = new CustomPropertyToolPart();
            

            allToolParts[0] = standardToolParts;
            allToolParts[1] = customToolParts;
            allToolParts[2] = new CustomToolpart();

            return allToolParts;
        }

    }
}

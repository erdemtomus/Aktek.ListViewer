using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Reflection;
using Microsoft.SharePoint;

namespace Aktek.List
{
    public static class CustomExtensions
    {
        public static Panel GetPropertyPanel(this ToolPart currentToolPart, Table table, String sTitle)
        {
            Panel controlPanel = new Panel();
            controlPanel.ID = "propertyPanelHideDisplay";
            controlPanel.Attributes.Add("id", currentToolPart.ClientID + "_" + controlPanel.ID);
            controlPanel.Controls.Add(table);

            Literal lt = new Literal();
            String sScript = "<script language='javascript'>\n" +
                            " var objDiv = document.getElementById('" + currentToolPart.ClientID + "_" + controlPanel.ID + "');\n" +
                            " objDiv.parentNode.parentNode.parentNode.attributes.removeNamedItem('colspan');\n" +
                            " objDiv.parentNode.parentNode.parentNode.attributes.removeNamedItem('class'); \n" +
                            "</script>";
            lt.Text = sScript;
            controlPanel.Controls.Add(lt);

            Type type = typeof(SPSite);
            Assembly assembly = type.Assembly;

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public |BindingFlags.NonPublic;

            Panel propertyPanel = (Panel)assembly.CreateInstance("Microsoft.SharePoint.WebPartPages.TPPanel", false, bindingFlags,null,
               new object[] { sTitle, controlPanel, true }, null, null);

            return propertyPanel;
        }

    }
}

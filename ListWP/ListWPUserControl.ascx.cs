using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Linq;

namespace Aktek.List.ListWP
{
    public partial class ListWPUserControl : UserControl
    {
        protected string prefix = "";
        protected bool navigationShow = true;
        protected int currentPage = 1;
        protected int listCount;
        protected string orderField;
        protected int listSize; //sayfa başına item sayısı

        protected string errorStr;
        protected SPList myList;
        protected string contentListName;
        protected string TemplateRepeatingPart;
        protected string TemplateRepeatingPartAlternative;
        protected string NonRepeatingPart;
        protected int ColumnCount;
        protected string Filters;
        protected string OrderBy;
        protected string CharLimit;
        protected int MaxItems;
        protected string outputHTML = "";
        protected int totalPage = 1;
        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            prefix = this.UniqueID.ToString();
            navigationShow = true;
        }

        public void ListItems(string configListName, string configListItemSelected,string contentListName)
        {

            try
            {
                SPList configList = SPContext.Current.Site.RootWeb.Lists[configListName];
                //SPListItem listItem = litC..geti
                SPListItem item = null;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SPQuery query = new SPQuery();
                    query.Query = String.Format("<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where>", configListItemSelected);
                    query.RowLimit = 1;
                    SPListItemCollection items = configList.GetItems(query);
                    item = items[0];
                });

                
                //configuration list columns
                this.TemplateRepeatingPart = (item["TemplateRepeatingPart"] != null ? item["TemplateRepeatingPart"].ToString() : "");
                this.TemplateRepeatingPartAlternative = (item["TemplateRepeatingPartAlternative"] != null ? item["TemplateRepeatingPartAlternative"].ToString() : "");
                this.NonRepeatingPart = (item["NonRepeatingPart"] != null ? item["NonRepeatingPart"].ToString() : "<!>CONTENT</!>");
                this.ColumnCount = (item["ColumnCount"] != null ? Convert.ToInt32(item["ColumnCount"].ToString()) : 0);
                this.Filters = (item["Filters"] != null ? item["Filters"].ToString() : "");
                this.OrderBy = (item["OrderBy"] != null ? item["OrderBy"].ToString() : "");
                this.CharLimit = (item["CharLimit"] != null ? item["CharLimit"].ToString() : "");
                this.MaxItems = (item["MaxItems"] != null ? Convert.ToInt32(item["MaxItems"].ToString()) : 0);

                this.contentListName = contentListName;
                MakeRender();
            }
            catch (Exception)
            {
                
            }

        }

        public void MakeRender()
        {
            SPWeb myweb = null;
            try
            {
                SPSite currentSiteCollection = new SPSite(SPContext.Current.Web.Url);
                SPWeb currentWebSite = currentSiteCollection.OpenWeb();
                prefix = this.UniqueID.ToString();
                SPSite mySiteCol = new SPSite(SPContext.Current.Web.Url);


                myweb = mySiteCol.OpenWeb();
                myList = myweb.Lists[contentListName]; //içerik geldi...


                SPQuery query = new SPQuery();
                string prepareOrderQuery = "";

                if(!String.IsNullOrEmpty(this.OrderBy))
                    prepareOrderQuery = this.OrderBy;
                
                
                #region Where
                string prepareWhereQuery = "";
                if (!String.IsNullOrEmpty(this.Filters))
                    prepareWhereQuery = this.Filters;
                
                query.Query = String.Format("{0}{1}", prepareWhereQuery, prepareOrderQuery);

                #endregion
                SPListItemCollection collection = null;
                collection = myList.GetItems(query);

                    if (MaxItems == 0)
                    {
                        listCount = collection.Count;
                    }
                    else
                    {
                        if (MaxItems < collection.Count)
                            listCount = MaxItems;
                        else
                            listCount = collection.Count;
                    }

                    if (listSize == 0) //listSize = sayfadaki item sayısı
                    {
                        navigationShow = false;
                        listSize = listCount;
                    }
                    if (listSize == listCount)
                    {
                        navigationShow = false;
                    }
                    if (listSize > listCount)
                        navigationShow = false;


                    AdjustCurrentandTotalPage();
                    Page.DataBind();
                    int controlListSize = 0;
                    //outputHTML = "<tr> ";


                    for (int i = ((currentPage - 1) * listSize); ; i++)
                    {
                        if (controlListSize == listSize || i == listCount)
                        {
                            break;
                        }

                        if (!string.IsNullOrEmpty(TemplateRepeatingPartAlternative))
                        {
                            if (((i + 1) % ColumnCount) == 0)
                            {
                                outputHTML = String.Format("{0} {1}", outputHTML, GetHtml(collection[i], TemplateRepeatingPartAlternative));
                            }
                            else
                            {
                                outputHTML = String.Format("{0} {1}", outputHTML, GetHtml(collection[i], TemplateRepeatingPart));
                            }
                        }
                        else
                        {
                            outputHTML = String.Format("{0} {1}", outputHTML, GetHtml(collection[i], TemplateRepeatingPart));
                        }


                        //outputHTML = String.Format("{0} <td> {1} </td>", outputHTML, GetHtml(collection[i], templateFileName));



                        controlListSize++;
                     
                    }

                    outputHTML = NonRepeatingPart.Replace("<!>CONTENT</!>", outputHTML);
                  

               

            }
            catch (Exception ex)
            {
                //ExceptionPolicy.HandleException(ex, "Log Only Policy");
                errorStr = "Bilinmeyen Bir Hata oluştu." + ex.ToString();
            }
            finally
            {
                if (myweb != null)
                {
                    myweb.Dispose();
                    myweb = null;
                }
            }

        }



        public string GetHtml(SPListItem item, string templateFileName)
        {
            string templateParameters = "";

            try
            {
                SPSite currentSiteCollection = new SPSite(SPContext.Current.Web.Url);
                SPWeb currentWebSite = currentSiteCollection.OpenWeb();

                SPAttachmentCollection spac = null;
                try
                {
                    spac = item.Attachments;
                }
                catch
                {
                    spac = null;
                }

                string firstAttachmentUrl = "";
                string secondAttachmentUrl = "";
                string thirdAttachmentUrl = "";
                string secondAttachmentTitle = "";

                firstAttachmentUrl = (spac != null && spac.Count > 0) ? (item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[0]) : "";
                secondAttachmentUrl = (spac != null && spac.Count > 1) ? (item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[1]) : "";
                thirdAttachmentUrl = (spac != null && spac.Count > 2) ? (item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[2]) : "";
                if (templateFileName.LastIndexOf("\\") > -1)
                {
                    if (((templateFileName.Substring((templateFileName.LastIndexOf("\\")) + 1)).Split('/')[0]).LastIndexOf(templateFileName.ToString()) > -1)
                    {
                        firstAttachmentUrl = (spac != null && spac.Count > 0) ? (item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[0]) : "";
                    }
                    else
                    {
                        for (int i = 0; i < spac.Count; i++)
                        {
                            if (spac[i].Contains(".pdf") || spac[i].Contains(".zip"))
                            {
                                firstAttachmentUrl = (spac != null && spac.Count > 0) ? (item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[i]) : "";
                                break;
                            }
                        }
                        for (int i = 0; i < spac.Count; i++)
                        {
                            if (spac[i].Contains(".jpg") || spac[i].Contains(".gif"))
                            {
                                secondAttachmentUrl = (spac != null && spac.Count > 1) ? (item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[i]) : "";
                                secondAttachmentTitle = spac[i].Split(new char[] { '.' })[0];
                                break;
                            }
                        }
                    }

                }
                else
                {
                    firstAttachmentUrl = (spac != null && spac.Count > 0) ? (item.ParentList.ParentWeb.Url + item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[0]) : "";
                    secondAttachmentUrl = (spac != null && spac.Count > 1) ? (item.ParentList.ParentWeb.Url + item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[1]) : "";
                    thirdAttachmentUrl = (spac != null && spac.Count > 2) ? (item.ParentList.ParentWeb.Url + item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/Attachments/" + item.ID + "/" + spac[2]) : "";
                }

                if (spac != null)
                {
                    try
                    {
                        templateParameters = String.Format("<?xml version=\"1.0\" encoding=\"iso-8859-9\"?><parameters><imageSource>{0}</imageSource><listItemUrl>{1}</listItemUrl><attachmentUrl>{2}</attachmentUrl><attachment2Url>{3}</attachment2Url><attachment3Url>{4}</attachment3Url></parameters>",
                            Util.FormatXmlText(""),
                            (item["URLType"] != null && (item["URLType"]).ToString() == "True") ? Util.FormatXmlText(firstAttachmentUrl) : ((item["URL"] != null) ? item["URL"].ToString() : ""),
                            Util.FormatXmlText(((spac.Count > 0) ? (firstAttachmentUrl) : "")),
                            Util.FormatXmlText(((spac.Count > 0) ? (secondAttachmentUrl) : "")),
                            Util.FormatXmlText(((spac.Count > 0) ? (thirdAttachmentUrl) : "")));


                    }
                    catch
                    {
                        templateParameters = String.Format("<?xml version=\"1.0\" encoding=\"iso-8859-9\"?><parameters><imageSource>{0}</imageSource><listItemUrl>{1}</listItemUrl><attachmentUrl>{2}</attachmentUrl><attachment2Url>{3}</attachment2Url><attachment3Url>{4}</attachment3Url></parameters>",
                            Util.FormatXmlText(""),
                            Util.FormatXmlText(item.ParentList.ParentWeb.Url + item.ParentList.DefaultViewUrl.Substring(0, item.ParentList.DefaultViewUrl.LastIndexOf("/")) + "/DispForm.aspx?ID=" + item.ID),
                            Util.FormatXmlText(((spac.Count > 0) ? (firstAttachmentUrl) : "")),
                            Util.FormatXmlText(((spac.Count > 0) ? (secondAttachmentUrl) : "")),
                            Util.FormatXmlText(((spac.Count > 0) ? (thirdAttachmentUrl) : "")));
                    }
                }
                else
                {

                    string siteUrl = "";
                    try{siteUrl = SPContext.Current.Web.Site.Url.ToString();}
                    catch{}
                        string url = siteUrl;
                    if (siteUrl.EndsWith("/"))
                    {
                        url = siteUrl.Substring(0, siteUrl.Length - 1);
                    }

                    templateParameters = String.Format("<?xml version=\"1.0\" encoding=\"iso-8859-9\"?><parameters><listItemUrl>{0}</listItemUrl></parameters>",
                            Util.FormatXmlText(url + "/" + item.Url));

                }

                return Util.ProcessTemplateWithoutTPL(item, templateFileName, templateParameters, "utf-8");
            }
            catch (Exception ex)
            {
                errorStr = "Bilinmeyen Bir Hata oluştu GetHtml. + templateParameters=" + templateParameters;
            }
            return "";

        }

        private void AdjustCurrentandTotalPage()
        {
            try
            {
                prefix = this.UniqueID.ToString();
                if (Request[prefix + "_PostBack"] == null)
                {
                    currentPage = 1;
                    int res = listCount;
                    if (res == 1)
                    {
                        totalPage = 1;
                    }
                    else
                    {
                        totalPage = (int)((res - 1) / listSize) + 1;
                    }
                }
                else if (Request[prefix + "btnSearch.x"] != null)
                {
                    currentPage = 1;
                    int res = listCount;
                    if (res <= listSize)
                    {
                        totalPage = 1;
                    }
                    else
                    {
                        totalPage = ((int)((res - 1) / listSize)) + 1;
                    }
                }
                else
                {
                    try
                    {
                        currentPage = Convert.ToInt32(Request[prefix + "_CurrentPage"]);
                    }
                    catch
                    {

                    }
                    try
                    {
                        totalPage = Convert.ToInt32(Request[prefix + "_TotalPage"]);
                    }
                    catch
                    {
                    }

                    if (Request["btnFirst_" + prefix + ".x"] != null)
                        currentPage = 1;
                    else if (Request["btnBack_" + prefix + ".x"] != null)
                        currentPage = currentPage - 1;
                    else if (Request["btnNext_" + prefix + ".x"] != null)
                        currentPage = currentPage + 1;
                    else if (Request["btnLast_" + prefix + ".x"] != null)
                        currentPage = totalPage;
                    else if (Request["cmbPage_" + prefix] != null)
                    {
                        if (Request.Form.GetValues("cmbPage_" + prefix)[0] != Request[prefix + "_CurrentPage"])
                            currentPage = Convert.ToInt32(Request.Form.GetValues("cmbPage_" + prefix)[0]);
                    }
                }

            }
            catch (Exception ex)
            {
                errorStr = "Bilinmeyen Bir Hata oluştu";
            }


        }
    }
}

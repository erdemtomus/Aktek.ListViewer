using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;

namespace Aktek.List
{
    class Util
    {
        public static string FormatXmlText(string xmlElement)
        {
            if (xmlElement != null)
            {
                return xmlElement.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("'", "&apos;").Replace("\"", "&quot;");
            }
            else
                return "";
        }

        public static string FormatXmlSqlText(string sqlElement)
        {
            if (sqlElement != null)
            {
                sqlElement = sqlElement.Replace("'", "''");
                sqlElement = sqlElement.Replace("--", "");
                //sqlElement = sqlElement.Replace(";", "");
                //sqlElement = sqlElement.Replace("=", ":");
                return sqlElement;
            }
            else
                return "";
        }

        public static string FormatSqlText(string sqlElement)
        {
            if (sqlElement != null)
            {
                sqlElement = sqlElement.Replace("'", "''");
                sqlElement = sqlElement.Replace("--", "");
                sqlElement = sqlElement.Replace(";", "");
                sqlElement = sqlElement.Replace("=", ":");
                return sqlElement;
            }
            else
                return "";
        }

        public static string ProcessTemplate(SPListItem item, string templateFileName, string templateParameters, string encodingType)
        {
            XmlDocument xmlParam = null;
            try
            {
                string template = Util.GetUrlResponse(templateFileName, false, true, null, null, null, encodingType);

                xmlParam = new XmlDocument();
                xmlParam.LoadXml(templateParameters);

                Regex regex = new Regex(
                    @"<!>\w*:*\w*</!>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );


                Match match = regex.Match(template);
                while (match.Success)
                {
                    int fieldLength = -1;
                    string[] matchTexts = match.ToString().Substring(3, match.Length - 7).Split(':');
                    if (matchTexts.Length > 1)
                    {
                        fieldLength = Convert.ToInt32(matchTexts[1]);
                    }
                    string matchText = matchTexts[0];
                    try
                    {
                        string matchedVal = "";
                        if (fieldLength == -1 || xmlParam.SelectSingleNode("//" + matchText).InnerText.Length <= fieldLength)
                        {
                            matchedVal = xmlParam.SelectSingleNode("//" + matchText).InnerText;
                        }
                        else
                        {
                            matchedVal = xmlParam.SelectSingleNode("//" + matchText).InnerText.Substring(0, fieldLength) + "...";
                        }
                        //modified begum 06.03.2009

                        if (matchTexts[0] == "Created" || matchTexts[0] == "Date" || matchTexts[0] == "HaberTarihi" || matchTexts[0] == "DATE")
                        {
                            if (!String.IsNullOrEmpty(matchedVal))
                            {
                                DateTime _date = Convert.ToDateTime(matchedVal.ToString());
                                matchedVal = _date.ToString("dd/MM/yyyy"); //_date.Day.ToString() + "/" + _date.Month.ToString() + "/" + _date.Year.ToString();
                            }
                            else
                                matchedVal = "";
                        }
                        //modified begum 06.03.2009


                        template = regex.Replace(template, matchedVal, 1);
                    }
                    catch
                    {
                        string matchedVal = "";
                        try
                        {
                            if (fieldLength == -1 || item[matchText].ToString().Length <= fieldLength)
                            {
                                matchedVal = item[matchText].ToString();
                            }
                            else
                            {
                                matchedVal = item[matchText].ToString().Substring(0, fieldLength) + "...";
                            }
                        }
                        catch
                        {
                            matchedVal = "";
                        }
                        //modified begum 06.03.2009
                        if (matchTexts[0] == "Created" || matchTexts[0] == "Date" || matchTexts[0] == "HaberTarihi" || matchTexts[0] == "DATE")
                        {
                            if (!String.IsNullOrEmpty(matchedVal))
                            {
                                DateTime _date = Convert.ToDateTime(matchedVal.ToString());
                                matchedVal = _date.ToString("dd/MM/yyyy");// _date.Day.ToString() + "/" + _date.Month.ToString() + "/" + _date.Year.ToString();
                            }
                            else
                                matchedVal = "";
                        }
                        //modified begum 06.03.2009


                        if (matchTexts[0] == "DATE")
                        {
                            if (!String.IsNullOrEmpty(matchedVal))
                            {
                                DateTime _date = Convert.ToDateTime(matchedVal.ToString());
                                matchedVal = _date.ToString("dd/MM/yyyy"); // _date.Day.ToString() + "/" + _date.Month.ToString() + "/" + _date.Year.ToString() + " " + _date.Hour.ToString() + ":" + _date.Minute.ToString("mm");
                            }
                            else
                                matchedVal = "";
                        }
                        //modified Galip 23.07.2010


                        template = regex.Replace(template, matchedVal, 1);
                    }
                    match = match.NextMatch();
                }



                return template;
            }
            catch (Exception ex)
            {
                //ExceptionPolicy.HandleException(ex, "Log Only Policy");
                return "";
            }
            finally
            {
                xmlParam = null;
            }
        }

        public static string ProcessTemplateWithoutTPL(SPListItem item, string templateFileName, string templateParameters, string encodingType)
        {
            XmlDocument xmlParam = null;
            try
            {
                //string template = Util.GetUrlResponse(templateFileName, false, true, null, null, null, encodingType);
                string template = templateFileName;

                xmlParam = new XmlDocument();
                xmlParam.LoadXml(templateParameters);

                Regex regex = new Regex(
                    @"<!>\w*:*\w*</!>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );


                Match match = regex.Match(template);
                while (match.Success)
                {
                    int fieldLength = -1;
                    string[] matchTexts = match.ToString().Substring(3, match.Length - 7).Split(':');
                    if (matchTexts.Length > 1)
                    {
                        fieldLength = Convert.ToInt32(matchTexts[1]);
                    }
                    string matchText = matchTexts[0];
                    try
                    {
                        string matchedVal = "";
                        if (fieldLength == -1 || xmlParam.SelectSingleNode("//" + matchText).InnerText.Length <= fieldLength)
                        {
                            matchedVal = xmlParam.SelectSingleNode("//" + matchText).InnerText;
                        }
                        else
                        {
                            matchedVal = xmlParam.SelectSingleNode("//" + matchText).InnerText.Substring(0, fieldLength) + "...";
                        }
                        //modified begum 06.03.2009

                        if (matchTexts[0] == "Created" || matchTexts[0] == "Date" || matchTexts[0] == "HaberTarihi" || matchTexts[0] == "DATE")
                        {
                            if (!String.IsNullOrEmpty(matchedVal))
                            {
                                DateTime _date = Convert.ToDateTime(matchedVal.ToString());
                                matchedVal = _date.ToString("dd/MM/yyyy"); //_date.Day.ToString() + "/" + _date.Month.ToString() + "/" + _date.Year.ToString();
                            }
                            else
                                matchedVal = "";
                        }
                        //modified begum 06.03.2009


                        template = regex.Replace(template, matchedVal, 1);
                    }
                    catch
                    {
                        string matchedVal = "";
                        try
                        {
                            if (fieldLength == -1 || item[matchText].ToString().Length <= fieldLength)
                            {
                                matchedVal = item[matchText].ToString();
                            }
                            else
                            {
                                matchedVal = item[matchText].ToString().Substring(0, fieldLength) + "...";
                            }
                        }
                        catch
                        {
                            matchedVal = "";
                        }
                        //modified begum 06.03.2009
                        if (matchTexts[0] == "Created" || matchTexts[0] == "Date" || matchTexts[0] == "HaberTarihi" || matchTexts[0] == "DATE")
                        {
                            if (!String.IsNullOrEmpty(matchedVal))
                            {
                                DateTime _date = Convert.ToDateTime(matchedVal.ToString());
                                matchedVal = _date.ToString("dd/MM/yyyy");// _date.Day.ToString() + "/" + _date.Month.ToString() + "/" + _date.Year.ToString();
                            }
                            else
                                matchedVal = "";
                        }
                        //modified begum 06.03.2009


                        if (matchTexts[0] == "DATE")
                        {
                            if (!String.IsNullOrEmpty(matchedVal))
                            {
                                DateTime _date = Convert.ToDateTime(matchedVal.ToString());
                                matchedVal = _date.ToString("dd/MM/yyyy"); // _date.Day.ToString() + "/" + _date.Month.ToString() + "/" + _date.Year.ToString() + " " + _date.Hour.ToString() + ":" + _date.Minute.ToString("mm");
                            }
                            else
                                matchedVal = "";
                        }
                        //modified Galip 23.07.2010


                        template = regex.Replace(template, matchedVal, 1);
                    }
                    match = match.NextMatch();
                }



                return template;
            }
            catch (Exception ex)
            {
                //ExceptionPolicy.HandleException(ex, "Log Only Policy");
                return "";
            }
            finally
            {
                xmlParam = null;
            }
        }


        public static string ProcessTemplate(SPList item, string templateFileName, string templateParameters, string encodingType)
        {
            XmlDocument xmlParam = null;
            try
            {
                string template = Util.GetUrlResponse(templateFileName, false, true, null, null, null, encodingType);

                xmlParam = new XmlDocument();
                xmlParam.LoadXml(templateParameters);

                Regex regex = new Regex(
                    @"<!>\w*:*\w*</!>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );


                Match match = regex.Match(template);
                while (match.Success)
                {
                    int fieldLength = -1;
                    string[] matchTexts = match.ToString().Substring(3, match.Length - 7).Split(':');
                    if (matchTexts.Length > 1)
                    {
                        fieldLength = Convert.ToInt32(matchTexts[1]);
                    }
                    string matchText = matchTexts[0];
                    try
                    {
                        string matchedVal = "";
                        if (fieldLength == -1 || xmlParam.SelectSingleNode("//" + matchText).InnerText.Length <= fieldLength)
                        {
                            matchedVal = xmlParam.SelectSingleNode("//" + matchText).InnerText;
                        }
                        else
                        {
                            matchedVal = xmlParam.SelectSingleNode("//" + matchText).InnerText.Substring(0, fieldLength) + "...";
                        }

                        template = regex.Replace(template, matchedVal, 1);
                    }
                    catch
                    {
                        template = regex.Replace(template, matchText, 1);
                    }
                    match = match.NextMatch();
                }



                return template;
            }
            catch (Exception ex)
            {
                //ExceptionPolicy.HandleException(ex, "Log Only Policy");
                return "";
            }
            finally
            {
                xmlParam = null;
            }
        }

        public static string ProcessTemplate(string templateFileName, string templateParameters, string encodingType)
        {
            XmlDocument xmlParam = null;
            try
            {
                string template = GetResourceByGuid(templateFileName, encodingType);

                xmlParam = new XmlDocument();
                xmlParam.LoadXml(templateParameters);

                Regex regex = new Regex(
                    @"<!>\w*</!>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );

                Match match = regex.Match(template);
                while (match.Success)
                {
                    string matchText = match.ToString();
                    template = regex.Replace(template, xmlParam.SelectSingleNode("//" + matchText.Substring(3, matchText.Length - 7)).InnerText, 1);
                    match = match.NextMatch();
                }

                return template;
            }
            catch (Exception ex)
            {
                //ExceptionPolicy.HandleException(ex, "Log Only Policy");
                return "";
            }
            finally
            {
                xmlParam = null;
            }
        }

        public static string GetUrlResponse(string url, bool proxyEnabled, bool authEnabled,
            string proxyAddress, string credentialUser, string credentialPwd, string encodingType)
        {
            try
            {
                byte[] buffer = new byte[1000];

                StringBuilder sbResponse = new StringBuilder("");
                int intSize = 0;

                if (HttpContext.Current != null)
                {
                    //url= HttpContext.Current.Server.UrlEncode(url);
                }
                else
                {
                    url = url.Replace(" ", "+");
                }

                WebRequest request = WebRequest.Create(url);

                WebProxy myProxy = null;

                if (proxyEnabled == true)
                {
                    myProxy = new WebProxy();
                    Uri newUri = new Uri(proxyAddress);
                    myProxy.Address = newUri;
                    if (credentialUser == null || credentialUser == "")
                        myProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    else
                        myProxy.Credentials = new NetworkCredential(credentialUser, credentialPwd);

                    request.Proxy = myProxy;
                }

                if (authEnabled == true)
                {
                    if (credentialUser == null || credentialUser == "")
                        request.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    else
                        request.Credentials = new NetworkCredential(credentialUser, credentialPwd);
                }

                WebResponse response = request.GetResponse();


                while ((intSize = response.GetResponseStream().Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (intSize > System.Text.Encoding.GetEncoding(encodingType).GetCharCount(buffer))
                    {
                        intSize = System.Text.Encoding.GetEncoding(encodingType).GetCharCount(buffer);
                    }

                    //					char[] charArr = (System.Text.Encoding.GetEncoding(encodingType).GetChars(buffer,0,intSize));
                    //					
                    //					if(intSize > charArr.Length)
                    //					{
                    //						intSize = charArr.Length;
                    //					}
                    //					sbResponse.Append(charArr,0, intSize);
                    sbResponse.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, intSize));
                }

                response.Close();

                return sbResponse.ToString();
            }
            catch (Exception ex)
            {
                ex.Data.Add("URL", url);
                ex.Data.Add("Proxy Enabled = ", proxyEnabled.ToString());
                if (proxyEnabled)
                {
                    ex.Data.Add("Proxy Address = ", proxyAddress);
                }
                ////ExceptionPolicy.HandleException(ex, "Log Only Policy");
                return "";
            }
        }


        public static string GetResourceByGuid(string templateName, string encodingType)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(templateName, Encoding.GetEncoding(encodingType));
                string template = sr.ReadToEnd();
                sr.Close();
                sr = null;

                return template;
            }
            catch (Exception ex)
            {
                ////ExceptionPolicy.HandleException(ex, "Log Only Policy");
                throw ex;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }



    }
}

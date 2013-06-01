using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using Glossary.Framework;
using Glossary.Items;

namespace Glossary
{
    /// <summary>
    /// XML Implement class for Glossary
    /// </summary>
    public class GlossaryXml : IGlossary
    {
        private const string ConfigFile = "Glossary.xml";
        private const string ConfigFileFromXap = @"Config/" + ConfigFile;
        
        public SortType SortType { get; set; }


        public void Add(string term, string description)
        {
            Data = Data.Add(new TermItem
                                {
                                    ID = Guid.NewGuid(),
                                    Term = term,
                                    Description = description,
                                });
        }

        public bool CheckNewTermName(string term)
        {
            //case insensitive.
            var query = Data.Where(t => t.Term.ToLower() == term.ToLower());
            return (query.Count() > 0);
        }

        public bool CheckEditTermName(TermItem editingTermItem)
        {
            var query = from t in Data
                        where t.Term.ToLower() == editingTermItem.Term.ToLower() 
                                && t.ID != editingTermItem.ID
                        select t;
            return (query.Count() > 0);
        }

        public void Edit(TermItem termItem)
        {
            Remove(termItem.ID);
            Data = Data.Add(termItem);
        }

        public void Remove(Guid guid)
        {
            Data = from t in Data
                        where t.ID != guid
                        select t;
        }

        public IEnumerable<TermItem> Data { get; set; }

        public GlossaryXml()
        {
            SortType = SortType.Term; 
        }

        public bool Load()
        {
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //first run, copy the config file from xap to isolated storage.
                    if (!file.FileExists(ConfigFile))
                    {
                        //stream = App.GetResourceStream(new Uri(ConfigFileFromXap, UriKind.Relative)).Stream;
                        //var bytes = new byte[stream.Length];
                        //stream.Read(bytes, 0, bytes.Length);
                        //using (FileStream fileStream = file.OpenFile(ConfigFile, FileMode.Create))
                        //{
                        //    fileStream.Write(bytes, 0, bytes.Length);
                        //}

                        //Need to add GUID to the xml to handle tombstone for editing term.
                        var stream = App.GetResourceStream(new Uri(ConfigFileFromXap, UriKind.Relative)).Stream;
                        var reader = XmlReader.Create(stream);
                        var xDoc = XDocument.Load(reader);
                        Data = from xElem in xDoc.Descendants("TermItem")
                                    select new TermItem
                                    {
                                        ID = Guid.NewGuid(),
                                        Term = xElem.Attribute("Term").Value,
                                        Description = xElem.Attribute("Description").Value,
                                    };
                        reader.Close();
                        stream.Close();
                        Save();
                    }

                    var termItems = new List<TermItem>();
                    using (IsolatedStorageFileStream fileStream = file.OpenFile(ConfigFile, FileMode.Open, FileAccess.Read))
                    {
                        TextReader reader = new StreamReader(fileStream);
                        while (true)
                        {
                            string line = reader.ReadLine();
                            if (line != null)
                            {
                                System.Diagnostics.Debug.WriteLine(line);
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        reader = new StreamReader(fileStream);
                        var xs = new XmlSerializer(typeof(List<TermItem>));
                        termItems.AddRange((List<TermItem>)xs.Deserialize(reader));
                        reader.Close();
                    }
                    
                    if (SortType == SortType.Term)
                    {
                        Data = from t in termItems
                               orderby t.Term
                               select t;
                    }
                    else
                    {
                        Data = from t in termItems
                               orderby t.Description
                               select t;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ToastPromptHelper.ShowToastPrompt("We cannot load the glossary, please try again later.", 5000);
                return false;
            }
        }

        public bool Save()
        {
            Stream stream = null;
            TextWriter writer = null;
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (file.FileExists(ConfigFile))
                    {
                        stream = file.OpenFile(ConfigFile, FileMode.Truncate);
                    }
                    else
                    {
                        stream = file.OpenFile(ConfigFile, FileMode.CreateNew);
                    }
                    writer = new StreamWriter(stream);
                    var xs = new XmlSerializer(typeof(List<TermItem>));
                    xs.Serialize(writer, Data.ToList());
                    writer.Close();
                    stream.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (writer != null)
                {
                    writer.Dispose();
                }
            }
        }

        public bool SortData(SortType sortType)
        {
            if (sortType != SortType)
            {
                SortDataInternal(sortType);
                SortType = sortType;
            }
            return true;
        }

        private void SortDataInternal(SortType sortType)
        {
            if (sortType == SortType.Term)
            {
                Data = from t in Data
                       orderby t.Term
                       select t;
            }
            else
            {
                Data = from t in Data
                       orderby t.Description
                       select t;
            }
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glossary.Items;

namespace Glossary
{
    /// <summary>
    /// Interface of glossary, derived class would be xml, database, cloud, etc.
    /// </summary>
    public interface IGlossary
    {
        IEnumerable<TermItem> Data { get; set; }
        
        bool Load();
        bool Save();

        bool SortData(SortType sortType);
        void Add(string term, string description);
        void Edit(TermItem editingTermItem);
        void Remove(Guid guid);
        
        bool CheckNewTermName(string term);
        bool CheckEditTermName(TermItem editingTermItem);

    }
}

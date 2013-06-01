using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Glossary.Items
{
    public class TermItem
    {
        public Guid ID { get; set; }
        public string Term { get; set; }
        public string Description { get; set; }
        public TermItem Value {get { return this; }}
    }
}

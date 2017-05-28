using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BarCheck
{
    public class Capture : IDisposable
    {
        private readonly ListBox listBox;
        private readonly INotifyCollectionChanged incc;

        public Capture(ListBox listBox)
        {
            this.listBox = listBox;
            incc = listBox.ItemsSource as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += incc_CollectionChanged;
            }
        }

        void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                listBox.ScrollIntoView(e.NewItems[0]);
                listBox.SelectedItem = e.NewItems[0];
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (incc != null)
                    incc.CollectionChanged -= incc_CollectionChanged;
            }
        }
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

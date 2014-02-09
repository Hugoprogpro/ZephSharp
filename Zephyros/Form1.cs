using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Zephyros
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //HotKey k = new Zephyros.HotKey("d", new List<string>{"ctrl", "alt"});

            clojure.lang.Var v = clojure.lang.RT.var("clojure.core", "load-file");
            v.invoke(@"C:\Users\sdegutis\Desktop\Test.clj");

            //var wins = Zephyros.Window.GetWindows();
            //foreach (var win in wins)   
            //{
            //    if (win.IsVisible())
            //    {
            //        Rectangle r = win.GetRect();
            //        r.X += 10;
            //        win.Move(r);
            //        Console.WriteLine("HEY = " + win.Title());
            //    }
            //}
        }
    }
}

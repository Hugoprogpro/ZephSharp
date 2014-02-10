using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

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

namespace Zephyros
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Console.WriteLine("bla");
            reloadConfigs();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button != MouseButtons.Left)
                return;

            reloadConfigs();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void reloadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reloadConfigs();
        }

        private void reloadConfigs()
        {
            clojure.lang.Var v = clojure.lang.RT.var("clojure.core", "load-file");
            v.invoke(@"C:\Users\sdegutis\Desktop\Test.clj");
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;

namespace Zephyros
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            setupClojureStuff();
            reloadConfigs();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
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

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        private void reloadConfigs()
        {
            HotKey.ResetAll();
            clojure.lang.Var v = clojure.lang.RT.var("clojure.core", "load-file");
            v.invoke(@"C:\Users\sdegutis\Desktop\Test.clj");
        }

        private void setupClojureStuff()
        {
            string contents = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Zephyros.Setup.clj")).ReadToEnd();
            clojure.lang.Var v = clojure.lang.RT.var("clojure.core", "load-string");
            v.invoke(contents);
        }
    }
}

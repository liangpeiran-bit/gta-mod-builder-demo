using System;
using System.Windows.Forms;
using GTA;

public class DemoHelloMod : Script
{
    public DemoHelloMod()
    {
        KeyDown += OnKeyDown;
        Aborted += OnAborted;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F8)
        {
            GTA.UI.Screen.ShowSubtitle("DemoHelloMod build works!");
        }
    }

    private void OnAborted(object sender, EventArgs e)
    {
    }
}
using System;
using GTA;

public class Mod : Script
{
    public Mod()
    {
        Tick += OnTick;
        Interval = 100;
    }

    private void OnTick(object sender, EventArgs e)
    {
    }
}

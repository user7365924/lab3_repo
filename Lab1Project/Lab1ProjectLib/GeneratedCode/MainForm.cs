﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

public class MainForm : Form
{
    DateTimePicker dateTimePicker = null;
    ListBox listBox = null;
    TextBox textBox = null;
    ComboBox comboBox = null;
    List<UserEvent> events = new List<UserEvent>();
    System.Timers.Timer timer;

    public MainForm()
    {
        InitializeCompoents();
    }
	public void InitializeCompoents()
	{
        this.SuspendLayout();

        // Form
        this.MaximizeBox = false;
        this.Text = "Electronic secretary";
        this.AutoScaleDimensions = new SizeF(6F, 13F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(240, 300);
        this.StartPosition = FormStartPosition.CenterScreen;

        // DateTimePicker
        dateTimePicker = new DateTimePicker();
        dateTimePicker.Location = new Point(20, 20);
        dateTimePicker.Size = new Size(140, 20);
        dateTimePicker.Format = DateTimePickerFormat.Custom;
        dateTimePicker.CustomFormat = "MM/dd/yyyy HH:mm:ss";
        dateTimePicker.SuspendLayout();
        this.Controls.Add(dateTimePicker);

        // Buttons
        // add event
        Button buttonAdd = new Button();
        buttonAdd.Text = "Add";
        buttonAdd.Location = new Point(160, 20);
        buttonAdd.Size = new Size(60, 20);
        buttonAdd.Click += buttonAdd_Click;
        this.Controls.Add(buttonAdd);

        // TextBox
        textBox = new TextBox();
        textBox.Location = new Point(20, 45);
        textBox.Size = new Size(200, 40);
        textBox.ScrollBars = ScrollBars.Vertical;
        textBox.Multiline = true;
        this.Controls.Add(textBox);

        comboBox = new ComboBox();
        comboBox.Items.Add("normal");
        comboBox.Items.Add("important");
        comboBox.Items.Add("very important");
        comboBox.Location = new Point(20, 90);
        comboBox.Size = new Size(200, 20);
        this.Controls.Add(comboBox);

        // ListBox
        listBox = new ListBox();
        listBox.Location = new Point(20, 112);
        listBox.Size = new Size(200, 150);
        listBox.SelectedIndexChanged += listBox_SelectedIndexChanged;
        this.Controls.Add(listBox);

        // remove event
        Button buttonRemove = new Button();
        buttonRemove.Text = "Remove";
        buttonRemove.Location = new Point(20, 262);
        buttonRemove.Size = new Size(200, 20);
        buttonRemove.Click += buttonRemove_Click;
        this.Controls.Add(buttonRemove);

        timer = new System.Timers.Timer(1000*10);
        timer.Elapsed += timer_Elapsed;
        timer.Start();

        this.ResumeLayout(false);

        this.Load += form_Load;
        this.FormClosing += form_Close;
	}

    private void buttonAdd_Click(object sender, System.EventArgs e)
    {
        string dateTime = dateTimePicker.Text;
        string text = textBox.Text;

        int level = comboBox.SelectedIndex;
        if(level < 0)
        {
            level = 0;
        }
        UserEvent ue = new UserEvent(text, dateTime, level);
        events.Add(ue);

        char[] delimiters = { '|' };
        string line = ue.ToString();
        string[] items = line.Split(delimiters);
        if (items.Length == 3)
        {
            listBox.Items.Add(items[0] + " " + items[1]);
        }
    }

    private void buttonRemove_Click(object sender, System.EventArgs e)
    {
        int index = listBox.SelectedIndex;

        if(index >= 0)
        {
            listBox.Items.RemoveAt(index);
            events.RemoveAt(index);
        }
    }

    private void form_Load(object sender, System.EventArgs e)
    {
        char[] delimiters = {'|'};
        string[] lines = null;

        try
        {
            lines = System.IO.File.ReadAllLines(@"db.txt");
        }
        catch (System.IO.FileNotFoundException ex)
        {
            lines = null;
        }

        if (lines == null || lines.Length == 0)
        {
            return;
        }

        foreach(string line in lines)
        {
            string[] items = line.Split(delimiters);
            if(items.Length == 3)
            {
                UserEvent ue = new UserEvent(items[1], items[0], Int32.Parse(items[2]));
                events.Add(ue);
                listBox.Items.Add(items[0] + " " + items[1]);
            }
        }
    }

    private void form_Close(object sender, System.EventArgs e)
    {
        UserEvent[] items = events.ToArray();
        using (System.IO.StreamWriter file = new  System.IO.StreamWriter(@"db.txt"))
        {
            foreach (UserEvent item in items)
            {
                string s = item.ToString();
                file.WriteLine(s);
            }
        }
        timer.Stop();
    }

    private void listBox_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        int index = 0;

	index = listBox.SelectedIndex;

        if (index >= 0)
        {
            UserEvent ue = events[index];
            if(ue != null)
            {
                int level = ue.GetLevel();
                comboBox.SelectedIndex = level;
                string text = ue.GetText();
                textBox.Text = text;
                string dataTime = ue.GetDateAndTime();
                dateTimePicker.Text = dataTime;
            }
        }
    }

    private void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        DateTime dateTimeNow = DateTime.Now;
        char[] delimiters = { '|' };

        UserEvent[] items = events.ToArray();

        int index = 0;
        foreach (UserEvent item in items)
        {
            string s = item.ToString();
            string[] lines = s.Split(delimiters);
            if (lines.Length == 3)
            {
                DateTime dt = DateTime.Parse(lines[0]);
                var result = (dateTimeNow - dt).TotalSeconds;

                if(result < 300)
                {
                    listBox.Invoke(new Action(() => {listBox.SelectedIndex = index;}));
                    int n = Int32.Parse(lines[2]);
                    for(int i = 0; i <= n; i ++)
                    {
                        System.Media.SystemSounds.Beep.Play();
                        System.Threading.Thread.Sleep(1000);
                    }
                    
            
                }
            }
            index ++;
        }
    }
}


using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Road.WinForms;

partial class Form1 {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        plot1 = new OxyPlot.WindowsForms.PlotView();
        button1 = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // plot1
        // 
        plot1.Dock = System.Windows.Forms.DockStyle.Bottom;
        plot1.Location = new System.Drawing.Point(0, 343);
        plot1.Margin = new System.Windows.Forms.Padding(0);
        plot1.Name = "plot1";
        plot1.PanCursor = System.Windows.Forms.Cursors.Hand;
        plot1.Size = new System.Drawing.Size(806, 131);
        plot1.TabIndex = 0;
        plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
        plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
        plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
        // 
        // button1
        // 
        button1.Location = new System.Drawing.Point(678, 428);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(116, 34);
        button1.TabIndex = 1;
        button1.Text = "Train";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(806, 474);
        Controls.Add(button1);
        Controls.Add(plot1);
        Text = "Form1";
        ResumeLayout(false);
    }

    private System.Windows.Forms.Button button1;

    private OxyPlot.WindowsForms.PlotView plot1;

    #endregion
}

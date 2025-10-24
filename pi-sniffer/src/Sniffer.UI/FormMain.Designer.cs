using System.Drawing;
using System.Windows.Forms;

namespace Sniffer.UI;

partial class FormMain
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.cmbInterfaces = new ComboBox();
        this.btnStart = new Button();
        this.btnStop = new Button();
        this.dgvDevices = new DataGridView();
        this.dgvPackets = new DataGridView();
        this.txtPacketDetails = new TextBox();
        this.txtFilter = new TextBox();
        this.lblFilter = new Label();
        ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPackets)).BeginInit();
        this.SuspendLayout();
        // 
        // cmbInterfaces
        // 
        this.cmbInterfaces.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbInterfaces.FormattingEnabled = true;
        this.cmbInterfaces.Location = new Point(12, 12);
        this.cmbInterfaces.Name = "cmbInterfaces";
        this.cmbInterfaces.Size = new Size(260, 23);
        this.cmbInterfaces.TabIndex = 0;
        // 
        // btnStart
        // 
        this.btnStart.Location = new Point(278, 12);
        this.btnStart.Name = "btnStart";
        this.btnStart.Size = new Size(75, 23);
        this.btnStart.TabIndex = 1;
        this.btnStart.Text = "Start";
        this.btnStart.UseVisualStyleBackColor = true;
        this.btnStart.Click += new EventHandler(this.btnStart_Click);
        // 
        // btnStop
        // 
        this.btnStop.Enabled = false;
        this.btnStop.Location = new Point(359, 12);
        this.btnStop.Name = "btnStop";
        this.btnStop.Size = new Size(75, 23);
        this.btnStop.TabIndex = 2;
        this.btnStop.Text = "Stop";
        this.btnStop.UseVisualStyleBackColor = true;
        this.btnStop.Click += new EventHandler(this.btnStop_Click);
        // 
        // dgvDevices
        // 
        this.dgvDevices.AllowUserToAddRows = false;
        this.dgvDevices.AllowUserToDeleteRows = false;
        this.dgvDevices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.dgvDevices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvDevices.Location = new Point(12, 50);
        this.dgvDevices.MultiSelect = false;
        this.dgvDevices.Name = "dgvDevices";
        this.dgvDevices.ReadOnly = true;
        this.dgvDevices.RowTemplate.Height = 25;
        this.dgvDevices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvDevices.Size = new Size(422, 200);
        this.dgvDevices.TabIndex = 3;
        this.dgvDevices.SelectionChanged += new EventHandler(this.dgvDevices_SelectionChanged);
        // 
        // dgvPackets
        // 
        this.dgvPackets.AllowUserToAddRows = false;
        this.dgvPackets.AllowUserToDeleteRows = false;
        this.dgvPackets.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dgvPackets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvPackets.Location = new Point(12, 268);
        this.dgvPackets.MultiSelect = false;
        this.dgvPackets.Name = "dgvPackets";
        this.dgvPackets.ReadOnly = true;
        this.dgvPackets.RowTemplate.Height = 25;
        this.dgvPackets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvPackets.Size = new Size(760, 200);
        this.dgvPackets.TabIndex = 4;
        this.dgvPackets.SelectionChanged += new EventHandler(this.dgvPackets_SelectionChanged);
        // 
        // txtPacketDetails
        // 
        this.txtPacketDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.txtPacketDetails.Location = new Point(440, 50);
        this.txtPacketDetails.Multiline = true;
        this.txtPacketDetails.Name = "txtPacketDetails";
        this.txtPacketDetails.ReadOnly = true;
        this.txtPacketDetails.ScrollBars = ScrollBars.Vertical;
        this.txtPacketDetails.Size = new Size(332, 200);
        this.txtPacketDetails.TabIndex = 5;
        // 
        // txtFilter
        // 
        this.txtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.txtFilter.Location = new Point(607, 12);
        this.txtFilter.Name = "txtFilter";
        this.txtFilter.PlaceholderText = "IP veya MAC filtrele";
        this.txtFilter.Size = new Size(165, 23);
        this.txtFilter.TabIndex = 6;
        this.txtFilter.TextChanged += new EventHandler(this.txtFilter_TextChanged);
        // 
        // lblFilter
        // 
        this.lblFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.lblFilter.AutoSize = true;
        this.lblFilter.Location = new Point(548, 16);
        this.lblFilter.Name = "lblFilter";
        this.lblFilter.Size = new Size(53, 15);
        this.lblFilter.TabIndex = 7;
        this.lblFilter.Text = "Filtre:";
        // 
        // FormMain
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(784, 481);
        this.Controls.Add(this.lblFilter);
        this.Controls.Add(this.txtFilter);
        this.Controls.Add(this.txtPacketDetails);
        this.Controls.Add(this.dgvPackets);
        this.Controls.Add(this.dgvDevices);
        this.Controls.Add(this.btnStop);
        this.Controls.Add(this.btnStart);
        this.Controls.Add(this.cmbInterfaces);
        this.MinimumSize = new Size(800, 520);
        this.Name = "FormMain";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Pi Sniffer";
        this.FormClosing += new FormClosingEventHandler(this.FormMain_FormClosing);
        this.Load += new EventHandler(this.FormMain_Load);
        ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPackets)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private ComboBox cmbInterfaces;
    private Button btnStart;
    private Button btnStop;
    private DataGridView dgvDevices;
    private DataGridView dgvPackets;
    private TextBox txtPacketDetails;
    private TextBox txtFilter;
    private Label lblFilter;
}

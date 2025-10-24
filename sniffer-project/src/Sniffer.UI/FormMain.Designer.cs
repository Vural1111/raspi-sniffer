using System.Drawing;
using System.Windows.Forms;

namespace Sniffer.UI;

partial class FormMain
{
    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        splitContainer = new SplitContainer();
        gridPackets = new DataGridView();
        columnTimestamp = new DataGridViewTextBoxColumn();
        columnProtocol = new DataGridViewTextBoxColumn();
        columnSource = new DataGridViewTextBoxColumn();
        columnDestination = new DataGridViewTextBoxColumn();
        columnInfo = new DataGridViewTextBoxColumn();
        panelTop = new Panel();
        btnRefresh = new Button();
        btnStartStop = new Button();
        comboDevices = new ComboBox();
        lblDevice = new Label();
        txtDetails = new TextBox();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridPackets).BeginInit();
        panelTop.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainer
        // 
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Location = new Point(0, 0);
        splitContainer.Name = "splitContainer";
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.Panel1.Controls.Add(gridPackets);
        splitContainer.Panel1.Controls.Add(panelTop);
        splitContainer.Panel2.Controls.Add(txtDetails);
        splitContainer.Size = new Size(984, 561);
        splitContainer.SplitterDistance = 350;
        splitContainer.TabIndex = 0;
        // 
        // gridPackets
        // 
        gridPackets.AllowUserToAddRows = false;
        gridPackets.AllowUserToDeleteRows = false;
        gridPackets.AllowUserToResizeRows = false;
        gridPackets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gridPackets.Columns.AddRange(new DataGridViewColumn[] { columnTimestamp, columnProtocol, columnSource, columnDestination, columnInfo });
        gridPackets.Dock = DockStyle.Fill;
        gridPackets.Location = new Point(0, 46);
        gridPackets.MultiSelect = false;
        gridPackets.Name = "gridPackets";
        gridPackets.ReadOnly = true;
        gridPackets.RowHeadersVisible = false;
        gridPackets.RowTemplate.Height = 25;
        gridPackets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridPackets.Size = new Size(984, 304);
        gridPackets.TabIndex = 1;
        gridPackets.SelectionChanged += GridPacketsSelectionChanged;
        // 
        // columnTimestamp
        // 
        columnTimestamp.DataPropertyName = nameof(PacketViewModel.Timestamp);
        columnTimestamp.HeaderText = "Zaman";
        columnTimestamp.Name = "columnTimestamp";
        columnTimestamp.ReadOnly = true;
        columnTimestamp.Width = 160;
        // 
        // columnProtocol
        // 
        columnProtocol.DataPropertyName = nameof(PacketViewModel.Protocol);
        columnProtocol.HeaderText = "Protokol";
        columnProtocol.Name = "columnProtocol";
        columnProtocol.ReadOnly = true;
        columnProtocol.Width = 90;
        // 
        // columnSource
        // 
        columnSource.DataPropertyName = nameof(PacketViewModel.Source);
        columnSource.HeaderText = "Kaynak";
        columnSource.Name = "columnSource";
        columnSource.ReadOnly = true;
        columnSource.Width = 200;
        // 
        // columnDestination
        // 
        columnDestination.DataPropertyName = nameof(PacketViewModel.Destination);
        columnDestination.HeaderText = "Hedef";
        columnDestination.Name = "columnDestination";
        columnDestination.ReadOnly = true;
        columnDestination.Width = 200;
        // 
        // columnInfo
        // 
        columnInfo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        columnInfo.DataPropertyName = nameof(PacketViewModel.Info);
        columnInfo.HeaderText = "Bilgi";
        columnInfo.Name = "columnInfo";
        columnInfo.ReadOnly = true;
        // 
        // panelTop
        // 
        panelTop.Controls.Add(btnRefresh);
        panelTop.Controls.Add(btnStartStop);
        panelTop.Controls.Add(comboDevices);
        panelTop.Controls.Add(lblDevice);
        panelTop.Dock = DockStyle.Top;
        panelTop.Location = new Point(0, 0);
        panelTop.Name = "panelTop";
        panelTop.Size = new Size(984, 46);
        panelTop.TabIndex = 0;
        // 
        // btnRefresh
        // 
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.Location = new Point(714, 10);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(120, 27);
        btnRefresh.TabIndex = 3;
        btnRefresh.Text = "Yenile";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += BtnRefreshClick;
        // 
        // btnStartStop
        // 
        btnStartStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnStartStop.Location = new Point(840, 10);
        btnStartStop.Name = "btnStartStop";
        btnStartStop.Size = new Size(132, 27);
        btnStartStop.TabIndex = 2;
        btnStartStop.Text = "Başlat";
        btnStartStop.UseVisualStyleBackColor = true;
        btnStartStop.Click += BtnStartStopClick;
        // 
        // comboDevices
        // 
        comboDevices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        comboDevices.DropDownStyle = ComboBoxStyle.DropDownList;
        comboDevices.FormattingEnabled = true;
        comboDevices.Location = new Point(87, 12);
        comboDevices.Name = "comboDevices";
        comboDevices.Size = new Size(621, 23);
        comboDevices.TabIndex = 1;
        // 
        // lblDevice
        // 
        lblDevice.AutoSize = true;
        lblDevice.Location = new Point(12, 16);
        lblDevice.Name = "lblDevice";
        lblDevice.Size = new Size(69, 15);
        lblDevice.TabIndex = 0;
        lblDevice.Text = "Ağ Aygıtı:";
        // 
        // txtDetails
        // 
        txtDetails.Dock = DockStyle.Fill;
        txtDetails.Location = new Point(0, 0);
        txtDetails.Multiline = true;
        txtDetails.Name = "txtDetails";
        txtDetails.ReadOnly = true;
        txtDetails.ScrollBars = ScrollBars.Both;
        txtDetails.Size = new Size(984, 207);
        txtDetails.TabIndex = 0;
        txtDetails.WordWrap = false;
        // 
        // FormMain
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(984, 561);
        Controls.Add(splitContainer);
        MinimumSize = new Size(800, 450);
        Name = "FormMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Raspi Sniffer";
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)gridPackets).EndInit();
        panelTop.ResumeLayout(false);
        panelTop.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private System.ComponentModel.IContainer? components;
    private SplitContainer splitContainer;
    private DataGridView gridPackets;
    private Panel panelTop;
    private Button btnRefresh;
    private Button btnStartStop;
    private ComboBox comboDevices;
    private Label lblDevice;
    private TextBox txtDetails;
    private DataGridViewTextBoxColumn columnTimestamp;
    private DataGridViewTextBoxColumn columnProtocol;
    private DataGridViewTextBoxColumn columnSource;
    private DataGridViewTextBoxColumn columnDestination;
    private DataGridViewTextBoxColumn columnInfo;
}

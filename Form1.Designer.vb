<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.grpConnection = New System.Windows.Forms.GroupBox()
        Me.lblConnectionStatus = New System.Windows.Forms.Label()
        Me.lblBluetoothName = New System.Windows.Forms.Label()
        Me.btnRefreshPorts = New System.Windows.Forms.Button()
        Me.btnClosePort = New System.Windows.Forms.Button()
        Me.btnOpenPort = New System.Windows.Forms.Button()
        Me.cmbComPorts = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.grpCpuMonitor = New System.Windows.Forms.GroupBox()
        Me.lblCpuColor = New System.Windows.Forms.Label()
        Me.lblCpuValue = New System.Windows.Forms.Label()
        Me.btnStopCpu = New System.Windows.Forms.Button()
        Me.btnStartCpu = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.grpRamMonitor = New System.Windows.Forms.GroupBox()
        Me.lblRamValue = New System.Windows.Forms.Label()
        Me.btnStopRam = New System.Windows.Forms.Button()
        Me.btnStartRam = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.grpEeprom = New System.Windows.Forms.GroupBox()
        Me.txtBinaryInput = New System.Windows.Forms.TextBox()
        Me.btnWrite = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.grpLedDisplay = New System.Windows.Forms.GroupBox()
        Me.pnlLeds = New System.Windows.Forms.Panel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.timerCpu = New System.Windows.Forms.Timer(Me.components)
        Me.timerRam = New System.Windows.Forms.Timer(Me.components)
        Me.timerPortCheck = New System.Windows.Forms.Timer(Me.components)
        Me.grpConnection.SuspendLayout()
        Me.grpCpuMonitor.SuspendLayout()
        Me.grpRamMonitor.SuspendLayout()
        Me.grpEeprom.SuspendLayout()
        Me.grpLedDisplay.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpConnection
        '
        Me.grpConnection.Controls.Add(Me.lblConnectionStatus)
        Me.grpConnection.Controls.Add(Me.lblBluetoothName)
        Me.grpConnection.Controls.Add(Me.btnRefreshPorts)
        Me.grpConnection.Controls.Add(Me.btnClosePort)
        Me.grpConnection.Controls.Add(Me.btnOpenPort)
        Me.grpConnection.Controls.Add(Me.cmbComPorts)
        Me.grpConnection.Controls.Add(Me.Label2)
        Me.grpConnection.Controls.Add(Me.Label1)
        Me.grpConnection.Location = New System.Drawing.Point(12, 12)
        Me.grpConnection.Name = "grpConnection"
        Me.grpConnection.Size = New System.Drawing.Size(380, 180)
        Me.grpConnection.TabIndex = 0
        Me.grpConnection.TabStop = False
        Me.grpConnection.Text = "連線設定"
        '
        'lblConnectionStatus
        '
        Me.lblConnectionStatus.AutoSize = True
        Me.lblConnectionStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblConnectionStatus.ForeColor = System.Drawing.Color.Red
        Me.lblConnectionStatus.Location = New System.Drawing.Point(120, 140)
        Me.lblConnectionStatus.Name = "lblConnectionStatus"
        Me.lblConnectionStatus.Size = New System.Drawing.Size(104, 20)
        Me.lblConnectionStatus.TabIndex = 7
        Me.lblConnectionStatus.Text = "Disconnect"
        '
        'lblBluetoothName
        '
        Me.lblBluetoothName.AutoSize = True
        Me.lblBluetoothName.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.lblBluetoothName.Location = New System.Drawing.Point(120, 60)
        Me.lblBluetoothName.Name = "lblBluetoothName"
        Me.lblBluetoothName.Size = New System.Drawing.Size(104, 17)
        Me.lblBluetoothName.TabIndex = 6
        Me.lblBluetoothName.Text = "ODD-01-0001"
        '
        'btnRefreshPorts
        '
        Me.btnRefreshPorts.Location = New System.Drawing.Point(280, 90)
        Me.btnRefreshPorts.Name = "btnRefreshPorts"
        Me.btnRefreshPorts.Size = New System.Drawing.Size(80, 30)
        Me.btnRefreshPorts.TabIndex = 5
        Me.btnRefreshPorts.Text = "Refresh"
        Me.btnRefreshPorts.UseVisualStyleBackColor = True
        '
        'btnClosePort
        '
        Me.btnClosePort.Enabled = False
        Me.btnClosePort.Location = New System.Drawing.Point(200, 90)
        Me.btnClosePort.Name = "btnClosePort"
        Me.btnClosePort.Size = New System.Drawing.Size(70, 30)
        Me.btnClosePort.TabIndex = 4
        Me.btnClosePort.Text = "Close"
        Me.btnClosePort.UseVisualStyleBackColor = True
        '
        'btnOpenPort
        '
        Me.btnOpenPort.Location = New System.Drawing.Point(120, 90)
        Me.btnOpenPort.Name = "btnOpenPort"
        Me.btnOpenPort.Size = New System.Drawing.Size(70, 30)
        Me.btnOpenPort.TabIndex = 3
        Me.btnOpenPort.Text = "Open"
        Me.btnOpenPort.UseVisualStyleBackColor = True
        '
        'cmbComPorts
        '
        Me.cmbComPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbComPorts.FormattingEnabled = True
        Me.cmbComPorts.Location = New System.Drawing.Point(120, 25)
        Me.cmbComPorts.Name = "cmbComPorts"
        Me.cmbComPorts.Size = New System.Drawing.Size(240, 23)
        Me.cmbComPorts.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 140)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(97, 15)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Connection Status:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(62, 15)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "COM Port:"
        '
        'grpCpuMonitor
        '
        Me.grpCpuMonitor.Controls.Add(Me.lblCpuColor)
        Me.grpCpuMonitor.Controls.Add(Me.lblCpuValue)
        Me.grpCpuMonitor.Controls.Add(Me.btnStopCpu)
        Me.grpCpuMonitor.Controls.Add(Me.btnStartCpu)
        Me.grpCpuMonitor.Controls.Add(Me.Label4)
        Me.grpCpuMonitor.Controls.Add(Me.Label3)
        Me.grpCpuMonitor.Location = New System.Drawing.Point(12, 200)
        Me.grpCpuMonitor.Name = "grpCpuMonitor"
        Me.grpCpuMonitor.Size = New System.Drawing.Size(380, 120)
        Me.grpCpuMonitor.TabIndex = 1
        Me.grpCpuMonitor.TabStop = False
        Me.grpCpuMonitor.Text = "CPU Loading Monitor"
        '
        'lblCpuColor
        '
        Me.lblCpuColor.BackColor = System.Drawing.Color.Black
        Me.lblCpuColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblCpuColor.Location = New System.Drawing.Point(280, 55)
        Me.lblCpuColor.Name = "lblCpuColor"
        Me.lblCpuColor.Size = New System.Drawing.Size(80, 30)
        Me.lblCpuColor.TabIndex = 5
        '
        'lblCpuValue
        '
        Me.lblCpuValue.AutoSize = True
        Me.lblCpuValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblCpuValue.Location = New System.Drawing.Point(120, 30)
        Me.lblCpuValue.Name = "lblCpuValue"
        Me.lblCpuValue.Size = New System.Drawing.Size(49, 20)
        Me.lblCpuValue.TabIndex = 4
        Me.lblCpuValue.Text = "0.0%"
        '
        'btnStopCpu
        '
        Me.btnStopCpu.Enabled = False
        Me.btnStopCpu.Location = New System.Drawing.Point(200, 70)
        Me.btnStopCpu.Name = "btnStopCpu"
        Me.btnStopCpu.Size = New System.Drawing.Size(70, 30)
        Me.btnStopCpu.TabIndex = 3
        Me.btnStopCpu.Text = "Stop"
        Me.btnStopCpu.UseVisualStyleBackColor = True
        '
        'btnStartCpu
        '
        Me.btnStartCpu.Enabled = False
        Me.btnStartCpu.Location = New System.Drawing.Point(120, 70)
        Me.btnStartCpu.Name = "btnStartCpu"
        Me.btnStartCpu.Size = New System.Drawing.Size(70, 30)
        Me.btnStartCpu.TabIndex = 2
        Me.btnStartCpu.Text = "Start"
        Me.btnStartCpu.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(280, 30)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(39, 15)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Color:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(20, 30)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(80, 15)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "CPU Loading:"
        '
        'grpRamMonitor
        '
        Me.grpRamMonitor.Controls.Add(Me.lblRamValue)
        Me.grpRamMonitor.Controls.Add(Me.btnStopRam)
        Me.grpRamMonitor.Controls.Add(Me.btnStartRam)
        Me.grpRamMonitor.Controls.Add(Me.Label6)
        Me.grpRamMonitor.Location = New System.Drawing.Point(12, 330)
        Me.grpRamMonitor.Name = "grpRamMonitor"
        Me.grpRamMonitor.Size = New System.Drawing.Size(380, 100)
        Me.grpRamMonitor.TabIndex = 2
        Me.grpRamMonitor.TabStop = False
        Me.grpRamMonitor.Text = "RAM Loading Monitor"
        '
        'lblRamValue
        '
        Me.lblRamValue.AutoSize = True
        Me.lblRamValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblRamValue.Location = New System.Drawing.Point(120, 30)
        Me.lblRamValue.Name = "lblRamValue"
        Me.lblRamValue.Size = New System.Drawing.Size(49, 20)
        Me.lblRamValue.TabIndex = 5
        Me.lblRamValue.Text = "0.0%"
        '
        'btnStopRam
        '
        Me.btnStopRam.Enabled = False
        Me.btnStopRam.Location = New System.Drawing.Point(200, 55)
        Me.btnStopRam.Name = "btnStopRam"
        Me.btnStopRam.Size = New System.Drawing.Size(70, 30)
        Me.btnStopRam.TabIndex = 3
        Me.btnStopRam.Text = "Stop"
        Me.btnStopRam.UseVisualStyleBackColor = True
        '
        'btnStartRam
        '
        Me.btnStartRam.Enabled = False
        Me.btnStartRam.Location = New System.Drawing.Point(120, 55)
        Me.btnStartRam.Name = "btnStartRam"
        Me.btnStartRam.Size = New System.Drawing.Size(70, 30)
        Me.btnStartRam.TabIndex = 2
        Me.btnStartRam.Text = "Start"
        Me.btnStartRam.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(20, 30)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(84, 15)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "RAM Loading:"
        '
        'grpEeprom
        '
        Me.grpEeprom.Controls.Add(Me.txtBinaryInput)
        Me.grpEeprom.Controls.Add(Me.btnWrite)
        Me.grpEeprom.Controls.Add(Me.Label7)
        Me.grpEeprom.Location = New System.Drawing.Point(12, 440)
        Me.grpEeprom.Name = "grpEeprom"
        Me.grpEeprom.Size = New System.Drawing.Size(380, 90)
        Me.grpEeprom.TabIndex = 3
        Me.grpEeprom.TabStop = False
        Me.grpEeprom.Text = "EEPROM Write"
        '
        'txtBinaryInput
        '
        Me.txtBinaryInput.Font = New System.Drawing.Font("Consolas", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.txtBinaryInput.Location = New System.Drawing.Point(120, 30)
        Me.txtBinaryInput.MaxLength = 4
        Me.txtBinaryInput.Name = "txtBinaryInput"
        Me.txtBinaryInput.Size = New System.Drawing.Size(100, 26)
        Me.txtBinaryInput.TabIndex = 2
        '
        'btnWrite
        '
        Me.btnWrite.Enabled = False
        Me.btnWrite.Location = New System.Drawing.Point(240, 28)
        Me.btnWrite.Name = "btnWrite"
        Me.btnWrite.Size = New System.Drawing.Size(80, 30)
        Me.btnWrite.TabIndex = 1
        Me.btnWrite.Text = "Write"
        Me.btnWrite.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(20, 35)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(81, 15)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "Binary Input (4bit):"
        '
        'grpLedDisplay
        '
        Me.grpLedDisplay.Controls.Add(Me.pnlLeds)
        Me.grpLedDisplay.Controls.Add(Me.Label8)
        Me.grpLedDisplay.Location = New System.Drawing.Point(410, 12)
        Me.grpLedDisplay.Name = "grpLedDisplay"
        Me.grpLedDisplay.Size = New System.Drawing.Size(380, 120)
        Me.grpLedDisplay.TabIndex = 4
        Me.grpLedDisplay.TabStop = False
        Me.grpLedDisplay.Text = "WS2812 LED Status"
        '
        'pnlLeds
        '
        Me.pnlLeds.Location = New System.Drawing.Point(20, 50)
        Me.pnlLeds.Name = "pnlLeds"
        Me.pnlLeds.Size = New System.Drawing.Size(340, 50)
        Me.pnlLeds.TabIndex = 1
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(20, 25)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(123, 15)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "8 LED Status (5mm):"
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(700, 500)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(90, 30)
        Me.btnExit.TabIndex = 5
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'timerCpu
        '
        Me.timerCpu.Interval = 1000
        '
        'timerRam
        '
        Me.timerRam.Interval = 1000
        '
        'timerPortCheck
        '
        Me.timerPortCheck.Interval = 2000
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(810, 545)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.grpLedDisplay)
        Me.Controls.Add(Me.grpEeprom)
        Me.Controls.Add(Me.grpRamMonitor)
        Me.Controls.Add(Me.grpCpuMonitor)
        Me.Controls.Add(Me.grpConnection)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "113 學年度 工業類科學生技藝競賽"
        Me.grpConnection.ResumeLayout(False)
        Me.grpConnection.PerformLayout()
        Me.grpCpuMonitor.ResumeLayout(False)
        Me.grpCpuMonitor.PerformLayout()
        Me.grpRamMonitor.ResumeLayout(False)
        Me.grpRamMonitor.PerformLayout()
        Me.grpEeprom.ResumeLayout(False)
        Me.grpEeprom.PerformLayout()
        Me.grpLedDisplay.ResumeLayout(False)
        Me.grpLedDisplay.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents grpConnection As GroupBox
    Friend WithEvents lblConnectionStatus As Label
    Friend WithEvents lblBluetoothName As Label
    Friend WithEvents btnRefreshPorts As Button
    Friend WithEvents btnClosePort As Button
    Friend WithEvents btnOpenPort As Button
    Friend WithEvents cmbComPorts As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents grpCpuMonitor As GroupBox
    Friend WithEvents lblCpuColor As Label
    Friend WithEvents lblCpuValue As Label
    Friend WithEvents btnStopCpu As Button
    Friend WithEvents btnStartCpu As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents grpRamMonitor As GroupBox
    Friend WithEvents lblRamValue As Label
    Friend WithEvents btnStopRam As Button
    Friend WithEvents btnStartRam As Button
    Friend WithEvents Label6 As Label
    Friend WithEvents grpEeprom As GroupBox
    Friend WithEvents txtBinaryInput As TextBox
    Friend WithEvents btnWrite As Button
    Friend WithEvents Label7 As Label
    Friend WithEvents grpLedDisplay As GroupBox
    Friend WithEvents pnlLeds As Panel
    Friend WithEvents Label8 As Label
    Friend WithEvents btnExit As Button
    Friend WithEvents timerCpu As Timer
    Friend WithEvents timerRam As Timer
    Friend WithEvents timerPortCheck As Timer
End Class

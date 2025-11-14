Public Class Form1
    ' 核心服務模組
    Private serialManager As SerialPortManager
    Private portWatcher As ComPortWatcher
    Private cpuProvider As CpuLoadProvider
    Private eepromService As EepromService
    Private bleProtocol As BleProtocol
    Private ledDisplay As LedDisplayManager
    Private ledSync As LedSyncService

    ' 狀態變數
    Private stationNumber As String = "01"
    Private currentCpuUsage As Single = 0
    Private currentRamUsage As Single = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' P1: 設定視窗標題
        Me.Text = TitleService.GetFormTitle(stationNumber)

        ' P2: 設定藍牙模組名稱
        lblBluetoothName.Text = TitleService.GetBluetoothModuleName(stationNumber)

        ' 初始化核心模組
        InitializeServices()

        ' 初始化 UI
        InitializeUI()

        ' P3: 載入 COM Port 清單
        RefreshComPorts()

        ' 啟動定時檢查 COM Port 變化
        timerPortCheck.Start()
    End Sub

    Private Sub InitializeServices()
        ' 初始化序列埠管理
        serialManager = New SerialPortManager()
        AddHandler serialManager.ConnectionStatusChanged, AddressOf OnConnectionStatusChanged
        AddHandler serialManager.DataReceived, AddressOf OnSerialDataReceived

        ' 初始化 COM Port 監控
        portWatcher = New ComPortWatcher()
        AddHandler portWatcher.PortsChanged, AddressOf OnPortsChanged

        ' 初始化 CPU/RAM 監控
        cpuProvider = New CpuLoadProvider()

        ' 初始化 EEPROM 服務
        eepromService = New EepromService()

        ' 初始化通訊協定
        bleProtocol = New BleProtocol()

        ' P12: 初始化 LED 顯示模組
        ledDisplay = New LedDisplayManager()
        ledDisplay.InitializeLeds(pnlLeds, 10, 10, 8)

        ' 初始化 LED 同步服務
        ledSync = New LedSyncService(ledDisplay)
    End Sub

    Private Sub InitializeUI()
        ' 設定初始連線狀態
        UpdateConnectionStatus(False)

        ' 初始化數值顯示
        lblCpuValue.Text = "0.0%"
        lblRamValue.Text = "0.0%"
        lblCpuColor.BackColor = Color.Black
    End Sub

    ' P3: 刷新 COM Port 清單
    Private Sub RefreshComPorts()
        Try
            Dim ports As List(Of String) = portWatcher.GetAvailablePorts()
            cmbComPorts.Items.Clear()

            If ports.Count > 0 Then
                For Each port As String In ports
                    cmbComPorts.Items.Add(port)
                Next
                If cmbComPorts.Items.Count > 0 Then
                    cmbComPorts.SelectedIndex = 0
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error refreshing COM ports: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' P4: 開啟 COM Port
    Private Sub btnOpenPort_Click(sender As Object, e As EventArgs) Handles btnOpenPort.Click
        If cmbComPorts.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a COM port first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedPort As String = cmbComPorts.SelectedItem.ToString()
        If serialManager.OpenPort(selectedPort) Then
            ' 連線成功後的 UI 更新由 OnConnectionStatusChanged 處理
        Else
            MessageBox.Show("Failed to open " & selectedPort, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ' P5: 關閉 COM Port
    Private Sub btnClosePort_Click(sender As Object, e As EventArgs) Handles btnClosePort.Click
        serialManager.ClosePort()
    End Sub

    Private Sub btnRefreshPorts_Click(sender As Object, e As EventArgs) Handles btnRefreshPorts.Click
        RefreshComPorts()
    End Sub

    ' P6 & P4 & P5: 連線狀態變更處理
    Private Sub OnConnectionStatusChanged(connected As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() OnConnectionStatusChanged(connected))
            Return
        End If

        UpdateConnectionStatus(connected)
    End Sub

    Private Sub UpdateConnectionStatus(connected As Boolean)
        If connected Then
            ' P4: Connected 狀態
            lblConnectionStatus.Text = "Connected"
            lblConnectionStatus.ForeColor = Color.Green
            btnOpenPort.Enabled = False
            btnClosePort.Enabled = True
            btnStartCpu.Enabled = True
            btnStartRam.Enabled = True
            btnWrite.Enabled = True
        Else
            ' P5: Disconnect 狀態
            lblConnectionStatus.Text = "Disconnect"
            lblConnectionStatus.ForeColor = Color.Red
            btnOpenPort.Enabled = True
            btnClosePort.Enabled = False
            btnStartCpu.Enabled = False
            btnStartRam.Enabled = False
            btnStopCpu.Enabled = False
            btnStopRam.Enabled = False
            btnWrite.Enabled = False

            ' 停止監控
            timerCpu.Stop()
            timerRam.Stop()

            ' 關閉 LED 顯示
            ledDisplay.TurnOffAllLeds()
        End If
    End Sub

    ' P7: CPU Loading 監控
    Private Sub btnStartCpu_Click(sender As Object, e As EventArgs) Handles btnStartCpu.Click
        timerCpu.Start()
        btnStartCpu.Enabled = False
        btnStopCpu.Enabled = True
    End Sub

    Private Sub btnStopCpu_Click(sender As Object, e As EventArgs) Handles btnStopCpu.Click
        timerCpu.Stop()
        btnStartCpu.Enabled = True
        btnStopCpu.Enabled = False
    End Sub

    Private Sub timerCpu_Tick(sender As Object, e As EventArgs) Handles timerCpu.Tick
        Try
            ' 取得 CPU 使用率
            currentCpuUsage = cpuProvider.GetCpuUsage()
            lblCpuValue.Text = currentCpuUsage.ToString("F1") & "%"

            ' 取得對應顏色
            Dim cpuColor As Color = cpuProvider.GetColorForCpuUsage(currentCpuUsage)
            lblCpuColor.BackColor = cpuColor

            ' P12: 同步更新 LED 顯示
            ledSync.SyncCpuLoadingColor(cpuColor)

            ' 傳送至 MCU
            If serialManager.IsConnected Then
                Dim packet As Byte() = bleProtocol.CreateCpuLoadPacket(CByte(currentCpuUsage), cpuColor)
                serialManager.SendData(packet)
            End If
        Catch ex As Exception
            ' 錯誤處理
        End Try
    End Sub

    ' RAM Loading 監控
    Private Sub btnStartRam_Click(sender As Object, e As EventArgs) Handles btnStartRam.Click
        timerRam.Start()
        btnStartRam.Enabled = False
        btnStopRam.Enabled = True
    End Sub

    Private Sub btnStopRam_Click(sender As Object, e As EventArgs) Handles btnStopRam.Click
        timerRam.Stop()
        btnStartRam.Enabled = True
        btnStopRam.Enabled = False
    End Sub

    Private Sub timerRam_Tick(sender As Object, e As EventArgs) Handles timerRam.Tick
        Try
            currentRamUsage = cpuProvider.GetMemoryUsage()
            lblRamValue.Text = currentRamUsage.ToString("F1") & "%"
        Catch ex As Exception
            ' 錯誤處理
        End Try
    End Sub

    ' P8 & P9: EEPROM 寫入
    Private Sub btnWrite_Click(sender As Object, e As EventArgs) Handles btnWrite.Click
        Dim input As String = txtBinaryInput.Text.Trim()

        ' P8: 驗證輸入
        If Not eepromService.ValidateBinaryInput(input) Then
            MessageBox.Show("Not BIN Format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtBinaryInput.Clear()
            txtBinaryInput.Focus()
            Return
        End If

        ' 轉換為十進位
        Dim decimalValue As Integer = eepromService.BinaryToDecimal(input)
        If decimalValue < 0 OrElse decimalValue > 15 Then
            MessageBox.Show("Invalid value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' P9: 傳送封包至 MCU
        If serialManager.IsConnected Then
            Dim packet As Byte() = bleProtocol.CreateEepromPacket(CByte(decimalValue))
            serialManager.SendData(packet)
            MessageBox.Show($"Binary {input} (Decimal {decimalValue}) sent to EEPROM", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtBinaryInput.Clear()
        End If
    End Sub

    ' 接收序列埠資料
    Private Sub OnSerialDataReceived(data As Byte())
        If Me.InvokeRequired Then
            Me.Invoke(Sub() OnSerialDataReceived(data))
            Return
        End If

        ' P12: 處理 LED 更新資料
        ledSync.ProcessReceivedData(data)
    End Sub

    ' COM Port 變更事件
    Private Sub OnPortsChanged(ports As List(Of String))
        If Me.InvokeRequired Then
            Me.Invoke(Sub() OnPortsChanged(ports))
            Return
        End If

        RefreshComPorts()
    End Sub

    ' 定時檢查 COM Port (備用方案)
    Private Sub timerPortCheck_Tick(sender As Object, e As EventArgs) Handles timerPortCheck.Tick
        If Not serialManager.IsConnected Then
            RefreshComPorts()
        End If
    End Sub

    ' P10: 退出程式
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Application.Exit()
    End Sub

    ' 清理資源
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            ' 停止所有計時器
            timerCpu.Stop()
            timerRam.Stop()
            timerPortCheck.Stop()

            ' 關閉序列埠
            If serialManager IsNot Nothing Then
                serialManager.ClosePort()
                serialManager.Dispose()
            End If

            ' 停止監控
            If portWatcher IsNot Nothing Then
                portWatcher.StopWatching()
            End If

            ' 釋放資源
            If cpuProvider IsNot Nothing Then
                cpuProvider.Dispose()
            End If

            If ledDisplay IsNot Nothing Then
                ledDisplay.Dispose()
            End If
        Catch ex As Exception
            ' 忽略清理時的錯誤
        End Try
    End Sub
End Class

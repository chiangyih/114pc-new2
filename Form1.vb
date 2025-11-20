'''<summary>
''' 113 學年度工科賽電腦修護職類第二站 PC 端主程式
''' 崗位號碼：01
''' 功能：藍牙/USB 序列埠通訊、CPU/RAM 監控、EEPROM 寫入、WS2812 LED 同步顯示
''' 開發語言：Visual Basic .NET 2022
''' 目標框架：.NET 8.0 Windows Forms
'''</summary>
Public Class Form1
    ' ========================================
    ' 核心服務模組宣告
    ' ========================================

    ''' <summary>序列埠管理器：負責 COM Port 的開啟、關閉與資料傳輸</summary>
    Private serialManager As SerialPortManager

    ''' <summary>COM Port 監控器：即時偵測 COM Port 的熱插拔變化</summary>
    Private portWatcher As ComPortWatcher

    ''' <summary>效能監控器：取得 CPU 與 RAM 使用率</summary>
    Private cpuProvider As CpuLoadProvider

    ''' <summary>EEPROM 服務：處理二進位輸入驗證與資料轉換</summary>
    Private eepromService As EepromService

    ''' <summary>藍牙通訊協定：封裝與解析藍牙通訊封包</summary>
    Private bleProtocol As BleProtocol

    ''' <summary>LED 顯示管理器：管理 8 顆圓形 LED 燈號的繪製與顯示</summary>
    Private ledDisplay As LedDisplayManager

    ''' <summary>LED 同步服務：處理 PC 與 Arduino 端 LED 狀態的同步</summary>
    Private ledSync As LedSyncService

    ' ========================================
    ' 狀態變數
    ' ========================================

    ''' <summary>崗位號碼（用於計算藍牙模組名稱與顯示標題）</summary>
    Private stationNumber As String = "01"

    ''' <summary>目前 CPU 使用率（百分比，0-100）</summary>
    Private currentCpuUsage As Single = 0

    ''' <summary>目前 RAM 使用率（百分比，0-100）</summary>
    Private currentRamUsage As Single = 0

    ' ========================================
    ' 表單載入事件（程式啟動時執行）
    ' ========================================
    ''' <summary>
    ''' 表單載入事件處理程序
    ''' 執行時機：程式啟動時
    ''' 主要工作：初始化所有模組、設定 UI、載入 COM Port 清單
    ''' </summary>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' ========================================
        ' P1: 設定視窗標題列文字
        ' 顯示完整賽事資訊與崗位號碼
        ' ========================================
        Me.Text = TitleService.GetFormTitle(stationNumber)

        ' ========================================
        ' P2: 設定藍牙模組名稱
        ' 根據崗位號碼計算奇偶性並產生對應名稱
        ' 例如：崗位 01 → ODD-01-0001
        ' ========================================
        lblBluetoothName.Text = TitleService.GetBluetoothModuleName(stationNumber)

        ' 初始化核心服務模組
        InitializeServices()

        ' 初始化使用者介面
        InitializeUI()

        ' ========================================
        ' P3: 載入 COM Port 清單
        ' 偵測系統中所有可用的序列埠（USB/藍牙）
        ' ========================================
        RefreshComPorts()

        ' 啟動定時器，每 2 秒檢查一次 COM Port 變化（熱插拔偵測）
        timerPortCheck.Start()
    End Sub

    ' ========================================
    ' 初始化核心服務模組
    ' ========================================
    ''' <summary>
    ''' 初始化所有核心服務模組並註冊事件處理程序
    ''' 採用模組化設計，各模組職責單一
    ''' </summary>
    Private Sub InitializeServices()
        ' ----------------------------------------
        ' 初始化序列埠管理器
        ' ----------------------------------------
        serialManager = New SerialPortManager()
        ' 註冊連線狀態變更事件（當連線/斷線時觸發）
        AddHandler serialManager.ConnectionStatusChanged, AddressOf OnConnectionStatusChanged
        ' 註冊資料接收事件（當序列埠收到資料時觸發）
        AddHandler serialManager.DataReceived, AddressOf OnSerialDataReceived

        ' ----------------------------------------
        ' 初始化 COM Port 監控器
        ' ----------------------------------------
        portWatcher = New ComPortWatcher()
        ' 註冊 Port 變更事件（當 COM Port 插拔時觸發）
        AddHandler portWatcher.PortsChanged, AddressOf OnPortsChanged

        ' ----------------------------------------
        ' 初始化效能監控器
        ' 使用 PerformanceCounter 取得系統效能資訊
        ' ----------------------------------------
        cpuProvider = New CpuLoadProvider()

        ' ----------------------------------------
        ' 初始化 EEPROM 服務
        ' 負責二進位輸入驗證與資料轉換
        ' ----------------------------------------
        eepromService = New EepromService()

        ' ----------------------------------------
        ' 初始化藍牙通訊協定
        ' 負責封包的打包與解析
        ' ----------------------------------------
        bleProtocol = New BleProtocol()

        ' ----------------------------------------
        ' P12: 初始化 LED 顯示模組
        ' 在面板上繪製 8 顆圓形 LED 燈號（直徑 22 像素）
        ' 參數：容器控件、起始 X、起始 Y、間距
        ' ----------------------------------------
        ledDisplay = New LedDisplayManager()
        ledDisplay.InitializeLeds(pnlLeds, 10, 10, 8)

        ' ----------------------------------------
        ' 初始化 LED 同步服務
        ' 負責處理 Arduino 傳來的 LED 狀態並更新 UI
        ' ----------------------------------------
        ledSync = New LedSyncService(ledDisplay)
    End Sub

    ' ========================================
    ' 初始化使用者介面
    ' ========================================
    ''' <summary>
    ''' 設定 UI 控件的初始狀態
    ''' 包含連線狀態、數值顯示、顏色指示器等
    ''' </summary>
    Private Sub InitializeUI()
        ' 設定初始連線狀態為「未連線`
        UpdateConnectionStatus(False)

        ' 初始化數值顯示標籤
        lblCpuValue.Text = "0.0%"
        lblRamValue.Text = "0.0%"
    End Sub

    ' ========================================
    ' P3: 刷新 COM Port 清單
    ' ========================================
    ''' <summary>
    ''' 刷新 COM Port 下拉選單
    ''' 偵測系統中所有可用的序列埠（包含 USB 與藍牙虛擬埠）
    ''' </summary>
    Private Sub RefreshComPorts()
        Try
            ' 取得所有可用的 COM Port
            Dim ports As List(Of String) = portWatcher.GetAvailablePorts()

            ' 清空下拉選單
            cmbComPorts.Items.Clear()

            ' 將所有 Port 加入下拉選單
            If ports.Count > 0 Then
                For Each port As String In ports
                    cmbComPorts.Items.Add(port)
                Next

                ' 預設選擇第一個 Port
                If cmbComPorts.Items.Count > 0 Then
                    cmbComPorts.SelectedIndex = 0
                End If
            End If
        Catch ex As Exception
            ' 顯示錯誤訊息
            MessageBox.Show("Error refreshing COM ports: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ========================================
    ' P4: 開啟 COM Port（建立連線）
    ' ========================================
    ''' <summary>
    ''' 開啟按鈕點擊事件
    ''' 開啟選定的 COM Port 並建立連線
    ''' 成功後自動發送連線字元 'c' 給 Arduino
    ''' </summary>
    Private Sub btnOpenPort_Click(sender As Object, e As EventArgs) Handles btnOpenPort.Click
        ' 檢查是否已選擇 COM Port
        If cmbComPorts.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a COM port first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 取得選定的 COM Port 名稱（例如：COM3）
        Dim selectedPort As String = cmbComPorts.SelectedItem.ToString()

        ' 嘗試開啟序列埠
        If serialManager.OpenPort(selectedPort) Then
            ' 連線成功後的 UI 更新由 OnConnectionStatusChanged 事件處理
            ' serialManager 會自動觸發 ConnectionStatusChanged 事件
        Else
            ' 連線失敗，顯示錯誤訊息
            MessageBox.Show("Failed to open " & selectedPort, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ' ========================================
    ' P5: 關閉 COM Port（中斷連線）
    ' ========================================
    ''' <summary>
    ''' 關閉按鈕點擊事件
    ''' 關閉目前的 COM Port 連線
    ''' 自動發送斷線字元 'b' 給 Arduino
    ''' </summary>
    Private Sub btnClosePort_Click(sender As Object, e As EventArgs) Handles btnClosePort.Click
        ' 關閉序列埠連線
        ' 會自動觸發 ConnectionStatusChanged 事件並更新 UI
        serialManager.ClosePort()
    End Sub

    ''' <summary>
    ''' 刷新按鈕點擊事件
    ''' 手動刷新 COM Port 清單（熱插拔偵測）
    ''' </summary>
    Private Sub btnRefreshPorts_Click(sender As Object, e As EventArgs) Handles btnRefreshPorts.Click
        RefreshComPorts()
    End Sub

    ' ========================================
    ' P6 & P4 & P5: 連線狀態變更事件處理
    ' ========================================
    ''' <summary>
    ''' 連線狀態變更事件處理程序
    ''' 當序列埠連線或斷線時由 SerialPortManager 觸發
    ''' </summary>
    ''' <param name="connected">True=已連線，False=已斷線</param>
    Private Sub OnConnectionStatusChanged(connected As Boolean)
        ' 檢查是否需要跨執行緒呼叫（序列埠事件在不同執行緒）
        If Me.InvokeRequired Then
            ' 使用 Invoke 切換到 UI 執行緒
            Me.Invoke(Sub() OnConnectionStatusChanged(connected))
            Return
        End If

        ' 更新 UI 連線狀態
        UpdateConnectionStatus(connected)
    End Sub

    ''' <summary>
    ''' 更新 UI 上的連線狀態與相關控件啟用狀態
    ''' </summary>
    ''' <param name="connected">True=已連線，False=已斷線</param>
    Private Sub UpdateConnectionStatus(connected As Boolean)
        If connected Then
            ' ========================================
            ' P4: Connected 狀態（已連線）
            ' ========================================
            lblConnectionStatus.Text = "Connected"
            lblConnectionStatus.ForeColor = Color.Green

            ' 按鈕狀態控制
            btnOpenPort.Enabled = False      ' 停用開啟按鈕
            btnClosePort.Enabled = True      ' 啟用關閉按鈕
            btnStartCpu.Enabled = True       ' 啟用 CPU 監控
            btnStartRam.Enabled = True       ' 啟用 RAM 監控
            btnWrite.Enabled = True          ' 啟用 EEPROM 寫入
        Else
            ' ========================================
            ' P5: Disconnect 狀態（未連線）
            ' ========================================
            lblConnectionStatus.Text = "Disconnect"
            lblConnectionStatus.ForeColor = Color.Red

            ' 按鈕狀態控制
            btnOpenPort.Enabled = True       ' 啟用開啟按鈕
            btnClosePort.Enabled = False     ' 停用關閉按鈕
            btnStartCpu.Enabled = False      ' 停用 CPU 監控
            btnStartRam.Enabled = False      ' 停用 RAM 監控
            btnStopCpu.Enabled = False       ' 停用 CPU 停止按鈕
            btnStopRam.Enabled = False       ' 停用 RAM 停止按鈕
            btnWrite.Enabled = False         ' 停用 EEPROM 寫入

            ' 停止所有監控計時器
            timerCpu.Stop()
            timerRam.Stop()

            ' 關閉所有 LED 顯示（顯示為黑色）
            ledDisplay.TurnOffAllLeds()
        End If
    End Sub

    ' ========================================
    ' P7: CPU Loading 監控
    ' ========================================
    ''' <summary>
    ''' CPU 監控啟動按鈕點擊事件
    ''' 啟動計時器，每秒更新一次 CPU 使用率
    ''' </summary>
    Private Sub btnStartCpu_Click(sender As Object, e As EventArgs) Handles btnStartCpu.Click
        ' 啟動 CPU 監控計時器（間隔 1000ms = 1 秒）
        timerCpu.Start()

        ' 切換按鈕啟用狀態
        btnStartCpu.Enabled = False
        btnStopCpu.Enabled = True
    End Sub

    ''' <summary>
    ''' CPU 監控停止按鈕點擊事件
    ''' 停止計時器，暫停 CPU 使用率更新，並重置 LED 燈號
    ''' </summary>
    Private Sub btnStopCpu_Click(sender As Object, e As EventArgs) Handles btnStopCpu.Click
        ' 停止 CPU 監控計時器
        timerCpu.Stop()

        ' 切換按鈕啟用狀態
        btnStartCpu.Enabled = True
        btnStopCpu.Enabled = False

        ' 重置 LED 燈號為黑色（關閉狀態）
        ledSync.ResetLeds()
    End Sub

    ''' <summary>
    ''' CPU 監控計時器 Tick 事件（每秒執行一次）
    ''' 主要工作：
    ''' 1. 取得 CPU 使用率
    ''' 2. 判斷對應顏色（綠/黃/紅）
    ''' 3. 更新 UI 顯示
    ''' 4. 同步 LED 燈號顏色
    ''' 5. 傳送簡單文字格式至 Arduino
    ''' </summary>
    Private Sub timerCpu_Tick(sender As Object, e As EventArgs) Handles timerCpu.Tick
        Try
            ' ----------------------------------------
            ' 步驟 1：取得 CPU 使用率（百分比）
            ' ----------------------------------------
            currentCpuUsage = cpuProvider.GetCpuUsage()
            lblCpuValue.Text = currentCpuUsage.ToString("F1") & "%"

            ' ----------------------------------------
            ' 步驟 2：根據使用率判斷對應顏色
            ' 規則：0~50% 綠色、51~84% 黃色、≥85% 紅色
            ' ----------------------------------------
            Dim cpuColor As Color = cpuProvider.GetColorForCpuUsage(currentCpuUsage)

            ' ----------------------------------------
            ' 步驟 3：同步更新 LED 顯示
            ' 將 8 顆 LED 燈號全部更新為對應顏色
            ' ----------------------------------------
            ledSync.SyncCpuLoadingColor(cpuColor)

            ' ----------------------------------------
            ' 步驟 4：傳送 CPU Loading 文字格式至 MCU
            ' 格式：LOAD數值\n（例如：LOAD45.2\n）
            ' ----------------------------------------
            If serialManager.IsConnected Then
                Dim message As String = "LOAD" & currentCpuUsage.ToString("F1") & vbLf
                Dim data As Byte() = System.Text.Encoding.ASCII.GetBytes(message)
                serialManager.SendData(data)
            End If
        Catch ex As Exception
            ' 錯誤處理：捕捉並忽略異常，避免程式崩潰
        End Try
    End Sub

    ' ========================================
    ' RAM Loading 監控
    ' ========================================
    ''' <summary>
    ''' RAM 監控啟動按鈕點擊事件
    ''' 啟動計時器，每秒更新一次 RAM 使用率
    ''' </summary>
    Private Sub btnStartRam_Click(sender As Object, e As EventArgs) Handles btnStartRam.Click
        ' 啟動 RAM 監控計時器（間隔 1000ms = 1 秒）
        timerRam.Start()

        ' 切換按鈕啟用狀態
        btnStartRam.Enabled = False
        btnStopRam.Enabled = True
    End Sub

    ''' <summary>
    ''' RAM 監控停止按鈕點擊事件
    ''' 停止計時器，暫停 RAM 使用率更新，並重置 LED 燈號
    ''' </summary>
    Private Sub btnStopRam_Click(sender As Object, e As EventArgs) Handles btnStopRam.Click
        ' 停止 RAM 監控計時器
        timerRam.Stop()

        ' 切換按鈕啟用狀態
        btnStartRam.Enabled = True
        btnStopRam.Enabled = False

        ' 重置 LED 燈號為黑色（關閉狀態）
        ledSync.ResetLeds()
    End Sub

    ''' <summary>
    ''' RAM 監控計時器 Tick 事件（每秒執行一次）
    ''' 取得記憶體使用率並更新 UI 顯示
    ''' 傳送簡單文字格式至 Arduino
    ''' </summary>
    Private Sub timerRam_Tick(sender As Object, e As EventArgs) Handles timerRam.Tick
        Try
            ' ----------------------------------------
            ' 步驟 1：取得記憶體使用率（百分比）
            ' ----------------------------------------
            currentRamUsage = cpuProvider.GetMemoryUsage()
            lblRamValue.Text = currentRamUsage.ToString("F1") & "%"

            ' ----------------------------------------
            ' 步驟 2：根據使用率判斷對應顏色
            ' 規則：0~50% 綠色、51~84% 黃色、≥85% 紅色
            ' ----------------------------------------
            Dim ramColor As Color = cpuProvider.GetColorForRamUsage(currentRamUsage)

            ' ----------------------------------------
            ' 步驟 3：同步更新 LED 顯示
            ' 將 8 顆 LED 燈號全部更新為對應顏色
            ' ----------------------------------------
            ledSync.SyncRamLoadingColor(ramColor)

            ' ----------------------------------------
            ' 步驟 4：傳送 RAM Loading 文字格式至 MCU
            ' 格式：LOAD數值\n（例如：LOAD62.8\n）
            ' ----------------------------------------
            If serialManager.IsConnected Then
                Dim message As String = "LOAD" & currentRamUsage.ToString("F1") & vbLf
                Dim data As Byte() = System.Text.Encoding.ASCII.GetBytes(message)
                serialManager.SendData(data)
            End If
        Catch ex As Exception
            ' 錯誤處理：捕捉並忽略異常
        End Try
    End Sub

    ' ========================================
    ' P8 & P9: EEPROM 寫入
    ' ========================================
    ''' <summary>
    ''' EEPROM 寫入按鈕點擊事件
    ''' 主要工作：
    ''' 1. 驗證二進位輸入格式（必須為 4 位元 0/1）
    ''' 2. 轉換為十進位數值（0-15）
    ''' 3. 封裝封包並傳送至 Arduino EEPROM
    ''' </summary>
    Private Sub btnWrite_Click(sender As Object, e As EventArgs) Handles btnWrite.Click
        ' 取得使用者輸入並移除前後空白
        Dim input As String = txtBinaryInput.Text.Trim()

        ' ----------------------------------------
        ' P8: 驗證二進位輸入格式
        ' 必須恰好為 4 個字元，且每個字元只能是 0 或 1
        ' ----------------------------------------
        If Not eepromService.ValidateBinaryInput(input) Then
            ' 格式錯誤，顯示警告訊息
            MessageBox.Show("Not BIN Format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtBinaryInput.Clear()      ' 清空輸入框
            txtBinaryInput.Focus()      ' 設定焦點回輸入框
            Return
        End If

        ' ----------------------------------------
        ' 轉換二進位為十進位
        ' 例如：0101 → 5, 1111 → 15
        ' ----------------------------------------
        Dim decimalValue As Integer = eepromService.BinaryToDecimal(input)

        ' 再次驗證數值範圍（應該在 0-15 之間）
        If decimalValue < 0 OrElse decimalValue > 15 Then
            MessageBox.Show("Invalid value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' ----------------------------------------
        ' P9: 傳送封包至 MCU
        ' 封包格式：[SOF][CMD][LEN][Data][CHK][EOF]
        ' ----------------------------------------
        If serialManager.IsConnected Then
            ' 建立 EEPROM 寫入封包
            Dim packet As Byte() = bleProtocol.CreateEepromPacket(CByte(decimalValue))

            ' 透過序列埠傳送
            serialManager.SendData(packet)

            ' 顯示成功訊息
            MessageBox.Show($"Binary {input} (Decimal {decimalValue}) sent to EEPROM", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' 清空輸入框，準備下次輸入
            txtBinaryInput.Clear()
        End If
    End Sub

    ' ========================================
    ' 序列埠資料接收事件處理
    ' ========================================
    ''' <summary>
    ''' 序列埠資料接收事件處理程序
    ''' 當序列埠收到資料時由 SerialPortManager 觸發
    ''' 主要用於接收 Arduino 傳來的 LED 狀態更新
    ''' </summary>
    ''' <param name="data">接收到的位元組陣列</param>
    Private Sub OnSerialDataReceived(data As Byte())
        ' 檢查是否需要跨執行緒呼叫（序列埠事件在不同執行緒）
        If Me.InvokeRequired Then
            ' 使用 Invoke 切換到 UI 執行緒
            Me.Invoke(Sub() OnSerialDataReceived(data))
            Return
        End If

        ' ----------------------------------------
        ' P12: 處理 LED 更新資料
        ' 解析 Arduino 傳來的 LED RGB 資料並同步更新 UI
        ' ----------------------------------------
        ledSync.ProcessReceivedData(data)
    End Sub

    ' ========================================
    ' COM Port 變更事件處理（熱插拔偵測）
    ' ========================================
    ''' <summary>
    ''' COM Port 變更事件處理程序
    ''' 當 COM Port 插入或拔除時由 ComPortWatcher 觸發
    ''' </summary>
    ''' <param name="ports">更新後的 COM Port 清單</param>
    Private Sub OnPortsChanged(ports As List(Of String))
        ' 檢查是否需要跨執行緒呼叫
        If Me.InvokeRequired Then
            Me.Invoke(Sub() OnPortsChanged(ports))
            Return
        End If

        ' 重新載入 COM Port 清單
        RefreshComPorts()
    End Sub

    ' ========================================
    ' 定時檢查 COM Port（備用方案）
    ' ========================================
    ''' <summary>
    ''' 定時檢查計時器 Tick 事件（每 2 秒執行一次）
    ''' 作為 ManagementEventWatcher 的備用方案
    ''' 僅在未連線時才執行檢查，避免不必要的資源消耗
    ''' </summary>
    Private Sub timerPortCheck_Tick(sender As Object, e As EventArgs) Handles timerPortCheck.Tick
        ' 僅在未連線狀態下才檢查 COM Port 變化
        If Not serialManager.IsConnected Then
            RefreshComPorts()
        End If
    End Sub

    ' ========================================
    ' P10: 退出程式
    ' ========================================
    ''' <summary>
    ''' 退出按鈕點擊事件
    ''' 安全關閉程式並釋放所有資源
    ''' </summary>
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        ' 關閉整個應用程式
        ' FormClosing 事件會自動觸發進行資源清理
        Application.Exit()
    End Sub

    ' ========================================
    ' 表單關閉事件（資源清理）
    ' ========================================
    ''' <summary>
    ''' 表單關閉事件處理程序
    ''' 執行時機：程式結束前
    ''' 主要工作：停止所有計時器、關閉序列埠、釋放所有資源
    ''' </summary>
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            ' ----------------------------------------
            ' 步驟 1：停止所有計時器
            ' ----------------------------------------
            timerCpu.Stop()          ' 停止 CPU 監控計時器
            timerRam.Stop()          ' 停止 RAM 監控計時器
            timerPortCheck.Stop()    ' 停止 Port 檢查計時器

            ' ----------------------------------------
            ' 步驟 2：關閉序列埠連線
            ' ----------------------------------------
            If serialManager IsNot Nothing Then
                serialManager.ClosePort()    ' 關閉 COM Port
                serialManager.Dispose()      ' 釋放序列埠資源
            End If

            ' ----------------------------------------
            ' 步驟 3：停止 COM Port 監控
            ' ----------------------------------------
            If portWatcher IsNot Nothing Then
                portWatcher.StopWatching()   ' 停止 ManagementEventWatcher
            End If

            ' ----------------------------------------
            ' 步驟 4：釋放效能監控器資源
            ' ----------------------------------------
            If cpuProvider IsNot Nothing Then
                cpuProvider.Dispose()        ' 釋放 PerformanceCounter
            End If

            ' ----------------------------------------
            ' 步驟 5：釋放 LED 顯示資源
            ' ----------------------------------------
            If ledDisplay IsNot Nothing Then
                ledDisplay.Dispose()         ' 釋放圖形資源
            End If
        Catch ex As Exception
            ' 忽略清理時的錯誤，避免影響程式關閉
        End Try
    End Sub
End Class

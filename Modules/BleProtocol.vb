'''<summary>
''' 藍牙通訊協定模組
''' 負責封包的打包與解析
''' 支援 ASCII 字串格式與二進位格式
'''</summary>
Public Class BleProtocol
    ' ========================================
    ' 命令碼定義（二進位格式使用）
    ' ========================================
    Public Const CMD_OPEN As Byte = &H01
    Public Const CMD_CLOSE As Byte = &H02
    Public Const CMD_CPU_LOAD As Byte = &H10
    Public Const CMD_EEPROM As Byte = &H20
    Public Const CMD_LED_UPDATE As Byte = &H10

    ' 封包標記（二進位格式使用）
    Public Const SOF As Byte = &HAA
    Public Const EOF As Byte = &H55

    ' ========================================
    ' 建立 CPU Loading 封包（ASCII 字串格式）
    ' 格式：#COLOR:RRGGBB
    ' 範例：#COLOR:FF0000 (紅色)
    ' ========================================
    ''' <summary>
    ''' 建立 CPU Loading 顏色命令（ASCII 字串格式）
    ''' </summary>
    ''' <param name="cpuUsage">CPU 使用率（0-100），未使用但保留以維持介面一致</param>
    ''' <param name="color">要顯示的顏色</param>
    ''' <returns>ASCII 字串的位元組陣列</returns>
    Public Function CreateCpuLoadPacket(cpuUsage As Byte, color As Color) As Byte()
        ' ----------------------------------------
        ' 建立 ASCII 字串格式：#COLOR:RRGGBB
        ' RR = 紅色 (00-FF 的 16 進位)
        ' GG = 綠色 (00-FF 的 16 進位)
        ' BB = 藍色 (00-FF 的 16 進位)
        ' ----------------------------------------
        Dim colorString As String = String.Format("#COLOR:{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B)
        
        ' 將字串轉換為 ASCII 位元組陣列
        Dim packet As Byte() = System.Text.Encoding.ASCII.GetBytes(colorString)
        
        Return packet
    End Function

    ' ========================================
    ' 建立 EEPROM 寫入封包（二進位格式）
    ' 格式：[SOF][CMD][LEN][Data][CHK][EOF]
    ' ========================================
    ''' <summary>
    ''' 建立 EEPROM 寫入封包（維持二進位格式）
    ''' </summary>
    ''' <param name="value">要寫入的數值（0-15）</param>
    ''' <returns>二進位封包的位元組陣列</returns>
    Public Function CreateEepromPacket(value As Byte) As Byte()
        Dim packet As New List(Of Byte)
        
        packet.Add(SOF)          ' 起始字元
        packet.Add(CMD_EEPROM)   ' 命令碼
        packet.Add(1)            ' 資料長度
        packet.Add(value)        ' 資料值
        
        ' 計算校驗碼
        Dim checksum As Byte = CalculateChecksum(packet.ToArray(), 1, packet.Count - 1)
        packet.Add(checksum)     ' 校驗碼
        packet.Add(EOF)          ' 結束字元
        
        Return packet.ToArray()
    End Function

    ' ========================================
    ' 解析 LED 更新封包（從 Arduino 接收）
    ' 支援二進位格式
    ' ========================================
    ''' <summary>
    ''' 解析從 Arduino 回傳的 LED 狀態封包
    ''' </summary>
    ''' <param name="buffer">接收到的位元組陣列</param>
    ''' <param name="ledColors">輸出參數：解析出的 LED 顏色陣列</param>
    ''' <returns>True=解析成功，False=解析失敗</returns>
    Public Function ParseLedUpdatePacket(buffer As Byte(), ByRef ledColors() As Color) As Boolean
        Try
            ' 驗證封包格式: [SOF][CMD][LEN][LED0_R][LED0_G][LED0_B]...[LED7_R][LED7_G][LED7_B][CHK][EOF]
            If buffer.Length < 28 Then Return False ' 最小長度檢查
            If buffer(0) <> SOF Then Return False
            If buffer(1) <> CMD_LED_UPDATE Then Return False
            
            Dim dataLen As Integer = buffer(2)
            If dataLen <> 24 Then Return False ' 8顆燈 * 3色彩
            
            ' 驗證校驗碼
            Dim receivedChecksum As Byte = buffer(3 + dataLen)
            Dim calculatedChecksum As Byte = CalculateChecksum(buffer, 1, dataLen + 2)
            
            If receivedChecksum <> calculatedChecksum Then Return False
            If buffer(3 + dataLen + 1) <> EOF Then Return False
            
            ' 解析 LED 顏色
            ReDim ledColors(7)
            For i As Integer = 0 To 7
                Dim r As Byte = buffer(3 + i * 3)
                Dim g As Byte = buffer(4 + i * 3)
                Dim b As Byte = buffer(5 + i * 3)
                ledColors(i) = Color.FromArgb(r, g, b)
            Next
            
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' ========================================
    ' 計算校驗碼（二進位格式使用）
    ' ========================================
    ''' <summary>
    ''' 計算封包的簡易校驗碼（總和 AND 0xFF）
    ''' </summary>
    Private Function CalculateChecksum(data As Byte(), startIndex As Integer, length As Integer) As Byte
        Dim sum As Integer = 0
        For i As Integer = startIndex To startIndex + length - 1
            sum += data(i)
        Next
        Return CByte(sum And &HFF)
    End Function

    ' ========================================
    ' 輔助方法：顏色轉 16 進位字串
    ' ========================================
    ''' <summary>
    ''' 將顏色物件轉換為 6 位 16 進位字串（RRGGBB）
    ''' </summary>
    ''' <param name="color">顏色物件</param>
    ''' <returns>6 位 16 進位字串，例如：FF0000</returns>
    Public Shared Function ColorToHexString(color As Color) As String
        Return String.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B)
    End Function

    ''' <summary>
    ''' 將 6 位 16 進位字串（RRGGBB）轉換為顏色物件
    ''' </summary>
    ''' <param name="hexString">6 位 16 進位字串，例如：FF0000</param>
    ''' <returns>顏色物件</returns>
    Public Shared Function HexStringToColor(hexString As String) As Color
        Try
            If hexString.Length <> 6 Then Return Color.Black
            
            Dim r As Integer = Convert.ToInt32(hexString.Substring(0, 2), 16)
            Dim g As Integer = Convert.ToInt32(hexString.Substring(2, 2), 16)
            Dim b As Integer = Convert.ToInt32(hexString.Substring(4, 2), 16)
            
            Return Color.FromArgb(r, g, b)
        Catch ex As Exception
            Return Color.Black
        End Try
    End Function
End Class

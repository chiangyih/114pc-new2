Public Class BleProtocol
    ' 命令碼定義
    Public Const CMD_OPEN As Byte = &H01
    Public Const CMD_CLOSE As Byte = &H02
    Public Const CMD_CPU_LOAD As Byte = &H10
    Public Const CMD_EEPROM As Byte = &H20
    Public Const CMD_LED_UPDATE As Byte = &H10

    ' 封包標記
    Public Const SOF As Byte = &HAA
    Public Const EOF As Byte = &H55

    Public Function CreateCpuLoadPacket(cpuUsage As Byte, color As Color) As Byte()
        ' 封包格式: [SOF][CMD][LEN][CPU][R][G][B][CHK][EOF]
        Dim packet As New List(Of Byte)
        
        packet.Add(SOF)
        packet.Add(CMD_CPU_LOAD)
        packet.Add(4) ' LEN: CPU + RGB
        packet.Add(cpuUsage)
        packet.Add(color.R)
        packet.Add(color.G)
        packet.Add(color.B)
        
        ' 計算校驗碼
        Dim checksum As Byte = CalculateChecksum(packet.ToArray(), 1, packet.Count - 1)
        packet.Add(checksum)
        packet.Add(EOF)
        
        Return packet.ToArray()
    End Function

    Public Function CreateEepromPacket(value As Byte) As Byte()
        Dim packet As New List(Of Byte)
        
        packet.Add(SOF)
        packet.Add(CMD_EEPROM)
        packet.Add(1)
        packet.Add(value)
        
        Dim checksum As Byte = CalculateChecksum(packet.ToArray(), 1, packet.Count - 1)
        packet.Add(checksum)
        packet.Add(EOF)
        
        Return packet.ToArray()
    End Function

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

    Private Function CalculateChecksum(data As Byte(), startIndex As Integer, length As Integer) As Byte
        Dim sum As Integer = 0
        For i As Integer = startIndex To startIndex + length - 1
            sum += data(i)
        Next
        Return CByte(sum And &HFF)
    End Function
End Class

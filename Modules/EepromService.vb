Imports System.Text.RegularExpressions

Public Class EepromService
    Public Function ValidateBinaryInput(input As String) As Boolean
        ' 驗證是否為 4 位二進位數值
        Dim pattern As String = "^[01]{4}$"
        Return Regex.IsMatch(input, pattern)
    End Function

    Public Function BinaryToDecimal(binaryStr As String) As Integer
        Try
            Return Convert.ToInt32(binaryStr, 2)
        Catch ex As Exception
            Return -1
        End Try
    End Function

    Public Function CreateEepromPacket(decimalValue As Byte) As Byte()
        ' 封包格式: [SOF][CMD][LEN][Data][CHK][EOF]
        Dim packet As New List(Of Byte)
        
        packet.Add(&HAA) ' SOF
        packet.Add(&H20) ' CMD_EEPROM
        packet.Add(1)    ' LEN
        packet.Add(decimalValue) ' Data
        
        ' 計算校驗碼
        Dim checksum As Byte = CalculateChecksum(packet.ToArray(), 1, packet.Count - 1)
        packet.Add(checksum) ' CHK
        packet.Add(&H55) ' EOF
        
        Return packet.ToArray()
    End Function

    Private Function CalculateChecksum(data As Byte(), startIndex As Integer, length As Integer) As Byte
        Dim sum As Integer = 0
        For i As Integer = startIndex To startIndex + length - 1
            sum += data(i)
        Next
        Return CByte(sum And &HFF)
    End Function
End Class

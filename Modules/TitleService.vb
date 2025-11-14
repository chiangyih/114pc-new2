Public Class TitleService
    Public Shared Function GetFormTitle(stationNumber As String) As String
        Return $"113 學年度 工業類科學生技藝競賽 電腦修護職種 新化高工 第二站 崗位號碼：{stationNumber}"
    End Function

    Public Shared Function GetBluetoothModuleName(stationNumber As String) As String
        Try
            ' 將崗位號碼轉為整數
            Dim number As Integer = Integer.Parse(stationNumber)
            
            ' 轉為二進位（8位元）
            Dim binary As String = Convert.ToString(number, 2).PadLeft(8, "0"c)
            
            ' 取最右位判斷奇偶
            Dim lastBit As Char = binary(binary.Length - 1)
            
            ' 取後 4 位作為 BBBB
            Dim last4Bits As String = binary.Substring(4, 4)
            
            If lastBit = "1"c Then
                ' 奇數
                Return $"ODD-{stationNumber.PadLeft(2, "0"c)}-{last4Bits}"
            Else
                ' 偶數
                Return $"EVEN-{stationNumber.PadLeft(2, "0"c)}-{last4Bits}"
            End If
        Catch ex As Exception
            Return "ERROR-00-0000"
        End Try
    End Function
End Class

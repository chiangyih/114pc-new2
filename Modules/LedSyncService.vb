Public Class LedSyncService
    Private protocol As BleProtocol
    Private ledDisplay As LedDisplayManager

    Public Sub New(ledDisplayManager As LedDisplayManager)
        protocol = New BleProtocol()
        ledDisplay = ledDisplayManager
    End Sub

    Public Sub ProcessReceivedData(data As Byte())
        Try
            ' 檢查是否為 LED 更新封包
            If data.Length >= 28 AndAlso data(0) = BleProtocol.SOF AndAlso data(1) = BleProtocol.CMD_LED_UPDATE Then
                Dim ledColors(7) As Color
                
                If protocol.ParseLedUpdatePacket(data, ledColors) Then
                    ' 同步更新 UI
                    ledDisplay.UpdateLedsFromArray(ledColors)
                End If
            End If
        Catch ex As Exception
            ' 錯誤處理
        End Try
    End Sub

    ''' <summary>
    ''' 同步 CPU Loading 顏色到所有 LED
    ''' </summary>
    Public Sub SyncCpuLoadingColor(color As Color)
        ' 當 CPU Loading 顯示時，同步更新所有燈號
        ledDisplay.UpdateAllLeds(color)
    End Sub

    ''' <summary>
    ''' 同步 RAM Loading 顏色到所有 LED
    ''' </summary>
    Public Sub SyncRamLoadingColor(color As Color)
        ' 當 RAM Loading 顯示時，同步更新所有燈號
        ledDisplay.UpdateAllLeds(color)
    End Sub

    ''' <summary>
    ''' 重置所有 LED 為黑色（關閉狀態）
    ''' </summary>
    Public Sub ResetLeds()
        ledDisplay.TurnOffAllLeds()
    End Sub
End Class

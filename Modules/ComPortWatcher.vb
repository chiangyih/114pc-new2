Imports System.IO.Ports
Imports System.Management

Public Class ComPortWatcher
    Private watcher As ManagementEventWatcher
    Private availablePorts As List(Of String)

    Public Event PortsChanged(ports As List(Of String))

    Public Sub New()
        availablePorts = New List(Of String)
    End Sub

    Public Function GetAvailablePorts() As List(Of String)
        Try
            Dim ports As String() = SerialPort.GetPortNames()
            availablePorts = New List(Of String)(ports)
            Array.Sort(ports)
            Return New List(Of String)(ports)
        Catch ex As Exception
            Return New List(Of String)()
        End Try
    End Function

    Public Sub StartWatching()
        Try
            ' 監聽 USB 裝置插拔事件
            Dim query As New WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3")
            watcher = New ManagementEventWatcher(query)
            AddHandler watcher.EventArrived, AddressOf OnDeviceChanged
            watcher.Start()
        Catch ex As Exception
            ' 如果無法啟動監聽，使用定時器備用方案
        End Try
    End Sub

    Public Sub StopWatching()
        Try
            If watcher IsNot Nothing Then
                watcher.Stop()
                watcher.Dispose()
            End If
        Catch ex As Exception
            ' 錯誤處理
        End Try
    End Sub

    Private Sub OnDeviceChanged(sender As Object, e As EventArrivedEventArgs)
        ' 延遲一下讓系統更新
        Threading.Thread.Sleep(500)
        Dim ports = GetAvailablePorts()
        RaiseEvent PortsChanged(ports)
    End Sub
End Class

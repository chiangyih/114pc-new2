Imports System.IO.Ports

Public Class SerialPortManager
    Private WithEvents serialPort As SerialPort
    Private _isConnected As Boolean = False

    Public Event DataReceived(data As Byte())
    Public Event ConnectionStatusChanged(connected As Boolean)

    Public Sub New()
        serialPort = New SerialPort()
        serialPort.BaudRate = 9600
        serialPort.DataBits = 8
        serialPort.Parity = Parity.None
        serialPort.StopBits = StopBits.One
    End Sub

    Public Function OpenPort(portName As String) As Boolean
        Try
            If serialPort.IsOpen Then
                serialPort.Close()
            End If

            serialPort.PortName = portName
            serialPort.Open()
            _isConnected = True
            RaiseEvent ConnectionStatusChanged(True)

            ' 發送連線字元 'c'
            SendData(New Byte() {Asc("c")})
            Return True
        Catch ex As Exception
            _isConnected = False
            RaiseEvent ConnectionStatusChanged(False)
            Return False
        End Try
    End Function

    Public Sub ClosePort()
        Try
            If serialPort.IsOpen Then
                ' 發送斷線字元 'b'
                SendData(New Byte() {Asc("b")})
                Threading.Thread.Sleep(100) ' 等待發送完成
                serialPort.Close()
            End If
            _isConnected = False
            RaiseEvent ConnectionStatusChanged(False)
        Catch ex As Exception
            ' 錯誤處理
        End Try
    End Sub

    Public Sub SendData(data As Byte())
        Try
            If serialPort.IsOpen Then
                serialPort.Write(data, 0, data.Length)
            End If
        Catch ex As Exception
            ' 錯誤處理
        End Try
    End Sub

    Private Sub SerialPort_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles serialPort.DataReceived
        Try
            Dim bytesToRead As Integer = serialPort.BytesToRead
            If bytesToRead > 0 Then
                Dim buffer(bytesToRead - 1) As Byte
                serialPort.Read(buffer, 0, bytesToRead)
                RaiseEvent DataReceived(buffer)
            End If
        Catch ex As Exception
            ' 錯誤處理
        End Try
    End Sub

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return _isConnected AndAlso serialPort.IsOpen
        End Get
    End Property

    Public Sub Dispose()
        If serialPort IsNot Nothing Then
            If serialPort.IsOpen Then
                ClosePort()
            End If
            serialPort.Dispose()
        End If
    End Sub
End Class

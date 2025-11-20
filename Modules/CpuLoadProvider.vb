Imports System.Diagnostics

Public Class CpuLoadProvider
    Private cpuCounter As PerformanceCounter
    Private memCounter As PerformanceCounter
    Private isInitialized As Boolean = False

    Public Sub New()
        Try
            cpuCounter = New PerformanceCounter("Processor", "% Processor Time", "_Total")
            memCounter = New PerformanceCounter("Memory", "% Committed Bytes In Use")
            ' 第一次讀取通常不準確，先讀取一次
            cpuCounter.NextValue()
            memCounter.NextValue()
            isInitialized = True
        Catch ex As Exception
            isInitialized = False
        End Try
    End Sub

    Public Function GetCpuUsage() As Single
        Try
            If isInitialized Then
                Return cpuCounter.NextValue()
            End If
            Return 0
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function GetMemoryUsage() As Single
        Try
            If isInitialized Then
                Return memCounter.NextValue()
            End If
            Return 0
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function GetColorForCpuUsage(usage As Single) As Color
        If usage >= 85 Then
            Return Color.Red ' ?85% 紅色
        ElseIf usage >= 51 Then
            Return Color.Yellow ' 51~84% 黃色
        Else
            Return Color.Lime ' 0~50% 綠色
        End If
    End Function

    ''' <summary>
    ''' 根據 RAM 使用率判斷對應顏色
    ''' 規則：0~50% 綠色、51~84% 黃色、?85% 紅色
    ''' </summary>
    Public Function GetColorForRamUsage(usage As Single) As Color
        If usage >= 85 Then
            Return Color.Red ' ?85% 紅色
        ElseIf usage >= 51 Then
            Return Color.Yellow ' 51~84% 黃色
        Else
            Return Color.Lime ' 0~50% 綠色
        End If
    End Function

    Public Sub Dispose()
        If cpuCounter IsNot Nothing Then
            cpuCounter.Dispose()
        End If
        If memCounter IsNot Nothing Then
            memCounter.Dispose()
        End If
    End Sub
End Class

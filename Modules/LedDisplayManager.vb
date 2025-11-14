Public Class LedDisplayManager
    Private ledPictures(7) As PictureBox
    Private ledColors(7) As Color
    Private Const LED_SIZE As Integer = 22 ' 直徑 22 像素

    Public Sub New()
        ' 初始化所有燈號為黑色
        For i As Integer = 0 To 7
            ledColors(i) = Color.Black
        Next
    End Sub

    Public Sub InitializeLeds(container As Control, startX As Integer, startY As Integer, spacing As Integer)
        ' 創建 8 顆圓形燈號
        For i As Integer = 0 To 7
            ledPictures(i) = New PictureBox()
            ledPictures(i).Width = LED_SIZE
            ledPictures(i).Height = LED_SIZE
            ledPictures(i).Left = startX + (i * (LED_SIZE + spacing))
            ledPictures(i).Top = startY
            ledPictures(i).BackColor = Color.Transparent
            
            container.Controls.Add(ledPictures(i))
            
            ' 繪製初始狀態
            DrawLed(i, Color.Black)
        Next
    End Sub

    Public Sub UpdateLed(index As Integer, color As Color)
        If index >= 0 AndAlso index <= 7 Then
            ledColors(index) = color
            DrawLed(index, color)
        End If
    End Sub

    Public Sub UpdateAllLeds(color As Color)
        For i As Integer = 0 To 7
            UpdateLed(i, color)
        Next
    End Sub

    Public Sub UpdateLedsFromArray(colors() As Color)
        If colors IsNot Nothing AndAlso colors.Length = 8 Then
            For i As Integer = 0 To 7
                UpdateLed(i, colors(i))
            Next
        End If
    End Sub

    Private Sub DrawLed(index As Integer, color As Color)
        If ledPictures(index) Is Nothing Then Return
        
        Dim bmp As New Bitmap(LED_SIZE, LED_SIZE)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.Clear(Color.Transparent)
            
            ' 繪製外圈邊框
            Using borderPen As New Pen(Color.Gray, 1)
                g.DrawEllipse(borderPen, 1, 1, LED_SIZE - 3, LED_SIZE - 3)
            End Using
            
            ' 填充顏色
            Using brush As New SolidBrush(color)
                g.FillEllipse(brush, 2, 2, LED_SIZE - 4, LED_SIZE - 4)
            End Using
            
            ' 添加光暈效果（如果不是黑色）
            If color <> Color.Black AndAlso color.GetBrightness() > 0.1 Then
                Dim centerX As Integer = LED_SIZE \ 2
                Dim centerY As Integer = LED_SIZE \ 2
                
                Using path As New Drawing2D.GraphicsPath()
                    path.AddEllipse(4, 4, LED_SIZE - 8, LED_SIZE - 8)
                    Using pthGrBrush As New Drawing2D.PathGradientBrush(path)
                        pthGrBrush.CenterColor = Color.FromArgb(180, Color.White)
                        pthGrBrush.SurroundColors = New Color() {Color.FromArgb(0, color)}
                        g.FillEllipse(pthGrBrush, 4, 4, LED_SIZE - 8, LED_SIZE - 8)
                    End Using
                End Using
            End If
        End Using
        
        ledPictures(index).Image = bmp
    End Sub

    Public Sub TurnOffAllLeds()
        UpdateAllLeds(Color.Black)
    End Sub

    Public Sub Dispose()
        For i As Integer = 0 To 7
            If ledPictures(i) IsNot Nothing Then
                If ledPictures(i).Image IsNot Nothing Then
                    ledPictures(i).Image.Dispose()
                End If
                ledPictures(i).Dispose()
            End If
        Next
    End Sub
End Class

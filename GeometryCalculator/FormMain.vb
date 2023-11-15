Imports System.Drawing.Text

Public Class FormMain
    Private isDrawing As Boolean = False
    Private startPoint As Point
    Private endPoint As Point
    Private bufferBitmap As Bitmap

    Private circleList As New List(Of Circle)
    Private lineList As New List(Of Line)
    Private pointList As New List(Of Point)

    Private brhBlack As New SolidBrush(Color.Black)
    Private brhRed As New SolidBrush(Color.Red)
    Private brhGreen As New SolidBrush(Color.Green)
    Private brhBlue As New SolidBrush(Color.Blue)
    Private brhMagenta As New SolidBrush(Color.Magenta)

    Dim fontNormal As New Font("Seoge UI", 8)
    Dim fontBig As New Font("Seoge UI", 14)

    Dim mode As Integer = -1
    Dim ptMin As Point
    Dim ptMax As Point
    Dim ptTangents() As PointF

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' Create the buffer bitmap
        bufferBitmap = New Bitmap(picView.Width, picView.Height)
    End Sub
    Private Sub picView_MouseDown(sender As Object, e As MouseEventArgs) Handles picView.MouseDown
        ' Start drawing when the mouse is clicked
        isDrawing = True
        startPoint = e.Location
    End Sub
    Private Sub picView_MouseUp(sender As Object, e As MouseEventArgs) Handles picView.MouseUp
        endPoint = e.Location
        ' Finish drawing when the mouse is released
        If isDrawing Then
            If radioCircle.Checked Then
                Dim circle As Circle = New Circle(startPoint, endPoint)
                circleList.Add(circle)
            End If

            If radioLine.Checked Then
                Dim line As Line = New Line(startPoint, endPoint)
                lineList.Add(line)
            End If

        End If
        isDrawing = False
        DrawOnBuffer()
        picView.Invalidate() ' Force the PictureBox to redraw
    End Sub
    Private Sub picView_MouseMove(sender As Object, e As MouseEventArgs) Handles picView.MouseMove
        If isDrawing Then
            endPoint = e.Location
            DrawOnBuffer()
            picView.Invalidate() ' Force the PictureBox to redraw
        End If
    End Sub

    Private Sub picView_MouseClick(sender As Object, e As MouseEventArgs) Handles picView.MouseClick
        If radioPoint.Checked Then
            pointList.Add(startPoint)
            DrawOnBuffer()
            picView.Invalidate()
        End If
    End Sub
    Private Sub picView_Paint(sender As Object, e As PaintEventArgs) Handles picView.Paint
        ' Display the buffer on the PictureBox
        e.Graphics.DrawImage(bufferBitmap, 0, 0)
    End Sub
    Private Sub DrawOnBuffer()
        ' Draw the elements on the buffer
        Using g As Graphics = Graphics.FromImage(bufferBitmap)
            g.TextRenderingHint = TextRenderingHint.AntiAlias
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.Clear(Color.White)

            ' Draw the current active element
            If isDrawing Then
                If radioCircle.Checked Then
                    Dim circle As Circle = New Circle(startPoint, endPoint)
                    g.DrawEllipse(Pens.Red, circle.rect)
                End If

                If radioLine.Checked Then
                    g.DrawLine(Pens.Red, startPoint, endPoint)
                End If

                If radioPoint.Checked Then
                    g.FillEllipse(brhBlue, startPoint.X - 2, startPoint.Y - 2, 4, 4)
                End If
            End If

            ' Draw the inactive elements
            For Each circle As Circle In circleList
                g.DrawEllipse(Pens.Magenta, circle.rect)
                g.FillEllipse(brhMagenta, CInt(circle.center.X - 2), CInt(circle.center.Y - 2), 4, 4)
                g.DrawString("(" + circle.center.X.ToString + ", " + circle.center.Y.ToString + ")",
                             fontNormal, brhMagenta, circle.center.X + 5, circle.center.Y + 5)
            Next

            For Each line As Line In lineList
                g.DrawLine(Pens.Green, line.startPt, line.endPt)
                DrawPointWithCoordinates(g, line.startPt, brhGreen, brhGreen, fontNormal)
                DrawPointWithCoordinates(g, line.endPt, brhGreen, brhGreen, fontNormal)
            Next

            For Each pt As Point In pointList
                DrawPointWithCoordinates(g, pt, brhBlue, brhBlack, fontNormal)
            Next

            ' Draw the found elements
            If mode = 1 Then
                DrawPointWithCoordinates(g, ptMin, brhRed, brhGreen, fontNormal, "Min")
                DrawPointWithCoordinates(g, ptMax, brhRed, brhGreen, fontNormal, "Max")
                g.DrawLine(Pens.Red, ptTangents(0), pointList.Item(0))
                DrawPointWithCoordinates(g, ptTangents(0), brhRed, brhGreen, fontNormal, "Tangent 1")
                g.DrawLine(Pens.Red, ptTangents(1), pointList.Item(0))
                DrawPointWithCoordinates(g, ptTangents(1), brhRed, brhGreen, fontNormal, "Tangent 2")
            End If
        End Using
    End Sub

    Private Sub DrawPointWithCoordinates(ByRef g As Graphics, pt As PointF, ByRef brhPoint As Brush, ByRef brhString As Brush, ByRef font As Font, Optional suffix As String = "")
        g.FillEllipse(brhPoint, CInt(pt.X - 2), CInt(pt.Y - 2), 4, 4)
        g.DrawString(suffix + "(" + pt.X.ToString + ", " + pt.Y.ToString + ")", font, brhString, pt.X + 5, pt.Y + 5)
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        circleList.Clear()
        lineList.Clear()
        pointList.Clear()
        mode = -1
        txtResult.Text = ""
        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Private Sub btnGetDistanceBetweenCircleAndPoint_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenCircleAndPoint.Click
        If circleList.Count = 0 Or pointList.Count = 0 Then
            MessageBox.Show("One circle and one point are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        mode = 1
        FindPointsOnCircle(circleList.Item(0), pointList.Item(0), ptMax, ptMin)
        ptTangents = FindTangentPoints(circleList.Item(0), pointList.Item(0))

        Dim tangentDistance = Math.Sqrt(CalculateDistance(circleList.Item(0).center, pointList.Item(0)) ^ 2 - circleList.Item(0).radius ^ 2)

        txtResult.Text = "Min distance between circle and point:" + CalculateDistance(pointList.Item(0), ptMin).ToString + vbCrLf +
            "Max distance between circle and point:" + CalculateDistance(pointList.Item(0), ptMax).ToString + vbCrLf +
            "Tangent distance between circle and point:" + tangentDistance.ToString
        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Sub FindPointsOnCircle(ByVal circle As Circle, ByVal point As Point, ByRef maxDistancePoint As Point, ByRef minDistancePoint As Point)
        ' Calculate the angle between the center of the circle and the point
        Dim angle As Double = Math.Atan2(point.Y - circle.center.Y, point.X - circle.center.X)

        ' Calculate the points on the circle with max and min distances
        minDistancePoint.X = circle.center.X + circle.radius * Math.Cos(angle)
        minDistancePoint.Y = circle.center.Y + circle.radius * Math.Sin(angle)

        maxDistancePoint.X = circle.center.X - circle.radius * Math.Cos(angle)
        maxDistancePoint.Y = circle.center.Y - circle.radius * Math.Sin(angle)
    End Sub

    Function FindTangentPoints(ByVal circle As Circle, ByVal point As PointF) As PointF()
        Dim tangentPoints(1) As PointF

        ' Calculate distance and direction from the center of the circle to the point
        Dim distance As Double = Math.Sqrt((point.X - circle.center.X) ^ 2 + (point.Y - circle.center.Y) ^ 2)
        Dim directionX As Double = (point.X - circle.center.X) / distance
        Dim directionY As Double = (point.Y - circle.center.Y) / distance

        ' Calculate the angle between the radius and the tangent line
        Dim angle As Double = Math.Asin(circle.radius / distance)

        ' Calculate the slopes and the intercepts of the tangent lines
        Dim slope1 As Double = Math.Tan(Math.Atan2(directionY, directionX) + angle)
        Dim slope2 As Double = Math.Tan(Math.Atan2(directionY, directionX) - angle)
        Dim intercept1 As Double = point.Y - slope1 * point.X
        Dim intercept2 As Double = point.Y - slope2 * point.X

        ' Get the tangent points
        tangentPoints(0).X = (circle.center.X + slope1 * circle.center.Y - slope1 * intercept1) / (slope1 ^ 2 + 1)
        tangentPoints(0).Y = slope1 * tangentPoints(0).X + intercept1

        tangentPoints(1).X = (circle.center.X + slope2 * circle.center.Y - slope2 * intercept2) / (slope2 ^ 2 + 1)
        tangentPoints(1).Y = slope2 * tangentPoints(1).X + intercept2
        Return tangentPoints
    End Function

    Function CalculateDistance(ByVal pt1 As PointF, ByVal pt2 As PointF) As Single
        Return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2))
    End Function

    Structure Line
        Public startPt As PointF
        Public endPt As PointF
        Public slope As Double
        Public intercept As Double

        Public Sub New(startPoint As PointF, endPoint As PointF)
            startPt = startPoint
            endPt = endPoint

            slope = (endPoint.Y - startPoint.Y) / (endPoint.X - startPoint.X)
            intercept = startPoint.Y - slope * startPoint.X
        End Sub
    End Structure

    Structure Circle
        Public radius As Double
        Public rect As Rectangle
        Public center As PointF


        Public Sub New(ByRef startPoint As Point, ByRef endPoint As Point)
            radius = Math.Sqrt((endPoint.X - startPoint.X) ^ 2 + (endPoint.Y - startPoint.Y) ^ 2)
            center.X = startPoint.X
            center.Y = startPoint.Y
            rect = New Rectangle(center.X - radius, center.Y - radius, 2 * radius, 2 * radius)
        End Sub
    End Structure

End Class

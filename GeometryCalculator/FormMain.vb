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

    Dim curMode As Mode = Mode.NA

    Dim curPoint As PointF
    Dim ptTangents() As PointF
    Dim curDistance As Single

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
            If curMode = Mode.CircleAndLine Then
                If radioMin.Checked Then
                    DrawPointWithCoordinates(g, curPoint, brhRed, brhGreen, fontNormal, "Min")
                End If
                If radioMax.Checked Then
                    DrawPointWithCoordinates(g, curPoint, brhRed, brhGreen, fontNormal, "Max")
                End If
                If radioTangent.Checked Then
                    g.DrawLine(Pens.Red, ptTangents(0), pointList(0))
                    g.DrawLine(Pens.Blue, ptTangents(0), circleList(0).center)
                    DrawPointWithCoordinates(g, ptTangents(0), brhRed, brhGreen, fontNormal, "Tangent 1")
                    g.DrawLine(Pens.Red, ptTangents(1), pointList(0))
                    g.DrawLine(Pens.Blue, ptTangents(1), circleList(0).center)
                    DrawPointWithCoordinates(g, ptTangents(1), brhRed, brhGreen, fontNormal, "Tangent 2")
                End If
            End If

            If curMode = Mode.PointAndLine Then
                g.DrawLine(Pens.Red, curPoint, pointList(0))
                DrawPointWithCoordinates(g, curPoint, brhBlue, brhBlack, fontNormal)
                If radioPerpendicular.Checked Then
                    If curPoint.X > lineList(0).startPt.X And curPoint.X > lineList(0).endPt.X Then
                        g.DrawLine(Pens.Magenta, curPoint, If(lineList(0).startPt.X > lineList(0).endPt.X, lineList(0).startPt, lineList(0).endPt))
                    ElseIf curPoint.X < lineList(0).startPt.X And curPoint.X < lineList(0).endPt.X Then
                        g.DrawLine(Pens.Magenta, curPoint, If(lineList(0).startPt.X < lineList(0).endPt.X, lineList(0).startPt, lineList(0).endPt))
                    End If
                End If
            End If

            If curMode = Mode.CircleAndCircle Then
                g.DrawLine(Pens.Blue, circleList(0).center, circleList(1).center)
            End If

            If curMode = Mode.PointAndTwoLines Then
                g.DrawLine(Pens.Blue, curPoint, pointList(0))
                DrawPointWithCoordinates(g, curPoint, brhRed, brhBlack, fontNormal)
            End If

        End Using
    End Sub

    Private Sub btnGetDistanceBetweenPointAndIntersection_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenPointAndIntersection.Click
        If Not (pointList.Count > 0 And lineList.Count > 1) Then
            MessageBox.Show("One point and two lines are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = Mode.PointAndTwoLines

        curPoint = GetIntersection(lineList(0), lineList(1))
        curDistance = CalculateDistance(curPoint, pointList(0))
        txtResult.Text = "Distance between a point and intersection of two lines:" + curDistance.ToString
        DrawOnBuffer()
        picView.Invalidate()
    End Sub
    Private Sub btnGetDistanceBetweenTwoCircles_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenTwoCircles.Click
        If circleList.Count < 2 Then
            MessageBox.Show("Two circles are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = Mode.CircleAndCircle
        Dim centerDistance As Single = CalculateDistance(circleList(0).center, circleList(1).center)
        Dim distance As Single = circleList(0).radius + circleList(1).radius

        If radioMax.Checked Then
            curDistance = centerDistance + (circleList(0).radius + circleList(1).radius)
            txtResult.Text = "Max distance between two circle:" + curDistance.ToString
        ElseIf radioMin.Checked Then
            curDistance = Math.Abs(centerDistance - (circleList(0).radius + circleList(1).radius))
            txtResult.Text = "Min distance between two circle:" + curDistance.ToString
        ElseIf radioTangent.Checked Then
            Dim externalDistance As Single = Math.Sqrt((circleList(0).radius - circleList(1).radius) ^ 2 + centerDistance ^ 2)
            curDistance = If(centerDistance < distance, externalDistance,
                    Math.Min(externalDistance, Math.Sqrt(centerDistance ^ 2 - (circleList(0).radius + circleList(1).radius) ^ 2)))
            txtResult.Text = "Tangent distance between two circle:" + curDistance.ToString
        End If

        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Private Sub btnGetDistanceBetweenPointAndLine_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenPointAndLine.Click
        If lineList.Count = 0 Or pointList.Count = 0 Then
            MessageBox.Show("One line and one point are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        curMode = Mode.PointAndLine
        Dim distance1 As Single = CalculateDistance(lineList(0).startPt, pointList(0))
        Dim distance2 As Single = CalculateDistance(lineList(0).endPt, pointList(0))

        If radioMax.Checked Then
            curPoint = If(distance1 > distance2, lineList(0).startPt, lineList(0).endPt)
            txtResult.Text = "Max distance between circle and point:" + If(distance1 > distance2, distance1, distance2).ToString
        End If

        If radioMin.Checked Then
            curPoint = If(distance1 < distance2, lineList(0).startPt, lineList(0).endPt)
            txtResult.Text = "Min distance between circle and point:" + If(distance1 < distance2, distance1, distance2).ToString
        End If

        If radioPerpendicular.Checked Then
            Dim xNearest As Double = (pointList(0).X + lineList(0).slope * pointList(0).Y - lineList(0).slope * lineList(0).intercept) / (lineList(0).slope ^ 2 + 1)
            Dim yNearest As Double = lineList(0).slope * xNearest + lineList(0).intercept
            curPoint = New Point(xNearest, yNearest)
            txtResult.Text = "Perpendicular distance between circle and point:" + CalculateDistance(curPoint, pointList(0)).ToString
        End If
        DrawOnBuffer()
        picView.Invalidate()
    End Sub
    Private Sub btnGetAngleBetweenTwoLines_Click(sender As Object, e As EventArgs) Handles btnGetAngleBetweenTwoLines.Click
        If lineList.Count < 2 Then
            MessageBox.Show("Two lines are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = Mode.LineAndLine
        txtResult.Text = "Angle between two lines: " + CalculateAngleBetweenLines(lineList(0), lineList(1)).ToString + " degree"
    End Sub

    Private Sub btnGetDistanceBetweenCircleAndPoint_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenCircleAndPoint.Click
        If circleList.Count = 0 Or pointList.Count = 0 Then
            MessageBox.Show("One circle and one point are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        curMode = Mode.CircleAndLine
        If radioMin.Checked Or radioMax.Checked Then
            FindPointsOnCircle(circleList(0), pointList(0), curPoint, curPoint)
        End If

        If radioMin.Checked Then
            txtResult.Text = "Min distance between circle and point:" + CalculateDistance(pointList(0), curPoint).ToString
        End If
        If radioMax.Checked Then
            txtResult.Text = "Max distance between circle and point:" + CalculateDistance(pointList(0), curPoint).ToString
        End If
        If radioTangent.Checked Then
            Dim distance As Single = CalculateDistance(circleList(0).center, pointList(0))
            If distance < circleList(0).radius Then
                MessageBox.Show("Tangent can be calculated for the point in circle.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim tangentDistance = Math.Sqrt(distance ^ 2 - circleList(0).radius ^ 2)
            ptTangents = FindTangentPoints(circleList(0), pointList(0))

            txtResult.Text = "Tangent distance between circle and point:" + tangentDistance.ToString
        End If

        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        circleList.Clear()
        lineList.Clear()
        pointList.Clear()
        curMode = Mode.NA
        txtResult.Text = ""
        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Private Sub DrawPointWithCoordinates(ByRef g As Graphics, pt As PointF, ByRef brhPoint As Brush, ByRef brhString As Brush, ByRef font As Font, Optional suffix As String = "")
        g.FillEllipse(brhPoint, CInt(pt.X - 2), CInt(pt.Y - 2), 4, 4)
        g.DrawString(suffix + "(" + pt.X.ToString + ", " + pt.Y.ToString + ")", font, brhString, pt.X + 5, pt.Y + 5)
    End Sub

    Function GetIntersection(line1 As Line, line2 As Line) As PointF
        ' Calculate the x-coordinate of the intersection point
        Dim x As Double = (line2.intercept - line1.intercept) / (line1.slope - line2.slope)

        ' Use the x-coordinate to calculate the y-coordinate
        Dim y As Double = line1.slope * x + line1.intercept

        Return New PointF(x, y)
    End Function

    ' Function to calculate the angle between two lines
    Function CalculateAngleBetweenLines(line1 As Line, line2 As Line) As Single
        ' Calculate slopes
        Dim slope1 As Double = line1.slope
        Dim slope2 As Double = line2.slope

        ' Calculate the angle using arctangent
        Dim angle As Single = Math.Atan(Math.Abs((slope2 - slope1) / (1 + slope1 * slope2)))

        ' Convert the angle from radians to degrees
        angle = angle * (180 / Math.PI)

        Return angle
    End Function

    Sub FindPointsOnCircle(ByVal circle As Circle, ByVal point As Point, ByRef maxDistancePoint As PointF, ByRef minDistancePoint As PointF)
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

    Enum Mode
        NA
        CircleAndLine
        LineAndLine
        PointAndLine
        CircleAndCircle
        PointAndTwoLines
    End Enum

End Class

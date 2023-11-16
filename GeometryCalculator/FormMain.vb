Imports System.Drawing.Text
Imports System.Net.Security

' Make sure to install the Math.NET Numerics library
' using NuGet Package Manager Console with the command:
' Install-Package MathNet.Numerics
Imports MathNet.Numerics.LinearAlgebra

Public Class FormMain
    Dim ptForOutlier As New List(Of PointF)

    Private isDrawing As Boolean = False
    Private startPoint As Point
    Private endPoint As Point
    Private bufferBitmap As Bitmap

    Private circles As New List(Of Circle)
    Private selCircles As New List(Of Circle)
    Private lines As New List(Of Line)
    Private selLines As New List(Of Line)
    Private points As New List(Of PointF)
    Private selPoints As New List(Of PointF)

    Private brhBlack As New SolidBrush(Color.Black)
    Private brhRed As New SolidBrush(Color.Red)
    Private brhGreen As New SolidBrush(Color.Green)
    Private brhBlue As New SolidBrush(Color.Blue)
    Private brhMagenta As New SolidBrush(Color.Magenta)

    Private penBoldMagenta As New Pen(brhMagenta, 3)
    Private penBoldGreen As New Pen(brhGreen, 3)

    Dim fontNormal As New Font("Seoge UI", 8)
    Dim fontBig As New Font("Seoge UI", 14)

    Dim curMode As GeometryType = GeometryType.NA

    Dim curPoint As PointF
    Dim ptTangents() As PointF
    Dim curDistance As Single

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' Create the buffer bitmap
        bufferBitmap = New Bitmap(picView.Width, picView.Height)

        ' Set example points
        ptForOutlier.Add(New PointF With {.X = -50.25, .Y = 68.12})
        ptForOutlier.Add(New PointF With {.X = 115.3, .Y = 42.8})
        ptForOutlier.Add(New PointF With {.X = 80.9, .Y = -50.66})
        ptForOutlier.Add(New PointF With {.X = -60.52, .Y = -50.06})
        ptForOutlier.Add(New PointF With {.X = -36.33, .Y = -70.78})

        ' Update data grid view with points
        ' Set up the DataGridView
        dataGrid.ColumnCount = 3
        dataGrid.Columns(0).HeaderText = "X"
        dataGrid.Columns(1).HeaderText = "Y"
        dataGrid.Columns(2).HeaderText = "Data Analysis"

        For Each pt As PointF In ptForOutlier
            dataGrid.Rows.Add(pt.X.ToString, pt.Y.ToString, "")
        Next

        UpdateUI()
    End Sub
    Private Sub radioFindOutlier_CheckedChanged(sender As Object, e As EventArgs) Handles radioFindOutlier.CheckedChanged
        UpdateUI()
        ClearView()
    End Sub

    Private Sub radioCalculatGeometry_CheckedChanged(sender As Object, e As EventArgs) Handles radioCalculatGeometry.CheckedChanged
        UpdateUI()
        ClearView()
    End Sub
    Private Sub UpdateUI()
        ' UI elements for geometry calculator
        grpElementType.Enabled = radioCalculatGeometry.Checked
        btnClearElements.Enabled = radioCalculatGeometry.Checked

        btnGetDistanceBetweenCircleAndPoint.Enabled = selPoints.Count > 0 And selCircles.Count > 0 And radioCalculatGeometry.Checked
        btnGetAngleBetweenTwoLines.Enabled = selLines.Count > 1 And radioCalculatGeometry.Checked
        btnGetDistanceBetweenPointAndLine.Enabled = selPoints.Count > 0 And selLines.Count > 0 And radioCalculatGeometry.Checked
        btnGetDistanceBetweenTwoCircles.Enabled = selCircles.Count > 1 And radioCalculatGeometry.Checked
        btnGetDistanceBetweenPointAndIntersection.Enabled = selPoints.Count > 0 And selLines.Count > 1 And radioCalculatGeometry.Checked

        radioMax.Enabled = (selPoints.Count > 0 And selCircles.Count > 0) Or (selPoints.Count > 0 And selLines.Count > 0) Or selCircles.Count > 1
        radioMin.Enabled = (selPoints.Count > 0 And selCircles.Count > 0) Or selPoints.Count > 0 And selLines.Count > 0 Or selCircles.Count > 1
        Dim preStatus As Boolean = radioTangent.Enabled
        radioTangent.Enabled = selCircles.Count > 1 Or (selPoints.Count > 0 And selCircles.Count > 0)
        If preStatus = True And radioTangent.Enabled = False Then
            radioMax.Checked = True
        End If

        preStatus = radioPerpendicular.Enabled
        radioPerpendicular.Enabled = selPoints.Count > 0 And selLines.Count > 0
        If preStatus = True And radioPerpendicular.Enabled = False Then
            radioMax.Checked = True
        End If
        grpMeasureType.Enabled = radioCalculatGeometry.Checked

        ' UI elements for outlier finder
        dataGrid.Enabled = radioFindOutlier.Checked
        btnClearPoints.Enabled = radioFindOutlier.Checked
        btnFindBFC.Enabled = radioFindOutlier.Checked
        btnFindTrendLine.Enabled = radioFindOutlier.Checked

    End Sub

    Private Sub ClearView()
        If bufferBitmap Is Nothing Then
            Exit Sub
        End If
        Using g As Graphics = Graphics.FromImage(bufferBitmap)
            g.TextRenderingHint = TextRenderingHint.AntiAlias
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.Clear(Color.White)
        End Using
        picView.Invalidate()
    End Sub

    Private Sub picView_MouseDown(sender As Object, e As MouseEventArgs) Handles picView.MouseDown
        If radioFindOutlier.Checked Then
            Exit Sub
        End If
        ' Start drawing when the mouse is clicked
        isDrawing = True
        startPoint = e.Location
    End Sub
    Private Sub picView_MouseUp(sender As Object, e As MouseEventArgs) Handles picView.MouseUp
        If radioFindOutlier.Checked Then
            Exit Sub
        End If

        endPoint = e.Location
        ' Finish drawing when the mouse is released
        If isDrawing Then
            If radioCircle.Checked And CalculateDistance(startPoint, endPoint) > 5 Then
                Dim circle As Circle = New Circle(startPoint, endPoint)
                circles.Add(circle)
            End If

            If radioLine.Checked And CalculateDistance(startPoint, endPoint) > 5 Then
                Dim line As Line = New Line(startPoint, endPoint)
                lines.Add(line)
            End If

        End If
        isDrawing = False
        DrawOnBuffer()
        picView.Invalidate() ' Force the PictureBox to redraw
    End Sub
    Private Sub picView_MouseMove(sender As Object, e As MouseEventArgs) Handles picView.MouseMove
        If radioFindOutlier.Checked Then
            Exit Sub
        End If

        If isDrawing Then
            endPoint = e.Location
            DrawOnBuffer()
            picView.Invalidate() ' Force the PictureBox to redraw
        End If
    End Sub

    Private Sub picView_MouseClick(sender As Object, e As MouseEventArgs) Handles picView.MouseClick
        If radioFindOutlier.Checked Then
            Exit Sub
        End If

        Dim found As Boolean = False
        Dim curPt As PointF = e.Location

        For Each circle As Circle In circles
            If Math.Abs(CalculateDistance(curPt, circle.center) - circle.radius) <= 2 Then
                found = True
                selCircles.Add(circle)
                circles.Remove(circle)
                GoTo lEnd
            End If
        Next

        For Each circle As Circle In selCircles
            If Math.Abs(CalculateDistance(curPt, circle.center) - circle.radius) <= 2 Then
                found = True
                circles.Add(circle)
                selCircles.Remove(circle)
                GoTo lEnd
            End If
        Next

        For Each line As Line In lines
            If Math.Abs(curPt.Y - (line.slope * curPt.X + line.intercept)) <= 2 Then
                found = True
                selLines.Add(line)
                lines.Remove(line)
                GoTo lEnd
            End If
        Next

        For Each line As Line In selLines
            If Math.Abs(curPt.Y - (line.slope * curPt.X + line.intercept)) <= 2 Then
                found = True
                lines.Add(line)
                selLines.Remove(line)
                GoTo lEnd
            End If
        Next

        For Each pt As PointF In points
            If CalculateDistance(curPt, pt) <= 2 Then
                found = True
                selPoints.Add(pt)
                points.Remove(pt)
                GoTo lEnd
            End If
        Next

        For Each pt As PointF In selPoints
            If CalculateDistance(curPt, pt) <= 2 Then
                found = True
                points.Add(pt)
                selPoints.Remove(pt)
                GoTo lEnd
            End If
        Next
lEnd:
        If Not found And radioPoint.Checked Then
            points.Add(startPoint)
        End If

        UpdateUI()

        DrawOnBuffer()
        picView.Invalidate()
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
            For Each circle As Circle In circles
                g.DrawEllipse(Pens.Magenta, circle.rect)
                g.FillEllipse(brhMagenta, CInt(circle.center.X - 2), CInt(circle.center.Y - 2), 4, 4)
                g.DrawString("(" + circle.center.X.ToString + ", " + circle.center.Y.ToString + ")",
                             fontNormal, brhMagenta, circle.center.X + 5, circle.center.Y + 5)
            Next

            For Each circle As Circle In selCircles
                g.DrawEllipse(penBoldMagenta, circle.rect)
                g.FillEllipse(brhMagenta, CInt(circle.center.X - 2), CInt(circle.center.Y - 2), 4, 4)
                g.DrawString("(" + circle.center.X.ToString + ", " + circle.center.Y.ToString + ")",
                             fontNormal, brhMagenta, circle.center.X + 5, circle.center.Y + 5)
            Next

            For Each line As Line In lines
                g.DrawLine(Pens.Green, line.startPt, line.endPt)
                DrawPointWithCoordinates(g, line.startPt, brhGreen, brhGreen, fontNormal)
                DrawPointWithCoordinates(g, line.endPt, brhGreen, brhGreen, fontNormal)
            Next

            For Each line As Line In selLines
                g.DrawLine(penBoldGreen, line.startPt, line.endPt)
                DrawPointWithCoordinates(g, line.startPt, brhGreen, brhGreen, fontNormal)
                DrawPointWithCoordinates(g, line.endPt, brhGreen, brhGreen, fontNormal)
            Next

            For Each pt As PointF In points
                DrawPointWithCoordinates(g, pt, brhBlue, brhBlack, fontNormal)
            Next

            For Each pt As PointF In selPoints
                DrawPointWithCoordinates(g, pt, brhBlue, brhBlack, fontNormal, 3)
            Next

            ' Draw the found elements
            If curMode = GeometryType.CircleAndPoint And selCircles.Count > 0 And selPoints.Count > 0 Then
                If radioMin.Checked Then
                    DrawPointWithCoordinates(g, curPoint, brhRed, brhGreen, fontNormal, 3, "Min")
                End If
                If radioMax.Checked Then
                    DrawPointWithCoordinates(g, curPoint, brhRed, brhGreen, fontNormal, 3, "Max")
                End If
                If radioTangent.Checked Then
                    g.DrawLine(Pens.Red, ptTangents(0), selPoints(0))
                    g.DrawLine(Pens.Blue, ptTangents(0), selCircles(0).center)
                    DrawPointWithCoordinates(g, ptTangents(0), brhRed, brhGreen, fontNormal, 2, "Tangent 1")
                    g.DrawLine(Pens.Red, ptTangents(1), selPoints(0))
                    g.DrawLine(Pens.Blue, ptTangents(1), selCircles(0).center)
                    DrawPointWithCoordinates(g, ptTangents(1), brhRed, brhGreen, fontNormal, 2, "Tangent 2")
                End If
            End If

            If curMode = GeometryType.PointAndLine And selPoints.Count > 0 And selLines.Count > 0 Then
                g.DrawLine(Pens.Red, curPoint, selPoints(0))
                DrawPointWithCoordinates(g, curPoint, brhBlue, brhBlack, fontNormal)
                If radioPerpendicular.Checked Then
                    If curPoint.X > selLines(0).startPt.X And curPoint.X > selLines(0).endPt.X Then
                        g.DrawLine(Pens.Magenta, curPoint, If(selLines(0).startPt.X > selLines(0).endPt.X, selLines(0).startPt, selLines(0).endPt))
                    ElseIf curPoint.X < selLines(0).startPt.X And curPoint.X < selLines(0).endPt.X Then
                        g.DrawLine(Pens.Magenta, curPoint, If(selLines(0).startPt.X < selLines(0).endPt.X, selLines(0).startPt, selLines(0).endPt))
                    End If
                End If
            End If

            If curMode = GeometryType.CircleAndCircle And selCircles.Count > 1 Then
                g.DrawLine(Pens.Blue, selCircles(0).center, selCircles(1).center)
            End If

            If curMode = GeometryType.PointAndTwoLines And selLines.Count > 1 Then
                g.DrawLine(Pens.Blue, curPoint, selPoints(0))
                DrawPointWithCoordinates(g, curPoint, brhRed, brhBlack, fontNormal)
            End If

        End Using
    End Sub

    Private Sub btnGetDistanceBetweenPointAndIntersection_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenPointAndIntersection.Click
        If Not (selPoints.Count > 0 And selLines.Count > 1) Then
            MessageBox.Show("One point and two lines are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = GeometryType.PointAndTwoLines

        curPoint = GetIntersection(selLines(0), selLines(1))
        curDistance = CalculateDistance(curPoint, selPoints(0))
        txtResult.Text = "Distance between a point and intersection of two lines:" + curDistance.ToString
        DrawOnBuffer()
        picView.Invalidate()
    End Sub
    Private Sub btnGetDistanceBetweenTwoCircles_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenTwoCircles.Click
        If selCircles.Count < 2 Then
            MessageBox.Show("Two circles are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = GeometryType.CircleAndCircle
        Dim centerDistance As Single = CalculateDistance(selCircles(0).center, selCircles(1).center)
        Dim distance As Single = selCircles(0).radius + selCircles(1).radius

        If radioMax.Checked Then
            curDistance = centerDistance + (selCircles(0).radius + selCircles(1).radius)
            txtResult.Text = "Max distance between two circle:" + curDistance.ToString
        ElseIf radioMin.Checked Then
            curDistance = Math.Abs(centerDistance - (selCircles(0).radius + selCircles(1).radius))
            txtResult.Text = "Min distance between two circle:" + curDistance.ToString
        ElseIf radioTangent.Checked Then
            Dim externalDistance As Single = Math.Sqrt((selCircles(0).radius - selCircles(1).radius) ^ 2 + centerDistance ^ 2)
            curDistance = If(centerDistance < distance, externalDistance,
                    Math.Min(externalDistance, Math.Sqrt(centerDistance ^ 2 - (selCircles(0).radius + selCircles(1).radius) ^ 2)))
            txtResult.Text = "Tangent distance between two circle:" + curDistance.ToString
        End If

        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Private Sub btnGetDistanceBetweenPointAndLine_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenPointAndLine.Click
        If selLines.Count = 0 Or selPoints.Count = 0 Then
            MessageBox.Show("One line and one point are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        curMode = GeometryType.PointAndLine
        Dim distance1 As Single = CalculateDistance(selLines(0).startPt, selPoints(0))
        Dim distance2 As Single = CalculateDistance(selLines(0).endPt, selPoints(0))

        If radioMax.Checked Then
            curPoint = If(distance1 > distance2, selLines(0).startPt, selLines(0).endPt)
            txtResult.Text = "Max distance between circle and point:" + If(distance1 > distance2, distance1, distance2).ToString
        End If

        If radioMin.Checked Then
            curPoint = If(distance1 < distance2, selLines(0).startPt, selLines(0).endPt)
            txtResult.Text = "Min distance between circle and point:" + If(distance1 < distance2, distance1, distance2).ToString
        End If

        If radioPerpendicular.Checked Then
            Dim xNearest As Double = (selPoints(0).X + selLines(0).slope * selPoints(0).Y - selLines(0).slope * selLines(0).intercept) / (selLines(0).slope ^ 2 + 1)
            Dim yNearest As Double = selLines(0).slope * xNearest + selLines(0).intercept
            curPoint = New Point(xNearest, yNearest)
            txtResult.Text = "Perpendicular distance between circle and point:" + CalculateDistance(curPoint, selPoints(0)).ToString
        End If
        DrawOnBuffer()
        picView.Invalidate()
    End Sub
    Private Sub btnGetAngleBetweenTwoLines_Click(sender As Object, e As EventArgs) Handles btnGetAngleBetweenTwoLines.Click
        If selLines.Count < 2 Then
            MessageBox.Show("Two selLines are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = GeometryType.LineAndLine
        txtResult.Text = "Angle between two selLines: " + CalculateAngleBetweenLines(selLines(0), selLines(1)).ToString + " degree"
    End Sub

    Private Sub btnGetDistanceBetweenCircleAndPoint_Click(sender As Object, e As EventArgs) Handles btnGetDistanceBetweenCircleAndPoint.Click
        If selCircles.Count = 0 Or selPoints.Count = 0 Then
            MessageBox.Show("One circle and one point are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        curMode = GeometryType.CircleAndPoint
        Dim maxPoint As PointF, minPoint As PointF
        If radioMin.Checked Or radioMax.Checked Then
            FindPointsOnCircle(selCircles(0), selPoints(0), maxPoint, minPoint)
        End If

        If radioMin.Checked Then
            curPoint = minPoint
            txtResult.Text = "Min distance between circle and point:" + CalculateDistance(selPoints(0), curPoint).ToString
        End If
        If radioMax.Checked Then
            curPoint = maxPoint
            txtResult.Text = "Max distance between circle and point:" + CalculateDistance(selPoints(0), curPoint).ToString
        End If
        If radioTangent.Checked Then
            Dim distance As Single = CalculateDistance(selCircles(0).center, selPoints(0))
            If distance < selCircles(0).radius Then
                MessageBox.Show("Tangent can be calculated for the point in circle.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim tangentDistance = Math.Sqrt(distance ^ 2 - selCircles(0).radius ^ 2)
            ptTangents = FindTangentPoints(selCircles(0), selPoints(0))

            txtResult.Text = "Tangent distance between circle and point:" + tangentDistance.ToString
        End If

        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Private Sub btnClearElements_Click(sender As Object, e As EventArgs) Handles btnClearElements.Click
        circles.Clear()
        selCircles.Clear()
        lines.Clear()
        selLines.Clear()
        points.Clear()
        selPoints.Clear()
        curMode = GeometryType.NA
        txtResult.Text = ""
        DrawOnBuffer()
        picView.Invalidate()
    End Sub

    Private Sub DrawPointWithCoordinates(ByRef g As Graphics, pt As PointF, ByRef brhPoint As Brush, ByRef brhString As Brush, ByRef font As Font, Optional sz As Integer = 2, Optional suffix As String = "")
        g.FillEllipse(brhPoint, CInt(pt.X - sz), CInt(pt.Y - sz), sz * 2, sz * 2)
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

    Sub FindPointsOnCircle(ByVal circle As Circle, ByVal point As PointF, ByRef maxDistancePoint As PointF, ByRef minDistancePoint As PointF)
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


    Private Sub btnClearPoints_Click(sender As Object, e As EventArgs) Handles btnClearPoints.Click
        dataGrid.Rows.Clear()
        ClearView()
    End Sub

    '----------------------------------------------------------------------------'
    ' Find and draw best fit circle
    '----------------------------------------------------------------------------'
    Private Sub btnFindBFC_Click(sender As Object, e As EventArgs) Handles btnFindBFC.Click
        FindAndDrawCircle()
        picView.Invalidate()
    End Sub

    Private Sub FindAndDrawCircle()
        UpdatePointsFromDataGrid()

        If ptForOutlier.Count < 3 Then
            MessageBox.Show("At least 3 points are needed for best fit circle.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Use the Least Squares method to find the best-fit circle
        Dim circle As Circle = CircleFit.FitCircle(ptForOutlier)

        ' Calculate the minimum distance of each point to circle
        For Each row As DataGridViewRow In dataGrid.Rows
            If row.Index >= dataGrid.Rows.Count - 1 Then
                Exit For
            End If

            Dim pt As PointF = ptForOutlier(row.Index)
            row.Cells(2).Value = CSng(Math.Abs(Math.Sqrt((pt.X - circle.center.X) * (pt.X - circle.center.X) +
                                           (pt.Y - circle.center.Y) * (pt.Y - circle.center.Y)) - circle.radius))
        Next

        ' Draw the best fit circle
        DrawCircle(circle)
    End Sub

    ' Display the result for Best Fit Circle in the PictureBox
    Private Sub DrawCircle(circle As Circle)
        ' Clear the PictureBox
        Dim g As Graphics = Graphics.FromImage(bufferBitmap)
        g.TextRenderingHint = TextRenderingHint.AntiAlias
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        g.Clear(Color.White)

        Dim dx As Single = picView.Width / 2
        Dim dy As Single = picView.Height / 2

        ' Draw the origin point and X-Y coordinate lines
        g.DrawEllipse(Pens.Green, dx - 2, dy - 2, 4, 4)
        g.DrawLine(Pens.Black, 0, dy, picView.Width, dy)
        g.DrawLine(Pens.Black, dx, 0, dx, picView.Height)

        ' Draw the points
        Dim fontNormal As New Font("Seoge UI", 8)
        Dim fontBig As New Font("Seoge UI", 14)
        Dim brhBlue As New SolidBrush(Color.Blue)
        Dim brhBlack As New SolidBrush(Color.Black)

        For Each pt As PointF In ptForOutlier
            ' Draw the point
            g.FillEllipse(brhBlue, dx + pt.X - 2, dy - pt.Y - 2, 4, 4)

            ' Draw a X-Y coordinates for the point.
            g.DrawString("(" + pt.X.ToString + ", " + pt.Y.ToString + ")", fontNormal, brhBlack, dx + pt.X + 5, dy - pt.Y - 5)
        Next

        Dim centerX As Single = Math.Floor(circle.center.X * 1000) / 1000
        Dim centerY As Single = Math.Floor(circle.center.Y * 1000) / 1000
        Dim radius As Single = Math.Floor(circle.radius * 1000) / 1000

        ' Draw the best-fit circle
        ' Draw center of the circle
        Dim brhRed As New SolidBrush(Color.Red)
        g.FillEllipse(brhRed, dx + centerX - 2, dy - centerY - 2, 4, 4)
        g.DrawString("(" + centerX.ToString + ", " + centerY.ToString + ")", fontNormal,
                        brhBlack, dx + centerX + 5, dy - centerY - 5)
        ' Draw radius of the circle
        g.DrawString("Radius:" + radius.ToString, fontBig, brhBlack, 10, 10)
        ' Draw the circle
        g.DrawEllipse(Pens.Red, dx + centerX - radius, dy - centerY - radius, 2 * radius, 2 * radius)

    End Sub

    '----------------------------------------------------------------------------'
    ' Find and draw trend line
    '----------------------------------------------------------------------------'
    Private Sub btnFindTrendLine_Click(sender As Object, e As EventArgs) Handles btnFindTrendLine.Click
        FindTrendLine()
        picView.Invalidate()
    End Sub

    Private Sub FindTrendLine()
        UpdatePointsFromDataGrid()
        If ptForOutlier.Count < 2 Then
            MessageBox.Show("At least 2 points are needed for trend line.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Use the Least Squares method to find the best-fit circle
        Dim lr As Line = TrendLineFinder.Calculate(ptForOutlier)

        ' Calculate the minimum distance of each point to circle
        For Each row As DataGridViewRow In dataGrid.Rows
            If row.Index >= dataGrid.Rows.Count - 1 Then
                Exit For
            End If

            Dim pt As PointF = ptForOutlier(row.Index)
            row.Cells(2).Value = CSng(Math.Abs(lr.slope * pt.X - pt.Y + lr.intercept) / Math.Sqrt(lr.slope ^ 2 + 1))
        Next

        ' Draw the best fit circle
        DrawTrendLine(lr)
    End Sub

    ' Display the result for Best Fit Circle in the PictureBox
    Private Sub DrawTrendLine(lr As Line)
        ' Clear the PictureBox
        Dim g As Graphics = Graphics.FromImage(bufferBitmap)
        g.TextRenderingHint = TextRenderingHint.AntiAlias
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        g.Clear(Color.White)

        Dim dx As Single = picView.Width / 2
        Dim dy As Single = picView.Height / 2

        ' Draw the origin point and X-Y coordinate lines
        g.DrawEllipse(Pens.Green, dx - 2, dy - 2, 4, 4)
        g.DrawLine(Pens.Black, 0, dy, picView.Width, dy)
        g.DrawLine(Pens.Black, dx, 0, dx, picView.Height)

        ' Draw the points
        Dim fontNormal As New Font("Seoge UI", 8)
        Dim fontBig As New Font("Seoge UI", 14)
        Dim brhBlue As New SolidBrush(Color.Blue)
        Dim brhRed As New SolidBrush(Color.Red)
        Dim brhBlack As New SolidBrush(Color.Black)

        For Each pt As PointF In ptForOutlier
            ' Draw the point
            g.FillEllipse(brhBlue, dx + pt.X - 3, dy - pt.Y - 3, 6, 6)

            ' Draw a X-Y coordinates for the point.
            g.DrawString("(" + pt.X.ToString + ", " + pt.Y.ToString + ")", fontNormal, brhBlack, dx + pt.X + 5, dy - pt.Y - 5)
        Next

        ' Draw the regression equation
        g.DrawString("y = " + CSng(lr.slope).ToString + "x " + If(lr.intercept > 0, "+ ", "") + CSng(lr.intercept).ToString, fontBig, brhBlack, 10, 10)

        ' Draw the trend line
        Dim xStart As Single = -picView.Width / 2
        Dim yStart As Single = CSng(lr.slope * xStart + lr.intercept)
        Dim xEnd As Single = picView.Width / 2
        Dim yEnd As Single = CSng(lr.slope * xEnd + lr.intercept)
        g.DrawLine(New Pen(Color.Green, 2), dx + xStart, dy - yStart, dx + xEnd, dy - yEnd)

        ' Draw the line from each point to its nearest point on trend line
        For Each pt As PointF In ptForOutlier
            Dim xNearest As Double = (pt.X + lr.slope * pt.Y - lr.slope * lr.intercept) / (lr.slope ^ 2 + 1)
            Dim yNearest As Double = lr.slope * xNearest + lr.intercept

            ' Draw the nearest point on the trend line
            g.FillEllipse(brhRed, CSng(dx + xNearest - 3), CSng(dy - yNearest - 3), 6, 6)

            ' Draw the line between two points
            g.DrawLine(Pens.Red, CSng(dx + pt.X), CSng(dy - pt.Y), CSng(dx + xNearest), CSng(dy - yNearest))
        Next

    End Sub

    Private Sub UpdatePointsFromDataGrid()
        ptForOutlier.Clear()
        ' Add new points from DataGridView
        For Each row As DataGridViewRow In dataGrid.Rows
            Dim x As Single
            Dim y As Single

            If row.Index >= dataGrid.Rows.Count - 1 Then
                Exit For
            End If

            Try
                ' Try to parse the values from the DataGridView cells
                If Single.TryParse(row.Cells(0).Value.ToString(), x) AndAlso Single.TryParse(row.Cells(1).Value.ToString(), y) Then
                    ptForOutlier.Add(New PointF With {.X = x, .Y = y})
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub

    Public Class CircleFit

        ' Use the Least Squares method to find the best-fit circle
        Public Shared Function FitCircle(points As List(Of PointF)) As Circle
            If points Is Nothing OrElse points.Count < 2 Then
                Throw New ArgumentException("At least two points are required for best fit circle.")
            End If

            Dim n As Integer = points.Count

            ' Construct the matrix A and vector B for the linear system Ax = B
            Dim A As Matrix(Of Double) = Matrix(Of Double).Build.Dense(n, 3)
            Dim B As Vector(Of Double) = Vector(Of Double).Build.Dense(n)

            For i As Integer = 0 To n - 1
                A(i, 0) = 2 * points(i).X
                A(i, 1) = 2 * points(i).Y
                A(i, 2) = -1
                B(i) = points(i).X ^ 2 + points(i).Y ^ 2
            Next

            ' Solve the linear system using Math.NET Numerics
            Dim x As Vector(Of Double) = A.Svd().Solve(B)

            ' Calculate circle parameters
            Dim centerX As Double = x(0)
            Dim centerY As Double = x(1)
            Dim radius As Double = Math.Sqrt(centerX ^ 2 + centerY ^ 2 - x(2))
            Dim pt As PointF
            pt.X = x(0)
            pt.Y = x(1)

            Return New Circle With {.center = pt, .radius = CSng(radius)}
        End Function

    End Class

    Public Class TrendLineFinder

        Public Shared Function Calculate(ByVal points As List(Of PointF)) As Line
            If points Is Nothing OrElse points.Count < 2 Then
                Throw New ArgumentException("At least two points are required for linear regression.")
            End If

            Dim n As Integer = points.Count
            Dim sumX As Double = 0
            Dim sumY As Double = 0
            Dim sumXY As Double = 0
            Dim sumX2 As Double = 0

            For Each point In points
                sumX += point.X
                sumY += point.Y
                sumXY += point.X * point.Y
                sumX2 += point.X * point.X
            Next

            Dim Slope As Double = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX)
            Dim Intercept As Double = (sumY - Slope * sumX) / n

            Return New Line With {.slope = Slope, .intercept = Intercept}
        End Function
    End Class

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

    Enum GeometryType
        NA
        CircleAndPoint
        LineAndLine
        PointAndLine
        CircleAndCircle
        PointAndTwoLines
    End Enum
End Class

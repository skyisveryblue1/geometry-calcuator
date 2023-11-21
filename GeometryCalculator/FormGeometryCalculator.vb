Imports System.Drawing.Text

Public Class FormGeometryCalculator
    Private brshBlack As New SolidBrush(Color.Black)
    Private brshRed As New SolidBrush(Color.Red)
    Private brshGreen As New SolidBrush(Color.Green)
    Private brshBlue As New SolidBrush(Color.Blue)
    Private brshMagenta As New SolidBrush(Color.Magenta)

    Private penBoldMagenta As New Pen(brshMagenta, 3)
    Private penBoldGreen As New Pen(brshGreen, 3)

    Dim fontNormal As New Font("Seoge UI", 8)
    Dim fontBig As New Font("Seoge UI", 14)

    Dim outlierPoints As New List(Of PointF)

    Private bufferBitmap As Bitmap
    Private zoomFactor As Single = 1.0

    Private circles As New List(Of Circle)
    Private selCircles As New List(Of Circle)
    Private lines As New List(Of Line)
    Private selLines As New List(Of Line)
    Private points As New List(Of PointF)
    Private selPoints As New List(Of PointF)

    Private foundCircle As Circle
    Private foundLine As Line

    Dim curMode As GeometryType = GeometryType.NA

    Dim curPoint As PointF
    Dim ptTangents() As PointF
    Dim curDistance As Single

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        picView.Width = 1000
        picView.Height = 750

        txtResult.Top = picView.Bottom + 10
        txtResult.Width = picView.Width
        panelGeometryCalculator.Left = picView.Right + 10
        panelOutlierFinder.Left = panelGeometryCalculator.Right + 10
        Me.ClientSize = New Size(panelOutlierFinder.Right + 10, txtResult.Bottom + 10)

        ' Add any initialization after the InitializeComponent() call.
        ' Create the buffer bitmap
        bufferBitmap = New Bitmap(picView.Width, picView.Height)

        ' Set example points
        outlierPoints.Add(New PointF With {.X = 350.25, .Y = 268.12})
        outlierPoints.Add(New PointF With {.X = 415.3, .Y = 142.8})
        outlierPoints.Add(New PointF With {.X = 680.9, .Y = 350.66})
        outlierPoints.Add(New PointF With {.X = 540.52, .Y = 150.06})
        outlierPoints.Add(New PointF With {.X = 276.33, .Y = 230.78})

        'outlierPoints.Add(New PointF With {.X = 200, .Y = 150.06})
        'outlierPoints.Add(New PointF With {.X = 200, .Y = 230.78})

        ' Update data grid view with points
        ' Set up the DataGridView
        dataGrid.ColumnCount = 3
        dataGrid.Columns(0).HeaderText = "X"
        dataGrid.Columns(1).HeaderText = "Y"
        dataGrid.Columns(2).HeaderText = "Data Analysis"

        For Each pt As PointF In outlierPoints
            dataGrid.Rows.Add(pt.X.ToString, pt.Y.ToString, "")
            points.Add(pt)
        Next

        UpdatePointsFromDataGrid()
        DrawAll()

        UpdateUI()
    End Sub

    Private Sub UpdateUI()
        ' UI elements for geometry calculator

        btnGetDistanceBetweenCircleAndPoint.Enabled = selPoints.Count > 0 And selCircles.Count > 0
        btnGetAngleBetweenTwoLines.Enabled = selLines.Count > 1
        btnGetDistanceBetweenPointAndLine.Enabled = selPoints.Count > 0 And selLines.Count > 0
        btnGetDistanceBetweenTwoCircles.Enabled = selCircles.Count > 1
        btnGetDistanceBetweenPointAndIntersection.Enabled = selPoints.Count > 0 And selLines.Count > 1

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

    Private Sub picView_MouseClick(sender As Object, e As MouseEventArgs) Handles picView.MouseClick

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
            If line.Contains(curPt) Then
                found = True
                selLines.Add(line)
                lines.Remove(line)
                GoTo lEnd
            End If
        Next

        For Each line As Line In selLines
            If line.Contains(curPt) Then
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

        UpdateUI()

        DrawAll()
    End Sub
    Private Sub picView_Paint(sender As Object, e As PaintEventArgs) Handles picView.Paint
        ' Display the buffer on the PictureBox
        e.Graphics.DrawImage(bufferBitmap, 0, 0, picView.Width * zoomFactor, picView.Height * zoomFactor)
    End Sub
    Private Sub DrawOnBuffer(ByRef g As Graphics)

        ' Draw the inactive elements
        For Each circle As Circle In circles
            g.DrawEllipse(Pens.Magenta, circle.rc)
            g.FillEllipse(brshMagenta, CInt(circle.center.X - 2), CInt(circle.center.Y - 2), 4, 4)
            g.DrawString("(" + circle.center.X.ToString + ", " + circle.center.Y.ToString + ")",
                            fontNormal, brshMagenta, circle.center.X + 5, circle.center.Y + 5)
        Next

        For Each circle As Circle In selCircles
            g.DrawEllipse(penBoldMagenta, circle.rc)
            g.FillEllipse(brshMagenta, CInt(circle.center.X - 2), CInt(circle.center.Y - 2), 4, 4)
            g.DrawString("(" + circle.center.X.ToString + ", " + circle.center.Y.ToString + ")",
                            fontNormal, brshMagenta, circle.center.X + 5, circle.center.Y + 5)
        Next

        For Each line As Line In lines
            g.DrawLine(Pens.Green, line.startPt, line.endPt)
            DrawPoint(g, line.startPt, brshGreen, brshGreen, fontNormal)
            DrawPoint(g, line.endPt, brshGreen, brshGreen, fontNormal)
        Next

        For Each line As Line In selLines
            g.DrawLine(penBoldGreen, line.startPt, line.endPt)
            DrawPoint(g, line.startPt, brshGreen, brshGreen, fontNormal)
            DrawPoint(g, line.endPt, brshGreen, brshGreen, fontNormal)
        Next

        For Each pt As PointF In points
            DrawPoint(g, pt, brshBlue, brshBlack, fontNormal)
        Next

        For Each pt As PointF In selPoints
            DrawPoint(g, pt, brshBlue, brshBlack, fontNormal, 3)
        Next

        ' Draw the found elements
        If curMode = GeometryType.CircleAndPoint And selCircles.Count > 0 And selPoints.Count > 0 Then
            If radioMin.Checked Then
                DrawPoint(g, curPoint, brshRed, brshGreen, fontNormal, 3, "Min")
            End If
            If radioMax.Checked Then
                DrawPoint(g, curPoint, brshRed, brshGreen, fontNormal, 3, "Max")
            End If
            If radioTangent.Checked Then
                g.DrawLine(Pens.Red, ptTangents(0), selPoints(0))
                g.DrawLine(Pens.Blue, ptTangents(0), selCircles(0).center)
                DrawPoint(g, ptTangents(0), brshRed, brshGreen, fontNormal, 2, "Tangent 1")
                g.DrawLine(Pens.Red, ptTangents(1), selPoints(0))
                g.DrawLine(Pens.Blue, ptTangents(1), selCircles(0).center)
                DrawPoint(g, ptTangents(1), brshRed, brshGreen, fontNormal, 2, "Tangent 2")
            End If
        End If

        If curMode = GeometryType.PointAndLine And selPoints.Count > 0 And selLines.Count > 0 Then
            g.DrawLine(Pens.Red, curPoint, selPoints(0))
            DrawPoint(g, curPoint, brshBlue, brshBlack, fontNormal)
            If radioPerpendicular.Checked Then
                If curPoint.X >= selLines(0).startPt.X And curPoint.X >= selLines(0).endPt.X Then
                    g.DrawLine(Pens.Magenta, curPoint, If(selLines(0).startPt.X > selLines(0).endPt.X, selLines(0).startPt, selLines(0).endPt))
                ElseIf curPoint.X <= selLines(0).startPt.X And curPoint.X <= selLines(0).endPt.X Then
                    g.DrawLine(Pens.Magenta, curPoint, If(selLines(0).startPt.X < selLines(0).endPt.X, selLines(0).startPt, selLines(0).endPt))
                End If
            End If
        End If

        If curMode = GeometryType.CircleAndCircle And selCircles.Count > 1 Then
            g.DrawLine(Pens.Blue, selCircles(0).center, selCircles(1).center)
        End If

        If curMode = GeometryType.PointAndTwoLines And selLines.Count > 1 Then
            g.DrawLine(Pens.Blue, curPoint, selPoints(0))
            DrawPoint(g, curPoint, brshRed, brshBlack, fontNormal)
        End If
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
        DrawAll()
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

        DrawAll()
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
            Dim xNearest As Double = If(selLines(0).IsVertical(), selLines(0).startPt.X,
                (selPoints(0).X + selLines(0).slope * selPoints(0).Y - selLines(0).slope * selLines(0).intercept) / (selLines(0).slope ^ 2 + 1))
            Dim yNearest As Double = If(selLines(0).IsVertical(), selPoints(0).Y, selLines(0).slope * xNearest + selLines(0).intercept)
            curPoint = New Point(xNearest, yNearest)
            txtResult.Text = "Perpendicular distance between circle and point:" + CalculateDistance(curPoint, selPoints(0)).ToString
        End If
        DrawAll()
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

        DrawAll()
    End Sub

    Private Sub btnClearAll_Click(sender As Object, e As EventArgs) Handles btnClearAll.Click
        circles.Clear()
        selCircles.Clear()
        lines.Clear()
        selLines.Clear()
        points.Clear()
        selPoints.Clear()
        curMode = GeometryType.NA
        txtResult.Text = ""
        ClearWorkingData()
        DrawAll()
    End Sub

    Private Sub btnClearPoints_Click(sender As Object, e As EventArgs) Handles btnClearPoints.Click
        ClearWorkingData()
        DrawAll()
    End Sub

    Private Sub DrawAll(Optional redraw As Boolean = True)
        ' Clear the PictureBox
        Dim g As Graphics = Graphics.FromImage(bufferBitmap)
        g.TextRenderingHint = TextRenderingHint.AntiAlias
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        g.Clear(Color.White)

        DrawPointsForOutlier(g)
        DrawBestFitCircle(g, foundCircle)
        DrawTrendLine(g, foundLine)
        DrawOnBuffer(g)

        If redraw Then picView.Invalidate()
    End Sub

    Private Sub DrawPointsForOutlier(ByRef g As Graphics)
        For Each pt As PointF In outlierPoints
            DrawPoint(g, pt, brshBlue, brshBlack, fontNormal, 2)
        Next
    End Sub

    ' Display the result for trend line in the PictureBox
    Private Sub DrawTrendLine(ByRef g As Graphics, ByRef ln As Line)
        If outlierPoints.Count < 2 Or ln Is Nothing Then Exit Sub

        ' Draw the regression equation
        g.DrawString(If(ln.IsVertical(), "x = " + CSng(ln.startPt.X).ToString,
                     "y = " + CSng(ln.slope).ToString + "x " + If(ln.intercept > 0, "+ ", "") + CSng(ln.intercept).ToString), fontBig, brshBlack, 10, 10)

        ' Draw the trend line
        g.DrawLine(New Pen(brshGreen, 2), ln.startPt, ln.endPt)

        ' Draw the line from each point to its nearest point on trend line
        For Each pt As PointF In outlierPoints
            ' Draw the point
            DrawPoint(g, pt, brshBlue, brshBlack, fontNormal, 3)

            If ln.IsVertical() Then
                g.FillEllipse(brshRed, pt.X - 3, pt.Y - 3, 6, 6)
                Continue For
            End If

            ' Draw the nearest point on the trend line
            Dim xNearest As Double = (pt.X + ln.slope * pt.Y - ln.slope * ln.intercept) / (ln.slope ^ 2 + 1)
            Dim yNearest As Double = ln.slope * xNearest + ln.intercept

            g.FillEllipse(brshRed, CSng(xNearest - 3), CSng(yNearest - 3), 6, 6)

            ' Draw the line between two points
            g.DrawLine(Pens.Red, CSng(pt.X), CSng(pt.Y), CSng(xNearest), CSng(yNearest))
        Next

    End Sub

    ' Display the result for Best Fit Circle in the PictureBox
    Private Sub DrawBestFitCircle(ByRef g As Graphics, ByRef circle As Circle)
        If outlierPoints.Count < 3 Or circle Is Nothing Then Exit Sub

        Dim centerX As Single = Math.Floor(circle.center.X * 1000) / 1000
        Dim centerY As Single = Math.Floor(circle.center.Y * 1000) / 1000
        Dim radius As Single = Math.Floor(circle.radius * 1000) / 1000

        For Each pt As PointF In outlierPoints
            DrawPoint(g, pt, brshBlue, brshBlack, fontNormal, 3)
        Next

        ' Draw the best-fit circle
        ' Draw center of the circle
        g.FillEllipse(brshRed, centerX - 2, centerY - 2, 4, 4)
            g.DrawString("(" + centerX.ToString + ", " + centerY.ToString + ")", fontNormal,
                        brshBlack, centerX + 5, centerY - 5)
        ' Draw radius of the circle
        g.DrawString("Radius:" + radius.ToString, fontBig, brshBlack, 10, 10)
        ' Draw the circle
        g.DrawEllipse(New Pen(Color.Red, 2), centerX - radius, centerY - radius, 2 * radius, 2 * radius)

    End Sub

    Private Sub btnSavePoint_Click(sender As Object, e As EventArgs) Handles btnSavePoint.Click
        Try
            Dim x As Single, y As Single
            If Single.TryParse(txtCurrentX.Text, x) AndAlso Single.TryParse(txtCurrentY.Text, y) Then
                Dim newPt As PointF = New PointF(x, y)
                For Each pt As PointF In outlierPoints
                    If CalculateDistance(pt, newPt) < 1.0 Then
                        MessageBox.Show("The same point has already existed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Exit Sub
                    End If
                Next
                points.Add(newPt)
                dataGrid.Rows.Add(x.ToString, y.ToString, "")
            End If
        Catch ex As Exception
        End Try

        UpdatePointsFromDataGrid()
        DrawAll()
    End Sub
    Private Sub FindBestFitCircle()
        foundCircle = Nothing
        UpdatePointsFromDataGrid()

        If outlierPoints.Count < 3 Then
            MessageBox.Show("At least 3 points are needed for best fit circle.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Use the Least Squares method to find the best-fit circle
        foundCircle = CircleFit.FitCircle(outlierPoints)

        ' Calculate the minimum distance of each point to circle
        For Each row As DataGridViewRow In dataGrid.Rows
            If row.Index >= dataGrid.Rows.Count - 1 Then Exit For

            Dim pt As PointF = outlierPoints(row.Index)
            row.Cells(2).Value = CSng(Math.Abs(Math.Sqrt((pt.X - foundCircle.center.X) * (pt.X - foundCircle.center.X) +
                                           (pt.Y - foundCircle.center.Y) * (pt.Y - foundCircle.center.Y)) - foundCircle.radius))
        Next
    End Sub

    '----------------------------------------------------------------------------'
    ' Find and draw best fit circle
    '----------------------------------------------------------------------------'
    Private Sub btnFindBFC_Click(sender As Object, e As EventArgs) Handles btnFindBFC.Click
        FindBestFitCircle()
        DrawAll()
    End Sub
    Private Sub btnAddFoundCircle_Click(sender As Object, e As EventArgs) Handles btnAddFoundCircle.Click
        If foundCircle Is Nothing Then Exit Sub

        circles.Add(New Circle(foundCircle))
        foundCircle = Nothing

        ClearWorkingData()
        DrawAll()
    End Sub
    '----------------------------------------------------------------------------'
    ' Find and draw trend line
    '----------------------------------------------------------------------------'
    Private Sub FindTrendLine()
        foundLine = Nothing

        UpdatePointsFromDataGrid()
        If outlierPoints.Count < 2 Then
            MessageBox.Show("At least 2 points are needed for trend line.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Use the Least Squares method to find the trend line
        foundLine = TrendLineFinder.Calculate(outlierPoints)

        ' Calculate the minimum distance of each point to circle
        For Each row As DataGridViewRow In dataGrid.Rows
            If row.Index >= dataGrid.Rows.Count - 1 Then
                Exit For
            End If

            Dim pt As PointF = outlierPoints(row.Index)
            row.Cells(2).Value = If(foundLine.IsVertical, 0, CSng(Math.Abs(foundLine.slope * pt.X - pt.Y + foundLine.intercept) / Math.Sqrt(foundLine.slope ^ 2 + 1)))
        Next
    End Sub
    Private Sub btnFindTrendLine_Click(sender As Object, e As EventArgs) Handles btnFindTrendLine.Click
        FindTrendLine()
        DrawAll()
    End Sub

    Private Sub btnAddFoundLine_Click(sender As Object, e As EventArgs) Handles btnAddFoundLine.Click
        If foundLine Is Nothing Then Exit Sub

        lines.Add(New Line(foundLine))
        foundLine = Nothing

        ClearWorkingData()
        DrawAll()
    End Sub
    Private Sub ClearWorkingData()
        dataGrid.Rows.Clear()
        outlierPoints.Clear()
        points.Clear()
        selPoints.Clear()
    End Sub
    Private Sub UpdatePointsFromDataGrid()
        outlierPoints.Clear()
        For Each row As DataGridViewRow In dataGrid.Rows
            If row.Index >= dataGrid.Rows.Count - 1 Then Exit For
            Try
                Dim x As Single, y As Single
                If Single.TryParse(row.Cells(0).Value.ToString(), x) AndAlso Single.TryParse(row.Cells(1).Value.ToString(), y) Then
                    outlierPoints.Add(New PointF With {.X = x, .Y = y})
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub
End Class

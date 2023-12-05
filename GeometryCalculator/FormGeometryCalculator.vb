Imports System.Drawing.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox
Imports MathNet.Numerics

Public Class FormGeometryCalculator

    Dim brshBlack As New SolidBrush(Color.Black)
    Dim brshRed As New SolidBrush(Color.Red)
    Dim brshGreen As New SolidBrush(Color.Green)
    Dim brshBlue As New SolidBrush(Color.Blue)
    Dim brshMagenta As New SolidBrush(Color.Magenta)

    Dim penBoldMagenta As New Pen(brshMagenta, 3)
    Dim penBoldGreen As New Pen(brshGreen, 3)

    Dim fontNormal As New Font("Seoge UI", 8)
    Dim fontBig As New Font("Seoge UI", 14)

    Dim bufferBitmap As Bitmap
    Dim zoomFactor As Single = 1.0

    Dim centerX As Single, centerY As Single
    Dim outlierPoints As New List(Of PointF)
    Dim circles As New List(Of Circle), selCircles As New List(Of Circle)
    Dim lines As New List(Of Line), selLines As New List(Of Line)
    Dim points As New List(Of PointF), selPoints As New List(Of PointF)

    Private foundCircle As Circle
    Private foundLine As Line

    Dim curMode As GeometryType = GeometryType.NA

    Dim activePoint As PointF
    Dim ptTangents() As PointF
    Dim curDistance As Single

    ' Create a ContextMenuStrip
    Dim clickedPt As PointF
    Dim contextMenu As New ContextMenuStrip()

    Dim minSelected As Boolean = False
    Dim maxSelected As Boolean = False
    Dim tangentSelected As Boolean = False
    Dim perpendicularSelected As Boolean = False

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        picView.Width = 1000
        picView.Height = 750
        centerX = picView.Width / 2
        centerY = picView.Height / 2

        hsbPicView.Location = New Point(picView.Left, picView.Bottom)
        hsbPicView.Size = New Size(picView.Width, 15)
        vsbPicView.Location = New Point(picView.Right, picView.Top)
        vsbPicView.Size = New Size(15, picView.Height)

        hsbPicView.Maximum = 0
        vsbPicView.Maximum = 0

        panelOutlierFinder.Left = vsbPicView.Right + 10
        dgvResults.Left = panelOutlierFinder.Left
        dgvResults.Top = panelOutlierFinder.Bottom + 10
        dgvResults.Height = hsbPicView.Bottom - dgvResults.Top
        Me.ClientSize = New Size(panelOutlierFinder.Right + 10, hsbPicView.Bottom + 10)

        ' Create the buffer bitmap
        bufferBitmap = New Bitmap(picView.Width, picView.Height)

        ' Update data grid view for result
        dgvResults.AllowUserToAddRows = False
        'dgvResults.AllowUserToDeleteRows = False
        dgvResults.ColumnCount = 3
        dgvResults.Columns(0).HeaderText = "Type"
        dgvResults.Columns(1).HeaderText = "Measurement"
        dgvResults.Columns(2).HeaderText = "Value"

        ' Set example points
        outlierPoints.Add(New PointF With {.X = 50.25, .Y = 168.12})
        outlierPoints.Add(New PointF With {.X = -55.3, .Y = 142.8})
        outlierPoints.Add(New PointF With {.X = 80.9, .Y = -250.66})
        outlierPoints.Add(New PointF With {.X = 400.52, .Y = 150.06})
        outlierPoints.Add(New PointF With {.X = -306.33, .Y = -230.78})

        ' Update data grid view with points
        dgvPoints.ColumnCount = 3
        dgvPoints.Columns(0).HeaderText = "X"
        dgvPoints.Columns(1).HeaderText = "Y"
        dgvPoints.Columns(2).HeaderText = "Data Analysis"
        For Each pt As PointF In outlierPoints
            dgvPoints.Rows.Add(pt.X.ToString, pt.Y.ToString, "")
            points.Add(pt)
        Next

        UpdatePointsFromDataGrid()

        UpdateUI()
        DrawAll()
    End Sub

    Private Sub UpdateUI()
        ' UI elements for geometry calculator
    End Sub
    Private Sub MenuItem_Clicked(sender As Object, e As EventArgs)
        If contextMenu Is Nothing Then Exit Sub

        minSelected = False
        maxSelected = False
        tangentSelected = False
        perpendicularSelected = False

        contextMenu.Hide() 'Sometimes the menu items can remain open.  May not be necessary for you.
        Dim item As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)
        If item IsNot Nothing Then
            Select Case item.Text
                Case "Delete"
                    If DeleteElement(clickedPt) Then
                        DrawAll()
                        Return
                    End If
                Case "Max"
                    maxSelected = True
                Case "Min"
                    minSelected = True
                Case "Tangent"
                    tangentSelected = True
                Case "Perpendicular"
                    perpendicularSelected = True
                Case Else
            End Select

            If selPoints.Count > 0 And selLines.Count > 0 Then
                GetDistanceBetweenPointAndLine()
            ElseIf selPoints.Count > 0 And selCircles.Count > 0 Then
                GetDistanceBetweenCircleAndPoint()
            ElseIf selCircles.Count > 1 Then
                GetDistanceBetweenTwoCircles()
            ElseIf selLines.Count > 1 Then
                If item.Text = "Create Intersection Point" Then
                    CreateIntersectionPointBetweenTwoLines()
                ElseIf item.Text.Contains("Delta") Then
                    GetAngleBetweenTwoLines(item.Text)
                End If
            End If
        End If
    End Sub
    Private Sub ShowContextMenu(point As Point)
        contextMenu = New ContextMenuStrip()

        ' Menu items for measurement
        Dim mnMax As ToolStripMenuItem = New ToolStripMenuItem("Max", Nothing, AddressOf MenuItem_Clicked)
        Dim mnMin As ToolStripMenuItem = New ToolStripMenuItem("Min", Nothing, AddressOf MenuItem_Clicked)
        Dim mnTangent As ToolStripMenuItem = New ToolStripMenuItem("Tangent", Nothing, AddressOf MenuItem_Clicked)
        Dim mnPerpendicular As ToolStripMenuItem = New ToolStripMenuItem("Perpendicular", Nothing, AddressOf MenuItem_Clicked)
        contextMenu.Items.Add(mnMax)
        contextMenu.Items.Add(mnMin)
        contextMenu.Items.Add(mnTangent)
        contextMenu.Items.Add(mnPerpendicular)
        mnMax.Enabled = (selPoints.Count > 0 And selCircles.Count > 0) Or (selPoints.Count > 0 And selLines.Count > 0) Or selCircles.Count > 1
        mnMin.Enabled = (selPoints.Count > 0 And selCircles.Count > 0) Or selPoints.Count > 0 And selLines.Count > 0 Or selCircles.Count > 1
        mnTangent.Enabled = selCircles.Count > 1 Or (selPoints.Count > 0 And selCircles.Count > 0)
        mnPerpendicular.Enabled = selPoints.Count > 0 And selLines.Count > 0
        Dim separator As New ToolStripSeparator()
        contextMenu.Items.Add(separator)
        ' Menu items for calculation
        If selLines.Count > 1 Then
            Dim mnCreateIntersectionPoint As ToolStripMenuItem = New ToolStripMenuItem("Create Intersection Point", Nothing, AddressOf MenuItem_Clicked)
            Dim mnCalculateAngle As ToolStripMenuItem = New ToolStripMenuItem("Calculate Angle", Nothing, AddressOf MenuItem_Clicked)
            contextMenu.Items.Add(mnCreateIntersectionPoint)
            Dim mn90Delta As ToolStripMenuItem = New ToolStripMenuItem("90- Delta", Nothing, AddressOf MenuItem_Clicked)
            Dim mn180Delta As ToolStripMenuItem = New ToolStripMenuItem("180- Delta", Nothing, AddressOf MenuItem_Clicked)
            Dim mn270Delta As ToolStripMenuItem = New ToolStripMenuItem("270- Delta", Nothing, AddressOf MenuItem_Clicked)
            Dim mn360Delta As ToolStripMenuItem = New ToolStripMenuItem("360- Delta", Nothing, AddressOf MenuItem_Clicked)
            mnCalculateAngle.DropDownItems.Add(mn90Delta)
            mnCalculateAngle.DropDownItems.Add(mn180Delta)
            mnCalculateAngle.DropDownItems.Add(mn270Delta)
            mnCalculateAngle.DropDownItems.Add(mn360Delta)
            contextMenu.Items.Add(mnCalculateAngle)

            contextMenu.Items.Add(separator)
        End If
        Dim mnDelete As ToolStripMenuItem = New ToolStripMenuItem("Delete", Nothing, AddressOf MenuItem_Clicked)
        contextMenu.Items.Add(mnDelete)

        contextMenu.Show(picView, point)
    End Sub

    Private Sub picView_MouseClick(sender As Object, e As MouseEventArgs) Handles picView.MouseClick
        clickedPt = New PointF(e.Location.X, e.Location.Y)

        clickedPt.X -= centerX
        clickedPt.Y = centerY - clickedPt.Y

        clickedPt.X /= zoomFactor
        clickedPt.Y /= zoomFactor

        Dim threshold As Single = 2
        If e.Button = MouseButtons.Right Then
            ShowContextMenu(e.Location)
            Exit Sub
        End If

        For Each circle As Circle In circles
            If Math.Abs(CalculateDistance(clickedPt, circle.center) - circle.radius) <= threshold Then
                If selLines.Count = 0 And (selPoints.Count + selCircles.Count) < 2 Then
                    selCircles.Add(circle)
                    circles.Remove(circle)
                Else
                    MessageBox.Show("It is possible only to select one point and one circle or two circles.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                GoTo lEnd
            End If
        Next

        For Each circle As Circle In selCircles
            If Math.Abs(CalculateDistance(clickedPt, circle.center) - circle.radius) <= threshold Then
                circles.Add(circle)
                selCircles.Remove(circle)
                GoTo lEnd
            End If
        Next

        For Each line As Line In lines
            If line.Contains(clickedPt, threshold) Then
                If selCircles.Count = 0 And selPoints.Count < 2 And selLines.Count < 2 Then
                    selLines.Add(line)
                    lines.Remove(line)
                Else
                    MessageBox.Show("It is possible only to select one line and one point or two lines.",
                                    "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                GoTo lEnd
            End If
        Next

        For Each line As Line In selLines
            If line.Contains(clickedPt, threshold) Then
                lines.Add(line)
                selLines.Remove(line)
                GoTo lEnd
            End If
        Next

        For Each pt As PointF In points
            If CalculateDistance(clickedPt, pt) <= threshold Then
                If selPoints.Count = 0 And (selLines.Count + selCircles.Count) < 2 Then
                    selPoints.Add(pt)
                    points.Remove(pt)
                Else
                    MessageBox.Show("It is possible only to select one line and one point or one circle and one point",
                                    "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                GoTo lEnd
            End If
        Next

        For Each pt As PointF In selPoints
            If CalculateDistance(clickedPt, pt) <= threshold Then
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
        'e.Graphics.Clear(Color.White)
        e.Graphics.TranslateTransform(-hsbPicView.Value, -vsbPicView.Value)
        e.Graphics.DrawImage(bufferBitmap, 0, 0, picView.Width * zoomFactor, picView.Height * zoomFactor)
    End Sub
    Private Sub DrawOnBuffer(ByRef g As Graphics)

        ' Draw the inactive elements
        For Each circle As Circle In circles
            g.DrawEllipse(Pens.Magenta, SR(circle.rc))
            g.FillEllipse(brshMagenta, centerX + CInt(circle.center.X - 2), centerY - circle.center.Y - 2, 4, 4)
            g.DrawString("(" + circle.center.X.ToString + ", " + circle.center.Y.ToString + ")",
                            fontNormal, brshMagenta, centerX + circle.center.X + 5, centerY - (circle.center.Y + 5))
        Next

        For Each circle As Circle In selCircles
            g.DrawEllipse(penBoldMagenta, SR(circle.rc))
            g.FillEllipse(brshMagenta, centerX + CInt(circle.center.X - 2), centerY - circle.center.Y - 2, 4, 4)
            g.DrawString("(" + circle.center.X.ToString + ", " + circle.center.Y.ToString + ")",
                            fontNormal, brshMagenta, centerX + circle.center.X + 5, centerY - (circle.center.Y + 5))
        Next

        For Each line As Line In lines
            g.DrawLine(Pens.Green, SP(line.startPt), SP(line.endPt))
            DrawPoint(g, line.startPt, brshGreen, brshGreen, fontNormal)
            DrawPoint(g, line.endPt, brshGreen, brshGreen, fontNormal)
        Next

        For Each line As Line In selLines
            g.DrawLine(penBoldGreen, SP(line.startPt), SP(line.endPt))
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
            If minSelected Then
                DrawPoint(g, activePoint, brshRed, brshGreen, fontNormal, 3, "Min")
            End If
            If maxSelected Then
                DrawPoint(g, activePoint, brshRed, brshGreen, fontNormal, 3, "Max")
            End If
            If tangentSelected Then
                g.DrawLine(Pens.Red, SP(ptTangents(0)), SP(selPoints(0)))
                g.DrawLine(Pens.Blue, SP(ptTangents(0)), SP(selCircles(0).center))
                DrawPoint(g, ptTangents(0), brshRed, brshGreen, fontNormal, 2, "Tangent 1")
                g.DrawLine(Pens.Red, SP(ptTangents(1)), SP(selPoints(0)))
                g.DrawLine(Pens.Blue, SP(ptTangents(1)), SP(selCircles(0).center))
                DrawPoint(g, ptTangents(1), brshRed, brshGreen, fontNormal, 2, "Tangent 2")
            End If
        End If

        If curMode = GeometryType.PointAndLine And selPoints.Count > 0 And selLines.Count > 0 Then
            g.DrawLine(Pens.Red, SP(activePoint), SP(selPoints(0)))
            DrawPoint(g, activePoint, brshBlue, brshBlack, fontNormal)
            If perpendicularSelected Then
                If activePoint.X >= selLines(0).startPt.X And activePoint.X >= selLines(0).endPt.X Then
                    g.DrawLine(Pens.Magenta, SP(activePoint), SP(If(selLines(0).startPt.X > selLines(0).endPt.X, selLines(0).startPt, selLines(0).endPt)))
                ElseIf activePoint.X <= selLines(0).startPt.X And activePoint.X <= selLines(0).endPt.X Then
                    g.DrawLine(Pens.Magenta, SP(activePoint), SP(If(selLines(0).startPt.X < selLines(0).endPt.X, selLines(0).startPt, selLines(0).endPt)))
                End If
            End If
        End If

        If curMode = GeometryType.CircleAndCircle And selCircles.Count > 1 Then
            g.DrawLine(Pens.Blue, SP(selCircles(0).center), SP(selCircles(1).center))
        End If

        If curMode = GeometryType.IntersectionBetweenTwoLines And selLines.Count > 1 Then
            DrawPoint(g, activePoint, brshRed, brshBlack, fontNormal)
        End If
    End Sub

    Private Sub GetDistanceBetweenPointAndLine()
        If selLines.Count = 0 Or selPoints.Count = 0 Then
            MessageBox.Show("One line and one point are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        curMode = GeometryType.PointAndLine
        Dim distance1 As Single = CalculateDistance(selLines(0).startPt, selPoints(0))
        Dim distance2 As Single = CalculateDistance(selLines(0).endPt, selPoints(0))

        If maxSelected Then
            activePoint = If(distance1 > distance2, selLines(0).startPt, selLines(0).endPt)
            dgvResults.Rows.Add("Point and Line", "Max", If(distance1 > distance2, distance1, distance2).ToString)
        End If

        If minSelected Then
            activePoint = If(distance1 < distance2, selLines(0).startPt, selLines(0).endPt)
            dgvResults.Rows.Add("Point and Line", "Min", If(distance1 < distance2, distance1, distance2).ToString)
        End If

        If perpendicularSelected Then
            Dim xNearest As Double = If(selLines(0).IsVertical(), selLines(0).startPt.X,
                (selPoints(0).X + selLines(0).slope * selPoints(0).Y - selLines(0).slope * selLines(0).intercept) / (selLines(0).slope ^ 2 + 1))
            Dim yNearest As Double = If(selLines(0).IsVertical(), selPoints(0).Y, selLines(0).slope * xNearest + selLines(0).intercept)
            activePoint = New Point(xNearest, yNearest)
            dgvResults.Rows.Add("Point and Line", "Perpendicular", CalculateDistance(activePoint, selPoints(0)).ToString)
        End If
        DrawAll()
    End Sub

    Private Sub GetDistanceBetweenCircleAndPoint()
        If selCircles.Count = 0 Or selPoints.Count = 0 Then
            MessageBox.Show("One circle and one point are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        curMode = GeometryType.CircleAndPoint
        Dim maxPoint As PointF, minPoint As PointF
        If minSelected Or maxSelected Then
            FindPointsOnCircle(selCircles(0), selPoints(0), maxPoint, minPoint)
        End If

        If minSelected Then
            activePoint = minPoint
            dgvResults.Rows.Add("Circle and Point", "Min", CalculateDistance(selPoints(0), activePoint).ToString)
        End If
        If maxSelected Then
            activePoint = maxPoint
            dgvResults.Rows.Add("Circle and Point", "Max", CalculateDistance(selPoints(0), activePoint).ToString)
        End If
        If tangentSelected Then
            Dim distance As Single = CalculateDistance(selCircles(0).center, selPoints(0))
            If distance < selCircles(0).radius Then
                MessageBox.Show("Tangent can be calculated for the point in circle.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim tangentDistance = Math.Sqrt(distance ^ 2 - selCircles(0).radius ^ 2)
            ptTangents = FindTangentPoints(selCircles(0), selPoints(0))

            dgvResults.Rows.Add("Circle and Point", "Tangent", tangentDistance.ToString)
        End If

        DrawAll()
    End Sub

    Private Sub GetDistanceBetweenTwoCircles()
        If selCircles.Count < 2 Then
            MessageBox.Show("Two circles are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = GeometryType.CircleAndCircle
        Dim centerDistance As Single = CalculateDistance(selCircles(0).center, selCircles(1).center)
        Dim distance As Single = selCircles(0).radius + selCircles(1).radius

        If maxSelected Then
            curDistance = centerDistance + (selCircles(0).radius + selCircles(1).radius)
            dgvResults.Rows.Add("Circle and Circle", "Max", curDistance.ToString)
        ElseIf minSelected Then
            curDistance = Math.Abs(centerDistance - (selCircles(0).radius + selCircles(1).radius))
            dgvResults.Rows.Add("Circle and Circle", "Min", curDistance.ToString)
        ElseIf tangentSelected Then
            Dim externalDistance As Single = Math.Sqrt((selCircles(0).radius - selCircles(1).radius) ^ 2 + centerDistance ^ 2)
            curDistance = If(centerDistance < distance, externalDistance,
                    Math.Min(externalDistance, Math.Sqrt(centerDistance ^ 2 - (selCircles(0).radius + selCircles(1).radius) ^ 2)))
            dgvResults.Rows.Add("Circle and Circle", "Tangent", curDistance.ToString)
        End If

        DrawAll()
    End Sub

    Private Sub GetAngleBetweenTwoLines(menuTitle As String)
        If selLines.Count < 2 Then
            MessageBox.Show("Two lines are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim parts As String() = menuTitle.Split("-"c)
        Dim delta As Integer = Integer.Parse(parts(0))
        curMode = GeometryType.AngleBetweenTwoLines
        dgvResults.Rows.Add("Line and Line", "Degree with delta(" + delta.ToString + ")",
                           (delta - CalculateAngleBetweenLines(selLines(0), selLines(1))).ToString)
    End Sub

    Private Sub CreateIntersectionPointBetweenTwoLines()
        If selLines.Count < 2 Then
            MessageBox.Show("Two lines are needed.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        curMode = GeometryType.IntersectionBetweenTwoLines

        activePoint = GetIntersection(selLines(0), selLines(1))
        points.Add(activePoint)
        dgvPoints.Rows.Add(activePoint.X.ToString, activePoint.Y.ToString, "")

        UpdatePointsFromDataGrid()
        DrawAll()
    End Sub

    Private Sub btnClearAll_Click(sender As Object, e As EventArgs) Handles btnClearAll.Click
        circles.Clear()
        selCircles.Clear()
        lines.Clear()
        selLines.Clear()
        points.Clear()
        selPoints.Clear()

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

        'Draw X-Y axis
        g.DrawLine(Pens.Black, 0, centerY, picView.Width, centerY)
        g.DrawLine(Pens.Black, centerX, 0, centerX, picView.Height)

        DrawPointsForOutlier(g)
        DrawBestFitCircle(g, foundCircle)
        DrawTrendLine(g, foundLine)
        DrawOnBuffer(g)

        If redraw Then picView.Invalidate()
    End Sub

    Public Sub DrawPoint(ByRef g As Graphics, pt As PointF, ByRef brshPoint As Brush, ByRef brshString As Brush, ByRef font As Font, Optional sz As Integer = 2, Optional suffix As String = "")
        Dim newPoint As PointF = SP(pt)
        g.FillEllipse(brshPoint, CInt(newPoint.X - sz), CInt(newPoint.Y - sz), sz * 2, sz * 2)
        g.DrawString(suffix + "(" + pt.X.ToString + ", " + pt.Y.ToString + ")", font, brshString, newPoint.X + 5, newPoint.Y + 5)
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
        g.DrawLine(New Pen(brshGreen, 2), SP(ln.startPt), SP(ln.endPt))

        ' Draw the line from each point to its nearest point on trend line
        For Each pt As PointF In outlierPoints
            ' Draw the point
            DrawPoint(g, pt, brshBlue, brshBlack, fontNormal, 3)

            If ln.IsVertical() Then
                g.FillEllipse(brshRed, centerX + pt.X - 3, centerY - pt.Y - 3, 6, 6)
                Continue For
            End If

            ' Draw the nearest point on the trend line
            Dim xNearest As Double = (pt.X + ln.slope * pt.Y - ln.slope * ln.intercept) / (ln.slope ^ 2 + 1)
            Dim yNearest As Double = ln.slope * xNearest + ln.intercept

            g.FillEllipse(brshRed, centerX + CSng(xNearest - 3), centerY - CSng(yNearest) - 3, 6, 6)

            ' Draw the line between two points
            g.DrawLine(Pens.Red, centerX + CSng(pt.X), centerY - CSng(pt.Y), centerX + CSng(xNearest), centerY - CSng(yNearest))
        Next

    End Sub

    ' Display the result for Best Fit Circle in the PictureBox
    Private Sub DrawBestFitCircle(ByRef g As Graphics, ByRef circle As Circle)
        If outlierPoints.Count < 3 Or circle Is Nothing Then Exit Sub

        Dim cX As Single = Math.Floor(circle.center.X * 1000) / 1000
        Dim cY As Single = Math.Floor(circle.center.Y * 1000) / 1000
        Dim radius As Single = Math.Floor(circle.radius * 1000) / 1000

        For Each pt As PointF In outlierPoints
            DrawPoint(g, pt, brshBlue, brshBlack, fontNormal, 3)
            Dim angle As Double = Math.Atan((pt.Y - cY) / (pt.X - cX))
            Dim pt1 As PointF = New PointF(cX + circle.radius * Math.Cos(angle), cY + circle.radius * Math.Sin(angle))
            Dim pt2 As PointF = New PointF(cX + circle.radius * Math.Cos(angle + Math.PI), cY + circle.radius * Math.Sin(angle + Math.PI))
            Dim distance1 As Single = CalculateDistance(pt, pt1)
            Dim distance2 As Single = CalculateDistance(pt, pt2)
            g.DrawLine(Pens.Blue, SP(pt), SP(If(distance1 > distance2, pt2, pt1)))
        Next

        ' Draw center of the circle
        g.FillEllipse(brshRed, centerX + cX - 2, centerY - cY - 2, 4, 4)
        g.DrawString("(" + cX.ToString + ", " + cY.ToString + ")", fontNormal,
                    brshBlack, centerX + cX + 5, centerY - (cY - 5))
        ' Draw radius of the circle
        g.DrawString("Radius:" + radius.ToString, fontBig, brshBlack, 10, 10)
        ' Draw the circle
        g.DrawEllipse(New Pen(Color.Red, 2), centerX + cX - radius, centerY - cY - radius, 2 * radius, 2 * radius)

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
                dgvPoints.Rows.Add(x.ToString, y.ToString, "")
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
        For Each row As DataGridViewRow In dgvPoints.Rows
            If row.Index >= dgvPoints.Rows.Count - 1 Then Exit For

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
        foundLine = Nothing
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
        foundLine = TrendLineFinder.Calculate(outlierPoints, -picView.Width / 2, -picView.Height / 2, picView.Width / 2, picView.Height / 2)

        ' Calculate the minimum distance of each point to circle
        For Each row As DataGridViewRow In dgvPoints.Rows
            If row.Index >= dgvPoints.Rows.Count - 1 Then
                Exit For
            End If

            Dim pt As PointF = outlierPoints(row.Index)
            row.Cells(2).Value = If(foundLine.IsVertical, 0, CSng(Math.Abs(foundLine.slope * pt.X - pt.Y + foundLine.intercept) / Math.Sqrt(foundLine.slope ^ 2 + 1)))
        Next
    End Sub
    Private Sub btnFindTrendLine_Click(sender As Object, e As EventArgs) Handles btnFindTrendLine.Click
        FindTrendLine()
        foundCircle = Nothing
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
        dgvPoints.Rows.Clear()
        outlierPoints.Clear()
        points.Clear()
        selPoints.Clear()
        curMode = GeometryType.NA

        minSelected = False
        maxSelected = False
        tangentSelected = False
        perpendicularSelected = False
    End Sub
    Private Sub UpdatePointsFromDataGrid()
        outlierPoints.Clear()
        For Each row As DataGridViewRow In dgvPoints.Rows
            If row.Index >= dgvPoints.Rows.Count - 1 Then Exit For
            Try
                Dim x As Single, y As Single
                If Single.TryParse(row.Cells(0).Value.ToString(), x) AndAlso Single.TryParse(row.Cells(1).Value.ToString(), y) Then
                    outlierPoints.Add(New PointF With {.X = x, .Y = y})
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub
    Private Sub RemoveWorkingPoint(ByRef point As PointF)
        RemovePoint(point, outlierPoints)
        For Each row As DataGridViewRow In dgvPoints.Rows
            If row.Index >= dgvPoints.Rows.Count - 1 Then Exit For
            Try
                Dim x As Single, y As Single
                If Single.TryParse(row.Cells(0).Value.ToString(), x) AndAlso Single.TryParse(row.Cells(1).Value.ToString(), y) Then
                    If point.X = x And point.Y = y Then dgvPoints.Rows.Remove(row)
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub
    Private Function DeleteElement(ByRef point As PointF) As Boolean
        For Each circle As Circle In circles
            If Math.Abs(CalculateDistance(point, circle.center) - circle.radius) <= 2 Then
                circles.Remove(circle)
                Return True
            End If
        Next
        For Each circle As Circle In selCircles
            If Math.Abs(CalculateDistance(point, circle.center) - circle.radius) <= 2 Then
                selCircles.Remove(circle)
                Return True
            End If
        Next
        For Each line As Line In lines
            If line.Contains(point) Then
                lines.Remove(line)
                Return True
            End If
        Next
        For Each line As Line In selLines
            If line.Contains(point) Then
                selLines.Remove(line)
                Return True
            End If
        Next
        For Each pt As PointF In points
            If CalculateDistance(point, pt) <= 2 Then
                points.Remove(pt)
                RemoveWorkingPoint(pt)
                Return True
            End If
        Next
        For Each pt As PointF In selPoints
            If CalculateDistance(point, pt) <= 2 Then
                selPoints.Remove(pt)
                RemoveWorkingPoint(pt)
                Return True
            End If
        Next
        Return False
    End Function

    Private Function SP(pt As PointF) As PointF
        Return New PointF(centerX + pt.X, centerY - pt.Y)
    End Function
    Private Function SR(rect As RectangleF) As RectangleF
        Dim newRect As RectangleF = New RectangleF(SP(rect.Location), rect.Size)
        Return newRect
    End Function

    Private Sub btnZoomIn_Click(sender As Object, e As EventArgs) Handles btnZoomIn.Click
        zoomFactor *= 1.2
        lblCurrentZoom.Text = CInt(zoomFactor * 100).ToString + "%"
        InitScroll()
        picView.Invalidate()
    End Sub

    Private Sub btnZoomOut_Click(sender As Object, e As EventArgs) Handles btnZoomOut.Click
        zoomFactor *= 0.8
        lblCurrentZoom.Text = CInt(zoomFactor * 100).ToString + "%"
        InitScroll()
        picView.Invalidate()
    End Sub

    Private Sub btnSetDefaultZoom_Click(sender As Object, e As EventArgs) Handles btnSetDefaultZoom.Click
        zoomFactor = 1
        lblCurrentZoom.Text = "100%"
        InitScroll()
        picView.Invalidate()
    End Sub

    Private Sub InitScroll()
        If zoomFactor > 1 Then
            hsbPicView.Maximum = CInt(picView.Width * zoomFactor)
            vsbPicView.Maximum = CInt(picView.Height * zoomFactor)
            hsbPicView.LargeChange = picView.Width
            vsbPicView.LargeChange = picView.Height
        Else
            hsbPicView.Maximum = 0
            vsbPicView.Maximum = 0
        End If
    End Sub
    Private Sub vsbPicView_Scroll(sender As Object, e As ScrollEventArgs) Handles vsbPicView.Scroll
        picView.Invalidate()
    End Sub

    Private Sub hsbPicView_Scroll(sender As Object, e As ScrollEventArgs) Handles hsbPicView.Scroll
        picView.Invalidate()
    End Sub
End Class

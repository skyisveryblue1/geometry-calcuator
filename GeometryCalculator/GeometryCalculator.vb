' Make sure to install the Math.NET Numerics library
' using NuGet Package Manager Console with the command:
' Install-Package MathNet.Numerics
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports MathNet.Numerics.LinearAlgebra

Module GeometryCalculator
    Public Sub RemovePoint(ByRef point As PointF, ByRef points As List(Of PointF))
        For Each pt As PointF In points
            If pt.X = point.X And pt.Y = point.Y Then
                points.Remove(pt)
                Exit Sub
            End If
        Next
    End Sub

    Public Function GetIntersection(ByRef line1 As Line, ByRef line2 As Line) As PointF
        If line1.IsVertical Then
            Return New PointF(line1.startPt.X, line1.startPt.X * line2.slope + line2.intercept)
        End If

        If line2.IsVertical Then
            Return New PointF(line2.startPt.X, line2.startPt.X * line1.slope + line1.intercept)
        End If

        ' Calculate the x-coordinate of the intersection point
        Dim x As Double = (line2.intercept - line1.intercept) / (line1.slope - line2.slope)

        ' Use the x-coordinate to calculate the y-coordinate
        Dim y As Double = line1.slope * x + line1.intercept
        Return New PointF(x, y)
    End Function

    ' Function to calculate the angle between two lines
    Public Function CalculateAngleBetweenLines(ByRef line1 As Line, ByRef line2 As Line) As Single
        ' Calculate slopes
        Dim slope1 As Double = If(line1.IsVertical, 0, line1.slope)
        Dim slope2 As Double = If(line2.IsVertical, 0, line2.slope)

        ' Calculate the angle using arctangent
        Dim angle As Single = Math.Atan(Math.Abs((slope2 - slope1) / (1 + slope1 * slope2)))

        If (line1.IsVertical And Not line2.IsVertical) Or (Not line1.IsVertical And line2.IsVertical) Then
            angle = Math.PI / 2 - angle
        End If

        ' Convert the angle from radians to degrees
        angle = angle * (180 / Math.PI)

        Return angle
    End Function

    Public Sub FindPointsOnCircle(ByVal circle As Circle, ByVal point As PointF, ByRef maxDistancePoint As PointF, ByRef minDistancePoint As PointF)
        ' Calculate the angle between the center of the circle and the point
        Dim angle As Double = Math.Atan2(point.Y - circle.center.Y, point.X - circle.center.X)

        ' Calculate the points on the circle with max and min distances
        minDistancePoint.X = circle.center.X + circle.radius * Math.Cos(angle)
        minDistancePoint.Y = circle.center.Y + circle.radius * Math.Sin(angle)

        maxDistancePoint.X = circle.center.X - circle.radius * Math.Cos(angle)
        maxDistancePoint.Y = circle.center.Y - circle.radius * Math.Sin(angle)
    End Sub

    Public Function FindTangentPoints(ByVal circle As Circle, ByVal point As PointF) As PointF()
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

    Public Function CalculateDistance(ByVal pt1 As PointF, ByVal pt2 As PointF) As Single
        Return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2))
    End Function
End Module

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
        Dim radius As Double = Math.Sqrt(x(0) ^ 2 + x(1) ^ 2 - x(2))
        Return New Circle(x(0), x(1), radius)
    End Function

End Class

Public Class TrendLineFinder
    Public Shared Function Calculate(ByVal points As List(Of PointF),
             limitStartX As Single, limitStartY As Single, limitEndX As Single, limitEndY As Single) As Line
        If points Is Nothing OrElse points.Count < 2 Then
            Return Nothing
        End If

        Dim n As Integer = points.Count
        Dim sumX As Double = 0, sumY As Double = 0
        Dim sumXY As Double = 0, sumX2 As Double = 0

        For Each point In points
            sumX += point.X
            sumY += point.Y
            sumXY += point.X * point.Y
            sumX2 += point.X * point.X
        Next

        Dim t As Double = (n * sumX2 - sumX * sumX)
        Dim Slope As Double = (n * sumXY - sumX * sumY) / t
        Dim Intercept As Double = (sumY - Slope * sumX) / n

        Dim ln As Line = New Line With {.slope = Slope, .intercept = Intercept}
        Dim startX As Single = If(ln.IsVertical(), points(0).X, limitStartX)
        Dim endX As Single = If(ln.IsVertical(), points(0).X, limitEndX)
        ln.startPt = New PointF(startX, If(ln.IsVertical(), limitStartY, startX * Slope + Intercept))
        ln.endPt = New PointF(endX, If(ln.IsVertical(), limitEndY, endX * Slope + Intercept))
        Return ln
    End Function
End Class

Public Class Line
    Public startPt As PointF
    Public endPt As PointF
    Public slope As Double
    Public intercept As Double

    Public Sub New()

    End Sub

    Public Sub New(ByRef other As Line)
        Me.slope = other.slope
        Me.intercept = other.intercept
        Me.startPt = New PointF(other.startPt)
        Me.endPt = New PointF(other.endPt)
    End Sub

    Public Sub New(startPoint As PointF, endPoint As PointF)
        startPt = startPoint
        endPt = endPoint

        slope = (endPoint.Y - startPoint.Y) / (endPoint.X - startPoint.X)
        intercept = startPoint.Y - slope * startPoint.X
    End Sub

    Public Function IsVertical()
        Return Double.IsInfinity(Me.slope) Or Double.IsNaN(Me.slope)
    End Function
    Public Function Contains(ByRef point As PointF, Optional threshold As Single = 2) As Boolean
        If Not IsVertical() Then
            If Math.Abs(point.Y - (Me.slope * point.X + Me.intercept)) <= threshold Then Return True
        Else
            Dim minY As Double = Math.Min(startPt.Y, endPt.Y)
            Dim maxY As Double = Math.Max(startPt.Y, endPt.Y)
            If Math.Abs(point.X - startPt.X) <= threshold And minY - 2 <= point.Y And maxY + 2 > point.Y Then Return True
        End If

        Return False
    End Function
End Class

Public Class Circle
    Public radius As Double
    Public rc As RectangleF
    Public center As PointF

    Public Sub New()

    End Sub
    Public Sub New(ByRef other As Circle)
        Me.radius = other.radius
        Me.center = New PointF(other.center)
        Me.rc = New RectangleF(center.X - radius, center.Y + radius, 2 * radius, 2 * radius)
    End Sub

    Public Sub New(cX As Single, cY As Single, radius As Single)
        Me.radius = radius
        Me.center = New PointF(cX, cY)
        Me.rc = New RectangleF(cX - radius, cY + radius, 2 * radius, 2 * radius)
    End Sub

    Public Sub New(ByRef startPoint As Point, ByRef endPoint As Point)
        radius = Math.Sqrt((endPoint.X - startPoint.X) ^ 2 + (endPoint.Y - startPoint.Y) ^ 2)
        center.X = startPoint.X
        center.Y = startPoint.Y
        rc = New RectangleF(center.X - radius, center.Y + radius, 2 * radius, 2 * radius)
    End Sub
End Class
Public Enum GeometryType
    NA
    CircleAndPoint
    AngleBetweenTwoLines
    PointAndLine
    CircleAndCircle
    IntersectionBetweenTwoLines
End Enum
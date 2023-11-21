' Make sure to install the Math.NET Numerics library
' using NuGet Package Manager Console with the command:
' Install-Package MathNet.Numerics
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports MathNet.Numerics.LinearAlgebra

Module GeometryCalculator
    Public Sub DrawPoint(ByRef g As Graphics, pt As PointF, ByRef brhPoint As Brush, ByRef brhString As Brush, ByRef font As Font, Optional sz As Integer = 2, Optional suffix As String = "")
        g.FillEllipse(brhPoint, CInt(pt.X - sz), CInt(pt.Y - sz), sz * 2, sz * 2)
        g.DrawString(suffix + "(" + pt.X.ToString + ", " + pt.Y.ToString + ")", font, brhString, pt.X + 5, pt.Y + 5)
    End Sub

    Public Function GetIntersection(ByRef line1 As Line, ByRef line2 As Line) As PointF
        ' Calculate the x-coordinate of the intersection point
        Dim x As Double = (line2.intercept - line1.intercept) / (line1.slope - line2.slope)

        ' Use the x-coordinate to calculate the y-coordinate
        Dim y As Double = line1.slope * x + line1.intercept

        Return New PointF(x, y)
    End Function

    ' Function to calculate the angle between two lines
    Public Function CalculateAngleBetweenLines(ByRef line1 As Line, ByRef line2 As Line) As Single
        ' Calculate slopes
        Dim slope1 As Double = line1.slope
        Dim slope2 As Double = line2.slope

        ' Calculate the angle using arctangent
        Dim angle As Single = Math.Atan(Math.Abs((slope2 - slope1) / (1 + slope1 * slope2)))

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
            Return Nothing
        End If

        Dim n As Integer = points.Count
        Dim sumX As Double = 0, sumY As Double = 0
        Dim sumXY As Double = 0, sumX2 As Double = 0
        Dim minX As Double = Double.MaxValue, maxX As Double = Double.MinValue
        Dim minY As Double = Double.MaxValue, maxY As Double = Double.MinValue

        For Each point In points
            sumX += point.X
            sumY += point.Y
            sumXY += point.X * point.Y
            sumX2 += point.X * point.X
            If point.X > maxX Then maxX = point.X
            If point.X < minX Then minX = point.X

            If point.Y > maxY Then maxY = point.Y
            If point.Y < minY Then minY = point.Y
        Next

        Dim t As Double = (n * sumX2 - sumX * sumX)
        Dim Slope As Double = (n * sumXY - sumX * sumY) / t
        Dim Intercept As Double = (sumY - Slope * sumX) / n

        Dim ln As Line = New Line With {.slope = Slope, .intercept = Intercept}
        ln.startPt = New PointF(minX, If(ln.IsVertical(), minY, minX * Slope + Intercept))
        ln.endPt = New PointF(maxX, If(ln.IsVertical(), maxY, maxX * Slope + Intercept))
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
    Public rc As Rectangle
    Public center As PointF

    Public Sub New()

    End Sub
    Public Sub New(ByRef other As Circle)
        Me.radius = other.radius
        Me.center = New PointF(other.center)
        Me.rc = New Rectangle(center.X - radius, center.Y - radius, 2 * radius, 2 * radius)
    End Sub

    Public Sub New(ByRef startPoint As Point, ByRef endPoint As Point)
        radius = Math.Sqrt((endPoint.X - startPoint.X) ^ 2 + (endPoint.Y - startPoint.Y) ^ 2)
        center.X = startPoint.X
        center.Y = startPoint.Y
        rc = New Rectangle(center.X - radius, center.Y - radius, 2 * radius, 2 * radius)
    End Sub
End Class
Public Enum GeometryType
    NA
    CircleAndPoint
    LineAndLine
    PointAndLine
    CircleAndCircle
    PointAndTwoLines
End Enum
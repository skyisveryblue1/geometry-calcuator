Public Class FormMain
    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.IsMdiContainer = True
        Me.WindowState = FormWindowState.Maximized

        OpenChildForm()
    End Sub

    Private Sub OpenChildForm()
        Dim childForm As New FormGeometryCalculator()
        childForm.MdiParent = Me
        childForm.Text = "Geometry Calculator " & Me.MdiChildren.Length
        childForm.Show()
    End Sub

    Private Sub GeometryCalculatorStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeometryCalculatorToolStripMenuItem.Click
        OpenChildForm()
    End Sub
End Class
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        picView = New PictureBox()
        GroupBox1 = New GroupBox()
        radioCircle = New RadioButton()
        radioLine = New RadioButton()
        radioPoint = New RadioButton()
        btnClear = New Button()
        GroupBox2 = New GroupBox()
        radioTangent = New RadioButton()
        radioMin = New RadioButton()
        radioMax = New RadioButton()
        btnGetDistanceBetweenCircleAndPoint = New Button()
        txtResult = New TextBox()
        btnGetAngleBetweenTwoLines = New Button()
        CType(picView, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        GroupBox2.SuspendLayout()
        SuspendLayout()
        ' 
        ' picView
        ' 
        picView.BorderStyle = BorderStyle.FixedSingle
        picView.Location = New Point(10, 11)
        picView.Name = "picView"
        picView.Size = New Size(653, 415)
        picView.TabIndex = 0
        picView.TabStop = False
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(radioCircle)
        GroupBox1.Controls.Add(radioLine)
        GroupBox1.Controls.Add(radioPoint)
        GroupBox1.Location = New Point(671, 9)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(169, 105)
        GroupBox1.TabIndex = 1
        GroupBox1.TabStop = False
        GroupBox1.Text = "Element Type"
        ' 
        ' radioCircle
        ' 
        radioCircle.AutoSize = True
        radioCircle.Location = New Point(32, 72)
        radioCircle.Name = "radioCircle"
        radioCircle.Size = New Size(55, 19)
        radioCircle.TabIndex = 0
        radioCircle.Text = "Circle"
        radioCircle.UseVisualStyleBackColor = True
        ' 
        ' radioLine
        ' 
        radioLine.AutoSize = True
        radioLine.Location = New Point(32, 47)
        radioLine.Name = "radioLine"
        radioLine.Size = New Size(47, 19)
        radioLine.TabIndex = 0
        radioLine.Text = "Line"
        radioLine.UseVisualStyleBackColor = True
        ' 
        ' radioPoint
        ' 
        radioPoint.AutoSize = True
        radioPoint.Checked = True
        radioPoint.Location = New Point(32, 22)
        radioPoint.Name = "radioPoint"
        radioPoint.Size = New Size(53, 19)
        radioPoint.TabIndex = 0
        radioPoint.TabStop = True
        radioPoint.Text = "Point"
        radioPoint.UseVisualStyleBackColor = True
        ' 
        ' btnClear
        ' 
        btnClear.Location = New Point(671, 120)
        btnClear.Name = "btnClear"
        btnClear.Size = New Size(169, 29)
        btnClear.TabIndex = 2
        btnClear.Text = "Clear All Elements"
        btnClear.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(radioTangent)
        GroupBox2.Controls.Add(radioMin)
        GroupBox2.Controls.Add(radioMax)
        GroupBox2.Location = New Point(671, 155)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(169, 105)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Measure Type"
        ' 
        ' radioTangent
        ' 
        radioTangent.AutoSize = True
        radioTangent.Location = New Point(32, 72)
        radioTangent.Name = "radioTangent"
        radioTangent.Size = New Size(67, 19)
        radioTangent.TabIndex = 0
        radioTangent.Text = "Tangent"
        radioTangent.UseVisualStyleBackColor = True
        ' 
        ' radioMin
        ' 
        radioMin.AutoSize = True
        radioMin.Location = New Point(32, 47)
        radioMin.Name = "radioMin"
        radioMin.Size = New Size(46, 19)
        radioMin.TabIndex = 0
        radioMin.Text = "Min"
        radioMin.UseVisualStyleBackColor = True
        ' 
        ' radioMax
        ' 
        radioMax.AutoSize = True
        radioMax.Checked = True
        radioMax.Location = New Point(32, 22)
        radioMax.Name = "radioMax"
        radioMax.Size = New Size(48, 19)
        radioMax.TabIndex = 0
        radioMax.TabStop = True
        radioMax.Text = "Max"
        radioMax.UseVisualStyleBackColor = True
        ' 
        ' btnGetDistanceBetweenCircleAndPoint
        ' 
        btnGetDistanceBetweenCircleAndPoint.Location = New Point(671, 268)
        btnGetDistanceBetweenCircleAndPoint.Name = "btnGetDistanceBetweenCircleAndPoint"
        btnGetDistanceBetweenCircleAndPoint.Size = New Size(169, 44)
        btnGetDistanceBetweenCircleAndPoint.TabIndex = 3
        btnGetDistanceBetweenCircleAndPoint.Text = "Get Distance between Circle and Point"
        btnGetDistanceBetweenCircleAndPoint.UseVisualStyleBackColor = True
        ' 
        ' txtResult
        ' 
        txtResult.Location = New Point(10, 438)
        txtResult.Multiline = True
        txtResult.Name = "txtResult"
        txtResult.ReadOnly = True
        txtResult.Size = New Size(653, 66)
        txtResult.TabIndex = 4
        ' 
        ' btnGetAngleBetweenTwoLines
        ' 
        btnGetAngleBetweenTwoLines.Location = New Point(671, 318)
        btnGetAngleBetweenTwoLines.Name = "btnGetAngleBetweenTwoLines"
        btnGetAngleBetweenTwoLines.Size = New Size(169, 44)
        btnGetAngleBetweenTwoLines.TabIndex = 3
        btnGetAngleBetweenTwoLines.Text = "Get Angle between Two Lines"
        btnGetAngleBetweenTwoLines.UseVisualStyleBackColor = True
        ' 
        ' FormMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(844, 507)
        Controls.Add(txtResult)
        Controls.Add(btnGetAngleBetweenTwoLines)
        Controls.Add(btnGetDistanceBetweenCircleAndPoint)
        Controls.Add(btnClear)
        Controls.Add(GroupBox2)
        Controls.Add(GroupBox1)
        Controls.Add(picView)
        Name = "FormMain"
        Text = "Geometrical Calculator"
        CType(picView, ComponentModel.ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents picView As PictureBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents radioCircle As RadioButton
    Friend WithEvents radioLine As RadioButton
    Friend WithEvents radioPoint As RadioButton
    Friend WithEvents btnClear As Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents radioTangent As RadioButton
    Friend WithEvents radioMin As RadioButton
    Friend WithEvents radioMax As RadioButton
    Friend WithEvents btnGetDistanceBetweenCircleAndPoint As Button
    Friend WithEvents txtResult As TextBox
    Friend WithEvents btnGetAngleBetweenTwoLines As Button

End Class

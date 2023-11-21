<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormGeometryCalculator
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
        txtResult = New TextBox()
        panelGeometryCalculator = New Panel()
        btnGetDistanceBetweenPointAndIntersection = New Button()
        btnClearAll = New Button()
        btnGetDistanceBetweenTwoCircles = New Button()
        grpMeasureType = New GroupBox()
        radioPerpendicular = New RadioButton()
        radioTangent = New RadioButton()
        radioMin = New RadioButton()
        radioMax = New RadioButton()
        btnGetDistanceBetweenPointAndLine = New Button()
        btnGetDistanceBetweenCircleAndPoint = New Button()
        btnGetAngleBetweenTwoLines = New Button()
        btnClearPoints = New Button()
        btnFindTrendLine = New Button()
        btnFindBFC = New Button()
        dataGrid = New DataGridView()
        panelOutlierFinder = New Panel()
        btnAddFoundLine = New Button()
        btnAddFoundCircle = New Button()
        btnSavePoint = New Button()
        Label2 = New Label()
        Label1 = New Label()
        txtCurrentY = New TextBox()
        txtCurrentX = New TextBox()
        CType(picView, ComponentModel.ISupportInitialize).BeginInit()
        panelGeometryCalculator.SuspendLayout()
        grpMeasureType.SuspendLayout()
        CType(dataGrid, ComponentModel.ISupportInitialize).BeginInit()
        panelOutlierFinder.SuspendLayout()
        SuspendLayout()
        ' 
        ' picView
        ' 
        picView.BorderStyle = BorderStyle.FixedSingle
        picView.Location = New Point(10, 11)
        picView.Name = "picView"
        picView.Size = New Size(653, 447)
        picView.TabIndex = 0
        picView.TabStop = False
        ' 
        ' txtResult
        ' 
        txtResult.Location = New Point(12, 464)
        txtResult.Multiline = True
        txtResult.Name = "txtResult"
        txtResult.ReadOnly = True
        txtResult.Size = New Size(651, 82)
        txtResult.TabIndex = 4
        ' 
        ' panelGeometryCalculator
        ' 
        panelGeometryCalculator.Controls.Add(btnGetDistanceBetweenPointAndIntersection)
        panelGeometryCalculator.Controls.Add(btnClearAll)
        panelGeometryCalculator.Controls.Add(btnGetDistanceBetweenTwoCircles)
        panelGeometryCalculator.Controls.Add(grpMeasureType)
        panelGeometryCalculator.Controls.Add(btnGetDistanceBetweenPointAndLine)
        panelGeometryCalculator.Controls.Add(btnGetDistanceBetweenCircleAndPoint)
        panelGeometryCalculator.Controls.Add(btnGetAngleBetweenTwoLines)
        panelGeometryCalculator.Location = New Point(669, 73)
        panelGeometryCalculator.Name = "panelGeometryCalculator"
        panelGeometryCalculator.Size = New Size(280, 473)
        panelGeometryCalculator.TabIndex = 8
        ' 
        ' btnGetDistanceBetweenPointAndIntersection
        ' 
        btnGetDistanceBetweenPointAndIntersection.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnGetDistanceBetweenPointAndIntersection.Location = New Point(4, 435)
        btnGetDistanceBetweenPointAndIntersection.Name = "btnGetDistanceBetweenPointAndIntersection"
        btnGetDistanceBetweenPointAndIntersection.Size = New Size(266, 38)
        btnGetDistanceBetweenPointAndIntersection.TabIndex = 7
        btnGetDistanceBetweenPointAndIntersection.Text = "Get Distance between Point and Intersection of Two Lines"
        btnGetDistanceBetweenPointAndIntersection.UseVisualStyleBackColor = True
        ' 
        ' btnClearAll
        ' 
        btnClearAll.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnClearAll.Location = New Point(4, 114)
        btnClearAll.Name = "btnClearAll"
        btnClearAll.Size = New Size(266, 34)
        btnClearAll.TabIndex = 2
        btnClearAll.Text = "Clear All"
        btnClearAll.UseVisualStyleBackColor = True
        ' 
        ' btnGetDistanceBetweenTwoCircles
        ' 
        btnGetDistanceBetweenTwoCircles.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnGetDistanceBetweenTwoCircles.Location = New Point(4, 396)
        btnGetDistanceBetweenTwoCircles.Name = "btnGetDistanceBetweenTwoCircles"
        btnGetDistanceBetweenTwoCircles.Size = New Size(266, 34)
        btnGetDistanceBetweenTwoCircles.TabIndex = 6
        btnGetDistanceBetweenTwoCircles.Text = "Get Distance between Two Circles"
        btnGetDistanceBetweenTwoCircles.UseVisualStyleBackColor = True
        ' 
        ' grpMeasureType
        ' 
        grpMeasureType.Controls.Add(radioPerpendicular)
        grpMeasureType.Controls.Add(radioTangent)
        grpMeasureType.Controls.Add(radioMin)
        grpMeasureType.Controls.Add(radioMax)
        grpMeasureType.Location = New Point(4, 154)
        grpMeasureType.Name = "grpMeasureType"
        grpMeasureType.Size = New Size(266, 122)
        grpMeasureType.TabIndex = 1
        grpMeasureType.TabStop = False
        grpMeasureType.Text = "Measure Type"
        ' 
        ' radioPerpendicular
        ' 
        radioPerpendicular.AutoSize = True
        radioPerpendicular.Location = New Point(32, 97)
        radioPerpendicular.Name = "radioPerpendicular"
        radioPerpendicular.Size = New Size(98, 19)
        radioPerpendicular.TabIndex = 0
        radioPerpendicular.Text = "Perpendicular"
        radioPerpendicular.UseVisualStyleBackColor = True
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
        ' btnGetDistanceBetweenPointAndLine
        ' 
        btnGetDistanceBetweenPointAndLine.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnGetDistanceBetweenPointAndLine.Location = New Point(4, 358)
        btnGetDistanceBetweenPointAndLine.Name = "btnGetDistanceBetweenPointAndLine"
        btnGetDistanceBetweenPointAndLine.Size = New Size(266, 34)
        btnGetDistanceBetweenPointAndLine.TabIndex = 5
        btnGetDistanceBetweenPointAndLine.Text = "Get Distance between Point and Line"
        btnGetDistanceBetweenPointAndLine.UseVisualStyleBackColor = True
        ' 
        ' btnGetDistanceBetweenCircleAndPoint
        ' 
        btnGetDistanceBetweenCircleAndPoint.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnGetDistanceBetweenCircleAndPoint.Location = New Point(4, 280)
        btnGetDistanceBetweenCircleAndPoint.Name = "btnGetDistanceBetweenCircleAndPoint"
        btnGetDistanceBetweenCircleAndPoint.Size = New Size(266, 34)
        btnGetDistanceBetweenCircleAndPoint.TabIndex = 3
        btnGetDistanceBetweenCircleAndPoint.Text = "Get Distance between Circle and Point"
        btnGetDistanceBetweenCircleAndPoint.UseVisualStyleBackColor = True
        ' 
        ' btnGetAngleBetweenTwoLines
        ' 
        btnGetAngleBetweenTwoLines.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnGetAngleBetweenTwoLines.Location = New Point(4, 319)
        btnGetAngleBetweenTwoLines.Name = "btnGetAngleBetweenTwoLines"
        btnGetAngleBetweenTwoLines.Size = New Size(266, 34)
        btnGetAngleBetweenTwoLines.TabIndex = 3
        btnGetAngleBetweenTwoLines.Text = "Get Angle between Two Lines"
        btnGetAngleBetweenTwoLines.UseVisualStyleBackColor = True
        ' 
        ' btnClearPoints
        ' 
        btnClearPoints.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnClearPoints.Location = New Point(51, 335)
        btnClearPoints.Name = "btnClearPoints"
        btnClearPoints.Size = New Size(250, 34)
        btnClearPoints.TabIndex = 10
        btnClearPoints.Text = "Clear Points"
        btnClearPoints.UseVisualStyleBackColor = True
        ' 
        ' btnFindTrendLine
        ' 
        btnFindTrendLine.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnFindTrendLine.Location = New Point(18, 431)
        btnFindTrendLine.Name = "btnFindTrendLine"
        btnFindTrendLine.Size = New Size(187, 34)
        btnFindTrendLine.TabIndex = 11
        btnFindTrendLine.Text = "Find Trend Line"
        btnFindTrendLine.UseVisualStyleBackColor = True
        ' 
        ' btnFindBFC
        ' 
        btnFindBFC.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnFindBFC.Location = New Point(18, 391)
        btnFindBFC.Name = "btnFindBFC"
        btnFindBFC.Size = New Size(187, 34)
        btnFindBFC.TabIndex = 12
        btnFindBFC.Text = "Find Best Fit Circle"
        btnFindBFC.UseVisualStyleBackColor = True
        ' 
        ' dataGrid
        ' 
        dataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dataGrid.Location = New Point(3, 64)
        dataGrid.Name = "dataGrid"
        dataGrid.Size = New Size(334, 250)
        dataGrid.TabIndex = 9
        ' 
        ' panelOutlierFinder
        ' 
        panelOutlierFinder.Controls.Add(btnAddFoundLine)
        panelOutlierFinder.Controls.Add(btnAddFoundCircle)
        panelOutlierFinder.Controls.Add(btnSavePoint)
        panelOutlierFinder.Controls.Add(Label2)
        panelOutlierFinder.Controls.Add(Label1)
        panelOutlierFinder.Controls.Add(txtCurrentY)
        panelOutlierFinder.Controls.Add(txtCurrentX)
        panelOutlierFinder.Controls.Add(dataGrid)
        panelOutlierFinder.Controls.Add(btnFindTrendLine)
        panelOutlierFinder.Controls.Add(btnClearPoints)
        panelOutlierFinder.Controls.Add(btnFindBFC)
        panelOutlierFinder.Location = New Point(955, 73)
        panelOutlierFinder.Name = "panelOutlierFinder"
        panelOutlierFinder.Size = New Size(337, 469)
        panelOutlierFinder.TabIndex = 13
        ' 
        ' btnAddFoundLine
        ' 
        btnAddFoundLine.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnAddFoundLine.Location = New Point(215, 431)
        btnAddFoundLine.Name = "btnAddFoundLine"
        btnAddFoundLine.Size = New Size(122, 34)
        btnAddFoundLine.TabIndex = 16
        btnAddFoundLine.Text = "Add Found Line"
        btnAddFoundLine.UseVisualStyleBackColor = True
        ' 
        ' btnAddFoundCircle
        ' 
        btnAddFoundCircle.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnAddFoundCircle.Location = New Point(215, 391)
        btnAddFoundCircle.Name = "btnAddFoundCircle"
        btnAddFoundCircle.Size = New Size(122, 34)
        btnAddFoundCircle.TabIndex = 16
        btnAddFoundCircle.Text = "Add Found Circle"
        btnAddFoundCircle.UseVisualStyleBackColor = True
        ' 
        ' btnSavePoint
        ' 
        btnSavePoint.Location = New Point(208, 6)
        btnSavePoint.Name = "btnSavePoint"
        btnSavePoint.Size = New Size(75, 52)
        btnSavePoint.TabIndex = 15
        btnSavePoint.Text = "Save Point"
        btnSavePoint.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(18, 38)
        Label2.Name = "Label2"
        Label2.Size = New Size(60, 15)
        Label2.TabIndex = 14
        Label2.Text = "Current Y:"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(18, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(60, 15)
        Label1.TabIndex = 14
        Label1.Text = "Current X:"
        ' 
        ' txtCurrentY
        ' 
        txtCurrentY.Location = New Point(83, 35)
        txtCurrentY.Name = "txtCurrentY"
        txtCurrentY.Size = New Size(100, 23)
        txtCurrentY.TabIndex = 14
        ' 
        ' txtCurrentX
        ' 
        txtCurrentX.Location = New Point(83, 6)
        txtCurrentX.Name = "txtCurrentX"
        txtCurrentX.Size = New Size(100, 23)
        txtCurrentX.TabIndex = 13
        ' 
        ' FormGeometryCalculator
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1300, 551)
        Controls.Add(panelOutlierFinder)
        Controls.Add(panelGeometryCalculator)
        Controls.Add(txtResult)
        Controls.Add(picView)
        MaximizeBox = False
        Name = "FormGeometryCalculator"
        Text = "Geometrical Calculator & Outlier Finder"
        CType(picView, ComponentModel.ISupportInitialize).EndInit()
        panelGeometryCalculator.ResumeLayout(False)
        grpMeasureType.ResumeLayout(False)
        grpMeasureType.PerformLayout()
        CType(dataGrid, ComponentModel.ISupportInitialize).EndInit()
        panelOutlierFinder.ResumeLayout(False)
        panelOutlierFinder.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents picView As PictureBox
    Friend WithEvents txtResult As TextBox
    Friend WithEvents panelGeometryCalculator As Panel
    Friend WithEvents btnGetDistanceBetweenPointAndIntersection As Button
    Friend WithEvents btnClearAll As Button
    Friend WithEvents btnGetDistanceBetweenTwoCircles As Button
    Friend WithEvents grpMeasureType As GroupBox
    Friend WithEvents radioPerpendicular As RadioButton
    Friend WithEvents radioTangent As RadioButton
    Friend WithEvents radioMin As RadioButton
    Friend WithEvents radioMax As RadioButton
    Friend WithEvents btnGetDistanceBetweenPointAndLine As Button
    Friend WithEvents btnGetDistanceBetweenCircleAndPoint As Button
    Friend WithEvents btnGetAngleBetweenTwoLines As Button
    Friend WithEvents btnClearPoints As Button
    Friend WithEvents btnFindTrendLine As Button
    Friend WithEvents btnFindBFC As Button
    Friend WithEvents dataGrid As DataGridView
    Friend WithEvents panelOutlierFinder As Panel
    Friend WithEvents btnSavePoint As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtCurrentY As TextBox
    Friend WithEvents txtCurrentX As TextBox
    Friend WithEvents btnAddFoundLine As Button
    Friend WithEvents btnAddFoundCircle As Button

End Class

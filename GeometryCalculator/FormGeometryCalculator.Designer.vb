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
        btnClearAll = New Button()
        btnClearPoints = New Button()
        btnFindTrendLine = New Button()
        btnFindBFC = New Button()
        dgvPoints = New DataGridView()
        panelOutlierFinder = New Panel()
        btnAddFoundLine = New Button()
        btnAddFoundCircle = New Button()
        btnSavePoint = New Button()
        Label2 = New Label()
        Label1 = New Label()
        txtCurrentY = New TextBox()
        txtCurrentX = New TextBox()
        btnZoomIn = New Button()
        btnZoomOut = New Button()
        btnSetDefaultZoom = New Button()
        lblCurrentZoom = New Label()
        dgvResults = New DataGridView()
        hsbPicView = New HScrollBar()
        vsbPicView = New VScrollBar()
        CType(picView, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgvPoints, ComponentModel.ISupportInitialize).BeginInit()
        panelOutlierFinder.SuspendLayout()
        CType(dgvResults, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' picView
        ' 
        picView.BackColor = Color.White
        picView.BorderStyle = BorderStyle.FixedSingle
        picView.Location = New Point(10, 11)
        picView.Name = "picView"
        picView.Size = New Size(631, 447)
        picView.TabIndex = 0
        picView.TabStop = False
        ' 
        ' btnClearAll
        ' 
        btnClearAll.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnClearAll.Location = New Point(191, 320)
        btnClearAll.Name = "btnClearAll"
        btnClearAll.Size = New Size(162, 34)
        btnClearAll.TabIndex = 2
        btnClearAll.Text = "Clear All"
        btnClearAll.UseVisualStyleBackColor = True
        ' 
        ' btnClearPoints
        ' 
        btnClearPoints.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnClearPoints.Location = New Point(13, 320)
        btnClearPoints.Name = "btnClearPoints"
        btnClearPoints.Size = New Size(162, 34)
        btnClearPoints.TabIndex = 10
        btnClearPoints.Text = "Clear Points"
        btnClearPoints.UseVisualStyleBackColor = True
        ' 
        ' btnFindTrendLine
        ' 
        btnFindTrendLine.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnFindTrendLine.Location = New Point(22, 402)
        btnFindTrendLine.Name = "btnFindTrendLine"
        btnFindTrendLine.Size = New Size(187, 34)
        btnFindTrendLine.TabIndex = 11
        btnFindTrendLine.Text = "Find Trend Line"
        btnFindTrendLine.UseVisualStyleBackColor = True
        ' 
        ' btnFindBFC
        ' 
        btnFindBFC.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnFindBFC.Location = New Point(22, 362)
        btnFindBFC.Name = "btnFindBFC"
        btnFindBFC.Size = New Size(187, 34)
        btnFindBFC.TabIndex = 12
        btnFindBFC.Text = "Find Best Fit Circle"
        btnFindBFC.UseVisualStyleBackColor = True
        ' 
        ' dgvPoints
        ' 
        dgvPoints.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvPoints.Location = New Point(6, 64)
        dgvPoints.Name = "dgvPoints"
        dgvPoints.Size = New Size(361, 250)
        dgvPoints.TabIndex = 9
        ' 
        ' panelOutlierFinder
        ' 
        panelOutlierFinder.Controls.Add(btnAddFoundLine)
        panelOutlierFinder.Controls.Add(btnClearAll)
        panelOutlierFinder.Controls.Add(btnAddFoundCircle)
        panelOutlierFinder.Controls.Add(btnSavePoint)
        panelOutlierFinder.Controls.Add(Label2)
        panelOutlierFinder.Controls.Add(Label1)
        panelOutlierFinder.Controls.Add(txtCurrentY)
        panelOutlierFinder.Controls.Add(txtCurrentX)
        panelOutlierFinder.Controls.Add(dgvPoints)
        panelOutlierFinder.Controls.Add(btnFindTrendLine)
        panelOutlierFinder.Controls.Add(btnClearPoints)
        panelOutlierFinder.Controls.Add(btnFindBFC)
        panelOutlierFinder.Location = New Point(669, 12)
        panelOutlierFinder.Name = "panelOutlierFinder"
        panelOutlierFinder.Size = New Size(370, 446)
        panelOutlierFinder.TabIndex = 13
        ' 
        ' btnAddFoundLine
        ' 
        btnAddFoundLine.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnAddFoundLine.Location = New Point(223, 402)
        btnAddFoundLine.Name = "btnAddFoundLine"
        btnAddFoundLine.Size = New Size(122, 34)
        btnAddFoundLine.TabIndex = 16
        btnAddFoundLine.Text = "Add Found Line"
        btnAddFoundLine.UseVisualStyleBackColor = True
        ' 
        ' btnAddFoundCircle
        ' 
        btnAddFoundCircle.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnAddFoundCircle.Location = New Point(223, 362)
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
        ' btnZoomIn
        ' 
        btnZoomIn.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point)
        btnZoomIn.Location = New Point(444, 22)
        btnZoomIn.Name = "btnZoomIn"
        btnZoomIn.Size = New Size(31, 30)
        btnZoomIn.TabIndex = 14
        btnZoomIn.Text = "+"
        btnZoomIn.UseVisualStyleBackColor = True
        ' 
        ' btnZoomOut
        ' 
        btnZoomOut.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point)
        btnZoomOut.Location = New Point(481, 22)
        btnZoomOut.Name = "btnZoomOut"
        btnZoomOut.Size = New Size(31, 30)
        btnZoomOut.TabIndex = 14
        btnZoomOut.Text = "-"
        btnZoomOut.UseVisualStyleBackColor = True
        ' 
        ' btnSetDefaultZoom
        ' 
        btnSetDefaultZoom.Location = New Point(520, 22)
        btnSetDefaultZoom.Name = "btnSetDefaultZoom"
        btnSetDefaultZoom.Size = New Size(53, 30)
        btnSetDefaultZoom.TabIndex = 15
        btnSetDefaultZoom.Text = "100%"
        btnSetDefaultZoom.UseVisualStyleBackColor = True
        ' 
        ' lblCurrentZoom
        ' 
        lblCurrentZoom.AutoSize = True
        lblCurrentZoom.BackColor = Color.White
        lblCurrentZoom.Location = New Point(402, 30)
        lblCurrentZoom.Name = "lblCurrentZoom"
        lblCurrentZoom.Size = New Size(35, 15)
        lblCurrentZoom.TabIndex = 16
        lblCurrentZoom.Text = "100%"
        ' 
        ' dgvResults
        ' 
        dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvResults.Location = New Point(669, 464)
        dgvResults.Name = "dgvResults"
        dgvResults.RowTemplate.Height = 25
        dgvResults.Size = New Size(370, 150)
        dgvResults.TabIndex = 17
        ' 
        ' hsbPicView
        ' 
        hsbPicView.Location = New Point(10, 455)
        hsbPicView.Name = "hsbPicView"
        hsbPicView.Size = New Size(631, 16)
        hsbPicView.TabIndex = 18
        ' 
        ' vsbPicView
        ' 
        vsbPicView.Location = New Point(644, 12)
        vsbPicView.Name = "vsbPicView"
        vsbPicView.Size = New Size(14, 441)
        vsbPicView.TabIndex = 19
        ' 
        ' FormGeometryCalculator
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1047, 621)
        Controls.Add(vsbPicView)
        Controls.Add(hsbPicView)
        Controls.Add(dgvResults)
        Controls.Add(lblCurrentZoom)
        Controls.Add(btnSetDefaultZoom)
        Controls.Add(btnZoomOut)
        Controls.Add(btnZoomIn)
        Controls.Add(panelOutlierFinder)
        Controls.Add(picView)
        MaximizeBox = False
        Name = "FormGeometryCalculator"
        Text = "Geometrical Calculator & Outlier Finder"
        CType(picView, ComponentModel.ISupportInitialize).EndInit()
        CType(dgvPoints, ComponentModel.ISupportInitialize).EndInit()
        panelOutlierFinder.ResumeLayout(False)
        panelOutlierFinder.PerformLayout()
        CType(dgvResults, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents picView As PictureBox
    Friend WithEvents btnClearAll As Button
    Friend WithEvents btnClearPoints As Button
    Friend WithEvents btnFindTrendLine As Button
    Friend WithEvents btnFindBFC As Button
    Friend WithEvents dgvPoints As DataGridView
    Friend WithEvents panelOutlierFinder As Panel
    Friend WithEvents btnSavePoint As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtCurrentY As TextBox
    Friend WithEvents txtCurrentX As TextBox
    Friend WithEvents btnAddFoundLine As Button
    Friend WithEvents btnAddFoundCircle As Button
    Friend WithEvents btnZoomIn As Button
    Friend WithEvents btnZoomOut As Button
    Friend WithEvents btnSetDefaultZoom As Button
    Friend WithEvents lblCurrentZoom As Label
    Friend WithEvents dgvResults As DataGridView
    Friend WithEvents hsbPicView As HScrollBar
    Friend WithEvents vsbPicView As VScrollBar

End Class

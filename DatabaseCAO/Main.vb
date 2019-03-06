﻿Imports System.Data.OleDb

Public Class Main
    Private dAdapter As New OleDbDataAdapter, dSet As DataSet = New DataSet
    Private dConnect As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\Resources\CAO.accdb")
    ' -- Note: You need Microsoft Access Engine 2010 to use OleDBCommand
    ' -> Instantiating my data adapter and data set


    Private Sub MyProject(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            dConnect.Open()
        Catch ex As Exception
            MsgBox("Error. Couldn't connect to Access database.")
        End Try
        FillGrid()
        CreateTotalsColumn()
    End Sub
    '
    ' -> Opening my database connection (with try/catch)


    Private Sub FillGrid()
        dSet.Clear()
        dAdapter.SelectCommand = New OleDbCommand("Select * From [5M0536 Module Results]", dConnect)
        dAdapter.Fill(dSet, "Results")
        myGrid.DataSource = dSet.Tables("Results")
    End Sub
    '
    ' -> Filling my data adapter with imported data
    ' -> Setting myGrid with my data imported


    Private Sub CreateTotalsColumn()
        myGrid.Columns.Add("TotalPoints", "CAO Points")
        For i = 0 To (myGrid.Columns.Count - 1)
            myGrid.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
        Next
        myGrid.AutoResizeColumns()
        FillTotalsColumn()
    End Sub
    Public Sub FillTotalsColumn()
        Dim grade As New Integer, points As New Double
        For row As Integer = 0 To RowCounter() - 1
            points = 0
            For col As Integer = 3 To 11
                grade = myGrid.Rows(row).Cells(col).Value
                points += If(grade >= 80, 38.75,
                          If(grade >= 65 And grade < 80, 32.5,
                          If(grade >= 50 And grade < 65, 16.35, 0)))
            Next
            myGrid.Rows(row).Cells(12).Value = points
        Next
    End Sub
    '
    ' -> Adding new total column and adding values to new column
    ' -> Resizing all columns and updating row-counter


    Private Sub ViewStudent(sender As Object, e As EventArgs) Handles btnView.Click
        Dim selectedRow As Integer, counter As Integer
        selectedRow = myGrid.CurrentRow.Index
        counter = 0
        For Each singleBox In TextBoxes()
            singleBox.Text = myGrid.Rows(selectedRow).Cells(counter).Value
            counter += 1
        Next
    End Sub


    Private Sub InsertLoad(sender As Object, e As EventArgs) Handles btnInsert.Click, btnLoad.Click, btnDelete.Click

        If CType(sender, Button).Name.ToString = "btnInsert" Then

            ' -> Feature to implement <-
            ' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ' Maintain integrity of database // Check text boxes
            ' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            dAdapter.InsertCommand = New OleDbCommand("INSERT INTO [5M0536 Module Results] Values (@1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12)", dConnect)
            For i As Int16 = 0 To 11
                dAdapter.InsertCommand.Parameters.AddWithValue(i, TextBoxes(i).Text)
            Next
            dAdapter.InsertCommand.ExecuteNonQuery()
        End If

        If CType(sender, Button).Name.ToString = "btnDelete" Then

            ' -> Feature to implement <-
            ' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ' Warn user you are about to delete selected student
            ' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            dAdapter.DeleteCommand = New OleDbCommand("DELETE FROM [5M0536 Module Results] where PPSN='" & myGrid.CurrentRow.Cells(0).Value & "'", dConnect)
            dAdapter.DeleteCommand.ExecuteNonQuery()
        End If

        FillGrid()
        FillTotalsColumn()
    End Sub
    '
    ' -> Load / Insert New Record


    Private Function TextBoxes() As IEnumerable(Of TextBox)
        Return New List(Of TextBox) From {
                    txtPPSN, txtForename, txtSurname, txt5N2928, txt5N2929, txt5N0548,
                    txt5N2434, txt5N2927, txt5N18396, txt5N0783, txt5N0690, txt5N1356}
    End Function

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

        ' -> Feature to implement <-
        ' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        ' // Right now it only updates first name to 'Morgan'
        ' // However this is just a display on how to use the OleDbCommand "Update"
        ' // Took a lot of guess work so far to get this to work - MAJOR SET BACK
        ' // Going to finish implementation on next commit
        ' // Only have to modify marks!
        ' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        dAdapter.UpdateCommand = New OleDbCommand("UPDATE [5M0536 Module Results] set FirstName ='" & "Morgan" & "'where PPSN='" & myGrid.CurrentRow.Cells(0).Value & "'", dConnect)
        dAdapter.UpdateCommand.ExecuteNonQuery()
    End Sub

    Public Function RowCounter() As String
        Return dSet.Tables("Results").Rows.Count
    End Function


    '   Temporary links commented leading to resources for code used in my assignment
    'how I can show the sum of in a datagridview column?
    'https://stackoverflow.com/questions/3779729/how-i-can-show-the-sum-of-in-a-datagridview-column

    ' how to make vb.net scalable window 
    ' https://www.techrepublic.com/article/manage-winform-controls-using-the-anchor-And-dock-properties/

    ' -> Returns value from NameID of the selected row
    ' -> Gets cell index
    ' ->   https://social.msdn.microsoft.com/Forums/windows/en-US/7ce81f9a-7047-444d-b75b-ef548b2ec635/datagridview-select-rows-and-retrieve-the-values-of-the-cells?forum=winformsdatacontrols
    ' -> https://www.daniweb.com/programming/software-development/threads/94061/get-the-selected-row-in-datagridview

End Class
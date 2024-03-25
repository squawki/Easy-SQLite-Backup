Imports System.IO
Imports System.Data.SQLite

Imports System.Runtime.InteropServices
Imports System.Reflection
Imports System.IO.Enumeration
Public Class Form1
    Private configFilePath As String = "config.ini"
    Dim backup_enabled As Boolean
    Dim backup_nextbackupseconds As Integer = 9999


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadConfiguration()
    End Sub

    Private Sub BackupDatabase(ByVal sourcelocation As String, ByVal backuplocation As String, ByVal silent As Boolean, ByVal appendtimestamp As Boolean, ByVal writelog As Boolean)

        Dim _backuplocation As String = backuplocation



        If appendtimestamp = True Then

            Dim directoryPath As String = Path.GetDirectoryName(_backuplocation)
            Dim fileName As String = Path.GetFileNameWithoutExtension(_backuplocation)
            Dim fileExtension As String = Path.GetExtension(_backuplocation)

            ' Generate timestamp
            Dim timestamp As String = DateTime.Now.ToString("ddMMyyyy_HHmmss")

            ' Append timestamp before the extension
            Dim newFileName As String = fileName & "_" & timestamp & fileExtension

            ' Form the new directory string
            Dim newDirectoryString As String = Path.Combine(directoryPath, newFileName)

            _backuplocation = newDirectoryString
        End If



        If radio_filecopy.Checked Then

            '  Offline Copy Method
            Try
                ' Check if the source file exists
                If File.Exists(sourcelocation) Then
                    ' Copy the source file to the backup destination
                    File.Copy(sourcelocation, _backuplocation, True)
                    If silent = False Then
                        MessageBox.Show("Backup completed successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                Else
                    If silent = False Then
                        MessageBox.Show("Source database file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If
            Catch ex As Exception
                If silent = False Then
                    MessageBox.Show("Error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        End If


        If radio_vacumm.Checked Then

            'Vacumm SQLiteMethod
            Try
                ' Check if the source file exists
                If File.Exists(sourcelocation) Then
                    ' Connect to the source database
                    Using sourceConnection As New SQLiteConnection($"Data Source={sourcelocation}")
                        sourceConnection.Open()

                        ' Execute VACUUM command to optimize and clean up the source database
                        Using cmd As New SQLiteCommand("VACUUM", sourceConnection)
                            cmd.ExecuteNonQuery()

                        End Using
                    End Using

                    ' Copy the optimized source database to the backup destination
                    File.Copy(sourcelocation, _backuplocation, True)

                    If Not silent Then
                        MessageBox.Show("Backup completed successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                Else
                    If Not silent Then
                        MessageBox.Show("Source database file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If
            Catch ex As Exception
                If Not silent Then
                    MessageBox.Show("Error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try

        End If


    End Sub

    Private Sub txtSourceLocation_Click(sender As Object, e As EventArgs) Handles txtSourceLocation.DoubleClick
        ' Display the OpenFileDialog to select the source database file
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "SQLite Database File (*.db)|*.db"
        openFileDialog.Title = "Select Source Database File"
        openFileDialog.InitialDirectory = "C:\"
        openFileDialog.RestoreDirectory = True

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            txtSourceLocation.Text = openFileDialog.FileName
        End If
    End Sub

    Private Sub txtBackupLocation_Click(sender As Object, e As EventArgs) Handles txtBackupLocation.DoubleClick
        Dim saveFileDialog As New SaveFileDialog()
        saveFileDialog.Filter = "SQLite Database File (*.db)|*.db"
        saveFileDialog.Title = "Select Backup Destination File"
        saveFileDialog.InitialDirectory = "C:\"
        saveFileDialog.RestoreDirectory = True

        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            txtBackupLocation.Text = saveFileDialog.FileName
        End If
    End Sub

    Private Sub btnBackupNow_Click(sender As Object, e As EventArgs) Handles btnBackupNow.Click

        BackupDatabase(txtSourceLocation.Text, txtBackupLocation.Text, False, chk_appendTimeStamp.Checked, False)

    End Sub

    Private Sub SaveConfiguration()

        Dim backupmode As String = ""
        If radio_vacumm.Checked Then backupmode = "VACUUM"
        If radio_filecopy.Checked Then backupmode = "FILECOPY"
        Try
            ' Create a StreamWriter to write to the config file
            Using writer As New StreamWriter(configFilePath)
                ' Write the source file path and backup folder path to the config file
                writer.WriteLine("SourceFilePath=" & txtSourceLocation.Text)
                writer.WriteLine("BackupFolderPath=" & txtBackupLocation.Text)
                writer.WriteLine("AppendTimestamp=" & chk_appendTimeStamp.Checked)
                writer.WriteLine("AutomatedInterval=" & numeric_automatedInterval.Value)
                writer.WriteLine("AutomatedAutoRun=" & chk_automated_runStart.Checked)
                writer.WriteLine("BackupMode=" & backupmode)

            End Using
        Catch ex As Exception
            MessageBox.Show("Error occurred while saving configuration: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub LoadConfiguration()
        Dim backupmode As String = ""

        If File.Exists(configFilePath) Then
            Try
                ' Read configuration from the config file
                Dim lines As String() = File.ReadAllLines(configFilePath)
                For Each line As String In lines
                    Dim parts As String() = line.Split("="c)
                    If parts.Length = 2 Then
                        Dim key As String = parts(0)
                        Dim value As String = parts(1)
                        If key = "SourceFilePath" Then
                            txtSourceLocation.Text = value
                        ElseIf key = "BackupFolderPath" Then
                            txtBackupLocation.Text = value
                        ElseIf key = "AppendTimestamp" Then
                            chk_appendTimeStamp.Checked = value
                        ElseIf key = "AutomatedInterval" Then
                            numeric_automatedInterval.Value = value
                        ElseIf key = "AutomatedAutoRun" Then
                            chk_automated_runStart.Checked = value
                        ElseIf key = "BackupMode" Then
                            backupmode = value

                        End If
                    End If
                Next
                If chk_automated_runStart.Checked = False Then
                    backup_enabled = False
                    btn_automatedEnabled.Text = "Disabled"
                    btn_automatedEnabled.BackColor = Color.LightGray

                Else
                    backup_enabled = True
                    btn_automatedEnabled.Text = "Enabled"
                    btn_automatedEnabled.BackColor = Color.LightGreen

                End If

                Select Case backupmode
                    Case "VACUUM"
                        radio_vacumm.Checked = True
                    Case "FILECOPY"
                        radio_filecopy.Checked = True
                    Case Else
                        radio_vacumm.Checked = True
                End Select


            Catch ex As Exception
                MessageBox.Show("Error occurred while loading configuration: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            btn_automatedEnabled.Text = "Disabled"
            backup_enabled = False
            calculateNextBackup()
        End If
    End Sub

    Private Sub btnSaveConfig_Click(sender As Object, e As EventArgs) Handles btnSaveConfig.Click
        SaveConfiguration()
    End Sub

    Private Sub btn_automatedEnabled_Click(sender As Object, e As EventArgs) Handles btn_automatedEnabled.Click
        Select Case btn_automatedEnabled.Text
            Case "Enabled"
                backup_enabled = False
                btn_automatedEnabled.Text = "Disabled"
                btn_automatedEnabled.BackColor = Color.LightGray
            Case "Disabled"
                backup_enabled = True
                btn_automatedEnabled.Text = "Enabled"
                btn_automatedEnabled.BackColor = Color.LightGreen
                backup_nextbackupseconds = numeric_automatedInterval.Value * 60
        End Select
        calculateNextBackup()
    End Sub


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        calculateNextBackup()

        If backup_enabled = True Then
            Select Case pnl_automatedIndicator.BackColor
                Case Color.White
                    pnl_automatedIndicator.BackColor = Color.LightGreen
                Case Color.LightGreen
                    pnl_automatedIndicator.BackColor = Color.White
                Case Color.Purple
                    pnl_automatedIndicator.BackColor = Color.White
            End Select
            backup_nextbackupseconds = backup_nextbackupseconds - 1

            If backup_nextbackupseconds < 0 Then
                pnl_automatedIndicator.BackColor = Color.Purple


                BackupDatabase(txtSourceLocation.Text, txtBackupLocation.Text, True, chk_appendTimeStamp.Checked, False)

                ResetBackup()
            End If

        Else
            pnl_automatedIndicator.BackColor = Color.White



        End If

    End Sub
    Private Sub calculateNextBackup()


        If backup_enabled = True Then
            Dim hours As Integer = backup_nextbackupseconds \ 3600
            Dim minutes As Integer = (backup_nextbackupseconds Mod 3600) \ 60
            Dim seconds As Integer = backup_nextbackupseconds Mod 60

            Dim formattedTime As String = String.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds)
            lblNextBackupDisplay.Text = formattedTime

        Else
            lblNextBackupDisplay.Text = "Never"
        End If

    End Sub

    Private Sub numeric_automatedInterval_ValueChanged(sender As Object, e As EventArgs) Handles numeric_automatedInterval.ValueChanged
        ResetBackup()

    End Sub
    Private Sub ResetBackup()
        backup_nextbackupseconds = numeric_automatedInterval.Value * 60
        calculateNextBackup()
    End Sub

    Private Sub numeric_automatedInterval_KeyUp(sender As Object, e As KeyEventArgs) Handles numeric_automatedInterval.KeyUp
        ResetBackup()

    End Sub

End Class


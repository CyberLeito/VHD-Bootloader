Imports System.IO
Imports System.IO.Compression
Imports Microsoft.Win32
Imports System
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Forms.Form
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Text


Public Class Form1
    Dim AppName As String = "VHD Bootloader"
    Dim bootmode As String
    Dim UIDval As String
    Dim temp As String = Path.GetTempPath() 'Appdata\local\temp\

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
       
    End Sub


    'Sub BCDINFO()

    '    If System.IO.File.Exists(temp & "BCDinfo.txt") = True Then
    '        File.Delete(temp & "BCDinfo.txt")
    '    End If

    '    Dim oProcess As New Process()
    '    'Dim oStartInfo As New ProcessStartInfo("cmd.exe", "/c BCDEDIT>%temp%\BCDinfo.txt")
    '    Dim oStartInfo As New ProcessStartInfo("cmd.exe", "/c BCDEDIT>C:\BCDinfo.txt")
    '    'oStartInfo.UseShellExecute = True
    '    oStartInfo.WindowStyle = ProcessWindowStyle.Hidden
    '    oStartInfo.CreateNoWindow = True
    '    'oStartInfo.Verb = "RunAs"
    '    oProcess.StartInfo = oStartInfo
    '    oProcess.Start()
    '    oProcess.WaitForExit()
    '    oProcess.Close()
    '    'oProcess.Kill()
    'End Sub

    Sub BCDWrite()
        Dim application As New ProcessStartInfo("cmd.exe") With
                         {.RedirectStandardInput = True, .UseShellExecute = False}
        Dim process As New Process
        'application.WindowStyle = ProcessWindowStyle.Hidden
        ' application.CreateNoWindow = True
        process = process.Start(application)
        Dim command As String = "BCDEDIT>C:\BCDinfo.txt" _
           & vbCrLf & "exit"

        'myStreamWriter.WriteLine(Forms.Keys.Control + Forms.Keys.C)
        process.StandardInput.WriteLine(command)
        'process.WaitForExit()
        'System.Threading.Thread.Sleep(1000)
        'process.Kill()
        process.Close()
    End Sub


    Public Function FindStringInFile() As Boolean
        Dim Reader As System.IO.StreamReader
        Reader = New IO.StreamReader("C:\BCDinfo.txt")
        Dim stringReader As String
        Try
            While Reader.Peek <> -1
                stringReader = Reader.ReadLine()
                If InStr(stringReader, ".efi") > 0 Then Return True
            End While
            Reader.Close()
            Return False
        Catch ex As Exception
            MessageBox.Show("Exception: " & ex.Message)
            Return False
        End Try
    End Function



    'Private Sub Form2_FormClosed(sender As Object, e As FormClosedEventArgs) _
    'Handles Me.FormClosed
    '    'If System.IO.File.Exists("C:\defguid.txt") = True Then
    '    '    File.Delete("C:\defguid.txt")
    '    'End If
    '    'If System.IO.File.Exists("C:\BCDinfo.txt") = True Then
    '    '    File.Delete("C:\BCDinfo.txt")
    '    'End If
    'End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        BCDWrite()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If System.IO.File.Exists("C:\BCDinfo.txt") = True Then
            If FindStringInFile() Then
                'MessageBox.Show("Serial number found")
                Label1.Text = "Boot mode: UEFI"
                Label1.Visible = True
                bootmode = "UEFI"

            Else
                Label1.Text = "Boot mode: Legacy"
                Label1.Visible = True
                bootmode = "Legacy"
            End If
        Else
            MessageBox.Show("Step1 incomplete")
        End If
        Button3.Enabled = True
    End Sub


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        UEFIdeploy()
       
        'If (bootmode = "UEFI") Then

        'Else

        'End If
    End Sub



    Sub UEFIdeploy()
        If MessageBox.Show("Make Sure VHD for " & bootmode & " is copied to 'root' of your harddrive" & vbNewLine & _
                      "VHD must be named: ""Win10DDAC.vhd""" & vbNewLine & _
                      "Are you sure you want to continue?", AppName, _
             MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

            cloneBCD()

            System.Threading.Thread.Sleep(2000)
            LoadUID() 'truncateUID

            ' part2() 'device (VHD)

            part3() 'OSdevice (VHD)

        End If

    End Sub

    Sub cloneBCD()
        Dim application As New ProcessStartInfo("cmd.exe") With
                            {.RedirectStandardInput = True, .UseShellExecute = False}
        Dim process As New Process
        application.WindowStyle = ProcessWindowStyle.Hidden
        application.CreateNoWindow = True
        process = process.Start(application)
        Dim command As String = "bcdedit /copy {default} /d ""Windows 10 DDAC"">C:\defguid.txt" _
           & vbCrLf & "exit"

        'myStreamWriter.WriteLine(Forms.Keys.Control + Forms.Keys.C)
        process.StandardInput.WriteLine(command)
        'process.WaitForExit()
        'System.Threading.Thread.Sleep(1000)
        'process.Kill()
        process.Close()
    End Sub

    Sub part2()
        Dim application As New ProcessStartInfo("cmd.exe") With
                        {.RedirectStandardInput = True, .UseShellExecute = False}
        Dim process As New Process
        application.WindowStyle = ProcessWindowStyle.Hidden
        application.CreateNoWindow = True
        process = process.Start(application)

        Dim command As String = "bcdedit /set " & UIDval & " device vhd=[locate]\Win10DDAC.vhd" _
           & vbCrLf & "exit"
        process.StandardInput.WriteLine(command)
        process.Close()
    End Sub

    Sub part3()
        Dim application As New ProcessStartInfo("cmd.exe") With
                        {.RedirectStandardInput = True, .UseShellExecute = False}
        Dim process As New Process
        application.WindowStyle = ProcessWindowStyle.Hidden
        application.CreateNoWindow = True
        process = process.Start(application)

        Dim command As String = "bcdedit /set " & UIDval & " osdevice vhd=[locate]\Win10DDAC.vhd" _
           & vbCrLf & "exit"
        process.StandardInput.WriteLine(command)
        process.Close()
    End Sub

    Sub LoadUID()
        Dim fileReader As String
        fileReader = My.Computer.FileSystem.ReadAllText("C:\defguid.txt")
        Dim MidWords As String = Mid(fileReader, 38)
        Dim lenth As Integer = Len(MidWords)
        'Dim MidWords As String = Mid(TestString, 5)
        Dim FirstWord As String = Mid(MidWords, 1, lenth - 3)
        UIDval = FirstWord
        Label2.Text = "UUID: " & FirstWord
        Label2.Visible = True
        'MsgBox(FirstWord)
    End Sub





End Class

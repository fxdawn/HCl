Imports System.Windows.Forms
Imports System.Text
Imports System.Net.Sockets
Imports System.Net
Imports System
Imports System.Diagnostics
Imports System.Net.NetworkInformation
Imports System.Text.RegularExpressions
Imports System.Management
Imports System.IO
Public Class Form1
    Dim k As String
    Dim r As String
    Dim b As Object
    Private Property str As String
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        End
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim x As Array
        Dim i As Int32 = 0
        Dim stritem As String
        x = Split(TextBox1.Text, vbCrLf)
        For Each stritem In x
            i = i + 1
            If Not Regex.IsMatch(stritem.ToString(), "^([0-9a-fA-F]{2})(([/\s:][0-9a-fA-F]{2}){5})$") Then
                If Not Regex.IsMatch(stritem.ToString(), "^$") Then
                    MsgBox("第" & i & "行的MAC地址有误，请检查")
                End If
            End If
            Try
                Dim s As New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                Dim localEndPoint As New IPEndPoint(IPAddress.Parse("202.193.160.123"), 20015)
                'Dim localEndPoint As New IPEndPoint(IPAddress.Parse("192.168.2.14"), 20015)
                s.Connect(localEndPoint)
                Dim byte1(), byte2(), byte3(), byte4() As Byte
                byte1 = {&H32, &H33, &H33, &H33, &H33, &H33, &H33, &H33, &H33, &H33, &H33, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &HAC, &H13, &H3B, &H7C}
                byte2 = System.Text.Encoding.UTF8.GetBytes(stritem)
                byte3 = {&H0, &H0, &H0, &H2}
                ReDim byte4(byte1.Length + byte2.Length + byte3.Length)
                byte1.CopyTo(byte4, 0)
                byte2.CopyTo(byte4, byte1.Length)
                byte3.CopyTo(byte4, byte1.Length + byte2.Length)
                s.Send(byte4)
                s.Close()
            Catch ex As Exception
            End Try
        Next
        MsgBox("数据已发送。" & vbCrLf & "连接网络时直接拨号就好了，不需要使用万恶的出校控制器。")
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = GetMac()
        Dim ipaddr As String
        ipaddr = GetIpV4()
        Label2.Text = "当前IP：" & GetIpV4()
    End Sub
    Private Sub Socket(p1 As Object)
        Throw New NotImplementedException
    End Sub
    Public Function StrToHex(ByRef Data As String) As String
        'Dim B() As Array
        Dim sVal As String
        Dim sHex As String = ""
        While Data.Length > 0
            sVal = Conversion.Hex(Strings.Asc(Data.Substring(0, 1).ToString()))
            Data = Data.Substring(1, Data.Length - 1)
            sHex = sHex & sVal
        End While
        Return sHex
    End Function
    Public Function GetMac() As String
        Try
            Dim stringMAC As String = ""
            Dim stringIP As String = ""
            Dim query As ManagementObjectSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration")
            Dim queryCollection As ManagementObjectCollection = query.Get
            For Each mo As ManagementObject In queryCollection
                If CBool(mo("IPEnabled").ToString) = True Then
                    stringMAC += mo("MACAddress").ToString()
                End If
            Next
            Return stringMAC
        Catch ex As Exception
        End Try
        Return String.Empty
    End Function

    Public Function GetIpV4() As String
        '有可能会返回IPv6的地址
        '于是就用了GetIPv4
        Dim myHost As String = Dns.GetHostName
        Dim ipEntry As IPHostEntry = Dns.GetHostEntry(myHost)
        Dim ip As String = ""

        For Each tmpIpAddress As IPAddress In ipEntry.AddressList
            If tmpIpAddress.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                Dim ipAddress As String = tmpIpAddress.ToString
                ip = ipAddress
                Exit For
            End If
        Next

        If ip = "" Then
            Throw New Exception("No 10. IP found!")
        End If

        Return ip

    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
    End Sub
    Function GetEnvironmentVariable()

        Dim strComputerName As String

        strComputerName = Environment.GetEnvironmentVariable("UserProFile")

        Return (strComputerName)

    End Function

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim IntR As Integer
        IntR = MsgBox("确认清空？", vbYesNo, "清空确认")
        If IntR = vbYes Then
            TextBox1.Text = ""
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim sw As System.IO.StreamWriter
        sw = New System.IO.StreamWriter(GetEnvironmentVariable() & "\desktop\MAC地址.txt", True)
        sw.WriteLine(TextBox1.Text)
        sw.Close()
        MsgBox("已保存到桌面""MAC地址.txt""文件。" & vbCrLf & "粘贴到获取到了172地址的电脑上发送数据即可连接。")
    End Sub

End Class

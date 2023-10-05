Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.Json
Imports UdpClassLib2

Class MainWindow
    Dim rng As Random
    Dim port%
    Dim udp As UdpClient
    Dim endpoint As IPEndPoint

    Dim server_ip$ = "127.0.0.1"
    Dim server_port% = 803


    Sub PrepareSendMessage()
        Dim msg As New MessagePacket With {
            .display_name = txbDisplayName.Text,
            .contents = txbMessage.Text
        }

        Dim msg_json = JsonSerializer.Serialize(msg)
        Dim msg_bytes = Encoding.ASCII.GetBytes(msg_json)

        Debug.Print(msg_json)

        udp.Send(msg_bytes, msg_bytes.Length)
    End Sub


    Private Sub txbMessage_KeyDown(sender As Object, e As KeyEventArgs) Handles txbMessage.KeyDown
        If e.Key = Key.Return Then
            PrepareSendMessage()
        End If
    End Sub


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        rng = New Random
        port = rng.Next(1, Short.MaxValue)

        udp = New UdpClient(port)
        endpoint = New IPEndPoint(IPAddress.Any, port)

        Dim server_ep As New IPEndPoint(IPAddress.Parse(server_ip), server_port)
        udp.Connect(server_ep)

        Dim hi_msg = Encoding.ASCII.GetBytes($"hi_{port}")
        udp.Send(hi_msg, hi_msg.Length)

        udp.BeginReceive(New AsyncCallback(AddressOf Receiver), Nothing)
    End Sub


    Sub Receiver(res As IAsyncResult)
        Try
            Dim remote_ep As New IPEndPoint(server_ip, server_port)
            Dim bytes = udp.EndReceive(res, Nothing)

            'Debug.Print(String.Join(" ", bytes))

            txbChatLog.Dispatcher.Invoke(
                Sub()
                    txbChatLog.AppendText(Encoding.ASCII.GetString(bytes) + vbCrLf)
                End Sub)
        Catch ex As Exception
            txbChatLog.Dispatcher.Invoke(
                Sub()
                    txbChatLog.AppendText($"Unable to send message: ""{ txbMessage.Text }""" + vbCrLf)
                    txbChatLog.AppendText(ex.ToString + vbCrLf)
                End Sub)
        Finally
            txbMessage.Dispatcher.Invoke(Sub() txbMessage.Clear())
        End Try

        Try
            udp.BeginReceive(New AsyncCallback(AddressOf Receiver), Nothing)
        Catch odex As ObjectDisposedException
        End Try
    End Sub


    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        Dim bye_msg = Encoding.ASCII.GetBytes($"bye_{port}")
        udp.Send(bye_msg, bye_msg.Length)

        udp.Close()
    End Sub
End Class

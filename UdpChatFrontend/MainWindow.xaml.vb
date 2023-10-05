Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Class MainWindow
    Dim rng As Random
    Dim port%
    Dim udp As UdpClient
    Dim endpoint As IPEndPoint

    Dim server_ip$ = "127.0.0.1"
    Dim server_port% = 803

    Sub SendMessage()
        Dim msg = Encoding.ASCII.GetBytes(txbMessage.Text)
        udp.Send(msg, msg.Length)
    End Sub

    Private Sub txbMessage_KeyDown(sender As Object, e As KeyEventArgs) Handles txbMessage.KeyDown
        If e.Key = Key.Return Then
            SendMessage()
        End If
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        rng = New Random
        port = rng.Next(1, Short.MaxValue)

        udp = New UdpClient(port)
        endpoint = New IPEndPoint(IPAddress.Any, port)

        Dim server_ep As New IPEndPoint(IPAddress.Parse(server_ip), server_port)
        udp.Connect(server_ep)

        udp.BeginReceive(New AsyncCallback(AddressOf Receiver), Nothing)
    End Sub

    Private Sub Window_Unloaded(sender As Object, e As RoutedEventArgs)

        udp.Close()
    End Sub

    Sub Receiver(res As IAsyncResult)
        Dim remote_ep As New IPEndPoint(server_ip, server_port)
        Dim bytes = udp.EndReceive(res, Nothing)
        'Dim bytes = udp.ReceiveAsync()

        Debug.Print(String.Join(" ", bytes))

        txbChatLog.Dispatcher.Invoke(
            Sub()
                txbChatLog.AppendText(Encoding.ASCII.GetString(bytes) + vbCrLf)
            End Sub)

        udp.BeginReceive(New AsyncCallback(AddressOf Receiver), Nothing)
    End Sub

End Class

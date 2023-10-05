Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.Json
Imports UdpClassLib2

Module Program
    Sub Main(args As String())
        Dim done = False
        'Dim ipAddr$ = "127.0.0.1"
        Dim server_port = 803

        Dim udp As New UdpClient(server_port)
        Dim endpoint As New IPEndPoint(IPAddress.Any, server_port)

        Dim remote_endpoints As New HashSet(Of IPEndPoint)

        Console.WriteLine($"Listening to port { server_port }")

        Do
            Try
                Dim data = udp.Receive(endpoint)
                Dim msg_str$ = Encoding.ASCII.GetString(data)

                Dim remote_ip$ = endpoint.ToString
                Dim remote_ep = IPEndPoint.Parse(remote_ip)

                ' register user
                If msg_str.StartsWith("hi_") Then
                    remote_endpoints.Add(endpoint)

                    Console.WriteLine($"{endpoint} joined the conversation.")

                    Continue Do
                End If

                If msg_str.StartsWith("bye_") Then
                    remote_endpoints.Remove(endpoint)

                    Console.WriteLine($"{endpoint} left the conversation.")

                    Continue Do
                End If

                Dim message = JsonSerializer.Deserialize(Of MessagePacket)(msg_str)

                Console.WriteLine("Client [{0}]:", remote_ip)
                Console.WriteLine(msg_str)

                ' send to all the clients
                Dim display_name$ = IIf(Len(message.display_name) > 0, message.display_name, $"anonymous_{remote_ep.Port}")
                Dim res$ = $"{Date.Now:HH:mm} [{display_name}]: {message.contents}"

                For Each ep In remote_endpoints
                    'Try
                    udp.Send(Encoding.ASCII.GetBytes(res$), Len(res), ep)
                    'Catch ex As Exception
                    '    remote_endpoints.Remove(ep)
                    '    Console.WriteLine($"Removed {ep} due to the above exception.")
                    'End Try
                Next
            Catch ex As SocketException
                ' WSAECONNRESET = 10054
                ' Ref: https://learn.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2
                If ex.ErrorCode = 10054 Then

                Else
                    Console.WriteLine(ex.ToString)
                End If

                'remote_endpoints.Remove(endpoint)
                'Console.WriteLine($"Removed {endpoint} due to the above exception.")
            End Try
        Loop Until done

    End Sub
End Module

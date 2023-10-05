Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Module Program
    Sub Main(args As String())
        Dim done = False
        'Dim ipAddr$ = "127.0.0.1"
        Dim server_port = 803

        Dim udp As New UdpClient(server_port)
        Dim endpoint As New IPEndPoint(IPAddress.Any, server_port)

        Dim remote_endpoints As New HashSet(Of IPEndPoint)

        Do
            Try
                Dim data = udp.Receive(endpoint)
                Dim remote_ep$ = endpoint.ToString

                Dim message$ = Encoding.ASCII.GetString(data)

                Console.WriteLine("Client [{0}]:", remote_ep)

                Dim temp = IPEndPoint.Parse(remote_ep)

                remote_endpoints.Add(endpoint)

                Console.WriteLine(message)

                ' Todo: broadcast to all remote_ports
                'udp.Send({1}, 1, endpoint)
                ' Dim res$ = "[Server]: Yahaha"

                Dim res$ = $"Client [{endpoint}]: " + message

                For Each ep In remote_endpoints
                    udp.Send(Encoding.ASCII.GetBytes(res$), Len(res), ep)
                Next
            Catch ex As Exception
                Console.WriteLine(ex.ToString)
            End Try
        Loop Until done

    End Sub
End Module
